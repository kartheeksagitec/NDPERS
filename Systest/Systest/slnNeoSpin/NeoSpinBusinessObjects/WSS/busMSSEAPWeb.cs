using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.BusinessObjects;
using System.Data;
using System.Collections.ObjectModel;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;
using Sagitec.DataObjects;
using Sagitec.Common;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busMSSEAPWeb : busPersonAccountEAP
    {
        public string istrAcknowledgementText { get; set; }  //upgrade PIR-2423
        public int iintEnrollmentRequestID { get; set; }
        ////Set Visibility for the update button
        public bool SetVisibilityForUpdatePersonAccountButton()
        {
            if(icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
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

        //PIR 6961
        public void LoadPersonAccountId(int aintRequestID)
        {
            icdoPersonAccount = new cdoPersonAccount();
            DataTable ltdbWssPersonAccountEnrollmentRequest = Select<cdoWssPersonAccountEnrollmentRequest>(new string[1] { "WSS_PERSON_ACCOUNT_ENROLLMENT_REQUEST_ID" },
                new object[1] { aintRequestID }, null, null);
            icdoPersonAccount.person_account_id = Convert.ToInt32(ltdbWssPersonAccountEnrollmentRequest.Rows[0]["TARGET_PERSON_ACCOUNT_ID"]);
            icdoPersonAccount.person_employment_dtl_id = Convert.ToInt32(ltdbWssPersonAccountEnrollmentRequest.Rows[0]["PERSON_EMPLOYMENT_DTL_ID"]);
        }

        //PIR 6961
        public Collection<busWssPersonAccountEnrollmentRequestAck> iclbPersonAccountEnrollmentRequestAck { get; set; }
        public string ProviderOrg { get; set; }
        public void LoadEAPAckDetailsForView(int aintrequestid)
        {
            busBase lbusbase = new busBase();
            iclbPersonAccountEnrollmentRequestAck = new Collection<busWssPersonAccountEnrollmentRequestAck>();

            DataTable ldtbLifeAckDetailsView = Select("cdoWssPersonAccountEnrollmentRequestAck.SelectAckforView",
                new object[1] { aintrequestid });//enroll req gotta load
            StringBuilder lstrAcknowledgement_text = new StringBuilder();
            iclbPersonAccountEnrollmentRequestAck = lbusbase.GetCollection<busWssPersonAccountEnrollmentRequestAck>(ldtbLifeAckDetailsView);
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
                    lstrAcknowledgement_text.Append(lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text);
                    lstrAcknowledgement_text.Append("<br/>");
                }
                lstrAcknowledgement_text.Append(lobjPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text);
                lstrAcknowledgement_text.Append("<br/>");
            }
            istrAcknowledgementText = lstrAcknowledgement_text.ToString();
            DataTable WssPersonAccountEnrollmentRequest = Select<cdoWssPersonAccountEnrollmentRequest>(new string[1] { "WSS_PERSON_ACCOUNT_ENROLLMENT_REQUEST_ID" },
                new object[1] { aintrequestid }, null, null);
            if (WssPersonAccountEnrollmentRequest.Rows.Count > 0)
            {
                int aintorg_id = Convert.ToInt32(WssPersonAccountEnrollmentRequest.Rows[0]["PROVIDER_ORG_ID"]);
                DataTable ldtbOrganization = Select<cdoOrganization>(new string[1] { "ORG_ID" },
                new object[1] { aintorg_id }, null, null);
                if (ldtbOrganization.Rows.Count > 0)
                    ProviderOrg = ldtbOrganization.Rows[0]["ORG_NAME"].ToString();
            }

        }



        // PIR 10695 - to show current enrollment in 'Plan Details' panel
        public DateTime istrStartDate { get; set; }
        public DateTime istrEndDate { get; set; }
        public string istrPlanParticipation { get; set; }

        // PIR 10695 new collection
        public Collection<busPersonAccountEAPHistory> iclbMSSHistory { get; set; }

        public void LoadMSSHistory()
        {
            int iintIter = 0;
            if (iclbEAPHistory.IsNull())
                LoadEAPHistory();

            iclbMSSHistory = new Collection<busPersonAccountEAPHistory>();
            foreach (busPersonAccountEAPHistory lobjHistory in iclbEAPHistory)
            {
                if (iintIter == 0)
                {
                    istrStartDate = lobjHistory.icdoPersonAccountEapHistory.start_date;
                    istrEndDate = lobjHistory.icdoPersonAccountEapHistory.end_date;
                    istrPlanParticipation = lobjHistory.icdoPersonAccountEapHistory.plan_participation_status_description;
                }
                else
                {
                    if ((lobjHistory.icdoPersonAccountEapHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                        && (lobjHistory.icdoPersonAccountEapHistory.start_date != lobjHistory.icdoPersonAccountEapHistory.end_date))
                    {
                        iclbMSSHistory.Add(lobjHistory);
                        break;
                    }
                }
                iintIter++;
            }
        }
    }
}
