#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Collections.Generic;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPersonEmployment : busPersonEmploymentGen
    {
        private Collection<busPersonEmployment> _iclbOtherEmployment;
        public DateTime idtelastDateOfService { get; set; }
        public bool iblnMessageToShowAfterSaveClick { get; set; }
        //PIR 26517
        public string istrMessageToShowAfterPostClick { get; set; }
        public Collection<busPersonEmployment> iclbOtherEmployment
        {
            get
            {
                return _iclbOtherEmployment;
            }

            set
            {
                _iclbOtherEmployment = value;
            }
        }

        // PIR 8852
        //busWssEmploymentChangeRequest lobjWssEmpChReq = new busWssEmploymentChangeRequest
        //{
        //    icdoWssEmploymentChangeRequest = new cdoWssEmploymentChangeRequest()
        //};
        //PIR 8975
        public bool iblnIsEmploymentChangeRequestExists { get; set; }
        // PIR 8912
        public string istrDBPlan
        {
            get
            {
                if (ibusOrganization.IsNull())
                    LoadOrganization();
                if (ibusOrganization.iclbOrgPlan.IsNull())
                    ibusOrganization.LoadOrgPlan();
                if (ibusOrganization.iclbOrgPlan.Any(i => i.ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDC))
                    return "Contribution";
                if (ibusOrganization.iclbOrgPlan.Any(i => (i.ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDB ||
                        i.ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueHB)))       //PIR 25920  New DC plan
                    return "Benefit";
                return string.Empty;
            }
        }
        public int iintPlanID { get; set; }
        public void LoadOtherEmployment()
        {
            if (_iclbOtherEmployment == null)
            {
                _iclbOtherEmployment = new Collection<busPersonEmployment>();
            }
            DataTable ldtOtherEmployment = busNeoSpinBase.Select("cdoPersonEmployment.LoadOtherEmployment", new object[2] { icdoPersonEmployment.person_id, icdoPersonEmployment.person_employment_id });
            _iclbOtherEmployment = GetCollection<busPersonEmployment>(ldtOtherEmployment, "icdoPersonEmployment");
            foreach (busPersonEmployment lobjEmpHistory in _iclbOtherEmployment)
            {
                lobjEmpHistory.LoadOrganization();
            }
        }

        // PIR-8298
        public void LoadLastPaycheckDate()
        {
            DataTable ldtPaycheckDate = busNeoSpinBase.Select("cdoPersonEmployment.LoadLastPaycheckDate", new object[2] { ibusPerson.icdoPerson.person_id, icdoPersonEmployment.org_id});
                       
            if (ldtPaycheckDate.Rows.Count > 0)
            {          
                string strPayCheckDate = ldtPaycheckDate.Rows[0]["DATE_OF_LAST_REGULAR_PAYCHECK"].ToString();
                //PIR 8852 - load Employment Change Request data in data object and set boolena to true
                //lobjWssEmpChReq.icdoWssEmploymentChangeRequest.LoadData(ldtPaycheckDate.Rows[0]);
                iblnIsEmploymentChangeRequestExists = true;
                if (strPayCheckDate.IsNotEmpty() && icdoPersonEmployment.date_of_last_regular_paycheck == DateTime.MinValue)
                {
                    icdoPersonEmployment.date_of_last_regular_paycheck = Convert.ToDateTime(strPayCheckDate);
                    //PIR 8852
                    //icdoPersonEmployment.new_date_of_last_paycheck = icdoPersonEmployment.date_of_last_regular_paycheck;
                }
            }
        }
        // PIR-8298 end


        public busPersonEmploymentDetail ibusLatestEmploymentDetail { get; set; }

        public void LoadLatestPersonEmploymentDetail()
        {
            if (ibusLatestEmploymentDetail == null)
            {
                ibusLatestEmploymentDetail = new busPersonEmploymentDetail();
                ibusLatestEmploymentDetail.icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail();
            }

            if (icolPersonEmploymentDetail == null)
                LoadPersonEmploymentDetail();

            if (icolPersonEmploymentDetail.Count > 0)
            {
                ibusLatestEmploymentDetail = icolPersonEmploymentDetail.First();
            }
        }

        //This is to check whether there record exists for same period
        public bool CheckEmploymentOverlapping()
        {
            bool lblnRecordMatch = false;
            if (_iclbOtherEmployment.IsNull())
                LoadOtherEmployment();
            foreach (busPersonEmployment _busPersonEmploymentHistoryTemp in _iclbOtherEmployment)
            {
                if ((icdoPersonEmployment.org_id == _busPersonEmploymentHistoryTemp.icdoPersonEmployment.org_id)
                    && (
                            (busGlobalFunctions.CheckDateOverlapping(
                                        icdoPersonEmployment.start_date,
                                        _busPersonEmploymentHistoryTemp.icdoPersonEmployment.start_date,
                                        _busPersonEmploymentHistoryTemp.icdoPersonEmployment.end_date
                                        )
                            )
                            ||
                            (busGlobalFunctions.CheckDateOverlapping(
                                    icdoPersonEmployment.end_date,
                                    _busPersonEmploymentHistoryTemp.icdoPersonEmployment.start_date,
                                    _busPersonEmploymentHistoryTemp.icdoPersonEmployment.end_date
                                    )
                            )
                        )
                    )
                {
                    lblnRecordMatch = true;
                    break;
                }
            }
            return lblnRecordMatch;
        }

        public bool ValidatePersonEnrollmentAge()
        {
            bool iblnIsAgeLessThan18 = false;
            if (ibusPerson.IsNull())
                LoadPerson();
            if (busGlobalFunctions.CalulateAge(ibusPerson.icdoPerson.date_of_birth, icdoPersonEmployment.start_date) < 18)
            {
                iblnIsAgeLessThan18 = true;
            }
            return iblnIsAgeLessThan18;
        }

        public bool CheckDetailExists()
        {
            DataTable ldtbPAEmpDtl = Select<cdoPersonEmploymentDetail>(new string[1] { "person_employment_id" },
                                            new object[1] { icdoPersonEmployment.person_employment_id }, null, null);
            if (ldtbPAEmpDtl.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            
            if (!String.IsNullOrEmpty(icdoPersonEmployment.istrOrgCodeID))
            {
                ibusOrganization = new busOrganization();
                ibusOrganization.FindOrganizationByOrgCode(icdoPersonEmployment.istrOrgCodeID);
                icdoPersonEmployment.org_id = ibusOrganization.icdoOrganization.org_id;
            }

            if (ibusPerson == null)
                LoadPerson();

            if (!CheckDetailExists())
                icdoPersonEmployment.start_date = icdoPersonEmployment.new_start_date;

            base.BeforeValidate(aenmPageMode);
        }

        public override void BeforePersistChanges()
        {

            if (icdoPersonEmployment.end_date != DateTime.MinValue)
            {
                if(icdoPersonEmployment.start_date.Month == icdoPersonEmployment.end_date.Month && 
                    icdoPersonEmployment.start_date.Year == icdoPersonEmployment.end_date.Year)
                    iblnIsHireDateSameMonthAsTerminationDate = true;
                UpdateTerminateEmployment();

                if (iblnIsHireDateSameMonthAsTerminationDate)
                    istrMessageToShowAfterPostClick = busGlobalFunctions.GetMessageTextByMessageID(10505, iobjPassInfo);
            }
           
            //PIR 9803
            if(ibusPerson.icolPersonEmployment == null)
            {
                ibusPerson.LoadPersonEmployment();
            }
            if(icdoPersonEmployment.person_employment_id == 0)
            {
                ibusPerson.icdoPerson.welcome_batch_letter_sent_flag = busConstant.Flag_No;
                ibusPerson.icdoPerson.Update();
            }

            //if (ibusPerson.icolPersonEmployment.Count >= 1)
            //{
            //    foreach (busPersonEmployment lobjPersonEmployment in ibusPerson.icolPersonEmployment)
            //    {
            //        //PIR 19003
            //        if (lobjPersonEmployment.icdoPersonEmployment.end_date != DateTime.MinValue && string.IsNullOrEmpty(ibusPerson.icdoPerson.welcome_batch_letter_sent_flag))
            //        {
            //            ibusPerson.icdoPerson.welcome_batch_letter_sent_flag = busConstant.Flag_No;
            //            ibusPerson.icdoPerson.Update();
            //        }
            //    }
                
            //}
            //PIR 8852 - Update SGT_WSS_EMPLOYMENT_CHANGE_REQUEST table with date_of_last_regular_paycheck
            //if (lobjWssEmpChReq.icdoWssEmploymentChangeRequest.employment_change_request_id > 0)
            //{
            //    lobjWssEmpChReq.icdoWssEmploymentChangeRequest.date_of_last_regular_paycheck = icdoPersonEmployment.new_date_of_last_paycheck;
            //    lobjWssEmpChReq.icdoWssEmploymentChangeRequest.Update();
            //}

            //for UAT PIR - 2043 get the previous org code id
            if (icdoPersonEmployment.ihstOldValues.Count > 0)
            {
                iintPreviousOrgId = Convert.ToInt32(icdoPersonEmployment.ihstOldValues["org_id"].ToString());
            }
            base.BeforePersistChanges();
        }

        public bool iblnIsHireDateSameMonthAsTerminationDate { get; set; }
        public void UpdateTerminateEmployment(bool aobjFromDeathNotification = false)
        {
            // Updating the Employment detail with Employment End Date
            if (icolPersonEmploymentDetail == null)
                LoadPersonEmploymentDetail();
            bool lblnDeferredCompFlagSet = false;
            foreach (busPersonEmploymentDetail lobjEmploymentDetail in icolPersonEmploymentDetail)
            {
                if (lobjEmploymentDetail.icdoPersonEmploymentDetail.end_date == DateTime.MinValue)
                {
                    lobjEmploymentDetail.icdoPersonEmploymentDetail.end_date = icdoPersonEmployment.end_date;
                    lobjEmploymentDetail.icdoPersonEmploymentDetail.Update();
                }

                /* SYSTEST-PIR-ID-1603 */
                if (lobjEmploymentDetail.iclbAllPersonAccountEmpDtl.IsNull())
                    lobjEmploymentDetail.LoadAllPersonAccountEmploymentDetails();
                foreach (busPersonAccountEmploymentDetail lobjPAEmpDtl in lobjEmploymentDetail.iclbAllPersonAccountEmpDtl)
                {
                    /* SYSTEST-PIR-ID-1603 */
                    if (!lblnDeferredCompFlagSet)
                    {
                        if ((lobjPAEmpDtl.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDeferredCompensation) &&
                            (lobjPAEmpDtl.icdoPersonAccountEmploymentDetail.election_value == busConstant.PersonAccountElectionValueEnrolled) &&
                            (lobjPAEmpDtl.icdoPersonAccountEmploymentDetail.person_account_id > 0))
                        {
                            if (lobjPAEmpDtl.ibusPersonAccount.IsNull())
                                lobjPAEmpDtl.LoadPersonAccount();
                            busPersonAccountDeferredComp lobjDefComp = new busPersonAccountDeferredComp
                            {
                                icdoPersonAccount = new cdoPersonAccount(),
                                icdoPersonAccountDeferredComp = new cdoPersonAccountDeferredComp()
                            };
                            lobjDefComp.icdoPersonAccount = lobjPAEmpDtl.ibusPersonAccount.icdoPersonAccount;
                            if (lobjDefComp.FindPersonAccountDeferredComp(lobjDefComp.icdoPersonAccount.person_account_id))
                            {
                                if (lobjDefComp.icdoPersonAccountDeferredComp.file_457_sent_flag != busConstant.Flag_No)
                                {
                                    lobjDefComp.icdoPersonAccountDeferredComp.file_457_sent_flag = busConstant.Flag_No;
                                    lobjDefComp.icdoPersonAccountDeferredComp.Update();
                                    lblnDeferredCompFlagSet = true;
                                }
                            }
                        }
                    }
                }
            }

            // Status will be checked while generating Termination Letter generation batch.4
            //PIR 19003 - A termination date should not re-generate if an employment end date is updated.
            DateTime ldtePrevEmpEndDate = DateTime.MinValue;
            if (icdoPersonEmployment.ihstOldValues.Count > 0)
                ldtePrevEmpEndDate = Convert.ToDateTime(icdoPersonEmployment.
                                                            ihstOldValues[enmPersonEmployment.end_date.ToString()]);
            if (ldtePrevEmpEndDate == DateTime.MinValue)
                icdoPersonEmployment.termination_letter_status_value = busConstant.TerminationLetterStatusNotSent;
            //prod pir 4251 : busPerson was not getting loaded when Post button was clicked from ESS
			if (ibusPerson == null)
                LoadPerson();	
            if (ibusPerson.icolPersonAccount == null)
                ibusPerson.LoadPersonAccount();

                foreach (busPersonAccount lbusPersonAccount in ibusPerson.icolPersonAccount)
                {
                    if (lbusPersonAccount.ibusPlan == null)
                        lbusPersonAccount.LoadPlan();
                    if ((lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth ||
                        lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental ||
                        lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision) && 
                        (lbusPersonAccount.ibusPersonAccountGHDV == null))
                        lbusPersonAccount.LoadPersonAccountGHDV();
                    if (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdFlex && lbusPersonAccount.ibusPersonAccountFlex == null)
                        lbusPersonAccount.LoadPersonAccountFlex();
                    if (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife && lbusPersonAccount.ibusPersonAccountLife == null)
                        lbusPersonAccount.LoadPersonAccountLife();

                if (lbusPersonAccount.icdoPersonAccount.history_change_date < icdoPersonEmployment.end_date.GetFirstDayofNextMonth() &&
                    lbusPersonAccount.ibusPlan.IsRetirementPlan() &&
                   lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled)
                {
                    if (!IsOtherEmploymentOpenForPersonAccount(lbusPersonAccount))
                    {
                        busPersonAccountRetirement lobjPARetirement = new busPersonAccountRetirement
                        {
                            icdoPersonAccount = lbusPersonAccount.icdoPersonAccount,
                            icdoPersonAccountRetirement = new cdoPersonAccountRetirement()
                        };
                        if (lobjPARetirement.FindPersonAccountRetirement(lobjPARetirement.icdoPersonAccount.person_account_id))
                        {
                            lobjPARetirement.ibusPerson = ibusPerson;
                            lobjPARetirement.ibusPlan = lbusPersonAccount.ibusPlan;
                            lobjPARetirement.LoadPlanEffectiveDate();
                            lobjPARetirement.icdoPersonAccount.person_employment_dtl_id = lobjPARetirement.GetEmploymentDetailID();
                            if (lobjPARetirement.icdoPersonAccount.person_employment_dtl_id == 0)
                                lobjPARetirement.icdoPersonAccount.person_employment_dtl_id = lobjPARetirement.GetEmploymentDetailID(lbusPersonAccount.ibusPlan.icdoPlan.plan_id, lbusPersonAccount.icdoPersonAccount.person_account_id, icdoPersonEmployment.end_date);
                            lobjPARetirement.LoadPersonEmploymentDetail();
                            lobjPARetirement.ibusPersonEmploymentDetail.LoadPersonEmployment();
                            lobjPARetirement.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                            lobjPARetirement.LoadOrgPlan();

                            // Change the status to Suspended.
                            lobjPARetirement.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusRetimentSuspended;
                            //PIR 26517
                            //if (iblnIsHireDateSameMonthAsTerminationDate && !IsInsurancePlanHadPriorHistorytoTerminationDate(lbusPersonAccount, icdoPersonEmployment.end_date))
                            //    lobjPARetirement.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusRetirmentCancelled;

                            lobjPARetirement.icdoPersonAccount.history_change_date = icdoPersonEmployment.end_date.GetFirstDayofNextMonth();
                            lobjPARetirement.icdoPersonAccount.ienuObjectState = ObjectState.Update;
                            lobjPARetirement.icdoPersonAccount.reason_id = 332;
                            //lobjPARetirement.icdoPersonAccount.reason_value = busConstant.BCBSEmploymentTermination;
                            //PIR 25920 carry forward Additional contribution percent while termination
                            if (lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDC2025 && lobjPARetirement.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                                lobjPARetirement.iintAddlEEContributionPercent = lobjPARetirement.icdoPersonAccount.addl_ee_contribution_percent_temp;
                            else
                                lobjPARetirement.iintAddlEEContributionPercent = lobjPARetirement.icdoPersonAccount.addl_ee_contribution_percent;

                            // Calling the Save operation, that will insert the History.
                            lobjPARetirement.iarrChangeLog.Add(lobjPARetirement.icdoPersonAccount);
                            lobjPARetirement.iarrChangeLog.Add(lobjPARetirement.icdoPersonAccountRetirement);
                            lobjPARetirement.BeforeValidate(utlPageMode.All);
                            lobjPARetirement.BeforePersistChanges();
                            lobjPARetirement.PersistChanges();
                            lobjPARetirement.icdoPersonAccount.iblnIsEmploymentEnded = true;//PIR 20259
                            lobjPARetirement.AfterPersistChanges();
                            busGlobalFunctions.PostESSMessage(icdoPersonEmployment.org_id, lbusPersonAccount.ibusPlan.icdoPlan.plan_id, iobjPassInfo); // PIR 9115 
                        }
                    }
                }
                else if (((!iblnIsHireDateSameMonthAsTerminationDate) &&
                        (lbusPersonAccount.icdoPersonAccount.history_change_date < (idtmSuspendEffectiveDateDefaultInsPlans != DateTime.MinValue ? idtmSuspendEffectiveDateDefaultInsPlans : icdoPersonEmployment.end_date.GetFirstDayofNextMonth().AddMonths(1))) &&
                        (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdEAP) &&
                        (lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled))
                        || (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdEAP && IsEmploymentStartNEndateInSameMonthConditions(lbusPersonAccount)))
                {
                    if (!IsOtherEmploymentOpenForPersonAccount(lbusPersonAccount))
                    {
                        /* PIR-ID 1709 */
                        busPersonAccountEAP lobjPAEAP = new busPersonAccountEAP { icdoPersonAccount = new cdoPersonAccount() };
                        lobjPAEAP.icdoPersonAccount = lbusPersonAccount.icdoPersonAccount;
                        lobjPAEAP.ibusPlan = lbusPersonAccount.ibusPlan;
                        lobjPAEAP.ibusPerson = ibusPerson;
                        lobjPAEAP.LoadPlanEffectiveDate();
                        lobjPAEAP.icdoPersonAccount.person_employment_dtl_id = lobjPAEAP.GetEmploymentDetailID();
                        lobjPAEAP.LoadPersonEmploymentDetail();
                        lobjPAEAP.ibusPersonEmploymentDetail.LoadPersonEmployment();
                        lobjPAEAP.LoadOrgPlan();

                        // Change the status to Suspended.
                        lobjPAEAP.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceSuspended;
                        lobjPAEAP.icdoPersonAccount.history_change_date = iblnIsHireDateSameMonthAsTerminationDate ? (idtelastDateOfService != DateTime.MinValue ? idtelastDateOfService.GetFirstDayofNextMonth() : icdoPersonEmployment.end_date.GetFirstDayofNextMonth()) :                            
                                                                          idtelastDateOfService != DateTime.MinValue ? idtelastDateOfService.GetFirstDayofNextMonth().AddMonths(1) :
                                                                          icdoPersonEmployment.end_date.GetFirstDayofNextMonth().AddMonths(1);
                        //PIR 26517
                        if(iblnIsHireDateSameMonthAsTerminationDate && !IsInsurancePlanHadPriorHistorytoTerminationDate(lbusPersonAccount, icdoPersonEmployment.end_date))
                            lobjPAEAP.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceCancelled;

                        lobjPAEAP.icdoPersonAccount.ienuObjectState = ObjectState.Update;
                        lobjPAEAP.icdoPersonAccount.reason_id = 332;
                       //lobjPAEAP.icdoPersonAccount.reason_value = busConstant.BCBSEmploymentTermination;

                            // Calling the Save operation, that will insert the History.
                            lobjPAEAP.iarrChangeLog.Add(lobjPAEAP.icdoPersonAccount);
                            lobjPAEAP.BeforeValidate(utlPageMode.All);
                            lobjPAEAP.BeforePersistChanges();
                            lobjPAEAP.PersistChanges();
                            lobjPAEAP.icdoPersonAccount.iblnIsEmploymentEnded = true; //PIR 20259
                            lobjPAEAP.AfterPersistChanges();
                            busGlobalFunctions.PostESSMessage(icdoPersonEmployment.org_id, busConstant.PlanIdEAP, iobjPassInfo); // PIR 9115
                        }
                    }
                    else if (((!iblnIsHireDateSameMonthAsTerminationDate) &&
                            (lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled) &&
                            (lbusPersonAccount.icdoPersonAccount.history_change_date < (idtmSuspendEffectiveDateDefaultInsPlans != DateTime.MinValue ? idtmSuspendEffectiveDateDefaultInsPlans : icdoPersonEmployment.end_date.GetFirstDayofNextMonth().AddMonths(1))) &&
                            (((lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth) &&
                            ((lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeNonState || lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeState) &&
                            (lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value.IsNullOrEmpty()))) ||
                            ((lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental) &&
                            (lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeActive)) ||
                            ((lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision) &&
                            (lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.vision_insurance_type_value == busConstant.VisionInsuranceTypeActive)))) 
                            || ((lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth || lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental || lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision)
                            && IsEmploymentStartNEndateInSameMonthConditions(lbusPersonAccount)))
                    {
                        if (!IsOtherEmploymentOpenForPersonAccount(lbusPersonAccount))
                        {
                            busPersonAccountGhdv lobjPAGHDV = new busPersonAccountGhdv { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };
                            lobjPAGHDV.icdoPersonAccount = lbusPersonAccount.icdoPersonAccount;
                            lobjPAGHDV.ibusPlan = lbusPersonAccount.ibusPlan;
                            lobjPAGHDV.ibusPerson = ibusPerson;
                            lobjPAGHDV.LoadPlanEffectiveDate();
                            lobjPAGHDV.icdoPersonAccount.person_employment_dtl_id = lobjPAGHDV.GetEmploymentDetailID();
                            lobjPAGHDV.LoadPersonEmploymentDetail();
                            lobjPAGHDV.ibusPersonEmploymentDetail.LoadPersonEmployment();
                            lobjPAGHDV.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                            lobjPAGHDV.LoadOrgPlan();
                            lobjPAGHDV.FindGHDVByPersonAccountID(lobjPAGHDV.icdoPersonAccount.person_account_id);

                        // Change the status to Suspended.
                        lobjPAGHDV.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceSuspended;
                        lobjPAGHDV.icdoPersonAccount.history_change_date = iblnIsHireDateSameMonthAsTerminationDate ? (idtelastDateOfService != DateTime.MinValue ? idtelastDateOfService.GetFirstDayofNextMonth() : icdoPersonEmployment.end_date.GetFirstDayofNextMonth()) :
                                                                            idtelastDateOfService != DateTime.MinValue ? idtelastDateOfService.GetFirstDayofNextMonth().AddMonths(1) :
                                                                            icdoPersonEmployment.end_date.GetFirstDayofNextMonth().AddMonths(1);

                        //PIR 26517
                        if (iblnIsHireDateSameMonthAsTerminationDate && !IsInsurancePlanHadPriorHistorytoTerminationDate(lbusPersonAccount, icdoPersonEmployment.end_date))
                            lobjPAGHDV.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceCancelled;

                        lobjPAGHDV.icdoPersonAccount.ienuObjectState = ObjectState.Update;
                        lobjPAGHDV.icdoPersonAccountGhdv.ienuObjectState = ObjectState.Update;
                        lobjPAGHDV.icdoPersonAccount.reason_id = 332;
                        lobjPAGHDV.icdoPersonAccount.reason_value = busConstant.BCBSEmploymentTermination;
                        lobjPAGHDV.icdoPersonAccountGhdv.reason_id = 332;
                        lobjPAGHDV.icdoPersonAccountGhdv.reason_value = busConstant.BCBSEmploymentTermination;

                        // Calling the Save operation, that will insert the History.
                        lobjPAGHDV.iarrChangeLog.Add(lobjPAGHDV.icdoPersonAccount);
                        lobjPAGHDV.iarrChangeLog.Add(lobjPAGHDV.icdoPersonAccountGhdv);
                        lobjPAGHDV.iblnIsFromTerminationPost = true;
                        lobjPAGHDV.BeforeValidate(utlPageMode.Update);
                        lobjPAGHDV.BeforePersistChanges();
                        lobjPAGHDV.PersistChanges();
                        lobjPAGHDV.icdoPersonAccount.iblnIsEmploymentEnded = true; //PIR 20259
                        lobjPAGHDV.AfterPersistChanges();
                        busGlobalFunctions.PostESSMessage(icdoPersonEmployment.org_id, lbusPersonAccount.icdoPersonAccount.plan_id, iobjPassInfo); // PIR 9115
                    }
                }
                else if (((!iblnIsHireDateSameMonthAsTerminationDate) &&
                    (lbusPersonAccount.icdoPersonAccount.history_change_date < (idtmSuspendEffectiveDateDefaultInsPlans != DateTime.MinValue ? idtmSuspendEffectiveDateDefaultInsPlans : icdoPersonEmployment.end_date.GetFirstDayofNextMonth().AddMonths(1))) &&
                    lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife && 
                    lbusPersonAccount.ibusPersonAccountLife.icdoPersonAccountLife.life_insurance_type_value != busConstant.LifeInsuranceTypeRetireeMember &&
                    lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled) 
                    || (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife && IsEmploymentStartNEndateInSameMonthConditions(lbusPersonAccount)))
                {
                    if (!IsOtherEmploymentOpenForPersonAccount(lbusPersonAccount))
                    {
                        busPersonAccountLife lobjPALife = new busPersonAccountLife { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountLife = new cdoPersonAccountLife() };
                        lobjPALife.icdoPersonAccount = lbusPersonAccount.icdoPersonAccount;
                        lobjPALife.ibusPerson = ibusPerson;
                        lobjPALife.ibusPlan = lbusPersonAccount.ibusPlan;
                        lobjPALife.LoadPlanEffectiveDate();
                        lobjPALife.icdoPersonAccount.person_employment_dtl_id = lobjPALife.GetEmploymentDetailID();
                        lobjPALife.LoadPersonEmploymentDetail();
                        lobjPALife.ibusPersonEmploymentDetail.LoadPersonEmployment();
                        lobjPALife.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                        lobjPALife.LoadOrgPlan();
                        lobjPALife.LoadLifeOptionData();
                        lobjPALife.LoadPaymentElection();
                        lobjPALife.LoadProviderOrgPlan();
                        lobjPALife.FindPersonAccountLife(lobjPALife.icdoPersonAccount.person_account_id);

                        // Change the status to Suspended.
                        lobjPALife.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceSuspended;
                        lobjPALife.icdoPersonAccount.history_change_date = iblnIsHireDateSameMonthAsTerminationDate ? (idtelastDateOfService != DateTime.MinValue ? idtelastDateOfService.GetFirstDayofNextMonth() : icdoPersonEmployment.end_date.GetFirstDayofNextMonth()) :
                                                                           idtelastDateOfService != DateTime.MinValue ? idtelastDateOfService.GetFirstDayofNextMonth().AddMonths(1) :
                                                                           icdoPersonEmployment.end_date.GetFirstDayofNextMonth().AddMonths(1);
                        //PIR 26517
                        if (iblnIsHireDateSameMonthAsTerminationDate && !IsInsurancePlanHadPriorHistorytoTerminationDate(lbusPersonAccount, icdoPersonEmployment.end_date))
                            lobjPALife.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceCancelled;

                        lobjPALife.icdoPersonAccount.ienuObjectState = ObjectState.Update;
                        lobjPALife.icdoPersonAccountLife.ienuObjectState = ObjectState.Update;

                        lobjPALife.icdoPersonAccount.reason_id = 332;
                        lobjPALife.icdoPersonAccount.reason_value = busConstant.BCBSEmploymentTermination;
                        lobjPALife.icdoPersonAccountLife.reason_id = 332;
                        lobjPALife.icdoPersonAccountLife.reason_value = busConstant.BCBSEmploymentTermination;

                        // Calling the Save operation, that will insert the History.
                        lobjPALife.iarrChangeLog.Add(lobjPALife.icdoPersonAccount);
                        lobjPALife.iarrChangeLog.Add(lobjPALife.icdoPersonAccountLife);
                        lobjPALife.BeforeValidate(utlPageMode.Update);
                        lobjPALife.BeforePersistChanges();
                        lobjPALife.PersistChanges();
                        lobjPALife.icdoPersonAccount.iblnIsEmploymentEnded = true; //PIR 20259
                        lobjPALife.AfterPersistChanges();
                        busGlobalFunctions.PostESSMessage(icdoPersonEmployment.org_id, busConstant.PlanIdGroupLife, iobjPassInfo); // PIR 9115
                    }
                }
                else if (((!iblnIsHireDateSameMonthAsTerminationDate) &&
                        (lbusPersonAccount.icdoPersonAccount.history_change_date < (idtmSuspendEffectiveDateDCandFlex != DateTime.MinValue ? idtmSuspendEffectiveDateDCandFlex : icdoPersonEmployment.end_date.GetFirstDayofNextMonth())) &&
                          (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDeferredCompensation || lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdOther457 ) && 
                          (lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompEnrolled))
                          || ((lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDeferredCompensation || lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdOther457) && IsEmploymentStartNEndateInSameMonthConditions(lbusPersonAccount)))
                {
                    if (!IsOtherEmploymentOpenForPersonAccount(lbusPersonAccount))
                    {
                        busPersonAccountDeferredComp lobjPADefComp = new busPersonAccountDeferredComp { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountDeferredComp = new cdoPersonAccountDeferredComp() };
                        lobjPADefComp.icdoPersonAccount = lbusPersonAccount.icdoPersonAccount;
                        lobjPADefComp.ibusPerson = ibusPerson;
                        lobjPADefComp.ibusPlan = lbusPersonAccount.ibusPlan;
                        lobjPADefComp.LoadPlanEffectiveDate();
                        lobjPADefComp.icdoPersonAccount.person_employment_dtl_id = lobjPADefComp.GetEmploymentDetailID();
                        lobjPADefComp.LoadPersonEmploymentDetail();
                        lobjPADefComp.ibusPersonEmploymentDetail.LoadPersonEmployment();
                        lobjPADefComp.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                        lobjPADefComp.LoadOrgPlan();
                        lobjPADefComp.FindPersonAccountDeferredComp(lobjPADefComp.icdoPersonAccount.person_account_id);

                        // Change the status to Suspended.
                        lobjPADefComp.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusDefCompSuspended;

                        //PIR 26517
                        if (iblnIsHireDateSameMonthAsTerminationDate && !IsInsurancePlanHadPriorHistorytoTerminationDate(lbusPersonAccount, icdoPersonEmployment.end_date))
                            lobjPADefComp.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusDefCompCancelled;

                        lobjPADefComp.icdoPersonAccount.history_change_date = idtmSuspendEffectiveDateDCandFlex != DateTime.MinValue ? idtmSuspendEffectiveDateDCandFlex : icdoPersonEmployment.end_date.GetFirstDayofNextMonth();
                        lobjPADefComp.icdoPersonAccount.ienuObjectState = ObjectState.Update;
                        lobjPADefComp.icdoPersonAccountDeferredComp.ienuObjectState = ObjectState.Update;
                        lobjPADefComp.icdoPersonAccount.reason_id = 332;
                        lobjPADefComp.icdoPersonAccount.reason_value = busConstant.BCBSEmploymentTermination;

                        // Calling the Save operation, that will insert the History.
                        lobjPADefComp.iarrChangeLog.Add(lobjPADefComp.icdoPersonAccount);
                        lobjPADefComp.iarrChangeLog.Add(lobjPADefComp.icdoPersonAccountDeferredComp);
                        lobjPADefComp.iblnIsFromTerminationPost = true;
                        //PIR 27241
                        if(aobjFromDeathNotification)
                            lobjPADefComp.idtmProviderEndDateWhenTEEM = idtmProviderEndDateFromDeathNotif != DateTime.MinValue ? idtmProviderEndDateFromDeathNotif : icdoPersonEmployment.end_date;
                        else
                            lobjPADefComp.idtmProviderEndDateWhenTEEM = idtmDefCompProviderEndDateWhenTEEM != DateTime.MinValue ? idtmDefCompProviderEndDateWhenTEEM : icdoPersonEmployment.end_date;
                        lobjPADefComp.BeforeValidate(utlPageMode.Update);
                        lobjPADefComp.BeforePersistChanges();
                        lobjPADefComp.PersistChanges();
                        lobjPADefComp.icdoPersonAccount.iblnIsEmploymentEnded = true; //PIR 20259
                        lobjPADefComp.AfterPersistChanges();
                        busGlobalFunctions.PostESSMessage(icdoPersonEmployment.org_id, busConstant.PlanIdDeferredCompensation, iobjPassInfo); // PIR 9115
                    }
                }
                else if (((!iblnIsHireDateSameMonthAsTerminationDate) &&
                        (lbusPersonAccount.icdoPersonAccount.history_change_date < (idtmSuspendEffectiveDateDCandFlex != DateTime.MinValue ? idtmSuspendEffectiveDateDCandFlex : icdoPersonEmployment.end_date.GetFirstDayofNextMonth())) &&
                        (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdFlex) &&
                        (lbusPersonAccount.ibusPersonAccountFlex.icdoPersonAccountFlexComp.flex_comp_type_value != busConstant.FlexCompTypeValueCOBRA) &&
                        lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled) || 
                        (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdFlex  && IsEmploymentStartNEndateInSameMonthConditions(lbusPersonAccount)))
                {
                    if (!IsOtherEmploymentOpenForPersonAccount(lbusPersonAccount))
                    {
                        busPersonAccountFlexComp lobjPAFlex = new busPersonAccountFlexComp { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountFlexComp = new cdoPersonAccountFlexComp() };
                        lobjPAFlex.icdoPersonAccount = lbusPersonAccount.icdoPersonAccount;
                        lobjPAFlex.ibusPerson = ibusPerson;
                        lobjPAFlex.ibusPlan = lbusPersonAccount.ibusPlan;
                        lobjPAFlex.LoadPlanEffectiveDate();
                        lobjPAFlex.icdoPersonAccount.person_employment_dtl_id = lobjPAFlex.GetEmploymentDetailID();
                        lobjPAFlex.LoadPersonEmploymentDetail();
                        lobjPAFlex.ibusPersonEmploymentDetail.LoadPersonEmployment();
                        lobjPAFlex.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                        lobjPAFlex.LoadOrgPlan();
                        lobjPAFlex.FindPersonAccountFlexComp(lobjPAFlex.icdoPersonAccount.person_account_id);
                        lobjPAFlex.LoadFlexCompOptionUpdate();

                        // Change the status to Suspended.
                        lobjPAFlex.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusFlexSuspended;
                        lobjPAFlex.icdoPersonAccount.history_change_date = iblnIsHireDateSameMonthAsTerminationDate ? (idtmSuspendEffectiveDateDCandFlex != DateTime.MinValue ? idtmSuspendEffectiveDateDCandFlex.GetFirstDayofNextMonth() : icdoPersonEmployment.end_date.GetFirstDayofNextMonth()) :
                                                                           idtmSuspendEffectiveDateDCandFlex != DateTime.MinValue ? idtmSuspendEffectiveDateDCandFlex :
                                                                           icdoPersonEmployment.end_date.GetFirstDayofNextMonth();

                        //PIR 26517
                        if (iblnIsHireDateSameMonthAsTerminationDate && !IsInsurancePlanHadPriorHistorytoTerminationDate(lbusPersonAccount, icdoPersonEmployment.end_date))
                            lobjPAFlex.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceCancelled;

                        lobjPAFlex.icdoPersonAccount.ienuObjectState = ObjectState.Update;
                        lobjPAFlex.icdoPersonAccountFlexComp.ienuObjectState = ObjectState.Update;
                        lobjPAFlex.icdoPersonAccount.reason_id = 332;
                        lobjPAFlex.icdoPersonAccount.reason_value = busConstant.BCBSEmploymentTermination;
                        lobjPAFlex.icdoPersonAccountFlexComp.reason_id = 332;
                        lobjPAFlex.icdoPersonAccountFlexComp.reason_value = busConstant.BCBSEmploymentTermination;

                        // Calling the Save operation, that will insert the History.
                        lobjPAFlex.iarrChangeLog.Add(lobjPAFlex.icdoPersonAccount);
                        lobjPAFlex.iarrChangeLog.Add(lobjPAFlex.icdoPersonAccountFlexComp);
                        lobjPAFlex.iblnIsFromTerminationPost = true;
                        lobjPAFlex.BeforeValidate(utlPageMode.Update);
                        lobjPAFlex.BeforePersistChanges();
                        lobjPAFlex.PersistChanges();
                        lobjPAFlex.icdoPersonAccount.iblnIsEmploymentEnded = true; //PIR 20259
                        lobjPAFlex.AfterPersistChanges();
                        busGlobalFunctions.PostESSMessage(icdoPersonEmployment.org_id, busConstant.PlanIdFlex, iobjPassInfo); // PIR 9115
                    }
                }
            }
        }

        public bool IsEmploymentStartNEndateInSameMonthConditions(busPersonAccount abusPersonAccount)
        {
            bool lblnResult = false;
            if (iblnIsHireDateSameMonthAsTerminationDate && 
                ((abusPersonAccount.icdoPersonAccount.plan_id != busConstant.PlanIdFlex && abusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                || (abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdFlex && abusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled)
                || ((abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDeferredCompensation || abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdOther457) && abusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompEnrolled)))
            {
                if ((abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental) ||
                    (abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision) ||
                    (abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth))
                {
                    if (abusPersonAccount.ibusPersonAccountGHDV.IsNull())
                        abusPersonAccount.LoadPersonAccountGHDV();
                    if (abusPersonAccount.ibusPersonAccountGHDV.IsNotNull() && abusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.person_account_ghdv_id > 0 && abusPersonAccount.ibusPersonAccountGHDV.iclbPersonAccountGHDVHistory.IsNull())
                        abusPersonAccount.ibusPersonAccountGHDV.LoadPersonAccountGHDVHistory();
                    if((abusPersonAccount.ibusPersonAccountGHDV.iclbPersonAccountGHDVHistory.Count == 1) ||
                       (abusPersonAccount.ibusPersonAccountGHDV.iclbPersonAccountGHDVHistory.Count > 1 && 
                       abusPersonAccount.ibusPersonAccountGHDV.iclbPersonAccountGHDVHistory[1].icdoPersonAccountGhdvHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended))
                    {
                        lblnResult = true;
                    }                    
                }
                else if(abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdEAP)
                {
                    if (abusPersonAccount.ibusPersonAccountEAP.IsNull())
                        abusPersonAccount.LoadPersonAccountEAP();
                    if (abusPersonAccount.ibusPersonAccountEAP.IsNotNull() && abusPersonAccount.ibusPersonAccountEAP.iclbEAPHistory.IsNull())
                        abusPersonAccount.ibusPersonAccountEAP.LoadEAPHistory(false);
                    if ((abusPersonAccount.ibusPersonAccountEAP.iclbEAPHistory.Count == 1) || 
                        (abusPersonAccount.ibusPersonAccountEAP.iclbEAPHistory.Count > 1 
                        && abusPersonAccount.ibusPersonAccountEAP.iclbEAPHistory[1].icdoPersonAccountEapHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended))
                    {
                        lblnResult = true;
                    }
                }
                else if (abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife)
                {
                    if (abusPersonAccount.ibusPersonAccountLife.IsNull())
                        abusPersonAccount.LoadPersonAccountLife();
                    if (abusPersonAccount.ibusPersonAccountLife.IsNotNull() && abusPersonAccount.ibusPersonAccountLife.iclbPersonAccountLifeHistory.IsNull())
                        abusPersonAccount.ibusPersonAccountLife.LoadHistory();
                    
                    busPersonAccountLifeHistory lbusPersonAccountEndedLifeHistory = abusPersonAccount.ibusPersonAccountLife.iclbPersonAccountLifeHistory.Where(i => i.icdoPersonAccountLifeHistory.effective_end_date != DateTime.MinValue).OrderByDescending(i => i.icdoPersonAccountLifeHistory.person_account_life_history_id).FirstOrDefault();
                    if ((lbusPersonAccountEndedLifeHistory.IsNull()) || (lbusPersonAccountEndedLifeHistory.IsNotNull() &&
                        lbusPersonAccountEndedLifeHistory.icdoPersonAccountLifeHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended))
                    {
                        lblnResult = true;
                    }
                }
                else if (abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdFlex)
                {
                    busPersonAccountFlexComp lbusPersonAccountFlexComp = new busPersonAccountFlexComp() { icdoPersonAccount = new cdoPersonAccount() };
                    lbusPersonAccountFlexComp.icdoPersonAccount = abusPersonAccount.icdoPersonAccount;
                    if (lbusPersonAccountFlexComp.iclbModifiedHistory.IsNull())
                        lbusPersonAccountFlexComp.LoadModifiedFlexCompHistory();
                    if ((lbusPersonAccountFlexComp.iclbModifiedHistory.Count == 1 ) ||
                        (lbusPersonAccountFlexComp.iclbModifiedHistory.Count > 1
                        && lbusPersonAccountFlexComp.iclbModifiedHistory[1].icdoPersonAccountFlexCompHistory.plan_participation_status_value == busConstant.PlanParticipationStatusFlexSuspended))
                    {
                        lblnResult = true;
                    }
                }
                else if ((abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDeferredCompensation || abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdOther457))
                {
                    if (abusPersonAccount.ibusPersonDeferredComp.IsNull())
                    {
                        abusPersonAccount.ibusPersonDeferredComp = new busPersonAccountDeferredComp();
                        abusPersonAccount.ibusPersonDeferredComp.FindPersonAccountDeferredComp(abusPersonAccount.icdoPersonAccount.person_account_id);
                    }
                    if (abusPersonAccount.ibusPersonDeferredComp.icolPADeferredCompHistory.IsNull())
                        abusPersonAccount.ibusPersonDeferredComp.LoadPADeffCompHistory();
                    if ((abusPersonAccount.ibusPersonDeferredComp.icolPADeferredCompHistory.Count == 1) ||
                        (abusPersonAccount.ibusPersonDeferredComp.icolPADeferredCompHistory.Count > 1
                        && abusPersonAccount.ibusPersonDeferredComp.icolPADeferredCompHistory[1].icdoPersonAccountDeferredCompHistory.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompSuspended))
                    {
                        lblnResult = true;
                    }
                }
            }
            return lblnResult;
        }

        public bool IsInsurancePlanHadPriorHistorytoTerminationDate(busPersonAccount abusPersonAccount, DateTime adtTerminationDate)
        {
            if ((abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental) ||
                (abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision) ||
                (abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth))
            {
                if (abusPersonAccount.ibusPersonAccountGHDV.IsNull())
                    abusPersonAccount.LoadPersonAccountGHDV();
                if (abusPersonAccount.ibusPersonAccountGHDV.IsNotNull() && abusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.person_account_ghdv_id > 0 && abusPersonAccount.ibusPersonAccountGHDV.iclbPersonAccountGHDVHistory.IsNull())
                    abusPersonAccount.ibusPersonAccountGHDV.LoadPersonAccountGHDVHistory();
                return abusPersonAccount.ibusPersonAccountGHDV.iclbPersonAccountGHDVHistory.Any(i => i.icdoPersonAccountGhdvHistory.start_date < adtTerminationDate);
            }
            else if (abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdEAP)
            {
                if (abusPersonAccount.ibusPersonAccountEAP.IsNull())
                    abusPersonAccount.LoadPersonAccountEAP();
                if (abusPersonAccount.ibusPersonAccountEAP.IsNotNull() && abusPersonAccount.ibusPersonAccountEAP.iclbEAPHistory.IsNull())
                    abusPersonAccount.ibusPersonAccountEAP.LoadEAPHistory(false);
                return abusPersonAccount.ibusPersonAccountEAP.iclbEAPHistory.Any(i=> i.icdoPersonAccountEapHistory.start_date < adtTerminationDate);
            }
            else if (abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife)
            {
                if (abusPersonAccount.ibusPersonAccountLife.IsNull())
                    abusPersonAccount.LoadPersonAccountLife();
                if (abusPersonAccount.ibusPersonAccountLife.iclbPersonAccountLifeHistory == null)
                    abusPersonAccount.ibusPersonAccountLife.LoadHistory();
                return abusPersonAccount.ibusPersonAccountLife.iclbPersonAccountLifeHistory.Any(i => i.icdoPersonAccountLifeHistory.effective_start_date < adtTerminationDate);
            }
            else if (abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdFlex)
            {
                if (abusPersonAccount.ibusPersonAccountFlex.IsNull())
                    abusPersonAccount.LoadPersonAccountFlex();
                if (abusPersonAccount.ibusPersonAccountFlex.iclbFlexCompHistory == null)
                    abusPersonAccount.ibusPersonAccountFlex.LoadFlexCompHistory();

                return abusPersonAccount.ibusPersonAccountFlex.iclbFlexCompHistory.Any(i => i.icdoPersonAccountFlexCompHistory.effective_start_date < adtTerminationDate);
            }
            else if(abusPersonAccount.ibusPlan.IsRetirementPlan())
            {
                if (abusPersonAccount.ibusPersonAccountRetirement.IsNull())
                    abusPersonAccount.LoadPersonAccountRetirement();
                if (abusPersonAccount.ibusPersonAccountRetirement.icolPersonAccountRetirementHistory == null)
                    abusPersonAccount.ibusPersonAccountRetirement.LoadPersonAccountRetirementHistory();

                return abusPersonAccount.ibusPersonAccountRetirement.icolPersonAccountRetirementHistory.Any(i => i.icdoPersonAccountRetirementHistory.start_date < adtTerminationDate);

            }
            else if(abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDeferredCompensation || abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdOther457)
            {
                busPersonAccountDeferredComp lobjPersonAccountDeferredComp = new busPersonAccountDeferredComp();
                if (lobjPersonAccountDeferredComp.FindPersonAccountDeferredComp(abusPersonAccount.icdoPersonAccount.person_account_id))
                {
                    if (lobjPersonAccountDeferredComp.icolPADeferredCompHistory == null)
                        lobjPersonAccountDeferredComp.LoadPADeffCompHistory();

                    return lobjPersonAccountDeferredComp.icolPADeferredCompHistory.Any(i => i.icdoPersonAccountDeferredCompHistory.start_date < adtTerminationDate);
                }
            }
            return true;
        }

        public bool IsOtherEmploymentOpenForPersonAccount(busPersonAccount abusPersonAccount)
        {
            bool lblnOtherEmploymentStillOpen = false;
            //PROD PIR : 4116 If No Other Employment opens with this plan election value enrolled.. then suspend the plan
            if (abusPersonAccount.iclbAccountEmploymentDetail == null)
                abusPersonAccount.LoadPersonAccountEmploymentDetails();
            foreach (busPersonAccountEmploymentDetail lbusPAEmpDetail in abusPersonAccount.iclbAccountEmploymentDetail)
            {
                if (lbusPAEmpDetail.ibusEmploymentDetail == null)
                    lbusPAEmpDetail.LoadPersonEmploymentDetail();
                if (lbusPAEmpDetail.ibusEmploymentDetail.ibusPersonEmployment == null)
                    lbusPAEmpDetail.ibusEmploymentDetail.LoadPersonEmployment();
                //Exclude the Current Employment
                if ((lbusPAEmpDetail.ibusEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.person_employment_id != icdoPersonEmployment.person_employment_id) &&
                   (lbusPAEmpDetail.ibusEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date == DateTime.MinValue))
                {
                    lblnOtherEmploymentStillOpen = true;
                    break;
                }
            }

            return lblnOtherEmploymentStillOpen;
        }

        public override busBase GetCorOrganization()
        {
            if (ibusOrganization == null)
                LoadOrganization();

            return ibusOrganization;
        }

        public override busBase GetCorPerson()
        {
            if (ibusPerson == null)
                LoadPerson();

            return ibusPerson;
        }

        //This method called when we click Hand Icon. That will reload the object and display the details
        public ArrayList ReloadHandButtonData()
        {
            ArrayList larrList = new ArrayList();
            if (!String.IsNullOrEmpty(icdoPersonEmployment.istrOrgCodeID))
            {
                ibusOrganization = new busOrganization();
                ibusOrganization.FindOrganizationByOrgCode(icdoPersonEmployment.istrOrgCodeID);
                icdoPersonEmployment.org_id = ibusOrganization.icdoOrganization.org_id;
            }
            larrList.Add(this);
            return larrList;
        }

        //PIR - 1274
        //before ending employment check if any contribution is reported after the end date entered.
        public bool IsContributionExistsAfterEmploymentEndDate()
        {
            if (icdoPersonEmployment.end_date != DateTime.MinValue)
            {
                if (icolPersonEmploymentDetail == null)
                    LoadPersonEmploymentDetail();
                decimal ldecTotalSalary = 0.00M;
                foreach (busPersonEmploymentDetail lobjPersonEmplDtl in icolPersonEmploymentDetail)
                {
                    if (lobjPersonEmplDtl.iclbRetirementContribution == null)
                        lobjPersonEmplDtl.LoadRetirementContribution();
                    //PROD PIR 5212 & 5230
                    //need to check whether salary amount is greater than zero , then throw error
                    //--start--//
                    /*
                    foreach (busPersonAccountRetirementContribution lobjRetirement in lobjPersonEmplDtl.iclbRetirementContribution)
                    {
                        //prod pir 4173 : employment end date validation changes
                        if (lobjRetirement.icdoPersonAccountRetirementContribution.effective_date != DateTime.MinValue &&
                            lobjRetirement.icdoPersonAccountRetirementContribution.effective_date.GetLastDayofMonth() > icdoPersonEmployment.end_date.GetLastDayofMonth())
                        {
                            if ((lobjRetirement.icdoPersonAccountRetirementContribution.transaction_type_value == busConstant.TransactionTypeInternalAdjustment)
                                || (lobjRetirement.icdoPersonAccountRetirementContribution.transaction_type_value == busConstant.TransactionTypePayrollAdjustment)
                                || (lobjRetirement.icdoPersonAccountRetirementContribution.transaction_type_value == busConstant.TransactionTypeRegularPayroll))
                            {
                                return true;
                            }
                        }
                    }
                    */
                    var lenmRetirementContribution = lobjPersonEmplDtl.iclbRetirementContribution.Where(o => o.icdoPersonAccountRetirementContribution.effective_date.GetLastDayofMonth() >
                        icdoPersonEmployment.end_date.GetLastDayofMonth() &&
                        (o.icdoPersonAccountRetirementContribution.transaction_type_value == busConstant.TransactionTypeInternalAdjustment ||
                        o.icdoPersonAccountRetirementContribution.transaction_type_value == busConstant.TransactionTypePayrollAdjustment ||
                        o.icdoPersonAccountRetirementContribution.transaction_type_value == busConstant.TransactionTypeRegularPayroll));
                    if (lenmRetirementContribution != null && lenmRetirementContribution.AsEnumerable().Count() > 0)
                    {
                        if (lenmRetirementContribution.AsEnumerable().GroupBy(o => o.icdoPersonAccountRetirementContribution.effective_date.GetFirstDayofCurrentMonth())
                            .Sum(o => o.Sum(i => i.icdoPersonAccountRetirementContribution.salary_amount)) > 0)
                        { return true; }
                    }
                    //--end--//
                }
            }
            return false;
        }

        # region UCS 60

        public override void AfterPersistChanges()
        {
            //UAT PIR 2373 people soft file changes
            if (icdoPersonEmployment.end_date != DateTime.MinValue)
            {
                UpdatePersonAccountPeoplesoftEventValue();
                //When Employment is ended (from screen or PS) and there is a first activity for WFL Process IDs 236,237,238,239,256 
                //(Activity IDs 27,32,28,31,74) in a Suspended status it should be updated to RESU
                busWorkflowHelper.UpdateSuspendedInstancestoResumed(icdoPersonEmployment.person_id);
            }

            //PIR 8975
            LoadLastPaycheckDate();

            InitiateWorkFlowForPayeeAccount();

            //UAT PIR 2043
            if (iintPreviousOrgId != 0)
                SetNPSPFlexCompFlag();
        }
        //initiate suspend workflow if the payee account status is not Suspend
        public void InitiateWorkFlowForPayeeAccount() //PIR 12393 & 12148
        {
            if (ibusPerson.icolPersonEmployment.IsNull())
                ibusPerson.LoadPersonEmployment();

            foreach (busPersonEmployment lobjPersonEmployment in ibusPerson.icolPersonEmployment)
            {
                if (lobjPersonEmployment.icolPersonEmploymentDetail.IsNull())
                    lobjPersonEmployment.LoadPersonEmploymentDetail();

                busPersonEmploymentDetail lobjPersonEmployemntDetail = new busPersonEmploymentDetail();
                if (lobjPersonEmployment.icolPersonEmploymentDetail.Count > 0)
                {
                    var lobjFilterPersonEmploymentDetail = lobjPersonEmployment.icolPersonEmploymentDetail.Where(lobjPersonEmpDtl => lobjPersonEmpDtl.icdoPersonEmploymentDetail.end_date != DateTime.MinValue);
                    if (lobjFilterPersonEmploymentDetail.Count() > 0)
                    {
                        lobjPersonEmployemntDetail = lobjFilterPersonEmploymentDetail.First();
                        lobjPersonEmployemntDetail.ibusPersonEmployment.icdoPersonEmployment = icdoPersonEmployment;
                        lobjPersonEmployemntDetail.InitializeSuspendWorkflowForPayeeAccount();
                        if (lobjPersonEmployemntDetail.iblnIsSuspendWorkflowInititated)
                            break;
                    }
                }
            }
        }

        //PIR-14214
        public void InitiateWorkFlowForServicePurchasePaymentInstallmentsTermination()
        {

            DataTable ldtbList = Select<cdoServicePurchaseHeader>(
            new string[2] { enmServicePurchaseHeader.person_id.ToString(), enmServicePurchaseHeader.action_status_value.ToString() },
            new object[2] { this.icdoPersonEmployment.person_id, busConstant.Service_Purchase_Action_Status_In_Payment }, null, null);

            if (ldtbList.Rows.Count > 0)
            {
                DataTable ldtActivityInstance = Select("entSolBpmActivityInstance.LoadRunningInstancesByPersonAndProcess",
                                                              new object[2] { this.icdoPersonEmployment.person_id, busConstant.Map_Service_Purchase_Payment_Installments_Termination });
                if ((ldtActivityInstance.Rows.Count == 0))
                {

                    busWorkflowHelper.InitiateBpmRequest(busConstant.Map_Service_Purchase_Payment_Installments_Termination, icdoPersonEmployment.person_id, 0, 0, iobjPassInfo);

                }
            }
        }
        # endregion

        #region UCS - 032

        public busPersonEmploymentDetail ibusESSLatestPersonEmploymentDetail { get; set; }

        public void ESSLoadPersonEmploymentDetail()
        {
            //PIR 22771 - Person Emp Detail sorted according to end date first.
            DataTable ldtbList = Select<cdoPersonEmploymentDetail>(
                new string[1] { "person_employment_id" },
                new object[1] { icdoPersonEmployment.person_employment_id }, null, "case when end_date is null then 0 else 1 end, start_date DESC, end_date DESC");
            icolPersonEmploymentDetail = GetCollection<busPersonEmploymentDetail>(ldtbList, "icdoPersonEmploymentDetail");
            foreach (busPersonEmploymentDetail lobjPersonEmploymentDetail in icolPersonEmploymentDetail)
            {
                lobjPersonEmploymentDetail.LoadPersonEmployment();
                lobjPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                lobjPersonEmploymentDetail.ESSLoadPersonAccountEmploymentDetail();
            }
        }

        #endregion

        // SYS PIR - 1748 -- APP-7018 is moved to Pension Plan maintenance
        // UAT PIR - 1472 -- APP-7018 is moved to Person Employment maintenance
        // Get normal retirement date for this person plan
        public string istrNormalRetirementDate { get; set; }
        public void GetNormalRetirementDate()
        {
            if (ibusPerson.IsNull()) LoadPerson();
            if (ibusPerson.icolPersonAccount.IsNull()) ibusPerson.LoadPersonAccount();
            foreach (busPersonAccount lobjPersonAccount in ibusPerson.icolPersonAccount)
            {
                if (lobjPersonAccount.ibusPlan == null)
                    lobjPersonAccount.LoadPlan();
                if (lobjPersonAccount.ibusPlan.IsRetirementPlan() &&
                   lobjPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled)
                {
                    if (lobjPersonAccount.icdoPersonAccount.Total_VSC == 0.00M)
                        lobjPersonAccount.LoadTotalVSC();
                    if (ibusPerson.IsNull())
                        LoadPerson();
                    //PIR - 2051
                    if (lobjPersonAccount.ibusPlan.IsNull())
                        lobjPersonAccount.LoadPlan();
                    DateTime idtTerminationDate = DateTime.MinValue;
                    GetOrgIdAsLatestEmploymentOrgId(lobjPersonAccount.icdoPersonAccount.person_account_id, busConstant.ApplicationBenefitTypeRetirement, ref idtTerminationDate);
                    DateTime ldtNormalRetirementDate = GetNormalRetirementDateBasedOnNormalEligibility(lobjPersonAccount.icdoPersonAccount.plan_id,
                                                                            lobjPersonAccount.ibusPlan.icdoPlan.plan_code, lobjPersonAccount.ibusPlan.icdoPlan.benefit_provision_id,
                                                                            busConstant.ApplicationBenefitTypeRetirement, ibusPerson.icdoPerson.date_of_birth,
                                                                            lobjPersonAccount.icdoPersonAccount.Total_VSC, 2, iobjPassInfo, idtTerminationDate,
                                                                            lobjPersonAccount.icdoPersonAccount.person_account_id, true, 0.00M);
                    istrNormalRetirementDate = ldtNormalRetirementDate.ToString(busConstant.DateFormatLongDate);
                    break;
                }
            }
        }
		//Prod PIR: 4606.  Property last day of Employment End Date Added.
        public string end_long_date_lastofmonth
        {
            get
            {
                return icdoPersonEmployment.end_date.GetLastDayofMonth().ToString(BusinessObjects.busConstant.DateFormatLongDate);
            }
        }
        public string Suspended_Plan_Start_Date_Long_Date { get; set; }
        public int lintBatchIDCOBRANotice { get; set; }
        public int lintBatchIDEmpTerminationNotices { get; set; }

        public override void LoadCorresProperties(string astrTemplateName)
        {
            GetNormalRetirementDate();
            if (astrTemplateName == "SFN-50084" || astrTemplateName == "APP-7308")
            {
                if (ibusPerson.IsNotNull() && ibusPerson.ibusCurrentEmployment == null)
                    ibusPerson.LoadCurrentEmployer();
            }
            base.LoadCorresProperties(astrTemplateName);
        }

        //UAT PIR  - 2043
        //
        private int iintPreviousOrgId { get; set; }
        public busOrganization ibusPreviousOrganization { get; set; }
        private void SetNPSPFlexCompFlag()
        {
            if ((iintPreviousOrgId != icdoPersonEmployment.org_id)
                || icdoPersonEmployment.end_date != DateTime.MinValue)
            {
                // PROD PIR ID 4624
                //if (ibusOrganization.IsNull())
                //    LoadOrganization();

                //if (ibusPreviousOrganization.IsNull())
                //    ibusPreviousOrganization = new busOrganization();

                //ibusPreviousOrganization.FindOrganization(iintPreviousOrgId);

                //if (ibusPreviousOrganization.icdoOrganization.peoplesoft_org_group_value == busConstant.PeopleSoftOrgGroupValueNonPSParoll
                //    || ibusOrganization.icdoOrganization.peoplesoft_org_group_value == busConstant.PeopleSoftOrgGroupValueNonPSParoll)
                //{
                if (icolPersonEmploymentDetail.IsNull())
                    LoadPersonEmploymentDetail();

                foreach (busPersonEmploymentDetail lobjPersonEmploymentDetail in icolPersonEmploymentDetail)
                {
                    // reload this to load the person account also
                    lobjPersonEmploymentDetail.LoadAllPersonAccountEmploymentDetails(true);

                    var lclbPersonAccountEmploymentDetail = lobjPersonEmploymentDetail.iclbAllPersonAccountEmpDtl
                        .Where(lobjPAED => lobjPAED.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdFlex)
                        .OrderByDescending(lobjPAED => lobjPAED.icdoPersonAccountEmploymentDetail.person_account_employment_dtl_id);

                    if (lclbPersonAccountEmploymentDetail.Count() > 0)
                    {
                        lclbPersonAccountEmploymentDetail.FirstOrDefault().LoadPersonAccount();
                        busPersonAccount lobjPersonAccount = lclbPersonAccountEmploymentDetail.FirstOrDefault().ibusPersonAccount;

                        lobjPersonAccount.icdoPersonAccount.npsp_flexcomp_flag = busConstant.Flag_Yes;
                        lobjPersonAccount.icdoPersonAccount.npsp_flexcomp_change_date = DateTime.Now;
                        lobjPersonAccount.icdoPersonAccount.Update();
                        break;
                    }
                }
                //}
            }
        }

        //UAT PIR 2373 people soft file changes
        /// <summary>
        /// method to update person account flag to TERM if end date is updated
        /// </summary>
        private void UpdatePersonAccountPeoplesoftEventValue()
        {
            //Reload the Person Employment Detail to Avoid the Record change Error.
            LoadPersonEmploymentDetail();

            //Avoid Record changed since last display Error (Maik mailed on 9/27/2010)
            List<int> llstUpdatedPersonAccounts = new List<int>();

            foreach (busPersonEmploymentDetail lobjEmploymentDetail in icolPersonEmploymentDetail)
            {
                if (lobjEmploymentDetail.iclbAllPersonAccountEmpDtl.IsNull())
                    lobjEmploymentDetail.LoadAllPersonAccountEmploymentDetails();
                foreach (busPersonAccountEmploymentDetail lobjPAEmpDtl in lobjEmploymentDetail.iclbAllPersonAccountEmpDtl)
                {
                    //UAT PIR 2373 people soft file changes
                    //--Start--//
                    if ((lobjPAEmpDtl.icdoPersonAccountEmploymentDetail.person_account_id > 0)
                        && (!(llstUpdatedPersonAccounts.Any(i => i == lobjPAEmpDtl.icdoPersonAccountEmploymentDetail.person_account_id))))
                    {
                        if (lobjPAEmpDtl.ibusPersonAccount == null)
                            lobjPAEmpDtl.LoadPersonAccount();

                        lobjPAEmpDtl.ibusPersonAccount.icdoPersonAccount.ps_file_change_event_value = busConstant.EmploymentHdrEnddated;
                        //lobjPAEmpDtl.ibusPersonAccount.icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                        lobjPAEmpDtl.ibusPersonAccount.icdoPersonAccount.Update();
                        llstUpdatedPersonAccounts.Add(lobjPAEmpDtl.ibusPersonAccount.icdoPersonAccount.person_account_id);
                    }
                    //--End--//
                }
            }
        }

        /// <summary>
        /// validation to check whether employment end date is greater than all employment details start date
        /// </summary>
        /// <returns></returns>
        public bool IsEndDateGreaterThanAllEmpDetailsStartDate()
        {
            bool lblnResult = true;
            if (icolPersonEmploymentDetail == null)
                LoadPersonEmploymentDetail();
            if (icdoPersonEmployment.end_date != DateTime.MinValue &&
                icolPersonEmploymentDetail.Where(o => o.icdoPersonEmploymentDetail.start_date > icdoPersonEmployment.end_date).Any())
            {
                lblnResult = false;
            }
            return lblnResult;
        }
        public Collection<busWssEmploymentAcaCert> iclbWssEmploymentAcaCerts { get; set; }
        public void LoadACAEligibilityCertification()
        {
            DataTable ldtACAEligibilityCertification = Select<cdoWssEmploymentAcaCert>(new string[1] { "person_employment_id" },
                                                               new object[1] { icdoPersonEmployment.person_employment_id }, null, null);

            iclbWssEmploymentAcaCerts = GetCollection<busWssEmploymentAcaCert>(ldtACAEligibilityCertification, "icdoWssEmploymentAcaCert");
            if (iclbWssEmploymentAcaCerts.Count > 0)
            {
                foreach (busWssEmploymentAcaCert lbusresult in iclbWssEmploymentAcaCerts)
                {
                    lbusresult.istrMethodDescription = busGlobalFunctions.GetDescriptionByCodeValue(7031, lbusresult.icdoWssEmploymentAcaCert.method, iobjPassInfo);
                    if (lbusresult.icdoWssEmploymentAcaCert.method == busConstant.ACACertificationMethodLookBack && lbusresult.icdoWssEmploymentAcaCert.lb_measure.IsNotNullOrEmpty())
                        lbusresult.istrLBMethodType = busGlobalFunctions.GetDescriptionByCodeValue(7032, lbusresult.icdoWssEmploymentAcaCert.lb_measure, iobjPassInfo);
                }
            }
        }

        public bool IsLookBackMeasureIsNull()
        {
            bool lblnResult = false;
            LoadACAEligibilityCertification();
            if (iclbWssEmploymentAcaCerts.Count > 0)
            {
                foreach (busWssEmploymentAcaCert lbusWssEmploymentAcaCert in iclbWssEmploymentAcaCerts)
                {
                    if (lbusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.lb_measure == null || lbusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.method == busConstant.ACACertificationMethodNewHire)
                        lblnResult = true;
                }
            }
            return lblnResult;
        }
        public bool IsAcaCertificationRecordNotPresent()
        {
            bool lblnResult = true;
            LoadACAEligibilityCertification();
            if (iclbWssEmploymentAcaCerts.Count == 0)
            {
                lblnResult = false;
            }
            return lblnResult;
        }
    }
}
