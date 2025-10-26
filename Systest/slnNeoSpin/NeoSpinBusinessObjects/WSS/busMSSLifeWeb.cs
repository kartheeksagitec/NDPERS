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
    public class busMSSLifeWeb : busPersonAccountLife
    {
        public int iintEnrollmentRequestID { get; set; }
        public cdoWssPersonAccountEnrollmentRequest icdoEnrollmentRequest { get; set; }
        public Collection<cdoWssPersonAccountLifeOption> iclbMSSLifeOption { get; set; }
        public decimal idecMSSSupplementalAmount { get; set; }
        public decimal idecMSSSpouseSupplementalAmount { get; set; }
        public string istrMSSSupplementalLevelOfCoverage { get; set; }
        public string istrMSSSpouseSupplementalLevelOfCoverage { get; set; }
        public string istrMSSSupplementalIsWaived { get; set; }
        public string istrMSSSpouseSupplementalIsWaived { get; set; }
        public string istrMSSSpouseName { get; set; }
        public DateTime idtMSSSpouseDOB { get; set; }
        public Collection<busPersonBeneficiary> iclbMSSPersonAccountBeneficiaries { get; set; }
        public string istrPremiumFlag { get; set; } // PIR 9778
        public void LoadMSSBeneficiariesForPlan()
        {
            iclbMSSPersonAccountBeneficiaries = new Collection<busPersonBeneficiary>();
            if (ibusPerson.IsNull())
                LoadPerson();
            if (ibusPerson.iclbPersonBeneficiary.IsNull())
                ibusPerson.LoadBeneficiary();
            foreach (busPersonBeneficiary lobjPersonBeneficiary in ibusPerson.iclbPersonBeneficiary)
            {
                if (lobjPersonBeneficiary.ibusPersonAccountBeneficiary.ibusPersonAccount.IsNull())
                    lobjPersonBeneficiary.ibusPersonAccountBeneficiary.LoadPersonAccount();
            }
            var lenumBeneficiaries = ibusPerson.iclbPersonBeneficiary.Where(lobjPerBen => lobjPerBen.ibusPersonAccountBeneficiary.ibusPersonAccount.icdoPersonAccount.plan_id == icdoPersonAccount.plan_id
                && (lobjPerBen.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date == DateTime.MinValue
                || lobjPerBen.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date > DateTime.Now));

            foreach (busPersonBeneficiary lobjPersonBene in lenumBeneficiaries)
                iclbMSSPersonAccountBeneficiaries.Add(lobjPersonBene);

        }

        // Load Premium YTD
        public Collection<busPersonAccountInsuranceContribution> iclbMSSInsurancePremiumDetails { get; set; }
        public void LoadMSSInsuranceDetails()
        {
            Collection<busPersonAccountInsuranceContribution> iclbTemp = new Collection<busPersonAccountInsuranceContribution>();//PIR 8503
            if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes)
            {
                DateTime CYTDStartDate = ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date;
                DateTime CYTDEndDDate = DateTime.MaxValue;
                DataTable ldtbList = Select("cdoPersonAccount.LoadInsuranceYTD",
                        new object[3] { CYTDStartDate, CYTDEndDDate, icdoPersonAccount.person_account_id });
                iclbTemp = GetCollection<busPersonAccountInsuranceContribution>(ldtbList, "icdoPersonAccountInsuranceContribution");

                //PIR 8503
                var enumLst = iclbTemp.Where(o => o.icdoPersonAccountInsuranceContribution.effective_date >=
                Convert.ToDateTime("10/01/2010")).OrderByDescending(o => o.icdoPersonAccountInsuranceContribution.effective_date)
                                                 .ThenByDescending(o => o.icdoPersonAccountInsuranceContribution.subsystem_description);// PIR 8620
                iclbTemp = enumLst.ToList().ToCollection();
                iclbMSSInsurancePremiumDetails = iclbTemp.Where(o => o.icdoPersonAccountInsuranceContribution.effective_date >=
                    iclbTemp[0].icdoPersonAccountInsuranceContribution.effective_date.AddMonths(-18)).ToList().ToCollection();
            }
        }
        //load change effective date
        public void LoadChangeEffectiveDate()
        {
            if (icdoEnrollmentRequest.wss_person_account_enrollment_request_id > 0)
            {
                icdoEnrollmentRequest.date_of_change = icdoPersonAccount.start_date;
                if (icdoPersonAccount.start_date == DateTime.MinValue)
                    icdoEnrollmentRequest.date_of_change = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.AddMonths(1);
            }
        }

        // PIR 9778
        public void LoadPretaxPayrollDeduction()
        {
            if (icdoPersonAccountLife.premium_conversion_indicator_flag == busConstant.Flag_Yes)
            {
                istrPremiumFlag = busConstant.Flag_Yes_Value;
            }
            else
            {
                istrPremiumFlag = busConstant.Flag_No_Value;
            }
        }

        public void LoadMSSLifeOption()
        {
            DataTable ldtbList;
            if (iclbMSSLifeOption == null)
                iclbMSSLifeOption = new Collection<cdoWssPersonAccountLifeOption>();
            if (icdoEnrollmentRequest.wss_person_account_enrollment_request_id > 0)
            {
                ldtbList = Select<cdoWssPersonAccountLifeOption>(new string[1] { "WSS_PERSON_ACCOUNT_ENROLLMENT_REQUEST_ID" },
                    new object[1] { icdoEnrollmentRequest.wss_person_account_enrollment_request_id }, null, null);
                iclbMSSLifeOption = cdoWssPersonAccountLifeOption.GetCollection<cdoWssPersonAccountLifeOption>(ldtbList);
            }
            else
            {
                if (iclbLifeOption.IsNull())
                    LoadLifeOption();

                foreach (busPersonAccountLifeOption lobjLifeOption in iclbLifeOption)
                {
                    cdoWssPersonAccountLifeOption lobjWssLifeOption = new cdoWssPersonAccountLifeOption();
                    lobjWssLifeOption.level_of_coverage_value = lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value;
                    lobjWssLifeOption.coverage_amount = lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount;
                    lobjWssLifeOption.plan_option_status_value = lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_value;               
                    iclbMSSLifeOption.Add(lobjWssLifeOption);
                }
            }
            foreach (cdoWssPersonAccountLifeOption lobjMSSLifeOption in iclbMSSLifeOption)
            {
                if (lobjMSSLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental)
                {
                    istrMSSSupplementalLevelOfCoverage = lobjMSSLifeOption.level_of_coverage_description;
                    idecMSSSupplementalAmount = lobjMSSLifeOption.coverage_amount;
                }
                if (lobjMSSLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental)
                {
                    istrMSSSpouseSupplementalLevelOfCoverage = lobjMSSLifeOption.level_of_coverage_description;
                    idecMSSSpouseSupplementalAmount = lobjMSSLifeOption.coverage_amount;
                }
            }
        }
        public void LoadAmountByLifeOption()
        {
            foreach (cdoWssPersonAccountLifeOption lobjLifeOption in iclbMSSLifeOption)
            {
                if (lobjLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental)
                {
                    idecMSSSupplementalAmount = lobjLifeOption.coverage_amount;
                }
                else if (lobjLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental)
                {
                    idecMSSSpouseSupplementalAmount = lobjLifeOption.coverage_amount;
                }
            }
        }

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

        //PIR 15447

        public bool iblnIsEnrollmentRequestRejectedOrPending
        {
            get
            {
                //if (ibusWSSPersonAccountEnrollmentRequest.IsNull())
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

        //pir 8507
        public Collection<busPersonAccountLifeOption> iclbMSSLifeOptionData { get; set; }
        public void LoadMSSLifeOptionData()
        {
            LoadLifeOptionData();
            // Supplemental amount should be displayed with the addition of Basic coverage amount
            decimal ldecBasicAmount = 0M;
            foreach (busPersonAccountLifeOption lobjOption in iclbLifeOption)
            {
                if (lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Basic)
                    ldecBasicAmount = lobjOption.icdoPersonAccountLifeOption.coverage_amount;
                if (lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental &&
                    lobjOption.icdoPersonAccountLifeOption.coverage_amount > 0M)
                    lobjOption.icdoPersonAccountLifeOption.coverage_amount += ldecBasicAmount;
            }
            iclbMSSLifeOptionData = new Collection<busPersonAccountLifeOption>();
            iclbMSSLifeOptionData = iclbLifeOption.Where(o => (o.icdoPersonAccountLifeOption.effective_end_date == DateTime.MinValue
                                                          || o.icdoPersonAccountLifeOption.effective_end_date > DateTime.Now)
                                                          && o.icdoPersonAccountLifeOption.coverage_amount != 0).ToList().ToCollection();
        }

        public override busBase GetCorPerson()
        {
            if (ibusPerson.IsNull()) LoadPerson();
            return ibusPerson;
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

        // PIR 10695
        public void LoadPreviousHistoryMSS(DateTime adteChangeEffectiveDate)
        {
            iclbPreviousHistory = new Collection<busPersonAccountLifeHistory>();

            if (iclbPersonAccountLifeHistory == null)
                LoadHistory();

            bool lblnBasicFound = false;
            bool lblnSuppFound = false;
            bool lblnDepSuppFound = false;
            bool lblnSpouseSuppFound = false;

            foreach (busPersonAccountLifeHistory lbusPAHistory in iclbPersonAccountLifeHistory)
            {
                if (lbusPAHistory.icdoPersonAccountLifeHistory.effective_start_date != adteChangeEffectiveDate &&
                    lbusPAHistory.icdoPersonAccountLifeHistory.effective_start_date != lbusPAHistory.icdoPersonAccountLifeHistory.effective_end_date &&
                    lbusPAHistory.icdoPersonAccountLifeHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                {
                    if (lbusPAHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_Basic)
                    {
                        if (!lblnBasicFound)
                        {
                            iclbPreviousHistory.Add(lbusPAHistory);
                            lblnBasicFound = true;
                        }
                    }
                    else if (lbusPAHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental)
                    {
                        if (!lblnSuppFound)
                        {
                            iclbPreviousHistory.Add(lbusPAHistory);
                            lblnSuppFound = true;
                        }
                    }
                    else if (lbusPAHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_DependentSupplemental)
                    {
                        if (!lblnDepSuppFound)
                        {
                            iclbPreviousHistory.Add(lbusPAHistory);
                            lblnDepSuppFound = true;
                        }
                    }
                    else if (lbusPAHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental)
                    {
                        if (!lblnSpouseSuppFound)
                        {
                            iclbPreviousHistory.Add(lbusPAHistory);
                            lblnSpouseSuppFound = true;
                        }
                    }
                }
                if (lblnBasicFound && lblnSuppFound && lblnDepSuppFound && lblnSpouseSuppFound) break;
            }
        }

        //wfmDefault.aspx file code conversion - btn_OpenPDF method 
        public string istrDownloadFileName
        {
            get
            {
                DataTable ldtbPathData = utlPassInfo.iobjPassInfo.isrvDBCache.GetCacheData("sgs_system_paths", "path_code = 'CorrPdf'");
                return ldtbPathData.Rows[0]["path_value"].ToString() + "SFN-53855.pdf";
            }
        }
    }
}
