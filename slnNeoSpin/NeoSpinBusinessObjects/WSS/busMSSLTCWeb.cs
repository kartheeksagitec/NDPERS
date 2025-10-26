using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.BusinessObjects;
using System.Data;
using System.Collections.ObjectModel;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busMSSLTCWeb : busPersonAccountLtc
    {
        public int iintEnrollmentRequestID { get; set; }
        public cdoWssPersonAccountEnrollmentRequest icdoEnrollmentRequest { get; set; }
        public void LoadRequest()
        {
            if (icdoEnrollmentRequest == null)
            {
                icdoEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest();
            }
            DataTable ldtbList = Select<cdoWssPersonAccountEnrollmentRequest>(new string[1] { "WSS_PERSON_ACCOUNT_ENROLLMENT_REQUEST_ID" },
                new object[1] { icdoEnrollmentRequest.wss_person_account_enrollment_request_id }, null, null);
            if (ldtbList.Rows.Count > 0)
            {
                icdoEnrollmentRequest.LoadData(ldtbList.Rows[0]);
            }
        }
        // Load Premium YTD
        public Collection<busPersonAccountInsuranceContribution> iclbMSSInsurancePremiumDetails { get; set; }
        public void LoadMSSInsuranceDetails()
        {
            if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes)
            {
                DateTime CYTDStartDate = ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date;
                DateTime CYTDEndDDate = DateTime.MaxValue;
                DataTable ldtbList = Select("cdoPersonAccount.LoadInsuranceYTD",
                        new object[3] { CYTDStartDate, CYTDEndDDate, icdoPersonAccount.person_account_id });
                iclbMSSInsurancePremiumDetails = GetCollection<busPersonAccountInsuranceContribution>(ldtbList, "icdoPersonAccountInsuranceContribution");
            }
        }
        public Collection<busPersonDependent> iclbMSSPersonDependent { get; set; }
        public void LoadMSSDependent()
        {
            iclbMSSPersonDependent = new Collection<busPersonDependent>();

            if (ibusPerson.IsNull())
                LoadPerson();

            if (ibusPerson.iclbPersonDependent.IsNull())
                ibusPerson.LoadDependent();

            foreach (busPersonDependent lobjPersonDependent in ibusPerson.iclbPersonDependent)
            {
                if (lobjPersonDependent.ibusPeronAccountDependent.ibusPersonAccount.IsNull())
                    lobjPersonDependent.ibusPeronAccountDependent.LoadPersonAccount();
            }

            var lenumDependentList = ibusPerson.iclbPersonDependent.Where(lobjPerDep => lobjPerDep.ibusPeronAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == icdoPersonAccount.plan_id
                && (lobjPerDep.ibusPeronAccountDependent.icdoPersonAccountDependent.end_date > DateTime.Now
                || lobjPerDep.ibusPeronAccountDependent.icdoPersonAccountDependent.end_date == DateTime.MinValue));

            foreach (busPersonDependent lobjPersonDependent in lenumDependentList)
            {
                lobjPersonDependent.LoadDependentInfo();
                iclbMSSPersonDependent.Add(lobjPersonDependent);
            }
        }
        public Collection<busPersonEmploymentDetail> iclbMSSPersonEmploymentDetail { get; set; }
        //load only contributing employers
        public void LoadMSSContributingEmployers()
        {
            if (iclbMSSPersonEmploymentDetail.IsNull())
                iclbMSSPersonEmploymentDetail = new Collection<busPersonEmploymentDetail>();

            if (iclbEmploymentDetail.IsNull())
                LoadAllPersonEmploymentDetails();

            //var lContributingEmployers = iclbEmploymentDetail.Where(lobjPersonEmploymentDTL => lobjPersonEmploymentDTL.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusContributing);

            foreach (busPersonEmploymentDetail lobjPersonEmploymentdtl in iclbEmploymentDetail)
            {
                iclbMSSPersonEmploymentDetail.Add(lobjPersonEmploymentdtl);
            }
        }

        ////Set Visibility for the update button
        public bool SetVisibilityForUpdatePersonAccountButton()
        {
            if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                return true;
            else
            {
                if (ibusPersonEmploymentDetail.IsNull())
                    LoadPersonEmploymentDetail();

                if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                    ibusPersonEmploymentDetail.LoadPersonEmployment();

                if (ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date == DateTime.MinValue)
                    return true;
            }
            return false;
        }

        public bool iblnIsEnrollmentRequestRejectedOrPending
        {
            get
            {
                if (ibusWSSPersonAccountEnrollmentRequest.IsNull())
                    LoadWSSEnrollmentRequestUpdate(iintEnrollmentRequestID);
                return (ibusWSSPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.status_value == busConstant.EnrollRequestStatusRejected
                    || ibusWSSPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.status_value == busConstant.EnrollRequestStatusPendingRequest);
            }
        }
        public bool iblnNoEnrollmentRequestExits
        {
            get
            {
                return (iintEnrollmentRequestID == 0);
            }
        }

        public Collection<busPersonAccountLtcOption> iclbLTCOptions { get; set; }

        public void LoadEnrolledCoverage()
        {
            iclbLTCOptions = new Collection<busPersonAccountLtcOption>();

            // Member Coverage
            DataTable ldtbMemberCoverage = busNeoSpinBase.Select("cdoPersonAccountLtcOption.LoadEnrolledMemberCoverages", new object[1] { icdoPersonAccount.person_account_id });
            iclbLtcOptionMember = GetCollection<busPersonAccountLtcOption>(ldtbMemberCoverage, "icdoPersonAccountLtcOption");
            foreach (busPersonAccountLtcOption lobjLtcOption in iclbLtcOptionMember)
            {
                if (lobjLtcOption.icdoPersonAccountLtcOption.person_id == 0)
                    lobjLtcOption.icdoPersonAccountLtcOption.person_id = icdoPersonAccount.person_id;
                iclbLTCOptions.Add(lobjLtcOption);
            }

            // Spouse Coverage
            DataTable ldtbSpouseCoverage = busNeoSpinBase.Select("cdoPersonAccountLtcOption.LoadEnrolledSpouseCoverages", new object[1] { icdoPersonAccount.person_account_id });
            iclbLtcOptionSpouse = GetCollection<busPersonAccountLtcOption>(ldtbSpouseCoverage, "icdoPersonAccountLtcOption");
            foreach (busPersonAccountLtcOption lobjLtcOption in iclbLtcOptionSpouse)
            {
                lobjLtcOption.icdoPersonAccountLtcOption.person_id = ibusPerson.ibusSpouse.icdoPerson.person_id;
                iclbLTCOptions.Add(lobjLtcOption);
            }
        }

        // PIR 10695 - to show current enrollment in 'Plan Details' panel
        public DateTime idtStartDate { get; set; }
        public DateTime idtEndDate { get; set; }
        public string istrPlanParticipation { get; set; }

        // PIR 10695
        public void LoadPreviousHistoryMSS()
        {
            int iintIter = 0;
            if (iclbPreviousHistory == null)
                iclbPreviousHistory = new Collection<busPersonAccountLtcOptionHistory>();

            if (iclbLtcHistory == null)
                LoadLtcOptionHistory();

            bool lblnMemberRel3Yrs = false;
            bool lblnMemberRel5Yrs = false;
            bool lblnSpouseRel3Yrs = false;
            bool lblnSpouseRel5Yrs = false;

            foreach (busPersonAccountLtcOptionHistory lbusLtcHistory in iclbLtcHistory)
            {
                if (iintIter == 0)
                {
                    idtStartDate = lbusLtcHistory.icdoPersonAccountLtcOptionHistory.effective_start_date;
                    idtEndDate = lbusLtcHistory.icdoPersonAccountLtcOptionHistory.effective_end_date;
                    istrPlanParticipation = lbusLtcHistory.icdoPersonAccountLtcOptionHistory.plan_participation_status_description;
                }
                else
                {
                    if ((lbusLtcHistory.icdoPersonAccountLtcOptionHistory.effective_start_date != idtStartDate)
                        && (lbusLtcHistory.icdoPersonAccountLtcOptionHistory.effective_start_date != lbusLtcHistory.icdoPersonAccountLtcOptionHistory.effective_end_date)
                        && (lbusLtcHistory.icdoPersonAccountLtcOptionHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled))
                    {
                        if ((lbusLtcHistory.icdoPersonAccountLtcOptionHistory.level_of_coverage_value == busConstant.LTCLevelOfCoverage3YRS) &&
                            (lbusLtcHistory.icdoPersonAccountLtcOptionHistory.ltc_relationship_value == busConstant.PersonAccountLtcRelationShipMember))
                        {
                            if (!lblnMemberRel3Yrs)
                            {
                                iclbPreviousHistory.Add(lbusLtcHistory);
                                lblnMemberRel3Yrs = true;
                            }
                        }
                        else if ((lbusLtcHistory.icdoPersonAccountLtcOptionHistory.level_of_coverage_value == busConstant.LTCLevelOfCoverage5YRS) &&
                           (lbusLtcHistory.icdoPersonAccountLtcOptionHistory.ltc_relationship_value == busConstant.PersonAccountLtcRelationShipMember))
                        {
                            if (!lblnMemberRel5Yrs)
                            {
                                iclbPreviousHistory.Add(lbusLtcHistory);
                                lblnMemberRel5Yrs = true;
                            }
                        }
                        else if ((lbusLtcHistory.icdoPersonAccountLtcOptionHistory.level_of_coverage_value == busConstant.LTCLevelOfCoverage3YRS) &&
                            (lbusLtcHistory.icdoPersonAccountLtcOptionHistory.ltc_relationship_value == busConstant.PersonAccountLtcRelationShipSpouse))
                        {
                            if (!lblnSpouseRel3Yrs)
                            {
                                iclbPreviousHistory.Add(lbusLtcHistory);
                                lblnSpouseRel3Yrs = true;
                            }
                        }
                        else if ((lbusLtcHistory.icdoPersonAccountLtcOptionHistory.level_of_coverage_value == busConstant.LTCLevelOfCoverage5YRS) &&
                           (lbusLtcHistory.icdoPersonAccountLtcOptionHistory.ltc_relationship_value == busConstant.PersonAccountLtcRelationShipSpouse))
                        {
                            if (!lblnSpouseRel5Yrs)
                            {
                                iclbPreviousHistory.Add(lbusLtcHistory);
                                lblnSpouseRel5Yrs = true;
                            }
                        }

                        if (lblnMemberRel3Yrs && lblnMemberRel5Yrs && lblnSpouseRel3Yrs && lblnSpouseRel5Yrs) break;
                    }
                }
                iintIter++;
            }
        }
    }
}
