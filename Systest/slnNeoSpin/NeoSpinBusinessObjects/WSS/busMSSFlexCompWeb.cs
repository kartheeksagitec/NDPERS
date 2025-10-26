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

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busMSSFlexCompWeb : busPersonAccountFlexComp
    {
        public int iintEnrollmentRequestID { get; set; }
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
        # region UCS 24       

        public decimal idecMSSMedicalSpendingAnnualPledgeAmt { get; set; }
        public decimal idecMSSDependentCareAnnualPledgeAmt { get; set; }
        public void LoadMSSAnnualPledgeAmount()
        {
            if (iclbFlexCompOption.IsNull())
                LoadFlexCompOptionUpdate();

            foreach (busPersonAccountFlexCompOption lobjPersonAccountFlexCompOpt in iclbFlexCompOption)
            {
                if (lobjPersonAccountFlexCompOpt.icdoPersonAccountFlexCompOption.level_of_coverage_value == busConstant.FlexLevelOfCoverageMedicareSpending)
                    idecMSSMedicalSpendingAnnualPledgeAmt = lobjPersonAccountFlexCompOpt.icdoPersonAccountFlexCompOption.annual_pledge_amount;
                if (lobjPersonAccountFlexCompOpt.icdoPersonAccountFlexCompOption.level_of_coverage_value == busConstant.FlexLevelOfCoverageDependentSpending)
                    idecMSSDependentCareAnnualPledgeAmt = lobjPersonAccountFlexCompOpt.icdoPersonAccountFlexCompOption.annual_pledge_amount;
            }
        }

        public string istrMSSPaymentOptionValue
        {
            get
            {
                string lstrReturnValue = string.Empty;
                if (icdoPersonAccount.person_account_id > 0)
                {
                    if (icdoPersonAccountFlexComp.direct_deposit_flag == busConstant.Flag_Yes)
                        lstrReturnValue = busConstant.FlexCompPaymentOptionDirectDeposit;
                    else
                        lstrReturnValue = busConstant.FlexCompPaymentOptionCheck;
                }
                return lstrReturnValue;
            }
        }
        public string istrMSSMailOptionValue
        {
            get
            {
                string lstrReturnValue = string.Empty;
                if (icdoPersonAccountFlexComp.person_account_id > 0)
                {
                    if (icdoPersonAccountFlexComp.inside_mail_flag == busConstant.Flag_Yes)
                        lstrReturnValue = busConstant.FlexCompMailOptionInsideMail;
                    else
                        lstrReturnValue = busConstant.FlexCompMailOptionUSPostalService;
                }
                return lstrReturnValue;
            }
        }       

        # endregion        
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


        // PIR 10695 - to show current enrollment in 'Plan Details' panel
        public DateTime idtStartDate { get; set; }
        public DateTime idtEndDate { get; set; }
        public string istrPlanParticipation { get; set; }

        // PIR 10695
        public void LoadPreviousHistoryMSS()
        {
            int iintIter = 0;
            iclbPreviousHistory = new Collection<busPersonAccountFlexCompHistory>();

            if (iclbFlexCompHistory == null)
                LoadFlexCompHistory();

            bool lblnMSRAFound = false;
            bool lblnDCRAFound = false;

            foreach (busPersonAccountFlexCompHistory lbusPAHistory in iclbFlexCompHistory)
            {
                if (iintIter == 0)
                {
                    idtStartDate = lbusPAHistory.icdoPersonAccountFlexCompHistory.effective_start_date;
                    idtEndDate = lbusPAHistory.icdoPersonAccountFlexCompHistory.effective_end_date;
                    istrPlanParticipation = lbusPAHistory.icdoPersonAccountFlexCompHistory.plan_participation_status_description;
                }
                else
                {
                    if ((lbusPAHistory.icdoPersonAccountFlexCompHistory.effective_start_date != idtStartDate)
                        && (lbusPAHistory.icdoPersonAccountFlexCompHistory.effective_start_date != lbusPAHistory.icdoPersonAccountFlexCompHistory.effective_end_date)
                        && (lbusPAHistory.icdoPersonAccountFlexCompHistory.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled))
                    {
                        if (lbusPAHistory.icdoPersonAccountFlexCompHistory.level_of_coverage_value == busConstant.FlexLevelOfCoverageMedicareSpending)
                        {
                            if (!lblnMSRAFound)
                            {
                                iclbPreviousHistory.Add(lbusPAHistory);
                                lblnMSRAFound = true;
                            }
                        }
                        else if (lbusPAHistory.icdoPersonAccountFlexCompHistory.level_of_coverage_value == busConstant.FlexLevelOfCoverageDependentSpending)
                        {
                            if (!lblnDCRAFound)
                            {
                                iclbPreviousHistory.Add(lbusPAHistory);
                                lblnDCRAFound = true;
                            }
                        }
                        if (lblnMSRAFound && lblnDCRAFound) break;
                    }                    
                }
                iintIter++;
            }
        }
    }
}
