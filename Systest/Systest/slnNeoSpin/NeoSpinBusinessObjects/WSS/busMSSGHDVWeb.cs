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
    public class busMSSGHDVWeb : busPersonAccountGhdv
    { 
        public int iintEnrollmentRequestId { get; set; }
        public Collection<busPersonEmploymentDetail> iclbMSSPersonEmploymentDetail { get; set; }
        public string istrPremiumFlag { get; set; } // PIR 9778
        public bool iblnEnrollInHealthAsTemporary { get; set; }
        // Load Premium YTD
        public Collection<busPersonAccountInsuranceContribution> iclbMSSInsurancePremiumDetails { get; set; }
        public void LoadMSSInsuranceDetails()
        {
            Collection<busPersonAccountInsuranceContribution> iclbTemp = new Collection<busPersonAccountInsuranceContribution>();//PIR 8502
            if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes)
            {
                DateTime CYTDStartDate = ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date;
                DateTime CYTDEndDDate = DateTime.MaxValue;
                DataTable ldtbList = Select("cdoPersonAccount.LoadInsuranceYTD",
                        new object[3] { CYTDStartDate, CYTDEndDDate, icdoPersonAccount.person_account_id });
                iclbTemp = GetCollection<busPersonAccountInsuranceContribution>(ldtbList, "icdoPersonAccountInsuranceContribution");

                //PIR 8502
                var enumLst = iclbTemp.Where(o => o.icdoPersonAccountInsuranceContribution.effective_date >=
                                Convert.ToDateTime("10/01/2010")).OrderByDescending(o => o.icdoPersonAccountInsuranceContribution.effective_date)
                                .ThenByDescending(o => o.icdoPersonAccountInsuranceContribution.subsystem_description);// PIR 8620
                iclbTemp = enumLst.ToList().ToCollection();
                iclbMSSInsurancePremiumDetails = iclbTemp.Where(o => o.icdoPersonAccountInsuranceContribution.effective_date >=
                    iclbTemp[0].icdoPersonAccountInsuranceContribution.effective_date.AddMonths(-18)).ToList().ToCollection();
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

        // PIR 9778
        public void LoadPretaxPayrollDeduction()
        {
            if (ibusPlan.icdoPlan.plan_id != busConstant.PlanIdGroupHealth)
            {
                if (icdoPersonAccountGhdv.premium_conversion_indicator_flag == busConstant.Flag_Yes)
                {
                    istrPremiumFlag = busConstant.Flag_Yes_Value;
                }
                else
                {
                    istrPremiumFlag = busConstant.Flag_No_Value;
                }
            }
        }

        //load only contributing employers
        public void LoadMSSContributingEmployers()
        {
            if (iclbMSSPersonEmploymentDetail.IsNull())
                iclbMSSPersonEmploymentDetail = new Collection<busPersonEmploymentDetail>();

            if (iclbEmploymentDetail.IsNull())
                LoadAllPersonEmploymentDetails();

            var lContributingEmployers = iclbEmploymentDetail.Where(lobjPersonEmploymentDTL => lobjPersonEmploymentDTL.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusContributing);

            foreach (busPersonEmploymentDetail lobjPersonEmploymentdtl in lContributingEmployers)
            {
                iclbMSSPersonEmploymentDetail.Add(lobjPersonEmploymentdtl);
            }
        }

        public bool SetVisibilityForUpdatePersonAccountButton()
        {
            if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                return true;
            else
            {
                if (ibusPersonEmploymentDetail.IsNull())
                    LoadPersonEmploymentDetail();

                if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date == DateTime.MinValue)
                    return true;
            }
            return false;
        }

        public bool iblnIsEnrollmentRequestRejectedOrPending
        {
            get
            {
                if (ibusWSSPersonAccountEnrollmentRequest.IsNull())
                    LoadWSSEnrollmentRequestUpdate(iintEnrollmentRequestId);
                return (ibusWSSPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.status_value == busConstant.EnrollRequestStatusRejected
                    || ibusWSSPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.status_value == busConstant.EnrollRequestStatusPendingRequest);
            }
        }
        public bool iblnNoEnrollmentRequestExits
        {
            get
            {
                return (iintEnrollmentRequestId == 0);
            }
        }

        // PIR 9796
        public bool IsSSNMissingForAnyDependents()
        {
            if (iclbMSSPersonDependent.IsNull())
                LoadMSSDependent();
            if (iclbMSSPersonDependent.IsNotNull() && iclbMSSPersonDependent.Count > 0)
            {
                if (iclbMSSPersonDependent.Where(o => o.icdoPersonDependent.dependent_ssn.IsNullOrEmpty()).Any())
                    return true;
            }
            return false;
        }

        //PIR--10124 Start 
        public bool IsFlexCompPlanNotAvailable()
        {
            bool lblnResult = false;
            if (this.ibusPersonEmploymentDetail == null)
                this.LoadPersonEmploymentDetail();
            if (this.ibusPersonEmploymentDetail.ibusPersonEmployment == null)
                this.ibusPersonEmploymentDetail.LoadPersonEmployment();
            if (this.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization == null)
                this.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
            if (this.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.iclbOrgPlan == null || this.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.iclbOrgPlan.Count == 0)
                this.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.LoadOrgPlan();

            busOrgPlan lbusOrgPlan = this.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.iclbOrgPlan.Where(o => o.icdoOrgPlan.plan_id == busConstant.PlanIdFlex).FirstOrDefault();
            if (lbusOrgPlan == null)
                lblnResult = true;
            else if (lbusOrgPlan != null)
            {
                if (lbusOrgPlan.icdoOrgPlan.participation_end_date != DateTime.MinValue && lbusOrgPlan.icdoOrgPlan.participation_end_date < DateTime.Now)
                    lblnResult = true;
            }
            return lblnResult;
        }
        //PIR--10124 End

        public string AnnualEnrollmentEffectiveYear
        {
            get
            {
                return Convert.ToString(Convert.ToDateTime(busGlobalFunctions.GetData3ByCodeValue(52, busConstant.AnnualEnrollment, iobjPassInfo)).Year);
            }
        }

        public DateTime AnnualEnrollmentEffectiveDate
        {
            get
            {
                return Convert.ToDateTime(busGlobalFunctions.GetData3ByCodeValue(52, busConstant.AnnualEnrollment, iobjPassInfo));
            }
        }

        public Collection<busPersonAccountGhdvHistory> iclbMSSHistory { get; set; }

        public void LoadMSSHistory()
        {
            if (iclbPersonAccountGHDVHistory.IsNull())
                LoadPersonAccountGHDVHistory();

            iclbMSSHistory = new Collection<busPersonAccountGhdvHistory>();
            DateTime ldteTempDate = AnnualEnrollmentEffectiveDate;
            if (ldteTempDate != DateTime.MinValue) ldteTempDate = ldteTempDate.AddDays(-1);
            foreach (busPersonAccountGhdvHistory lobjHistory in iclbPersonAccountGHDVHistory)
            {
                if (lobjHistory.icdoPersonAccountGhdvHistory.end_date == ldteTempDate)
                {
                    if (IsPlanHealthOrMedicare)
                    {
                        if (iclbCoverageRef.IsNull()) LoadCoverageCodeByFilter();
                        var lenumCoverageCodeList = iclbCoverageRef.Where(lobjCoverageRef => lobjCoverageRef.coverage_code == lobjHistory.icdoPersonAccountGhdvHistory.coverage_code);
                        string lstrCoverageCode = string.Empty;
                        if (lenumCoverageCodeList.Count() > 0)
                        {
                            char[] lsplitter = { '-' };
                            //  lintIndex = lenumCoverageCodeList.FirstOrDefault().short_description.IndexOf("-");
                            lobjHistory.istrCoverageCode = lenumCoverageCodeList.FirstOrDefault().short_description.Split(lsplitter).Last();
                        }
                    }
                    iclbMSSHistory.Add(lobjHistory);
                }
            }

        }



        // PIR 10695 - to show current enrollment in 'Plan Details' panel
        public DateTime istrStartDate { get; set; }
        public DateTime istrEndDate { get; set; }
        public string istrPlanParticipation { get; set; }

        // PIR 10695
        public void LoadMSSHistoryGhdv()
        {
            int iintIter = 0;
            if (iclbPersonAccountGHDVHistory.IsNull())
                LoadPersonAccountGHDVHistory();

            iclbMSSHistory = new Collection<busPersonAccountGhdvHistory>();
            foreach (busPersonAccountGhdvHistory lobjHistory in iclbPersonAccountGHDVHistory)
            {
                if (iintIter == 0)
                {
                    istrStartDate = lobjHistory.icdoPersonAccountGhdvHistory.start_date;
                    istrEndDate = lobjHistory.icdoPersonAccountGhdvHistory.end_date;
                    istrPlanParticipation = lobjHistory.icdoPersonAccountGhdvHistory.plan_participation_status_description;
                }
                else
                {
                    if ((lobjHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                        && (lobjHistory.icdoPersonAccountGhdvHistory.start_date != lobjHistory.icdoPersonAccountGhdvHistory.end_date))
                    {
                        if (IsPlanHealthOrMedicare)
                        {
                            if (iclbCoverageRef.IsNull()) LoadCoverageCodeByFilter();
                            var lenumCoverageCodeList = iclbCoverageRef.Where(lobjCoverageRef => lobjCoverageRef.coverage_code == lobjHistory.icdoPersonAccountGhdvHistory.coverage_code);
                            string lstrCoverageCode = string.Empty;
                            if (lenumCoverageCodeList.Count() > 0)
                            {
                                char[] lsplitter = { '-' };
                                lobjHistory.istrCoverageCode = lenumCoverageCodeList.FirstOrDefault().short_description.Split(lsplitter).Last();
                            }
                        }
                        iclbMSSHistory.Add(lobjHistory);
                        break;
                    }
                }
                iintIter++;
            }
        }
        //19997
        public bool IsMemberHasHDHPPlan()
        {
            if (ibusOrgPlan.IsNull())
                LoadOrgPlan(DateTime.Now.GetFirstDayofNextMonth());

            if (ibusPersonAccountGHDV.IsNull())
                LoadPersonAccountGHDV();

            busPersonAccountGhdvHistory lobjGHDVHistory = ibusPersonAccountGHDV?.LoadHistoryByDate(DateTime.Now);

            if (!string.IsNullOrEmpty(ibusOrgPlan.icdoOrgPlan.hsa_pre_tax_agreement) && ibusOrgPlan.icdoOrgPlan.hsa_pre_tax_agreement == "Y")
            {
                if (ibusPersonAccountGHDV?.icdoPersonAccountGhdv?.alternate_structure_code_value == "HDHP" || lobjGHDVHistory?.icdoPersonAccountGhdvHistory?.alternate_structure_code_value == "HDHP")
                    return true;
            }
        
            return false;
        }
    }
}
