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
using Sagitec.CustomDataObjects;
using System.Linq;
using NeoSpin.DataObjects;
using System.Collections.Generic;
using System.Globalization;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPersonEmploymentDetail : busPersonEmploymentDetailGen
    {
        public bool IsNewMode = false;
        private bool IsNewRecordFound = false;
        private bool IsEmploymentDetailModified = false;
        public DateTime idtRecertificationEffectiveDate
        {
            get
            {
                if (icdoPersonEmploymentDetail.end_date == DateTime.MinValue)
                {
                    return DateTime.Today;
                }
                return icdoPersonEmploymentDetail.end_date;
            }
        }

        public bool iblnIsJobClassElectedOfficial
        {
            get
            {
                if ((icdoPersonEmploymentDetail.job_class_value == busConstant.PersonJobClassNonStateElectedOfficial) ||
                            (icdoPersonEmploymentDetail.job_class_value == busConstant.PersonJobClassStateElectedOfficial))
                {
                    return true;
                }
                return false;
            }
        }

        public bool iblnIsTypeTemporary
        {
            get
            {
                if (icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                {
                    return true;
                }
                return false;
            }
        }

        // PIR 9115 functionality enable/disable property
        public string istrIsPIR9115Enabled
        {
            get
            {
                return busGlobalFunctions.GetData1ByCodeValue(52, "9115", iobjPassInfo);
            }
        }

        //this property will be used in missing contribution in employer report
        private string _istrMissingMonthYear;

        public string istrMissingMonthYear
        {
            get { return _istrMissingMonthYear; }
            set { _istrMissingMonthYear = value; }
        }

        public string istrMissingStartDate { get; set; }

        public string istrMissingEndDate { get; set; }

        public int iintPersonID { get; set; }

        public string istrPersonName { get; set; }

        public string istrDummy { get; set; }

        private bool _iblnIsEmployerJoinPlanAfterEmployment;
        public bool iblnIsEmployerJoinPlanAfterEmployment
        {
            get { return _iblnIsEmployerJoinPlanAfterEmployment; }
            set { _iblnIsEmployerJoinPlanAfterEmployment = value; }
        }
        private Collection<busPersonAccountEmploymentDetail> _iclbAllPersonAccountEmpDtl;

        public Collection<busPersonAccountEmploymentDetail> iclbAllPersonAccountEmpDtl
        {
            get { return _iclbAllPersonAccountEmpDtl; }
            set { _iclbAllPersonAccountEmpDtl = value; }
        }

        private Collection<busPersonAccountEmploymentDetail> _iclbPersonAccountEmpDtl;
        public Collection<busPersonAccountEmploymentDetail> iclbPersonAccountEmpDtl
        {
            get { return _iclbPersonAccountEmpDtl; }
            set { _iclbPersonAccountEmpDtl = value; }
        }

        private Collection<busPersonEmploymentDetail> _icolPersonEmploymentDetail;
        public Collection<busPersonEmploymentDetail> icolPersonEmploymentDetail
        {
            get { return _icolPersonEmploymentDetail; }
            set { _icolPersonEmploymentDetail = value; }
        }
        public bool IsRecertificationDateRequired()
        {
            if (icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOA)
            {
                if (icdoPersonEmploymentDetail.start_date != DateTime.MinValue)
                {
                    if (idtRecertificationEffectiveDate > icdoPersonEmploymentDetail.start_date.AddMonths(12))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public DataTable idtbPlanCacheData { get; set; }

        public bool IsEmployerJoinPlanAfterEmploymentForWaivedStatus(int AintPlanid)
        {
            Collection<busOrgPlan> lclbOrgPlan = new Collection<busOrgPlan>();
            DataTable ldtbList = Select<cdoOrgPlan>(new string[2] { "org_id", "plan_id" },
                                                    new object[2] { ibusPersonEmployment.icdoPersonEmployment.org_id, AintPlanid }, null, null);
            lclbOrgPlan = GetCollection<busOrgPlan>(ldtbList, "icdoOrgPlan");
            foreach (busOrgPlan lobjOrgPlan in lclbOrgPlan)
            {
                if (lobjOrgPlan.icdoOrgPlan.participation_start_date > ibusPersonEmployment.icdoPersonEmployment.start_date)
                {
                    return true;
                }
            }
            return false;
        }
        //This method is to validate Waived Status is Selected for Job Class Elected Official and 
        //Job Type Temporary and when employer joins plan after person employment
        public bool IsStatusWaived()
        {
            if (ibusPersonEmployment.IsNull())
                LoadPersonEmployment();
            if (ibusPersonEmployment.ibusPerson.IsNull())
                ibusPersonEmployment.LoadPerson();
            if (iclbPersonAccountEmpDtl != null)
            {
                foreach (busPersonAccountEmploymentDetail lobjPAEmpDtl in iclbPersonAccountEmpDtl)
                {
                    lobjPAEmpDtl.LoadPlan();
                    lobjPAEmpDtl.LoadPersonEmploymentDetail();
                    if ((lobjPAEmpDtl.ibusPlan.IsRetirementPlan() &&
                         (lobjPAEmpDtl.icdoPersonAccountEmploymentDetail.election_value == busConstant.PlanOptionStatusValueWaived)))
                    {
                        if ((lobjPAEmpDtl.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdMain) ||
                            (lobjPAEmpDtl.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdMain2020) || //PIR 20232 
                            (lobjPAEmpDtl.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdLE) ||
                            (lobjPAEmpDtl.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdLEWithoutPS) ||
                            (lobjPAEmpDtl.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdBCILawEnf) || // pir 7943
                            (lobjPAEmpDtl.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdStatePublicSafety) || (lobjPAEmpDtl.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdNG)) //PIR 25729
                        {
                            //BR - 060 - 04
                            //must allow RTW member to waive the plan 
                            //irrespective of the job class whether Elected Official or not                           
                            bool lblnIsRTWMember = false;
                            int lintRTWPayeeAccountId = 0;
                            lblnIsRTWMember = ibusPersonEmployment.ibusPerson.IsRTWMember(lobjPAEmpDtl.icdoPersonAccountEmploymentDetail.plan_id,
                                busConstant.PayeeStatusForRTW.IgnoreStatus, ref lintRTWPayeeAccountId); // UAT PIR ID 1216

                            //***********************************************
                            if (((!iblnIsJobClassElectedOfficial) && (!iblnIsTypeTemporary) &&
                                (!IsEmployerJoinPlanAfterEmploymentForWaivedStatus(lobjPAEmpDtl.icdoPersonAccountEmploymentDetail.plan_id)))
                                && (!lblnIsRTWMember))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        public string EmploymentDetailsExists()
        {
            DataTable ldtbPersonEmpDtl = Select<cdoPersonEmploymentDetail>(new string[1] { "person_employment_id" },
                                                                 new object[1] { icdoPersonEmploymentDetail.person_employment_id }, null, "start_date desc");
            if (ldtbPersonEmpDtl.Rows.Count > 0)
            {
                return ldtbPersonEmpDtl.Rows[0]["end_date"].ToString();
            }
            return string.Empty;
        }

        // PIR-8298
        public void LoadLastPaycheckDate()
        {
            DataTable ldtPaycheckDate = busNeoSpinBase.Select("cdoPersonEmploymentDetail.LoadLastPaycheckDate", new object[2] { ibusPersonEmployment.icdoPersonEmployment.person_id, ibusPersonEmployment.icdoPersonEmployment.org_id });
            if (ldtPaycheckDate.Rows.Count != 0)
            {
                string strPayCheckDate = ldtPaycheckDate.Rows[0]["DATE_OF_LAST_REGULAR_PAYCHECK"].ToString();
                if (strPayCheckDate.IsNotEmpty())
                {
                    //icdoPersonEmploymentDetail.date_of_last_paycheck = Convert.ToDateTime(strPayCheckDate);
                    if (ibusPersonEmployment.IsNull())
                        LoadPersonEmployment();
                    if (ibusPersonEmployment.icdoPersonEmployment.date_of_last_regular_paycheck == DateTime.MinValue)
                        ibusPersonEmployment.icdoPersonEmployment.date_of_last_regular_paycheck = Convert.ToDateTime(strPayCheckDate);
                }
            }
        }
        // PIR-8298 end

        //This is to check whether there record exists for same period,job class,job type and Organization
        public bool CheckEmploymentDetailOverlapping()
        {
            bool lblnRecordMatch = false;
            if (!icdoPersonEmploymentDetail.job_class_value.IsNullOrEmpty() && !icdoPersonEmploymentDetail.type_value.IsNullOrEmpty())
            {
                DataTable ldtOtherEmploymentDetails = busNeoSpinBase.Select("cdoPersonEmploymentDetail.IsRecordOvelapping",
                    new object[5] {ibusPersonEmployment.icdoPersonEmployment.org_id, icdoPersonEmploymentDetail.job_class_value, 
                               icdoPersonEmploymentDetail.type_value,icdoPersonEmploymentDetail.person_employment_dtl_id,ibusPersonEmployment.icdoPersonEmployment.person_id });
                _icolPersonEmploymentDetail = GetCollection<busPersonEmploymentDetail>(ldtOtherEmploymentDetails, "icdoPersonEmploymentDetail");
                foreach (busPersonEmploymentDetail lobjDtl in _icolPersonEmploymentDetail)
                {
                    if ((busGlobalFunctions.CheckDateOverlapping(icdoPersonEmploymentDetail.start_date,
                                      lobjDtl.icdoPersonEmploymentDetail.start_date, lobjDtl.icdoPersonEmploymentDetail.end_date))
                       ||
                       (busGlobalFunctions.CheckDateOverlapping(icdoPersonEmploymentDetail.end_date,
                                     lobjDtl.icdoPersonEmploymentDetail.start_date, lobjDtl.icdoPersonEmploymentDetail.end_date)))
                    {
                        lblnRecordMatch = true;
                        break;
                    }
                }
            }
            return lblnRecordMatch;
        }
        public bool IsPersonEnrolledForBothPermAndTemp()
        {
            if (iclbPersonAccountEmpDtl != null)
            {
                foreach (busPersonAccountEmploymentDetail lobjPAEmpDtl in iclbPersonAccountEmpDtl)
                {
                    lobjPAEmpDtl.LoadPlan();
                    if ((lobjPAEmpDtl.ibusPlan.IsRetirementPlan()) &&
                       (lobjPAEmpDtl.icdoPersonAccountEmploymentDetail.election_value == busConstant.PlanOptionStatusValueEnrolled))
                    {
                        if (ibusPersonEmployment.ibusPerson != null)
                        {
                            ibusPersonEmployment.ibusPerson.LoadPersonAccountEmploymentDetailEnrolled();
                            foreach (busPersonAccountEmploymentDetail lobjOldPAEmpDetail in ibusPersonEmployment.ibusPerson.icolEnrolledPersonAccountEmploymentDetail)
                            {
                                if ((lobjPAEmpDtl.icdoPersonAccountEmploymentDetail.plan_id == lobjOldPAEmpDetail.icdoPersonAccountEmploymentDetail.plan_id) &&
                                    (lobjOldPAEmpDetail.icdoPersonAccountEmploymentDetail.election_value == busConstant.PlanOptionStatusValueEnrolled) &&
                                    (lobjPAEmpDtl.icdoPersonAccountEmploymentDetail.person_employment_dtl_id != lobjOldPAEmpDetail.icdoPersonAccountEmploymentDetail.person_employment_dtl_id))
                                {
                                    lobjOldPAEmpDetail.LoadPersonEmploymentDetail();
                                    if (lobjOldPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.type_value != icdoPersonEmploymentDetail.type_value)
                                    {
                                        if (icdoPersonEmploymentDetail.start_date != DateTime.MinValue)
                                        {
                                            if (busGlobalFunctions.CheckDateOverlapping(icdoPersonEmploymentDetail.start_date,
                                                    lobjOldPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.start_date,
                                                    lobjOldPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date))
                                            {
                                                return true;
                                            }
                                        }
                                        if (icdoPersonEmploymentDetail.end_date != DateTime.MinValue)
                                        {
                                            if (busGlobalFunctions.CheckDateOverlapping(icdoPersonEmploymentDetail.end_date,
                                                  lobjOldPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.start_date,
                                                  lobjOldPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date))
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        public void SetButtonVisiblityFalse()
        {
            iblnIsPlanDB = false;
            iblnIsPlanDC = false;
            iblnIsPlanHB = false;		//PIR 25920 New DC Plan
            iblnIsPlanDeferredComp = false;
            iblnIsPlanGroupDental = false;
            iblnIsPlanGroupLife = false;
            iblnIsPlanGroupVision = false;
            iblnIsPlanGrouypHealth = false;
            iblnIsPlanMedicare = false;
            iblnIsPlanOther457 = false;
            iblnIsPlanLTC = false;
            iblnIsPlanHMO = false;
            iblnIsPlanEAP = false;
            iblnIsPlanFlexComp = false;
            iblnIsPlanGroupLife = false;
            iblnIsPlanHMO = false;
        }
        //This Method is to find out the Active plans for a Employer and Apply visibility to Plan Enrollment Buttons 
        //in Employment Details Screen.
        public void SetActivePlansByEmployer()
        {
            if (ibusPersonEmployment.IsNull())
                LoadPersonEmployment();
            if (ibusPersonEmployment.ibusPerson.IsNull())
                ibusPersonEmployment.LoadPerson();

            if (iclbPersonAccountEmpDtl == null)
                LoadPersonAccountEmploymentDetailWithInsuranceFilter();

            foreach (busPersonAccountEmploymentDetail lobjPAEmploymentDetail in iclbPersonAccountEmpDtl)
            {
                // Check whether plan is applicable for the current employment detail
                if (lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value == busConstant.PlanOptionStatusValueEnrolled)
                {
                    // Check whether person has enrolled for the plan
                    if (lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.person_account_id == 0)
                    {
                        // if not enrolled ,then set eligible for plan enrollment
                        if ((lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdHP) ||
                            (lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdJobService) ||
                            (lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdJudges) ||
                            (lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdLE) ||
                            (lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdLEWithoutPS) ||
                            (lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdMain) ||
                            (lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdMain2020) || //PIR 20232 
                            (lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdOasis) ||
                            (lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdNG) ||
                            (lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdBCILawEnf) || //pir 7943
                            (lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdStatePublicSafety)) //PIR 25729
                        {
                            iblnIsPlanDB = true;
                        }
                        if (lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdTFFR)
                        {
                            iblnIsPlanTFFR = true;
                        }
                        if (lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdTIAA)
                        {
                            iblnIsPlanTIAA = true;
                        }
                        else if ((lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDC) ||
                           (lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDC2020)) //PIR 20232
                        {
                            iblnIsPlanDC = true;
                        }
                        else if ((lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDeferredCompensation))
                        {
                            iblnIsPlanDeferredComp = true;
                        }
                        else if ((lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDental))
                        {
                            iblnIsPlanGroupDental = true;
                        }
                        else if ((lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdGroupHealth))
                        {
                            iblnIsPlanGrouypHealth = true;
                        }
                        else if ((lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdVision))
                        {
                            iblnIsPlanGroupVision = true;
                        }
                        else if (lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdMedicarePartD)
                        {
                            iblnIsPlanMedicare = true;
                        }
                        else if (lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdOther457)
                        {
                            iblnIsPlanOther457 = true;
                        }
                        else if ((lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdLTC))
                        {
                            iblnIsPlanLTC = true;
                        }
                        else if (lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdHMO)
                        {
                            iblnIsPlanHMO = true;
                        }
                        else if ((lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdEAP))
                        {
                            iblnIsPlanEAP = true;
                        }
                        else if (lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdFlex)
                        {
                            iblnIsPlanFlexComp = true;
                        }
                        else if (lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdGroupLife)
                        {
                            iblnIsPlanGroupLife = true;
                        }
						//PIR 25920 New DC Plan
                        else if (lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDC2025)
                        {
                            iblnIsPlanHB = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This will insert all the employer offered plans into Person Account Employment Detail
        /// If the Plan Entry already exists, it will not insert
        /// This is useful when the employer joins and then the employer introduce new plans
        /// </summary>
        public void InsertNewOfferedPlansIfAvailable()
        {
            if (ibusPersonEmployment.IsNull()) LoadPersonEmployment();
            if (ibusPersonEmployment.ibusOrganization.iclbOrganizationOfferedPlans == null)
                LoadPlansOffered();
            ////PIR 25920 New Plan DC 2025 load again with correct date from detail start date previously load with max date, it handle exception case if other than main plan exists and no other DB plan then DC 2025 is eligible with employment dtl start date
            if (ibusPersonEmployment.ibusOrganization.iclbOrganizationOfferedPlans.Any(lobjOrgPlan => lobjOrgPlan.icdoOrgPlan.plan_id != busConstant.PlanIdDC2025))
            {
                ibusPersonEmployment.ibusOrganization.LoadOrganizationOfferedPlans(icdoPersonEmploymentDetail.start_date, LoadEarliestContributingEmpDetailStartDate(), ibusPersonEmployment.icdoPersonEmployment.person_id);
            }
            foreach (busOrgPlan lobjOrgPlan in ibusPersonEmployment.ibusOrganization.iclbOrganizationOfferedPlans)
            {
                if (lobjOrgPlan.ibusPlan == null)
                    lobjOrgPlan.LoadPlanInfo();
					//PIR 25920 New Plan DC 2025 if DC25 plan is not in date range skip the isertion for the plan
                if (lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdDC2025 && !busGlobalFunctions.CheckDateOverlapping(icdoPersonEmploymentDetail.start_date, lobjOrgPlan.icdoOrgPlan.participation_start_date, lobjOrgPlan.icdoOrgPlan.participation_end_date))
                    continue;
                if (lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdDC || lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdDC2020) //PIR 20232
                {
                    if (lobjOrgPlan.IsPlanRestricted())
                    {
                        if (IsPersonEnrolledInDC(lobjOrgPlan.icdoOrgPlan.plan_id))
                        {
                            CheckAndInsertIfMemberAlreadyEnrolledInPlan(lobjOrgPlan.icdoOrgPlan.plan_id);
                            continue;
                        }
                    }
                }
                if (icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent)
                {
                    if (((ibusPersonEmployment.ibusOrganization.icdoOrganization.emp_category_value == busConstant.EmployerCategoryState)
                        && (lobjOrgPlan.ibusPlan.icdoPlan.benefit_type_value != busConstant.PlanBenefitTypeInsurance))
                        || (ibusPersonEmployment.ibusOrganization.icdoOrganization.emp_category_value != busConstant.EmployerCategoryState))
                    {
                        if (!icdoPersonEmploymentDetail.job_class_value.IsNullOrEmpty() && !icdoPersonEmploymentDetail.type_value.IsNullOrEmpty())
                        {
                            DataTable ldtbPlanjobClass = Select<cdoPlanJobClassCrossref>(new string[3] { "plan_id", "JOB_CLASS_VALUE", "JOB_TYPE_VALUE" },
                                               new object[3] { lobjOrgPlan.icdoOrgPlan.plan_id, icdoPersonEmploymentDetail.job_class_value, icdoPersonEmploymentDetail.type_value }, null, null);
                            //Checking whether plan is eligible for given job class and job type
                            if (ldtbPlanjobClass.Rows.Count > 0)
                            {
                                CheckAndInsertIfMemberAlreadyEnrolledInPlan(lobjOrgPlan.icdoOrgPlan.plan_id);
                            }
                        }
                    }
                    else if ((ibusPersonEmployment.ibusOrganization.icdoOrganization.emp_category_value == busConstant.EmployerCategoryState)
                        && ((lobjOrgPlan.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeInsurance)
                        || (lobjOrgPlan.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeFlex)))
                    { CheckAndInsertIfMemberAlreadyEnrolledInPlan(lobjOrgPlan.icdoOrgPlan.plan_id); }
                }
                else
                {
                    if (!icdoPersonEmploymentDetail.job_class_value.IsNullOrEmpty() && !icdoPersonEmploymentDetail.type_value.IsNullOrEmpty())
                    {
                        DataTable ldtbPlanjobClass = Select<cdoPlanJobClassCrossref>(new string[3] { "plan_id", "JOB_CLASS_VALUE", "JOB_TYPE_VALUE" },
                                           new object[3] { lobjOrgPlan.icdoOrgPlan.plan_id, icdoPersonEmploymentDetail.job_class_value, icdoPersonEmploymentDetail.type_value }, null, null);
                        //Checking whether plan is eligible for given job class and job type
                        if (ldtbPlanjobClass.Rows.Count > 0)
                        {
                            CheckAndInsertIfMemberAlreadyEnrolledInPlan(lobjOrgPlan.icdoOrgPlan.plan_id);
                        }
                    }
                }
            }
            if (IsNewRecordFound)
                LoadPersonAccountEmploymentDetailWithInsuranceFilter();
        }

        private void CheckAndInsertIfMemberAlreadyEnrolledInPlan(int aintPlanID)
        {
            if (iclbAllPersonAccountEmpDtl == null)
                LoadAllPersonAccountEmploymentDetails();
            if (!iclbAllPersonAccountEmpDtl.Any(i => i.icdoPersonAccountEmploymentDetail.plan_id == aintPlanID))
            {
                InsertPersonAccountEmploymentDetail(aintPlanID);
                IsNewRecordFound = true;
            }
        }

        /// <summary>
        /// This method will insert all the employer offered plans at first time
        /// </summary>
        public void InsertEmployerOfferedPlans()
        {
            if (ibusPersonEmployment.IsNull()) LoadPersonEmployment();
            if (ibusPersonEmployment.ibusOrganization.iclbOrganizationOfferedPlans == null)
                LoadPlansOffered();
            //PIR 25920 New Plan DC 2025 load again with correct date from detail start date previously load with max date, it handle exception case if other than main plan exists and no other DB plan then DC 2025 is eligible with employment dtl start date
            if (ibusPersonEmployment.ibusOrganization.iclbOrganizationOfferedPlans.Any(lobjOrgPlan => lobjOrgPlan.icdoOrgPlan.plan_id != busConstant.PlanIdDC2025))
            {
                ibusPersonEmployment.ibusOrganization.LoadOrganizationOfferedPlans(icdoPersonEmploymentDetail.start_date, LoadEarliestContributingEmpDetailStartDate(), ibusPersonEmployment.icdoPersonEmployment.person_id);
            }
            foreach (busOrgPlan lobjOrgPlan in ibusPersonEmployment.ibusOrganization.iclbOrganizationOfferedPlans)
            {
                if (lobjOrgPlan.ibusPlan.IsNull())
                    lobjOrgPlan.LoadPlanInfo();
					//PIR 25920 New Plan DC 2025 if DC25 plan is not in date range skip the isertion for the plan
                if (lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdDC2025 && !busGlobalFunctions.CheckDateOverlapping(icdoPersonEmploymentDetail.start_date, lobjOrgPlan.icdoOrgPlan.participation_start_date, lobjOrgPlan.icdoOrgPlan.participation_end_date))
                    continue;
                if (lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdDC || lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdDC2020) //PIR 20232
                {
                    if (lobjOrgPlan.IsPlanRestricted())
                    {
                        if (IsPersonEnrolledInDC(lobjOrgPlan.icdoOrgPlan.plan_id))
                        {
                            InsertPersonAccountEmploymentDetail(lobjOrgPlan.icdoOrgPlan.plan_id);
                            continue;
                        }
                    }
                }

                if (icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent)
                {
                    if (((ibusPersonEmployment.ibusOrganization.icdoOrganization.emp_category_value == busConstant.EmployerCategoryState)
                        && (lobjOrgPlan.ibusPlan.icdoPlan.benefit_type_value != busConstant.PlanBenefitTypeInsurance))
                        || (ibusPersonEmployment.ibusOrganization.icdoOrganization.emp_category_value != busConstant.EmployerCategoryState))
                    {
                        if (!icdoPersonEmploymentDetail.job_class_value.IsNullOrEmpty() && !icdoPersonEmploymentDetail.type_value.IsNullOrEmpty())
                        {
                            DataTable ldtbPlanjobClass = Select<cdoPlanJobClassCrossref>(new string[3] { "plan_id", "JOB_CLASS_VALUE", "JOB_TYPE_VALUE" },
                                               new object[3] { lobjOrgPlan.icdoOrgPlan.plan_id, icdoPersonEmploymentDetail.job_class_value, icdoPersonEmploymentDetail.type_value }, null, null);
                            //Checking whether plan is eligible for given job class and job type
                            if (ldtbPlanjobClass.Rows.Count > 0)
                            {
                                InsertPersonAccountEmploymentDetail(lobjOrgPlan.icdoOrgPlan.plan_id);                                
                            }
                        }
                    }
                    else if ((ibusPersonEmployment.ibusOrganization.icdoOrganization.emp_category_value == busConstant.EmployerCategoryState)
                        && ((lobjOrgPlan.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeInsurance)
                        || (lobjOrgPlan.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeFlex)))
                    { InsertPersonAccountEmploymentDetail(lobjOrgPlan.icdoOrgPlan.plan_id); }
                }
                // PIR 9376
                else
                {
                    if (!icdoPersonEmploymentDetail.job_class_value.IsNullOrEmpty() && !icdoPersonEmploymentDetail.type_value.IsNullOrEmpty())
                    {
                        DataTable ldtbPlanjobClass = Select<cdoPlanJobClassCrossref>(new string[3] { "plan_id", "JOB_CLASS_VALUE", "JOB_TYPE_VALUE" },
                                           new object[3] { lobjOrgPlan.icdoOrgPlan.plan_id, icdoPersonEmploymentDetail.job_class_value, icdoPersonEmploymentDetail.type_value }, null, null);
                        //Checking whether plan is eligible for given job class and job type
                        if (ldtbPlanjobClass.Rows.Count > 0)
                        {
                            InsertPersonAccountEmploymentDetail(lobjOrgPlan.icdoOrgPlan.plan_id);
                        }
                    }
                }
                //PIR 25920 New Plan DC 2025

                if (lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdDC2025 && lobjOrgPlan.ibusPlan.IsHBRetirementPlan())
                    UpdatePersonAccountContributionEndDate();   //ChangeADECwhileTempToPerm(lobjOrgPlan.icdoOrgPlan.plan_id);
            }
        }
        public void UpdatePersonAccountContributionEndDate()
        {
            ibusPersonEmployment.LoadPersonEmploymentDetail();
            if (ibusPersonEmployment.icolPersonEmploymentDetail.IsNotNull())
            {
                if (ibusPersonEmployment.ibusPerson.IsNull()) ibusPersonEmployment.LoadPerson();
                ibusPersonEmployment.ibusPerson.icolPersonAccount = null;
                ibusPersonEmployment.ibusPerson.LoadDC2025PersonAccount();
                if (ibusPersonEmployment.ibusPerson.ibusDC2025PersonAccount.icdoPersonAccount.person_account_id > 0)
                {
                    DataTable ldtPersonAccount = Select<cdoPersonAccount>(new string[1] { enmPersonAccount.person_account_id.ToString() },
                                           new object[1] { ibusPersonEmployment.ibusPerson.ibusDC2025PersonAccount.icdoPersonAccount.person_account_id }, null, null);
                    if (ldtPersonAccount.IsNotNull() && ldtPersonAccount.Rows.Count > 0)
                    {
                        int lintAllowDays = icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary ? 180 : 30;
                        bool lblnAllowUpdate = false;
                        if (icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                        {
                            if (ibusPersonEmployment.ibusPerson.ibusDC2025PersonAccount.icdoPersonAccount.addl_ee_contribution_percent_temp_end_date == DateTime.MinValue)
                            {
                                ibusPersonEmployment.ibusPerson.ibusDC2025PersonAccount.icdoPersonAccount.addl_ee_contribution_percent_temp_end_date = icdoPersonEmploymentDetail.start_date.AddDays(lintAllowDays);
                                lblnAllowUpdate = true;
                            }
                            if (ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent.ToString()] == DBNull.Value &&
                                ibusPersonEmployment.ibusPerson.ibusDC2025PersonAccount.icdoPersonAccount.addl_ee_contribution_percent_end_date < icdoPersonEmploymentDetail.start_date)
                            {
                                ibusPersonEmployment.ibusPerson.ibusDC2025PersonAccount.icdoPersonAccount.addl_ee_contribution_percent_end_date = DateTime.MinValue;
                                lblnAllowUpdate = true;
                            }
                        }
                        else if (icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent)
                        {
                            if (ibusPersonEmployment.ibusPerson.ibusDC2025PersonAccount.icdoPersonAccount.addl_ee_contribution_percent_end_date == DateTime.MinValue)
                            {
                                ibusPersonEmployment.ibusPerson.ibusDC2025PersonAccount.icdoPersonAccount.addl_ee_contribution_percent_end_date = icdoPersonEmploymentDetail.start_date.AddDays(lintAllowDays);
                                lblnAllowUpdate = true;
                            }
                            if (ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent_temp.ToString()] == DBNull.Value &&
                            ibusPersonEmployment.ibusPerson.ibusDC2025PersonAccount.icdoPersonAccount.addl_ee_contribution_percent_temp_end_date < icdoPersonEmploymentDetail.start_date)
                            {
                                ibusPersonEmployment.ibusPerson.ibusDC2025PersonAccount.icdoPersonAccount.addl_ee_contribution_percent_temp_end_date = DateTime.MinValue;
                                lblnAllowUpdate = true;
                            }
                        }
                        if (lblnAllowUpdate)
                            ibusPersonEmployment.ibusPerson.ibusDC2025PersonAccount.icdoPersonAccount.Update();
                    }
                }
            }
        }
        /// <summary>
        /// update the additional ee contribution while change the employment from Temp to permanent and reduce the contribution to 3 if it was greater than 3
        /// </summary>
        /// <param name="aintPlanID">work with DC25 plan</param>
        public void ChangeADECwhileTempToPerm(int aintPlanID)
        {
            ibusPersonEmployment.LoadPersonEmploymentDetail();
            if(ibusPersonEmployment.icolPersonEmploymentDetail.IsNotNull() && ibusPersonEmployment.icolPersonEmploymentDetail.Count > 1)
            { 
                if (ibusPersonEmployment.ibusPerson.IsNull()) ibusPersonEmployment.LoadPerson();
                ibusPersonEmployment.ibusPerson.LoadDC2025PersonAccount();
                //if(ibusPersonEmployment.ibusPerson.ibusDC2025PersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled)
                {
                    DataTable ldtPersonAccount = Select<cdoPersonAccount>(new string[1] { enmPersonAccount.person_account_id.ToString() },
                                       new object[1] { ibusPersonEmployment.ibusPerson.ibusDC2025PersonAccount.icdoPersonAccount.person_account_id }, null, null);
                    if (ldtPersonAccount.IsNotNull() && ldtPersonAccount.Rows.Count > 0)
                    {
                        if (icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                        {
                            if(ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent_temp.ToString()] != DBNull.Value)
                                UpdateADECWhileEmpChange((ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent_temp.ToString()].ToString()) == "0" ?
                                -1 : ibusPersonEmployment.ibusPerson.ibusDC2025PersonAccount.icdoPersonAccount.addl_ee_contribution_percent_temp);
                        }
                        if (icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent)
                        {
                            if (ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent.ToString()] != DBNull.Value)
                                UpdateADECWhileEmpChange((ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent.ToString()].ToString()) == "0" ?
                                -1 : ibusPersonEmployment.ibusPerson.ibusDC2025PersonAccount.icdoPersonAccount.addl_ee_contribution_percent);
                        }
                    }
                }
            }

        }
        public void UpdateADECWhileEmpChange(int aintAddlEEContributionPercent)
        {
            busWssPersonAccountEnrollmentRequest lbusWssPersonAccountEnrollmentRequest = new busWssPersonAccountEnrollmentRequest { icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest() };
            lbusWssPersonAccountEnrollmentRequest.iintAddlEEContributionPercent = aintAddlEEContributionPercent;
            lbusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id = icdoPersonEmploymentDetail.person_employment_dtl_id;
            //lbusWssPersonAccountEnrollmentRequest.iblnIsOnlyADECUpdate = true;
            //lbusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.plan_id = aintPlanID;
            lbusWssPersonAccountEnrollmentRequest.ibusPersonAccount = ibusPersonEmployment.ibusPerson.ibusDC2025PersonAccount;
            lbusWssPersonAccountEnrollmentRequest.ibusPersonAccount.icdoPersonAccount.person_employment_dtl_id = icdoPersonEmploymentDetail.person_employment_dtl_id;

            int lintAllowDays = icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary ? 180 : 30;
            if (icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary &&
                        ibusPersonEmployment.ibusPerson.ibusDC2025PersonAccount.icdoPersonAccount.addl_ee_contribution_percent_temp_end_date == DateTime.MinValue)
            {
                lbusWssPersonAccountEnrollmentRequest.ibusPersonAccount.icdoPersonAccount.addl_ee_contribution_percent_temp_end_date = icdoPersonEmploymentDetail.start_date.AddDays(lintAllowDays);
            }
            else if (icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent &&
                   ibusPersonEmployment.ibusPerson.ibusDC2025PersonAccount.icdoPersonAccount.addl_ee_contribution_percent_end_date == DateTime.MinValue)
            {
                lbusWssPersonAccountEnrollmentRequest.ibusPersonAccount.icdoPersonAccount.addl_ee_contribution_percent_end_date = icdoPersonEmploymentDetail.start_date.AddDays(lintAllowDays);
            }

            if (lbusWssPersonAccountEnrollmentRequest.ibusPersonAccount.IsNotNull() && lbusWssPersonAccountEnrollmentRequest.ibusPersonAccount.icdoPersonAccount.IsNotNull())
                lbusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.plan_id = lbusWssPersonAccountEnrollmentRequest.ibusPersonAccount.icdoPersonAccount.plan_id;
            lbusWssPersonAccountEnrollmentRequest.iblnIsFromEmployment = true;
            lbusWssPersonAccountEnrollmentRequest.btnUpdatePersonAccountADECPercentage_Click();
        }
        public void ChangeADECwhileTempToPermReduceADEC(int aintPlanID)
        {
            if (icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent)
            {
                if(ibusPersonEmployment.IsNull()) LoadPersonEmployment();
                if(ibusPersonEmployment.icolPersonEmploymentDetail.IsNull() || (ibusPersonEmployment.icolPersonEmploymentDetail.IsNotNull() && ibusPersonEmployment.icolPersonEmploymentDetail.Count == 0))
                    ibusPersonEmployment.LoadPersonEmploymentDetail();

                //busPersonAccountEmploymentDetail lbusPersonAccountEmploymentDetail = new busPersonAccountEmploymentDetail();
                //DataTable ldtbPersonAccountEmploymentDetail = Select<cdoPersonAccountEmploymentDetail>(new string[2] { "PLAN_ID", "PERSON_EMPLOYMENT_DTL_ID" }, new object[2] { aintPlanID, icdoPersonEmploymentDetail.person_employment_dtl_id }, null, "person_employment_dtl_id DESC");
                //if (ldtbPersonAccountEmploymentDetail.IsNotNull() && ldtbPersonAccountEmploymentDetail.Rows.Count > 0)
                //    lbusPersonAccountEmploymentDetail = GetCollection<busPersonAccountEmploymentDetail>(ldtbPersonAccountEmploymentDetail, "icdoPersonAccountEmploymentDetail")?.FirstOrDefault();

                //LoadMemberType(icdoPersonEmploymentDetail.start_date, lbusPersonAccountEmploymentDetail);

                //string lstrMemberType = icdoPersonEmploymentDetail.derived_member_type_value.IsNull() ? string.Empty : icdoPersonEmploymentDetail.derived_member_type_value;
                if (ibusPersonEmployment.icolPersonEmploymentDetail.IsNotNull() && ibusPersonEmployment.icolPersonEmploymentDetail.Count >= 1 )
                {
                    busPersonEmploymentDetail lbusPersonEmploymentDetailPrevPerm = null;
                    if(ibusPersonEmployment.icolPersonEmploymentDetail.Any(lobjPersonEmpDetail => lobjPersonEmpDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent))
                        lbusPersonEmploymentDetailPrevPerm = ibusPersonEmployment.icolPersonEmploymentDetail.Where(lobjPersonEmpDetail => lobjPersonEmpDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent && lobjPersonEmpDetail.icdoPersonEmploymentDetail.end_date.IsNotNull()).First();
                    busPersonEmploymentDetail lbusPersonEmploymentDetail = ibusPersonEmployment.icolPersonEmploymentDetail.Count > 1 ? ibusPersonEmployment.icolPersonEmploymentDetail[1] : ibusPersonEmployment.icolPersonEmploymentDetail[0];
                    //if got the employment is permanant then set previous value
                    if (lbusPersonEmploymentDetailPrevPerm.IsNotNull())
                    {
                        int lintMaxAddlPercentageByPreviousHistory = 0;
                        if (lbusPersonEmploymentDetailPrevPerm.ibusPersonEmployment.IsNull()) lbusPersonEmploymentDetailPrevPerm.LoadPersonEmployment();
                        if (lbusPersonEmploymentDetailPrevPerm.ibusPersonEmployment.ibusPerson.IsNull()) lbusPersonEmploymentDetailPrevPerm.ibusPersonEmployment.LoadPerson();
                        //if (lbusPersonEmploymentDetailPrevPerm.ibusPersonEmployment.ibusPerson.icolPersonAccountByPlan.IsNull())
                        lbusPersonEmploymentDetailPrevPerm.ibusPersonEmployment.ibusPerson.LoadPersonAccountByPlan(aintPlanID);
                        busPersonAccount lbusPersonAccount = new busPersonAccount();
                        if (lbusPersonEmploymentDetailPrevPerm.ibusPersonEmployment.ibusPerson.icolPersonAccountByPlan.Count > 0 )
                            lbusPersonAccount = lbusPersonEmploymentDetailPrevPerm.ibusPersonEmployment.ibusPerson.icolPersonAccountByPlan.First();
                        if (lbusPersonAccount.IsNotNull() && lbusPersonAccount.icdoPersonAccount.IsNotNull())
                        {
                            lbusPersonAccount.icdoPersonAccount.person_employment_dtl_id = lbusPersonEmploymentDetailPrevPerm.icdoPersonEmploymentDetail.person_employment_dtl_id;
                            lbusPersonAccount.LoadPersonAccountRetirement();
                            Collection<cdoCodeValue> lclbEligibleADECAmountValue = new Collection<cdoCodeValue>();
                            lclbEligibleADECAmountValue = lbusPersonAccount.ibusPersonAccountRetirement.LoadADECAmountValues();
                            if (lclbEligibleADECAmountValue.IsNotNull() && lclbEligibleADECAmountValue.Count > 0)
                            {
                                if (lbusPersonAccount.ibusPersonAccountRetirement.ibusHistoryAsofDate == null)
                                    //lbusPersonAccount.ibusPersonAccountRetirement.LoadPreviousHistory();
                                    lbusPersonAccount.ibusPersonAccountRetirement.LoadEnrolledHistoryByMonthYear(lbusPersonEmploymentDetailPrevPerm.icdoPersonEmploymentDetail.end_date.AddDays(1));
                                lintMaxAddlPercentageByPreviousHistory = lbusPersonAccount.ibusPersonAccountRetirement.ibusHistoryAsofDate.icdoPersonAccountRetirementHistory.addl_ee_contribution_percent;
                                //decimal iintMaxAddlPercentageByMemberType = GetMaxEEPreTaxAdditionalPercentage(lstrMemberType, icdoPersonEmploymentDetail.start_date, aintPlanID);
                                lintMaxAddlPercentageByPreviousHistory = lclbEligibleADECAmountValue.Count - 1 < lintMaxAddlPercentageByPreviousHistory ?
                                    lclbEligibleADECAmountValue.Count - 1 : lintMaxAddlPercentageByPreviousHistory;
                            }
                        }
                        busWssPersonAccountEnrollmentRequest lbusWssPersonAccountEnrollmentRequest = new busWssPersonAccountEnrollmentRequest { icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest() };
                        lbusWssPersonAccountEnrollmentRequest.iintAddlEEContributionPercent = Convert.ToInt32(lintMaxAddlPercentageByPreviousHistory);
                        lbusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id = icdoPersonEmploymentDetail.person_employment_dtl_id;
                        //lbusWssPersonAccountEnrollmentRequest.iblnIsOnlyADECUpdate = true;
                        //lbusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.plan_id = aintPlanID;
                        lbusWssPersonAccountEnrollmentRequest.ibusPersonAccount = lbusPersonAccount;
                        if(lbusWssPersonAccountEnrollmentRequest.ibusPersonAccount.IsNotNull() && lbusWssPersonAccountEnrollmentRequest.ibusPersonAccount.icdoPersonAccount.IsNotNull())
                            lbusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.plan_id = lbusWssPersonAccountEnrollmentRequest.ibusPersonAccount.icdoPersonAccount.plan_id;
                        lbusWssPersonAccountEnrollmentRequest.iblnIsFromEmployment = true;
                        lbusWssPersonAccountEnrollmentRequest.btnUpdatePersonAccountADECPercentage_Click();
                    }
                    //if previous record is temporary
                    else if (lbusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                    {
                        if (lbusPersonEmploymentDetail.ibusPersonEmployment.IsNull()) lbusPersonEmploymentDetail.LoadPersonEmployment();
                        if (lbusPersonEmploymentDetail.ibusPersonEmployment.ibusPerson.IsNull()) lbusPersonEmploymentDetail.ibusPersonEmployment.LoadPerson();
                        //if (lbusPersonEmploymentDetail.ibusPersonEmployment.ibusPerson.icolPersonAccountByPlan.IsNull())
                            lbusPersonEmploymentDetail.ibusPersonEmployment.ibusPerson.LoadPersonAccountByPlan(aintPlanID);
                        if (lbusPersonEmploymentDetail.ibusPersonEmployment.ibusPerson.icolPersonAccountByPlan.IsNotNull() && lbusPersonEmploymentDetail.ibusPersonEmployment.ibusPerson.icolPersonAccountByPlan.Count > 0)
                        {
                            busPersonAccount lbusPersonAccount = lbusPersonEmploymentDetail.ibusPersonEmployment.ibusPerson.icolPersonAccountByPlan.First();
                            if (lbusPersonAccount.IsNotNull() && lbusPersonAccount.icdoPersonAccount.IsNotNull())
                            {
                                lbusPersonAccount.icdoPersonAccount.person_employment_dtl_id = lbusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;
                                lbusPersonAccount.LoadPersonAccountRetirement();
                                if (lbusPersonAccount.ibusPersonAccountRetirement.lblnIsVisibleADECAmountList)
                                {
                                    if (lbusPersonAccount.ibusPersonAccountRetirement.ibusHistory == null)
                                        lbusPersonAccount.ibusPersonAccountRetirement.LoadPreviousHistory();

                                    busPersonAccountEmploymentDetail lbusPersonAccountEmploymentDetail = new busPersonAccountEmploymentDetail();
                                    DataTable ldtbPersonAccountEmploymentDetail = Select<cdoPersonAccountEmploymentDetail>(new string[2] { "PLAN_ID", "PERSON_EMPLOYMENT_DTL_ID" }, new object[2] { aintPlanID, lbusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id }, null, "person_employment_dtl_id DESC");
                                    if (ldtbPersonAccountEmploymentDetail.IsNotNull() && ldtbPersonAccountEmploymentDetail.Rows.Count > 0)
                                        lbusPersonAccountEmploymentDetail = GetCollection<busPersonAccountEmploymentDetail>(ldtbPersonAccountEmploymentDetail, "icdoPersonAccountEmploymentDetail")?.FirstOrDefault();

                                    LoadMemberType(icdoPersonEmploymentDetail.start_date, lbusPersonAccountEmploymentDetail);

                                    string lstrMemberType = icdoPersonEmploymentDetail.derived_member_type_value.IsNull() ? string.Empty : icdoPersonEmploymentDetail.derived_member_type_value;

                                    decimal iintMaxAddlPercentageByMemberType = GetMaxEEPreTaxAdditionalPercentage(lstrMemberType, icdoPersonEmploymentDetail.start_date, aintPlanID);

                                    if (lbusPersonAccount.ibusPersonAccountRetirement.ibusHistory.icdoPersonAccountRetirementHistory.addl_ee_contribution_percent > iintMaxAddlPercentageByMemberType)
                                    {
                                        //lbusPersonAccount.icdoPersonAccount.addl_ee_contribution_percent = 3;
                                        //lbusPersonAccount.icdoPersonAccount.modified_by = iobjPassInfo.istrUserID;
                                        //lbusPersonAccount.icdoPersonAccount.Update();
                                        busWssPersonAccountEnrollmentRequest lbusWssPersonAccountEnrollmentRequest = new busWssPersonAccountEnrollmentRequest { icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest() };
                                        lbusWssPersonAccountEnrollmentRequest.iintAddlEEContributionPercent = Convert.ToInt32(iintMaxAddlPercentageByMemberType);
                                        lbusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id = icdoPersonEmploymentDetail.person_employment_dtl_id;
                                        //lbusWssPersonAccountEnrollmentRequest.iblnIsOnlyADECUpdate = true;
                                        //lbusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.plan_id = aintPlanID;
                                        lbusWssPersonAccountEnrollmentRequest.ibusPersonAccount = lbusPersonAccount;
                                        lbusWssPersonAccountEnrollmentRequest.iblnIsFromEmployment = true;
                                        lbusWssPersonAccountEnrollmentRequest.btnUpdatePersonAccountADECPercentage_Click();

                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        public decimal GetMaxEEPreTaxAdditionalPercentage(string astrMemberTypeValue, DateTime adtEffectiveDate, int aintPlanId)
        {
            busPlanRetirementRate lobjPlanRetirement = new busPlanRetirementRate();
            lobjPlanRetirement = busRateHelper.GetRatesForMemberTypeAndEffectiveDate(astrMemberTypeValue, adtEffectiveDate, aintPlanId);
            if (lobjPlanRetirement.IsNotNull())
            {
                return lobjPlanRetirement.icdoPlanRetirementRate.ee_pretax_addl + lobjPlanRetirement.icdoPlanRetirementRate.ee_post_tax_addl;
            }
            else
                return 0.00m;            
        }
        public void InsertPersonAccountEmploymentDetail(int aintPlanID)
        {
            //PIR 16740
            if (aintPlanID != busConstant.PlanIdMedicarePartD)
            {
                busPersonAccountEmploymentDetail lobjPersonAccountEmpDtl = new busPersonAccountEmploymentDetail();
                lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail = new cdoPersonAccountEmploymentDetail();
                lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.person_employment_dtl_id = icdoPersonEmploymentDetail.person_employment_dtl_id;
                lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.plan_id = aintPlanID;
                lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.Insert();
            }
        }

        public void LoadAllPersonAccountEmploymentDetails(bool alblnLoadPersonAccount = false)
        {
            DataTable ldtbPAEmpDtl = Select<cdoPersonAccountEmploymentDetail>(
                new string[1] { "person_employment_dtl_id" },
                new object[1] { icdoPersonEmploymentDetail.person_employment_dtl_id }, null, null);
            iclbAllPersonAccountEmpDtl = GetCollection<busPersonAccountEmploymentDetail>(ldtbPAEmpDtl, "icdoPersonAccountEmploymentDetail");
            if (alblnLoadPersonAccount)
            {
                foreach (busPersonAccountEmploymentDetail lobjPersonAccEmpDtl in iclbAllPersonAccountEmpDtl)
                {
                    lobjPersonAccEmpDtl.LoadPersonAccount();
                }
            }
        }

        //this method is defined - to avoid the error on deleting the employment detail record
        public void LoadAllPersonAccountEmploymentDetails()
        {
            LoadAllPersonAccountEmploymentDetails(false);
        }

        public void LoadEnrolledPersonAccountEmploymentDetails()
        {
            iclbPersonAccountEmpDtl = new Collection<busPersonAccountEmploymentDetail>();

            if (iclbAllPersonAccountEmpDtl == null)
                LoadAllPersonAccountEmploymentDetails();

            var lenuEnrolledPAEmpDetail =
                iclbAllPersonAccountEmpDtl.Where(
                    lbusPAED => lbusPAED.icdoPersonAccountEmploymentDetail.election_value == busConstant.PersonAccountElectionValueEnrolled);

            if (lenuEnrolledPAEmpDetail != null)
            {
                foreach (var lbusPersonAccountEmploymentDetail in lenuEnrolledPAEmpDetail)
                {
                    iclbPersonAccountEmpDtl.Add(lbusPersonAccountEmploymentDetail);
                }                
            }
        }

        public void LoadEnrolledPersonAccountEmploymentDetailsWithPersonAccount()
        {
            iclbPersonAccountEmpDtl = new Collection<busPersonAccountEmploymentDetail>();

            if (iclbAllPersonAccountEmpDtl == null)
                LoadAllPersonAccountEmploymentDetails();

            var lenuEnrolledPAEmpDetail =
                iclbAllPersonAccountEmpDtl.Where(
                    lbusPAED => lbusPAED.icdoPersonAccountEmploymentDetail.election_value == busConstant.PersonAccountElectionValueEnrolled && lbusPAED.icdoPersonAccountEmploymentDetail.person_account_id > 0);

            if (lenuEnrolledPAEmpDetail != null)
            {
                foreach (var lbusPersonAccountEmploymentDetail in lenuEnrolledPAEmpDetail)
                {
                    iclbPersonAccountEmpDtl.Add(lbusPersonAccountEmploymentDetail);
                }
            }
        }

        // Load Person Account Employment Details for the employer and 
        //if the insurance plans enrolled under one employment will not appear for the other employments
        public void LoadPersonAccountEmploymentDetailWithInsuranceFilter()
        {
            iclbPersonAccountEmpDtl = new Collection<busPersonAccountEmploymentDetail>();

            if (iclbAllPersonAccountEmpDtl == null || iclbAllPersonAccountEmpDtl.Count == 0 || IsNewRecordFound) // UAT PIR ID 781
                LoadAllPersonAccountEmploymentDetails();

            if (ibusPersonEmployment == null)
                LoadPersonEmployment();
            if (ibusPersonEmployment.ibusPerson == null)
                ibusPersonEmployment.LoadPerson();

            if (ibusPersonEmployment.ibusPerson != null)
            {
                if (ibusPersonEmployment.ibusPerson.icolEnrolledPersonAccountEmploymentDetail == null)
                    ibusPersonEmployment.ibusPerson.LoadPersonAccountEmploymentDetailEnrolled();

                foreach (busPersonAccountEmploymentDetail lobjPersonAccountEmploymentDetail in iclbAllPersonAccountEmpDtl)
                {
                    if (lobjPersonAccountEmploymentDetail.ibusPlan == null)
                        lobjPersonAccountEmploymentDetail.LoadPlan();
                    if (lobjPersonAccountEmploymentDetail.ibusPersonAccount == null)
                        lobjPersonAccountEmploymentDetail.LoadPersonAccount();
                    if (lobjPersonAccountEmploymentDetail.ibusEmploymentDetail == null)
                        lobjPersonAccountEmploymentDetail.LoadPersonEmploymentDetail();

                    if (lobjPersonAccountEmploymentDetail.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeInsurance)
                    {
                        bool lbnlIsRecordexist = false;
                        foreach (busPersonAccountEmploymentDetail lobjOldPAEmploymentDetail in ibusPersonEmployment.ibusPerson.icolEnrolledPersonAccountEmploymentDetail)
                        {
                            if (lobjOldPAEmploymentDetail.ibusEmploymentDetail == null)
                                lobjOldPAEmploymentDetail.LoadPersonEmploymentDetail(false);

                            if ((lobjPersonAccountEmploymentDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.person_employment_id
                                        != lobjOldPAEmploymentDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.person_employment_id)
                                                && (lobjPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id
                                                             == lobjOldPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id)
                                && (lobjOldPAEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value == busConstant.PlanOptionStatusValueEnrolled))
                            {

                                //Ignore the Plan If Any other Employment Overlap with this current employment for this plan.
                                //PROD pir 5146 : new employer can offer insurance plan as of next month of date of join, so adding one month with employment start date
                                if (busGlobalFunctions.CheckDateOverlapping(lobjPersonAccountEmploymentDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.start_date.GetFirstDayofNextMonth(),
                                    lobjPersonAccountEmploymentDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date_no_null,
                                    lobjOldPAEmploymentDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.start_date,
                                    lobjOldPAEmploymentDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date_no_null))
                                {
                                    lbnlIsRecordexist = true;
                                    break;
                                }
                            }
                        }
                        if (!lbnlIsRecordexist)
                        {
                            iclbPersonAccountEmpDtl.Add(lobjPersonAccountEmploymentDetail);
                        }
                    }
                    else
                    {
                        iclbPersonAccountEmpDtl.Add(lobjPersonAccountEmploymentDetail);
                    }
                }
            }

            //Sort this collection by sort order till the Framework Fix
            if ((iclbPersonAccountEmpDtl != null) && (iclbPersonAccountEmpDtl.Count > 0))
                busGlobalFunctions.Sort<busPersonAccountEmploymentDetail>("ibusPlan.icdoPlan.sort_order", iclbPersonAccountEmpDtl);
        }
        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            base.BeforeValidate(aenmPageMode);
            if (aenmPageMode == utlPageMode.New)
            {
                IsNewMode = true;
                icdoPersonEmploymentDetail.job_class_value = icdoPersonEmploymentDetail.new_job_class_value;
                icdoPersonEmploymentDetail.official_list_value = icdoPersonEmploymentDetail.new_official_list_value;
                icdoPersonEmploymentDetail.status_value = icdoPersonEmploymentDetail.new_status_value;
                icdoPersonEmploymentDetail.type_value = icdoPersonEmploymentDetail.new_type_value;
            }
        }
        public override void BeforePersistChanges()
        {
            if (ibusPersonEmployment.IsNotNull())
            {
                // To insert person id in Audit log
                icdoPersonEmploymentDetail.person_id = ibusPersonEmployment.icdoPersonEmployment.person_id;
                icdoPersonEmploymentDetail.org_id = ibusPersonEmployment.icdoPersonEmployment.org_id;
                if (((icdoPersonEmploymentDetail.ihstOldValues.Count > 0 &&
                    Convert.ToString(icdoPersonEmploymentDetail.ihstOldValues["type_value"]) == busConstant.PersonJobTypePermanent) ||
                    (icdoPersonEmploymentDetail.person_employment_dtl_id == 0))
                    && iblnIsTypeTemporary) //PIR 22932
                {
                    IsPersonEnrolledinhealthonPreviousEmpolyment();
                }
            }

            IsEmploymentDetailModified = false;
            if (icdoPersonEmploymentDetail.ihstOldValues.Count > 0 &&
                (Convert.ToString(icdoPersonEmploymentDetail.ihstOldValues["job_class_value"]) != icdoPersonEmploymentDetail.job_class_value ||
                Convert.ToString(icdoPersonEmploymentDetail.ihstOldValues["type_value"]) != icdoPersonEmploymentDetail.type_value))
                IsEmploymentDetailModified = true;

            if (iclbPersonAccountEmpDtl != null)
            {
                if (ibusPersonEmployment == null)
                    LoadPersonEmployment();

                if (ibusPersonEmployment.ibusPerson == null)
                    ibusPersonEmployment.LoadPerson();

                foreach (busPersonAccountEmploymentDetail lobjPersonAccountEmpDtl in _iclbPersonAccountEmpDtl)
                {
                    if (lobjPersonAccountEmpDtl.ibusPlan == null)
                        lobjPersonAccountEmpDtl.LoadPlan();

                    if (lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.election_value == busConstant.PlanOptionStatusValueEnrolled)
                    {
                        //Link the Person Account Only if not set
                        if (lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.person_account_id == 0)
                        {
                            if (ibusPersonEmployment.ibusPerson.icolPersonAccount == null)
                                ibusPersonEmployment.ibusPerson.LoadPersonAccount(true);

                            // PROD PIR ID 5653
                            // Confirmed with Maik dated Mon 2/21/2011 11:08 PM
                            // No new account should be created if there exists a Withdrawn plan. Should be linked to the existing Withdrawn plan. Correct? – Yes 
                            // Is this applicable only to Def Comp plans or Retirement plans too ? – No, Def Comp only.
                            busPersonAccount lbusPersonAccount;
                            if (lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDeferredCompensation ||
                                lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdOther457)
                                lbusPersonAccount = ibusPersonEmployment.ibusPerson.icolPersonAccount.Where(i =>
                                                        i.ibusPlan.icdoPlan.plan_id == lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.plan_id).FirstOrDefault();
                            else
                                lbusPersonAccount = ibusPersonEmployment.ibusPerson.icolPersonAccount.Where(
                                    i => i.icdoPersonAccount.plan_id == lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.plan_id &&
                                        i.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetirementTransferredTIAACREF &&
                                        i.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusTransferToTFFR && //PROD PIR ID 6871
                                        i.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusTransferDC && //PIR 14656 - Confirmed on call with Maik
                                        (!i.IsWithDrawn())).FirstOrDefault();

                            if (lbusPersonAccount != null)
                            {
                                int lintRTWPayeeAccountID = 0;
                                if (lobjPersonAccountEmpDtl.ibusPlan.IsRetirementPlan())
                                {
                                    if (ibusPersonEmployment.ibusPerson.IsRTWMember(lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.plan_id,
                                        busConstant.PayeeStatusForRTW.IgnoreStatus, ref lintRTWPayeeAccountID))
                                    {
                                        //PIR 26089 Person account not linking when creating 2nd or later Person Employment Detail and enrolling Ret plan from dropdown.
                                        busPersonAccount lbusActivePersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                                        lbusActivePersonAccount = ibusPersonEmployment.ibusPerson.icolPersonAccount.Where(
                                                i => i.icdoPersonAccount.plan_id == lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.plan_id &&
                                                    (i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled ||
                                                    i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended) &&
                                                    i.icdoPersonAccount.end_date == DateTime.MinValue).FirstOrDefault();

                                        if (lbusActivePersonAccount.IsNotNull())
                                        {
                                            lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.person_account_id = lbusActivePersonAccount.icdoPersonAccount.person_account_id;
                                        }
                                        else
                                        {
                                            //UAT PIR 1482 - RTW case, if the old person account in Retired / Withdrawn / Cancelled / Suspended with Disa Payee Account, we should not link it.
                                            if (lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled)
                                            {
                                                lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.person_account_id = lbusPersonAccount.icdoPersonAccount.person_account_id;
                                            }
                                            //UAT PIR 1589 - Link with existing account when the person has suspended plan with disability payee account.
                                            else if (lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended)
                                            {
                                                busPayeeAccount lbusDisaPayeeAccount = new busPayeeAccount();
                                                lbusDisaPayeeAccount.FindPayeeAccount(lintRTWPayeeAccountID);
                                                if (lbusDisaPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                                                {
                                                    lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.person_account_id = lbusPersonAccount.icdoPersonAccount.person_account_id;

                                                    ////UAT PIR 1589 - Change the status of the Person Account into Enrolled                                                

                                                    //if (lbusPersonAccount.ibusPersonAccountRetirement == null)
                                                    //    lbusPersonAccount.LoadPersonAccountRetirement();

                                                    //if (lbusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccountRetirement.person_account_id > 0)
                                                    //{
                                                    //    lbusPersonAccount.ibusPersonAccountRetirement.ibusPerson = lbusPersonAccount.ibusPerson;
                                                    //    lbusPersonAccount.ibusPersonAccountRetirement.ibusPlan = lbusPersonAccount.ibusPlan;
                                                    //    lbusPersonAccount.ibusPersonAccountRetirement.LoadPlanEffectiveDate();
                                                    //    lbusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccount.person_employment_dtl_id = icdoPersonEmploymentDetail.person_employment_dtl_id;
                                                    //    lbusPersonAccount.ibusPersonAccountRetirement.LoadPersonEmploymentDetail();
                                                    //    lbusPersonAccount.ibusPersonAccountRetirement.ibusPersonEmploymentDetail.LoadPersonEmployment();
                                                    //    lbusPersonAccount.ibusPersonAccountRetirement.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                                                    //    lbusPersonAccount.ibusPersonAccountRetirement.LoadOrgPlan();

                                                    //    // Change the status to Suspended.
                                                    //    lbusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusRetirementEnrolled;
                                                    //    lbusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccount.history_change_date = lbusPersonAccount.ibusPersonAccountRetirement.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date;
                                                    //    lbusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccount.ienuObjectState = ObjectState.Update;

                                                    //    // Calling the Save operation, that will insert the History.
                                                    //    lbusPersonAccount.ibusPersonAccountRetirement.iarrChangeLog.Add(lbusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccount);
                                                    //    lbusPersonAccount.ibusPersonAccountRetirement.BeforeValidate(utlPageMode.All);
                                                    //    lbusPersonAccount.ibusPersonAccountRetirement.BeforePersistChanges();
                                                    //    lbusPersonAccount.ibusPersonAccountRetirement.PersistChanges();
                                                    //    lbusPersonAccount.ibusPersonAccountRetirement.AfterPersistChanges();
                                                    //}
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.person_account_id = lbusPersonAccount.icdoPersonAccount.person_account_id;
                                    }
                                }
                                else
                                {
                                    lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.person_account_id = lbusPersonAccount.icdoPersonAccount.person_account_id;
                                    bool lblnUpdate = false;
                                    //UAT PIR 2043 : Non Payroll Peoplesoft Org Group - Set the Flag whenever linked first time.
                                    if (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdFlex)
                                    {
                                        //PROD PIR ID 4624
                                        lbusPersonAccount.icdoPersonAccount.npsp_flexcomp_change_date = DateTime.Now;
                                        lbusPersonAccount.icdoPersonAccount.npsp_flexcomp_flag = busConstant.Flag_Yes;
                                        lblnUpdate = true;
                                    }
                                    if (lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                                    {
                                        lbusPersonAccount.icdoPersonAccount.ps_file_change_event_value = busConstant.NewEnrollment;
                                        //lbusPersonAccount.icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                                        lbusPersonAccount.icdoPersonAccount.Update();
                                    }
                                    else if (lblnUpdate)
                                        lbusPersonAccount.icdoPersonAccount.Update();
                                }
                            }
                        }
                        //Update should happen an all the save.. Only Linking Person Account has some condition before update
                        lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.Update();

                        //Updating DC Eligibility Date Whenever Employment Details Changes happens
                        if (lobjPersonAccountEmpDtl.ibusPlan.IsRetirementPlan() && (lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.person_account_id > 0))
                            UpdateDCEligibilityDateWhenEmploymentChanges(lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.person_account_id);

                        //uat pir 2373
                        //--Start--//
                        if (lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.person_account_id > 0)
                        {
                            //need not check null as person account is getting updated in above statements & should load always
                            lobjPersonAccountEmpDtl.LoadPersonAccount();
                            if (lobjPersonAccountEmpDtl.ibusEmploymentDetail == null)
                                lobjPersonAccountEmpDtl.LoadPersonEmploymentDetail();
                            if (lobjPersonAccountEmpDtl.ibusEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                            {
                                if (ibusPersonEmployment == null)
                                    LoadPersonEmployment();
                                if (ibusPersonEmployment.icolPersonEmploymentDetail == null)
                                    ibusPersonEmployment.LoadPersonEmploymentDetail();
                                busPersonEmploymentDetail lobjPersonEmploymentDetail = ibusPersonEmployment.icolPersonEmploymentDetail
                                                .Where(o => o.icdoPersonEmploymentDetail.start_date < lobjPersonAccountEmpDtl.ibusEmploymentDetail.icdoPersonEmploymentDetail.start_date)
                                                .FirstOrDefault();
                                if (lobjPersonEmploymentDetail != null)
                                {
                                    if (lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent)
                                    {
                                        lobjPersonAccountEmpDtl.ibusPersonAccount.icdoPersonAccount.ps_file_change_event_value = busConstant.EmploymentChangePermanentToTemporary;
                                        //lobjPersonAccountEmpDtl.ibusPersonAccount.icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                                        lobjPersonAccountEmpDtl.ibusPersonAccount.icdoPersonAccount.Update();
                                    }
                                }
                                else
                                {
                                    lobjPersonAccountEmpDtl.ibusPersonAccount.icdoPersonAccount.ps_file_change_event_value = busConstant.NewEnrollment;
                                    //lobjPersonAccountEmpDtl.ibusPersonAccount.icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                                    lobjPersonAccountEmpDtl.ibusPersonAccount.icdoPersonAccount.Update();
                                }
                            }
                            //prod pir 4861 : peoplesoft file changes to incorporate permanent to temporary change
                            else if (lobjPersonAccountEmpDtl.ibusEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent)
                            {
                                if (ibusPersonEmployment == null)
                                    LoadPersonEmployment();
                                if (ibusPersonEmployment.icolPersonEmploymentDetail == null)
                                    ibusPersonEmployment.LoadPersonEmploymentDetail();
                                busPersonEmploymentDetail lobjPersonEmploymentDetail = ibusPersonEmployment.icolPersonEmploymentDetail
                                                .Where(o => o.icdoPersonEmploymentDetail.start_date < lobjPersonAccountEmpDtl.ibusEmploymentDetail.icdoPersonEmploymentDetail.start_date)
                                                .FirstOrDefault();
                                if (lobjPersonEmploymentDetail != null)
                                {
                                    if (lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                                    {
                                        lobjPersonAccountEmpDtl.ibusPersonAccount.icdoPersonAccount.ps_file_change_event_value = busConstant.EmploymentChangePermanentToTemporary;
                                        //lobjPersonAccountEmpDtl.ibusPersonAccount.icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                                        lobjPersonAccountEmpDtl.ibusPersonAccount.icdoPersonAccount.Update();
                                    }
                                }
                                else
                                {
                                    lobjPersonAccountEmpDtl.ibusPersonAccount.icdoPersonAccount.ps_file_change_event_value = busConstant.NewEnrollment;
                                    //lobjPersonAccountEmpDtl.ibusPersonAccount.icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                                    lobjPersonAccountEmpDtl.ibusPersonAccount.icdoPersonAccount.Update();
                                }
                            }
                        }
                        //--End--//
                    }
                    else
                    {
                        // PROD PIR ID 6550 - Unlink the person account
                        if (lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.election_value != busConstant.PlanOptionStatusValueEnrolled &&
                            Convert.ToString(lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.ihstOldValues["election_value"]) == busConstant.PlanOptionStatusValueEnrolled)
                        {
                            lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.person_account_id = 0;
                            lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.Update();
                        }
                        //PIR 13399
                        if (lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.election_value == busConstant.PlanOptionStatusValueWaived &&
                           (Convert.ToString(lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.ihstOldValues["election_value"]) == string.Empty))
                        {
                            lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.is_waiver_report_generated = "N";
                            lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.person_account_id = 0;
                            lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.Update();
                        }
                    }
                }
            }

            //BR - 057 -10a The system must change the 'Payee Status to 'Review' if a new employment is recorded before the payment is issued. PIR - 1202
            if (ibusPersonEmployment == null)
                LoadPersonEmployment();
            if (ibusPersonEmployment.ibusPerson == null)
                ibusPersonEmployment.LoadPerson();
            if (ibusPersonEmployment.ibusPerson.iclbPayeeAccount == null)
                ibusPersonEmployment.ibusPerson.LoadPayeeAccount();
            if (ibusPersonEmployment.ibusPerson.iclbPayeeAccount.Count > 0)
            {
                if ((icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusContributing) ||
                    (icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOA) ||
                    (icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOAM) ||
                    (icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusFMLA))
                {
                    foreach (busPayeeAccount lobjPayeeAccount in ibusPersonEmployment.ibusPerson.iclbPayeeAccount)
                    {
                        //Commented since payee account status need to be loaded -- employment detail, two save actions happen back to back, so need to load each time
                        //if (lobjPayeeAccount.ibusPayeeAccountActiveStatus == null)
                        lobjPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                        if (lobjPayeeAccount.ibusPayeeAccountActiveStatus.IsNotNull())
                        {
                            string lstrStatus = busGlobalFunctions.GetData2ByCodeValue(2203, lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo);
                            if (lstrStatus == busConstant.PayeeAccountStatusRetirmentCancelled ||
                                 lstrStatus == busConstant.PayeeAccountStatusRetirementPaymentCompleted ||
                                 lstrStatus == busConstant.PayeeAccountStatusRefundProcessed ||
                                 lstrStatus == busConstant.PayeeAccountStatusSuspended)
                            { /*Dont do any operation if in cancelled/complete payment status*/ }
                            else
                            {
                                if (lobjPayeeAccount.ibusSoftErrors == null)
                                    lobjPayeeAccount.LoadErrors();
                                lobjPayeeAccount.iblnClearSoftErrors = false;
                                lobjPayeeAccount.ibusSoftErrors.iblnClearError = false;
                                lobjPayeeAccount.CreateReviewPayeeAccountStatus();
                                lobjPayeeAccount.iblnNewEmploymentIndicator = true;
                                lobjPayeeAccount.ValidateSoftErrors();
                                lobjPayeeAccount.UpdateValidateStatus();
                            }
                        }
                    }
                }
            }

            /* SYSTEST-PIR-ID-1603 */
            if (icdoPersonEmploymentDetail.end_date != DateTime.MinValue)
            {
                if (iclbAllPersonAccountEmpDtl.IsNull())
                    LoadAllPersonAccountEmploymentDetails();

                foreach (busPersonAccountEmploymentDetail lobjPAEmpDtl in iclbAllPersonAccountEmpDtl)
                {
                    if (lobjPAEmpDtl.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDeferredCompensation)
                    {
                        if (lobjPAEmpDtl.ibusPersonAccount.IsNull())
                            lobjPAEmpDtl.LoadPersonAccount();
                        if (lobjPAEmpDtl.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDeferredCompensation)
                        {
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
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            SetButtonVisiblityFalse();
            LoadPlansOffered();
            if (IsNewMode)
            {
                InsertEmployerOfferedPlans();
            }
            //PIR 23949 - When Employment is ended (from screen or PS) and there is a first activity for WFL Process IDs 236,237,238,239,256 
            //(Activity IDs 27,32,28,31,74) in a Suspended status it should be updated to RESU
            if (icdoPersonEmploymentDetail.end_date != DateTime.MinValue)
            {
                busWorkflowHelper.UpdateSuspendedInstancestoResumed(icdoPersonEmploymentDetail.person_id);
            }

            if (IsEmploymentDetailModified)
            {
                // PROD PIR ID 6551 
                // When Job class/type is modified, the new plans offered has to be reloaded.
                if (iclbAllPersonAccountEmpDtl == null) LoadAllPersonAccountEmploymentDetails(false);
                foreach (busPersonAccountEmploymentDetail lobjPAEmpDtl in iclbAllPersonAccountEmpDtl)
                    lobjPAEmpDtl.icdoPersonAccountEmploymentDetail.Delete();
                InsertEmployerOfferedPlans();
                LoadAllPersonAccountEmploymentDetails();
                iclbPersonAccountEmpDtl = new Collection<busPersonAccountEmploymentDetail>();
                foreach (busPersonAccountEmploymentDetail lobjPAEmpDtl in iclbAllPersonAccountEmpDtl)
                    iclbPersonAccountEmpDtl.Add(lobjPAEmpDtl);
            }

            LoadPersonAccountEmploymentDetailWithInsuranceFilter();
            SetActivePlansByEmployer();
            LoadPlansByEmployer();
            //UCS -060 - 10 
            InitiateWorkFlowForPayeeAccount();
            IsNewMode = false;

            //UAT PIR 2220
            PostESSmessage();

            //PROD PIR 4702
            //PostESSMessageForTransfer(); //PIR 11335 message not required

            var lenumPAEmpDet =
                iclbAllPersonAccountEmpDtl.Where(i => i.icdoPersonAccountEmploymentDetail.election_value == busConstant.PersonAccountElectionValueWaived);

            if (lenumPAEmpDet != null)
            {
                foreach (var lbusPersonAccountEmploymentDetail in lenumPAEmpDet)
                {
                    if (lbusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id != busConstant.PlanIdLTC)
                        InsertIntoEnrollmentData(lbusPersonAccountEmploymentDetail);                    
                }
            }

        }

        public void InsertIntoEnrollmentData(busPersonAccountEmploymentDetail lbusPersonAccountEmploymentDetail)
        {
            if (ibusPersonEmployment == null)
                LoadPersonEmployment();

            if (ibusPersonEmployment.ibusPerson == null)
                ibusPersonEmployment.LoadPerson();

            busEnrollmentData lobjEnrollmentData = new busEnrollmentData();
            lobjEnrollmentData.icdoEnrollmentData = new doEnrollmentData();

            lobjEnrollmentData.icdoEnrollmentData.source_id = lbusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.person_account_employment_dtl_id;
            lobjEnrollmentData.icdoEnrollmentData.plan_id = lbusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id;
            lobjEnrollmentData.icdoEnrollmentData.ssn = ibusPersonEmployment.ibusPerson.icdoPerson.ssn;
            lobjEnrollmentData.icdoEnrollmentData.ndpers_member_id = ibusPersonEmployment.ibusPerson.icdoPerson.person_id;
            lobjEnrollmentData.icdoEnrollmentData.peoplesoft_id = ibusPersonEmployment.ibusPerson.icdoPerson.peoplesoft_id;
            lobjEnrollmentData.icdoEnrollmentData.employer_org_id = ibusPersonEmployment.icdoPersonEmployment.org_id;
            lobjEnrollmentData.icdoEnrollmentData.employment_type_value = icdoPersonEmploymentDetail.type_value;
            lobjEnrollmentData.icdoEnrollmentData.start_date = lbusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.modified_date;
            lobjEnrollmentData.icdoEnrollmentData.monthly_premium = 0.0M;
            lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_No;
            lobjEnrollmentData.icdoEnrollmentData.peoplesoft_file_sent_flag = busConstant.Flag_Yes;
            lobjEnrollmentData.icdoEnrollmentData.Insert();

        }
        
        private void UpdateDCEligibilityDateWhenEmploymentChanges(int aintPersonAccountID)
        {
            //Update the DC Eligibility Date in Retirement Plan if the Member changes the employment detail (or new employment) and eligible to enroll in DC
            if (icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusContributing)
            {
                busPersonAccountRetirement lbusPersonAccountRetirement = new busPersonAccountRetirement();
                lbusPersonAccountRetirement.FindPersonAccountRetirement(aintPersonAccountID);
                //PROD PIR 4691
                if (IsDCPlanEligible())
                {
                    // PIR 11891 - Previous employment Code comment
                    //if (lbusPersonAccountRetirement.ibusPreviousEmploymentDetail == null)
                    //    lbusPersonAccountRetirement.LoadPreviousEmploymentDetail();
                    //if ((lbusPersonAccountRetirement.ibusPreviousEmploymentDetail != null && lbusPersonAccountRetirement.ibusPreviousEmploymentDetail.icdoPersonEmploymentDetail != null &&
                    //    lbusPersonAccountRetirement.ibusPreviousEmploymentDetail.icdoPersonEmploymentDetail.person_employment_id != icdoPersonEmploymentDetail.person_employment_id) ||
                    //    lbusPersonAccountRetirement.icdoPersonAccountRetirement.dc_eligibility_date == DateTime.MinValue)
                    //{
                        if (IsDcEligibilityDateRequired() // PIR 11483
                            && lbusPersonAccountRetirement.IsPopulateDCEligibilityDate(icdoPersonEmploymentDetail.start_date)) // PIR 11891
                        {
                            lbusPersonAccountRetirement.icdoPersonAccountRetirement.dc_eligibility_date = icdoPersonEmploymentDetail.start_date.AddDays(180);
                            lbusPersonAccountRetirement.icdoPersonAccountRetirement.Update();
                        }
                    //}
                }
                else if (lbusPersonAccountRetirement.icdoPersonAccountRetirement.dc_eligibility_date != DateTime.MinValue) //PROD PIR ID 5653
                {
                    lbusPersonAccountRetirement.icdoPersonAccountRetirement.dc_eligibility_date = DateTime.MinValue;
                    lbusPersonAccountRetirement.icdoPersonAccountRetirement.Update();
                }
            }
        }

        //PIR 11483
        public bool IsDcEligibilityDateRequired()
        {
            DateTime ldtDateCriteria = new DateTime(2013, 10, 1);
            if (icdoPersonEmploymentDetail.start_date < ldtDateCriteria)
            {
                int lintGetCountOfJobClass = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPlanJobClassCrossref.GetJobClassCount", new object[1] { icdoPersonEmploymentDetail.job_class_value },
                                                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                if (lintGetCountOfJobClass > 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// If the spouse has long career with state employer, he/she can only enroll in GroupHealth plan.
        /// If the member is transferring employment within one month, this rule may not be applicable as said by David in mail dated on 04-28-2010
        /// </summary>
        /// <returns></returns>
        public bool IsPersonValidToEnrollGroupHealth()
        {
            if (icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent) //PROD PIR ID 5529
            {
                // If Person Enrolling GH plan
                bool lblnIsEnrollingToGroupHealth = false;
                if (iclbPersonAccountEmpDtl == null)
                    LoadPersonAccountEmploymentDetailWithInsuranceFilter();
                foreach (busPersonAccountEmploymentDetail lobjPAEmploymentDetail in iclbPersonAccountEmpDtl)
                {
                    if ((lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value == busConstant.PersonAccountElectionValueEnrolled) &&
                        (lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdGroupHealth))
                        lblnIsEnrollingToGroupHealth = true;
                }
                if (lblnIsEnrollingToGroupHealth)
                {
                    if (ibusPersonEmployment != null)
                    {
                        bool lblnTransferEmploymentWithinMonth = false;
                        //If Member is transferring Employment within one month, we should not do this validation
                        if (ibusPersonEmployment.ibusPerson == null)
                            ibusPersonEmployment.LoadPerson();

                        if (ibusPersonEmployment.ibusPerson.icolPersonEmployment == null)
                            ibusPersonEmployment.ibusPerson.LoadPersonEmployment();

                        var lenuList = ibusPersonEmployment.ibusPerson.icolPersonEmployment.Where(i => i.icdoPersonEmployment.person_employment_id
                            != ibusPersonEmployment.icdoPersonEmployment.person_employment_id);
                        if (lenuList != null && lenuList.Count() > 0)
                        {
                            foreach (busPersonEmployment lbusOldEmployment in lenuList)
                            {
                                if (lbusOldEmployment.icdoPersonEmployment.end_date != DateTime.MinValue)
                                {
                                    int lintDiffInDays = busGlobalFunctions.DateDiffInDays(lbusOldEmployment.icdoPersonEmployment.end_date, ibusPersonEmployment.icdoPersonEmployment.start_date);
                                    if (lintDiffInDays > 0 && lintDiffInDays <= 31)
                                    {
                                        lblnTransferEmploymentWithinMonth = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (!lblnTransferEmploymentWithinMonth)
                        {
                            if (ibusPersonEmployment.ibusOrganization == null)
                                ibusPersonEmployment.LoadOrganization();
                            if ((ibusPersonEmployment.ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried) &&
                                (ibusPersonEmployment.ibusOrganization.icdoOrganization.emp_category_value == busConstant.EmployerCategoryState))
                            {
                                if (ibusPersonEmployment.ibusPerson.ibusSpouse == null)
                                    ibusPersonEmployment.ibusPerson.LoadSpouse();
                                if (ibusPersonEmployment.ibusPerson.ibusSpouse.icdoPerson.person_id > 0)
                                {
                                    // Load Spouse's object               
                                    if (ibusPersonEmployment.ibusPerson.ibusSpouse.icolPersonEmployment == null)
                                        ibusPersonEmployment.ibusPerson.ibusSpouse.LoadPersonEmployment();
                                    foreach (busPersonEmployment lobjSpouseEmployment in ibusPersonEmployment.ibusPerson.ibusSpouse.icolPersonEmployment)
                                    {
                                        // If the Spouse is currently employed to a state employer before this employment start date.
                                        if (lobjSpouseEmployment.ibusOrganization == null)
                                            lobjSpouseEmployment.LoadOrganization();
                                        if (lobjSpouseEmployment.icolPersonEmploymentDetail.IsNull())
                                            lobjSpouseEmployment.LoadPersonEmploymentDetail();
                                        if (lobjSpouseEmployment.icolPersonEmploymentDetail.Count > 0)
                                        {
                                            if ((lobjSpouseEmployment.icdoPersonEmployment.end_date == DateTime.MinValue) &&
                                               (lobjSpouseEmployment.ibusOrganization.icdoOrganization.emp_category_value == busConstant.EmployerCategoryState) &&
                                               (lobjSpouseEmployment.icolPersonEmploymentDetail[0].icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent) &&
                                               (lobjSpouseEmployment.icdoPersonEmployment.start_date < ibusPersonEmployment.icdoPersonEmployment.start_date))
                                                return false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Function to check whether the Person is Active Dependent to the enrolled plan
        /// </summary>
        /// <returns>Returns true if the Dependent Person is Active</returns>
        public bool IsActiveDependent()
        {
            bool lblnResult = false;

            if (ibusPersonEmployment == null)
                LoadPersonEmployment();

            if (ibusPersonEmployment.ibusPerson == null)
                ibusPersonEmployment.LoadPerson();

            if (ibusPersonEmployment.ibusPerson.iclbPersonDependentByDependent == null)
                ibusPersonEmployment.ibusPerson.LoadPersonDependentByDependent();

            foreach (busPersonDependent lobjDependent in ibusPersonEmployment.ibusPerson.iclbPersonDependentByDependent)
            {
                if (lobjDependent.iclbPersonAccountDependent == null)
                    lobjDependent.LoadPersonAccountDependent();

                if (iclbPersonAccountEmpDtl == null)
                    LoadEnrolledPersonAccountEmploymentDetails();

                //PROD PIR : 4095 . Filter only Enrolled Plans.
                var lenuList = iclbPersonAccountEmpDtl.Where(i => i.icdoPersonAccountEmploymentDetail.election_value == busConstant.PersonAccountElectionValueEnrolled);
                foreach (busPersonAccountEmploymentDetail lobjPAEmpDetail in lenuList)
                {
                    ///Check whether for the enrolled plan, he/she is an active dependent for the current time
                    foreach (busPersonAccountDependent lobjPADependent in lobjDependent.iclbPersonAccountDependent)
                    {
                        if (lobjPADependent.ibusPersonAccount == null)
                            lobjPADependent.LoadPersonAccount();

                        if ((lobjPAEmpDetail.icdoPersonAccountEmploymentDetail.plan_id == lobjPADependent.ibusPersonAccount.icdoPersonAccount.plan_id) &&
                            (busGlobalFunctions.CheckDateOverlapping(icdoPersonEmploymentDetail.start_date,
                                    lobjPADependent.icdoPersonAccountDependent.start_date,
                                    lobjPADependent.icdoPersonAccountDependent.end_date)))
                        {
                            lblnResult = true;
                            break;
                        }
                    }
                    if (lblnResult) break;
                }
                if (lblnResult) break;
            }
            return lblnResult;
        }

        // PIR 1101
        // member once enrolled in the DC plan can not enroll in DB plan.
        // Load all Person account
        // check if the person has ever enrolled in the DC plan irrespective of participation status
        public bool IsPersonEligibleToEnrollInDC()
        {
            //PIR 20232
            if (ibusPersonEmployment == null)
                LoadPersonEmployment();
            if (ibusPersonEmployment.ibusOrganization == null)
                ibusPersonEmployment.LoadOrganization();
            if (ibusPersonEmployment.ibusOrganization.ibusOrgPlan == null)
                ibusPersonEmployment.ibusOrganization.LoadOrgPlan();

            bool lblnpersonAlreadyEnrolledInDC = IsPersonEnrolledInDC(busConstant.PlanIdDC);
            bool lblnpersonAlreadyEnrolledInDC2020 = IsPersonEnrolledInDC(busConstant.PlanIdDC2020); //PIR 20232 ?query
            if (lblnpersonAlreadyEnrolledInDC || lblnpersonAlreadyEnrolledInDC2020)
            {
                LoadEnrolledPersonAccountEmploymentDetails(); // UAT PIR 1141 - PersonAccount Employment Detail is not refreshing.
                foreach (busPersonAccountEmploymentDetail lobjPersonAccountEmploymentDetail in iclbPersonAccountEmpDtl)
                {
                    if ((lobjPersonAccountEmploymentDetail.ibusPlan.IsDBRetirementPlan())
                        && (lobjPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.person_account_id == 0))
                        return false;
                }
            }
            return true;
        }
        
        private bool IsPersonEnrolledInDC(int aintPlanId)
        {
            if (ibusPersonEmployment == null)
                LoadPersonEmployment();
            if (ibusPersonEmployment.ibusPerson == null)
                ibusPersonEmployment.LoadPerson();
            bool lblnpersonAlreadyEnrolledInDC = false;
            lblnpersonAlreadyEnrolledInDC = ibusPersonEmployment.ibusPerson.IsMemberEnrolledInPlan(aintPlanId); // PIR 20232

            return lblnpersonAlreadyEnrolledInDC;
        }

        public bool IsDCPlanEligible()
        {
            if (!icdoPersonEmploymentDetail.job_class_value.IsNullOrEmpty() && !icdoPersonEmploymentDetail.type_value.IsNullOrEmpty())
            {
                DataTable ldtbDCPlanjobClass = new DataTable();
                ldtbDCPlanjobClass = Select<cdoPlanJobClassCrossref>(new string[4] { "plan_id", "JOB_CLASS_VALUE", "JOB_TYPE_VALUE", "DC_TRANSFER_ELIGIBLE" },
                                             new object[4] {busConstant.PlanIdDC,icdoPersonEmploymentDetail.job_class_value,
                                                          icdoPersonEmploymentDetail.type_value, busConstant.Flag_Yes }, null, null);//PROD PIR 4631
                if (ldtbDCPlanjobClass != null && ldtbDCPlanjobClass.Rows.Count == 0)
                {
                    ldtbDCPlanjobClass = Select<cdoPlanJobClassCrossref>(new string[4] { "plan_id", "JOB_CLASS_VALUE", "JOB_TYPE_VALUE", "DC_TRANSFER_ELIGIBLE" },
                                             new object[4] {busConstant.PlanIdDC2020,icdoPersonEmploymentDetail.job_class_value,
                                                          icdoPersonEmploymentDetail.type_value, busConstant.Flag_Yes }, null, null);//PIR 20232
                }
                //PIR 25920
                if (ldtbDCPlanjobClass != null && ldtbDCPlanjobClass.Rows.Count == 0)
                {
                    ldtbDCPlanjobClass = Select<cdoPlanJobClassCrossref>(new string[4] { "plan_id", "JOB_CLASS_VALUE", "JOB_TYPE_VALUE", "DC_TRANSFER_ELIGIBLE" },
                                             new object[4] {busConstant.PlanIdDC2025,icdoPersonEmploymentDetail.job_class_value,
                                                          icdoPersonEmploymentDetail.type_value, busConstant.Flag_Yes }, null, null);//PIR 25920
                }
                if (ldtbDCPlanjobClass.Rows.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        // Load retirement collection for this emp dtl id
        private Collection<busPersonAccountRetirementContribution> _iclbRetirementContribution;

        public Collection<busPersonAccountRetirementContribution> iclbRetirementContribution
        {
            get { return _iclbRetirementContribution; }
            set { _iclbRetirementContribution = value; }
        }

        public void LoadRetirementContribution()
        {
            DataTable ldtbRetirementContribution = Select<cdoPersonAccountRetirementContribution>(new string[1] { "person_employment_dtl_id" }, new object[1] { icdoPersonEmploymentDetail.person_employment_dtl_id }, null, null);
            _iclbRetirementContribution = GetCollection<busPersonAccountRetirementContribution>(ldtbRetirementContribution, "icdoPersonAccountRetirementContribution");
        }

        # region UCS-60

        //UCS-60  - 08
        //check if the employer is providing Life plan
        // but the member is not enrolled in that plan
        // UAT PIR - 1016
        // this validationis only for RTW member
        // UAT PIR - 1211
        // This validation is not only for RTW.. it is a common validation that if the employer offer LIfe and member is selelected waived / blank, it should throw an erro
        public bool IsMemberWaivingLifePlan()
        {
            if (iclbPersonAccountEmpDtl != null)
            {
                //is employer offer life
                busPersonAccountEmploymentDetail lbusPAEmpDetail = iclbAllPersonAccountEmpDtl
                                                                    .Where(i => i.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdGroupLife).FirstOrDefault();
                if (lbusPAEmpDetail != null)
                {
                    //prod pir 4097 : life enrollment is not necessary for temporary employment
                    if (icdoPersonEmploymentDetail.type_value != busConstant.PersonJobTypeTemporary &&
                        (lbusPAEmpDetail.icdoPersonAccountEmploymentDetail.election_value.IsNullOrEmpty() ||
                        lbusPAEmpDetail.icdoPersonAccountEmploymentDetail.election_value == busConstant.PersonAccountElectionValueWaived))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //initiate suspend workflow if the payee account status is not Suspend
        private void InitiateWorkFlowForPayeeAccount()
        {
            if (ibusPersonEmployment.IsNull())
                LoadPersonEmployment();
            if (ibusPersonEmployment.ibusPerson.IsNull())
                ibusPersonEmployment.LoadPerson();
            if (ibusPersonEmployment.ibusPerson.icolPersonEmployment.IsNull())
                ibusPersonEmployment.ibusPerson.LoadPersonEmployment();

            foreach (busPersonEmployment lobjPersonEmployment in ibusPersonEmployment.ibusPerson.icolPersonEmployment)
            {
                if (lobjPersonEmployment.icolPersonEmploymentDetail.IsNull())
                    lobjPersonEmployment.LoadPersonEmploymentDetail();

                busPersonEmploymentDetail lobjPersonEmployemntDetail = new busPersonEmploymentDetail();
                if (lobjPersonEmployment.icolPersonEmploymentDetail.Count > 0)
                {
                    var lobjEmploymentDetailWithEndDateList = lobjPersonEmployment.icolPersonEmploymentDetail.Where(lobjPersonEmpDtl => lobjPersonEmpDtl.icdoPersonEmploymentDetail.end_date != DateTime.MinValue
                        && lobjPersonEmpDtl.icdoPersonEmploymentDetail.person_employment_dtl_id != icdoPersonEmploymentDetail.person_employment_dtl_id);
                    if (lobjEmploymentDetailWithEndDateList.Count() > 0)
                    {
                        lobjPersonEmployemntDetail = lobjEmploymentDetailWithEndDateList.First();
                        lobjPersonEmployemntDetail.InitializeSuspendWorkflowForPayeeAccount();
                        if (lobjPersonEmployemntDetail.iblnIsSuspendWorkflowInititated)
                            break;
                    }
                }
            }
        }
        public bool iblnIsSuspendWorkflowInititated { get; set; }
        public void InitializeSuspendWorkflowForPayeeAccount()
        {
            if (ibusPersonEmployment == null)
                LoadPersonEmployment();
            if (ibusPersonEmployment.ibusPerson == null)
                ibusPersonEmployment.LoadPerson();
            if (ibusPersonEmployment.ibusPerson.iclbPayeeAccount == null)
                ibusPersonEmployment.ibusPerson.LoadPayeeAccount();

            if (ibusPersonEmployment.ibusPerson.iclbPayeeAccount.Count > 0)
            {
                foreach (busPayeeAccount lobjPayeeAccount in ibusPersonEmployment.ibusPerson.iclbPayeeAccount)
                {
                    if (lobjPayeeAccount.IsBenefitAccountTypeIsRetirmentOrDisabilityOrRefund()) //PIR 
                    {
                        if (lobjPayeeAccount.IsBenefitAccountTypeRefund())
                            lobjPayeeAccount.LoadActivePayeeStatus(); //As per Maik Call, for refund payee account, load latest payee account status 
                        else
                            lobjPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                        if (ibusPersonEmployment.icdoPersonEmployment.start_date < lobjPayeeAccount.icdoPayeeAccount.benefit_begin_date &&
                            !(lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusCancelled ||
                            lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusRefundCancelled ||
                            lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusDisabilityCancelled ||
                            lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusCancelPending ||
                            lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value == busConstant.TransactionTypeCancelRefund ||
                            lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusDisabilityPendingCancelled))
                        {
                            DataTable ldtActivityInstance = Select("entSolBpmActivityInstance.LoadAllInstancesByProcessAndReference",
                                                              new object[2] { busConstant.Map_Suspend_RTW_Payee_Account, lobjPayeeAccount.icdoPayeeAccount.payee_account_id });
                            if ((ldtActivityInstance.Rows.Count == 0) && (lobjPayeeAccount.icdoPayeeAccount.payee_account_id != 0))
                            {
                                //initiate workflow
                                InitializeWorkFlow(lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id, lobjPayeeAccount.icdoPayeeAccount.payee_account_id,
                                    busConstant.WorkflowProcessStatus_UnProcessed, busConstant.WorkflowProcessSource_Online);
                                iblnIsSuspendWorkflowInititated = true;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void InitializeWorkFlow(int aintPersonID, int aintReferenceID, string astrStatusValue, string astrSourceValue)
        {
            busWorkflowHelper.InitiateBpmRequest(busConstant.Map_Suspend_RTW_Payee_Account, aintPersonID, 0, aintReferenceID, iobjPassInfo, astrSourceValue);
        }

        # endregion

        #region Correspondence


        public override busBase GetCorPerson()
        {
            if (ibusPersonEmployment == null)
                LoadPersonEmployment();
            if (ibusPersonEmployment.ibusPerson == null)
                ibusPersonEmployment.LoadPerson();
            return ibusPersonEmployment.ibusPerson;
        }

        public override busBase GetCorOrganization()
        {
            if (ibusPersonEmployment == null)
                LoadPersonEmployment();
            if (ibusPersonEmployment.ibusOrganization == null)
                ibusPersonEmployment.LoadOrganization();
            return ibusPersonEmployment.ibusOrganization;
        }

        private string _Is24MonthLOA;
        public string Is24MonthLOA
        {
            get { return _Is24MonthLOA; }
            set { _Is24MonthLOA = value; }
        }

        private bool _IsVSC36orMore;
        public bool IsVSC36orMore
        {
            get { return _IsVSC36orMore; }
            set { _IsVSC36orMore = value; }
        }

        private bool _IsVSC6orMore;
        public bool IsVSC6orMore
        {
            get { return _IsVSC6orMore; }
            set { _IsVSC6orMore = value; }
        }

        private busPersonAccountRetirement _ibusRetirement;
        public busPersonAccountRetirement ibusRetirement
        {
            get { return _ibusRetirement; }
            set { _ibusRetirement = value; }
        }

        private string _RecertifiedDateNullIDentifier;
        public string RecertifiedDateNullIDentifier
        {
            get { return _RecertifiedDateNullIDentifier; }
            set { _RecertifiedDateNullIDentifier = value; }
        }

        private string _RecertificationNullAndEndaDateNotNullIdentifier;
        public string RecertificationNullAndEndaDateNotNullIdentifier
        {
            get { return _RecertificationNullAndEndaDateNotNullIdentifier; }
            set { _RecertificationNullAndEndaDateNotNullIdentifier = value; }
        }

        public string Future_Date
        {
            get { return DateTime.Now.AddDays(30).ToString(busConstant.DateFormatLongDate); }
        }

        private DateTime _EmploymentDetailStartDatePlus30Days;
        public DateTime EmploymentDetailStartDatePlus30Days
        {
            get
            {
                if (icdoPersonEmploymentDetail.start_date != DateTime.MinValue)
                    _EmploymentDetailStartDatePlus30Days = icdoPersonEmploymentDetail.start_date.AddDays(30);
                return _EmploymentDetailStartDatePlus30Days;
            }
        }

        public string EmploymentDetailStartDatePlus12months
        {
            get
            {
                return icdoPersonEmploymentDetail.start_date.AddMonths(12).ToString(busConstant.DateFormatLongDate);
            }
        }

        public string EmploymentDetailStartDatePlus24Months
        {
            get
            {
                return icdoPersonEmploymentDetail.start_date.AddMonths(24).ToString(busConstant.DateFormatLongDate);
            }
        }

        # region 22 correspondence
        //used in ENR - 5059
        //BeginningOfTheNextFiscalYear
        public string istrBeginningOfTheNextFiscalYear
        {
            get
            {
                if (DateTime.Now.Month < 7)
                    return DateTime.Now.Year.ToString();
                return DateTime.Now.AddYears(1).Year.ToString();
            }
        }

        //last contribution date for this person in retirement plan
        public DateTime idtLastContributionDate { get; set; }
        public string istrdtLastContributiondate
        {
            get
            {
                return idtLastContributionDate.ToString(busConstant.DateFormatLongDate);
            }
        }
        public void LoadLastContributionDate()
        {
            if (_iclbRetirementContribution.IsNull())
                LoadRetirementContribution();

            var lRetirementContribution = _iclbRetirementContribution.OrderByDescending(lobjContribution => lobjContribution.icdoPersonAccountRetirementContribution.effective_date);

            if (lRetirementContribution.Count() > 0)
                idtLastContributionDate = lRetirementContribution.FirstOrDefault().icdoPersonAccountRetirementContribution.effective_date;
        }

        public override void LoadCorresProperties(string astrTemplateName)
        {
            if (idtLastContributionDate == DateTime.MinValue)
                LoadLastContributionDate();
            if (iclbPersonAccountLifeCoverageDetails.IsNull())
                LoadLifeCoverageDetailsAndPremiumProvider();
            LoadDatesForEachPlan();
            SetIsMemberEnrolledInDBOrDC();
            if (ibusPersonEmployment.ibusPerson.istrLTCCarrierName.IsNullOrEmpty())
                ibusPersonEmployment.ibusPerson.LoadLTCProviderName();
            if (ibusPersonEmployment.ibusPerson.ibusCurrentEmployment == null)
             ibusPersonEmployment.ibusPerson.LoadCurrentEmployer();
            
        }

        //used in UCS 22 COR ENR - 5159
        public Collection<busPersonAccountLifeOption> iclbPersonAccountLifeCoverageDetails { get; set; }
        public busPersonAccountLife ibusPersonAccountActiveLife { get; set; }
        public void LoadLifeCoverageDetailsAndPremiumProvider()
        {
            iclbPersonAccountLifeCoverageDetails = new Collection<busPersonAccountLifeOption>();

            if (ibusPersonAccountActiveLife.IsNull())
                LoadActivePersonAccountLife();

            if (ibusPersonAccountActiveLife.icdoPersonAccountLife.person_account_id > 0)
            {
                if (ibusPersonAccountActiveLife.ibusProvider.IsNull()) //UAT PIR - 2023 -Loading provider name
                {
                    ibusPersonAccountActiveLife.LoadProvider();

                    istrLifeInsuranceProvider = ibusPersonAccountActiveLife.ibusProvider.icdoOrganization.org_name;
                }
                ibusPersonAccountActiveLife.LoadLifeOptionData();

                foreach (busPersonAccountLifeOption lobjPersonAccountLifeOption in ibusPersonAccountActiveLife.iclbLifeOption)
                {
                    iclbPersonAccountLifeCoverageDetails.Add(lobjPersonAccountLifeOption);
                }
            }
        }

        private void LoadActivePersonAccountLife()
        {
            if (ibusPersonEmployment.IsNull())
                LoadPersonEmployment();
            if (ibusPersonEmployment.ibusPerson.IsNull())
                ibusPersonEmployment.LoadPerson();

            if (ibusPersonAccountActiveLife == null)
                ibusPersonAccountActiveLife = new busPersonAccountLife
                {
                    icdoPersonAccount = new cdoPersonAccount(),
                    icdoPersonAccountLife = new cdoPersonAccountLife()
                };

            var lenumLife = iclbAllPersonAccountEmpDtl.Where(lobjPersonEmplDtl => lobjPersonEmplDtl.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdGroupLife
                   && lobjPersonEmplDtl.icdoPersonAccountEmploymentDetail.person_account_id > 0);
            if (lenumLife.Count() > 0)
                if (lenumLife.First().ibusPersonAccount.IsNotNull())
                {
                    if (lenumLife.First().ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
                    {
                        ibusPersonAccountActiveLife.FindPersonAccountLife(lenumLife.First().ibusPersonAccount.icdoPersonAccount.person_account_id);
                        ibusPersonAccountActiveLife.icdoPersonAccount = lenumLife.First().ibusPersonAccount.icdoPersonAccount;
                    }
                }
        }

        public string istrLifeInsuranceProvider { get; set; }
        public string istrIsMemberEnrolledInDB { get; set; }
        public string istrIsMemberEnrolledInDC { get; set; }
        public string istrPlanRetirementType { get; set; }

        public void SetIsMemberEnrolledInDBOrDC()
        {
            if (ibusPersonEmployment.IsNull())
                LoadPersonEmployment();
            if (ibusPersonEmployment.ibusPerson.IsNull())
                ibusPersonEmployment.LoadPerson();
            if (_iclbAllPersonAccountEmpDtl.IsNull())
                LoadAllPersonAccountEmploymentDetails();

            istrIsMemberEnrolledInDB = busConstant.Flag_No;
            istrIsMemberEnrolledInDC = busConstant.Flag_No;
            busPersonAccount lobjDCPersonAccount = new busPersonAccount();
            lobjDCPersonAccount = ibusPersonEmployment.ibusPerson.LoadActivePersonAccountByPlan(busConstant.PlanIdDC);
            if (lobjDCPersonAccount != null && lobjDCPersonAccount.icdoPersonAccount.person_account_id == 0)
            {
                lobjDCPersonAccount = ibusPersonEmployment.ibusPerson.LoadActivePersonAccountByPlan(busConstant.PlanIdDC2020); //PIR 20232
            }
            if (lobjDCPersonAccount.icdoPersonAccount.person_account_id > 0)
            {
                idtRetirementDate = lobjDCPersonAccount.icdoPersonAccount.start_date;
                istrIsMemberEnrolledInDC = busConstant.Flag_Yes;
                istrPlanRetirementType = busConstant.PlanRetirementTypeValueDC;
            }
            else
            {
                //check if member is enrolled in the DB
                if (ibusPersonEmployment.ibusPerson.icolPersonAccountByBenefitType.IsNull())
                    ibusPersonEmployment.ibusPerson.LoadPersonAccountByBenefitType(busConstant.PlanBenefitTypeRetirement);
                if (ibusPersonEmployment.ibusPerson.icolPersonAccountByBenefitType.Count > 0)
                {
                    var lenumPersonAccount = ibusPersonEmployment.ibusPerson.icolPersonAccountByBenefitType.OrderBy(lobjPA => lobjPA.icdoPersonAccount.start_date);
                    var lintCountOfDBPlans = lenumPersonAccount
                        .Where(lobjPerAcc => lobjPerAcc.ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDB ||
                        lobjPerAcc.ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueHB).Count();       //PIR 25920  New DC plan
                    if (lintCountOfDBPlans > 0)
                    {
                        istrIsMemberEnrolledInDB = busConstant.Flag_Yes;
                        istrPlanRetirementType = busConstant.PlanRetirementTypeValueDB;
                    }
                    idtRetirementDate = lenumPersonAccount.First().icdoPersonAccount.start_date;
                }
            }
        }

        # region PER-0252
        public string IsDBOrDC
        {
            get
            {
                if (istrIsMemberEnrolledInDC == busConstant.Flag_Yes || istrIsMemberEnrolledInDB == busConstant.Flag_Yes)
                    return busConstant.Flag_Yes;
                return busConstant.Flag_No;
            }
        }

        public DateTime idtRetirementDate { get; set; }
        public DateTime idtHealthDate { get; set; }
        public DateTime idtLifeDate { get; set; }
        public DateTime idtDeffCompDate { get; set; }
        public DateTime idtFlexDate { get; set; }
        public DateTime idtEmpStartDate { get; set; }

        public string istrMemberIsInHealth { get; set; }
        public string istrMemberIsInLife { get; set; }
        public string istrMemberIsInDeffComp { get; set; }
        public string istrMemberIsInFlexComp { get; set; }

        public void LoadDatesForEachPlan()
        {
            if (ibusPersonEmployment.IsNull())
                LoadPersonEmployment();

                idtEmpStartDate = ibusPersonEmployment.icdoPersonEmployment.start_date;

            if (ibusPersonEmployment.ibusPerson.IsNull())
                ibusPersonEmployment.LoadPerson();
            
            if (_iclbAllPersonAccountEmpDtl.IsNull())
                LoadAllPersonAccountEmploymentDetails();
            
            if (iclbAllPersonAccountEmpDtl.Count() > 0)
            {
                var lenumHealth = iclbAllPersonAccountEmpDtl.Where(lobjPersonEmplDtl => lobjPersonEmplDtl.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdGroupHealth
                    && lobjPersonEmplDtl.icdoPersonAccountEmploymentDetail.person_account_id > 0);
                if (lenumHealth.Count() > 0)
                    if (lenumHealth.First().ibusPersonAccount.IsNotNull())
                    {
                        istrMemberIsInHealth = busConstant.Flag_Yes;
                        idtHealthDate = lenumHealth.First().ibusPersonAccount.icdoPersonAccount.current_plan_start_date;
                    }

                //for life
                var lenumLife = iclbAllPersonAccountEmpDtl.Where(lobjPersonEmplDtl => lobjPersonEmplDtl.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdGroupLife
                    && lobjPersonEmplDtl.icdoPersonAccountEmploymentDetail.person_account_id > 0);
                if (lenumLife.Count() > 0)
                    if (lenumLife.First().ibusPersonAccount.IsNotNull())
                    {
                        istrMemberIsInLife = busConstant.Flag_Yes;
                        idtLifeDate = lenumLife.First().ibusPersonAccount.icdoPersonAccount.current_plan_start_date;
                    }
                //for Deferred COmp
                var lenumDefComp = iclbAllPersonAccountEmpDtl.Where(lobjPersonEmplDtl => lobjPersonEmplDtl.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDeferredCompensation
                    && lobjPersonEmplDtl.icdoPersonAccountEmploymentDetail.person_account_id > 0);
                if (lenumDefComp.Count() > 0)
                    if (lenumDefComp.First().ibusPersonAccount.IsNotNull())
                    {
                        istrMemberIsInDeffComp = busConstant.Flag_Yes;
                        idtDeffCompDate = lenumDefComp.First().ibusPersonAccount.icdoPersonAccount.current_plan_start_date;
                    }
                //for Flex COmp
                var lenumFlexComp = iclbAllPersonAccountEmpDtl.Where(lobjPersonEmplDtl => lobjPersonEmplDtl.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdFlex
                    && lobjPersonEmplDtl.icdoPersonAccountEmploymentDetail.person_account_id > 0);
                if (lenumFlexComp.Count() > 0)
                    if (lenumFlexComp.First().ibusPersonAccount.IsNotNull())
                    {
                        istrMemberIsInFlexComp = busConstant.Flag_Yes;
                        idtFlexDate = lenumFlexComp.First().ibusPersonAccount.icdoPersonAccount.current_plan_start_date;
                    }
            }
        }

        # endregion
        # endregion
        #endregion

        public Collection<cdoCodeValue> LoadJobClassByEmployerCategory()
        {
            if (ibusPersonEmployment.IsNull())
                LoadPersonEmployment();
            if (ibusPersonEmployment.ibusOrganization.IsNull())
                ibusPersonEmployment.LoadOrganization();
            //PROD Pir - 4555
            if (ibusPersonEmployment.ibusOrganization.iclbOrgPlan == null)
                ibusPersonEmployment.ibusOrganization.LoadOrgPlan();
            bool lblnLEExists = ibusPersonEmployment.ibusOrganization.iclbOrgPlan.Where(o => o.icdoOrgPlan.plan_id == busConstant.PlanIdLE ||
                                                                                    o.icdoOrgPlan.plan_id == busConstant.PlanIdLEWithoutPS ||
                                                                                    o.icdoOrgPlan.plan_id == busConstant.PlanIdBCILawEnf || // pir 7943
                                                                                    o.icdoOrgPlan.plan_id == busConstant.PlanIdStatePublicSafety || o.icdoOrgPlan.plan_id == busConstant.PlanIdNG).Any(); //PIR 25729
            Collection<cdoCodeValue> lclbJobClasses = new Collection<cdoCodeValue>();
            DataTable ldtbResult = Select<cdoCodeValue>(new string[1] { "CODE_ID" }, new object[1] { 322 }, null, null);
            ldtbResult = ldtbResult.AsEnumerable().Where(o =>
                (o.Field<string>("DATA1") == ibusPersonEmployment.ibusOrganization.icdoOrganization.emp_category_value) &&
                ((o.Field<string>("DATA3") == ibusPersonEmployment.ibusOrganization.icdoOrganization.org_code) || (o.Field<string>("DATA3").IsEmpty()))).AsDataTable();
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

        #region UCS - 032

        public busPersonAccountEmploymentDetail ibusESSPersonAccountEmpDetail { get; set; }

        public void ESSLoadPersonAccountEmploymentDetail()
        {
            DataTable ldtPAEmpDetail = Select<cdoPersonAccountEmploymentDetail>(new string[1] { "person_employment_dtl_id" },
                                                                                new object[1] { icdoPersonEmploymentDetail.person_employment_dtl_id }, null, null);
            ibusESSPersonAccountEmpDetail = new busPersonAccountEmploymentDetail { icdoPersonAccountEmploymentDetail = new cdoPersonAccountEmploymentDetail() };
            if (ldtPAEmpDetail.Rows.Count > 0)
                ibusESSPersonAccountEmpDetail.icdoPersonAccountEmploymentDetail.LoadData(ldtPAEmpDetail.Rows[0]);
        }

        #endregion

        # region UCS 24 member portal
        //is temporary employment detail within the first sixmonths
        public bool iblnIsMSSTempEmploymentDtlWithinFirstSixMonths { get; set; }
        public void SetMSSIsTempEmploymentDtlWithinFirstSixMonths(DateTime adtDateToBeCompared)
        {
            iblnIsMSSTempEmploymentDtlWithinFirstSixMonths = false;
            if (icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
            {
                if (busGlobalFunctions.CheckDateOverlapping(adtDateToBeCompared,
                                                                 icdoPersonEmploymentDetail.start_date,
                                                                 icdoPersonEmploymentDetail.start_date.AddMonths(6)))
                    iblnIsMSSTempEmploymentDtlWithinFirstSixMonths = true;
            }
        }

        public Collection<busWssPersonAccountEnrollmentRequest> iclbEnrollmentRequest { get; set; }
        public void LoadEnrollmentRequest()
        {
            if (iclbEnrollmentRequest.IsNull())
                iclbEnrollmentRequest = new Collection<busWssPersonAccountEnrollmentRequest>();

            DataTable ldtbList = Select<cdoWssPersonAccountEnrollmentRequest>(new string[1] { "person_employment_dtl_id" },
                new object[1] { icdoPersonEmploymentDetail.person_employment_dtl_id }, null, null);
            iclbEnrollmentRequest = GetCollection<busWssPersonAccountEnrollmentRequest>(ldtbList, "icdoWssPersonAccountEnrollmentRequest");
        }
        #endregion

        // SYS PIR ID 2357
        public bool IsEmploymentDetailLinkedToAnyAccounts()
        {
            if (iclbAllPersonAccountEmpDtl.IsNull()) LoadAllPersonAccountEmploymentDetails();
            if (iclbAllPersonAccountEmpDtl.Where(lobj => lobj.icdoPersonAccountEmploymentDetail.person_account_id > 0 &&
                    lobj.icdoPersonAccountEmploymentDetail.election_value == busConstant.PersonAccountElectionValueEnrolled).Any())
                return true;
            return false;
        }

        public void LoadMemberType(Collection<busPersonAccount> acblPersonAccount = null)
        {
            LoadMemberType(DateTime.Now, acblPersonAccount);
        }

        public void LoadMemberType(DateTime adtEffectiveDate, busPersonAccountEmploymentDetail abusPaEmpDetail)
        {
            if (abusPaEmpDetail != null)
            {
                if (abusPaEmpDetail.ibusPersonAccount == null)
                    abusPaEmpDetail.LoadPersonAccount();

                if (abusPaEmpDetail.ibusPersonAccount.ibusPersonEmploymentDetail == null)
                    abusPaEmpDetail.ibusPersonAccount.ibusPersonEmploymentDetail = this;

                if (abusPaEmpDetail.ibusPersonAccount.ibusOrgPlan == null)
                    abusPaEmpDetail.ibusPersonAccount.LoadOrgPlan(icdoPersonEmploymentDetail.start_date);

                if (abusPaEmpDetail.ibusPersonAccount.ibusOrgPlan.iclbOrgPlanMemberType == null)
                    abusPaEmpDetail.ibusPersonAccount.ibusOrgPlan.LoadMemberType();

                if (iclbPlanMemberTypeCrossref == null)
                    LoadPlanMemberTypeCrossRef();

                DateTime ldtEffectiveDate = adtEffectiveDate;
                if (icdoPersonEmploymentDetail.end_date_no_null < ldtEffectiveDate)
                    ldtEffectiveDate = icdoPersonEmploymentDetail.end_date;

                foreach (busPlanMemberTypeCrossref lbusPlanMemberTypeCrossref in iclbPlanMemberTypeCrossref)
                {
                    if (lbusPlanMemberTypeCrossref.icdoPlanMemberTypeCrossref.plan_id == abusPaEmpDetail.icdoPersonAccountEmploymentDetail.plan_id)
                    {
                        if (abusPaEmpDetail.ibusPersonAccount.ibusOrgPlan.iclbOrgPlanMemberType
                            .Any(i => i.icdoOrgPlanMemberType.member_type_value == lbusPlanMemberTypeCrossref.icdoPlanMemberTypeCrossref.member_type_value &&
                                busGlobalFunctions.CheckDateOverlapping(ldtEffectiveDate, i.icdoOrgPlanMemberType.start_date, i.icdoOrgPlanMemberType.end_date)))
                        {
                            icdoPersonEmploymentDetail.derived_member_type_value = lbusPlanMemberTypeCrossref.icdoPlanMemberTypeCrossref.member_type_value;
                            icdoPersonEmploymentDetail.derived_member_type_description = lbusPlanMemberTypeCrossref.icdoPlanMemberTypeCrossref.member_type_description;
                            icdoPersonEmploymentDetail.istrBenefitPlanForRetr = abusPaEmpDetail.ibusPersonAccount.ibusOrgPlan.iclbOrgPlanMemberType.Where(i => i.icdoOrgPlanMemberType.member_type_value == lbusPlanMemberTypeCrossref.icdoPlanMemberTypeCrossref.member_type_value
                            && busGlobalFunctions.CheckDateOverlapping(ldtEffectiveDate, i.icdoOrgPlanMemberType.start_date, i.icdoOrgPlanMemberType.end_date)).FirstOrDefault().icdoOrgPlanMemberType.benefit_plan;

                            icdoPersonEmploymentDetail.istrRHICBenefitPlanForRetr = abusPaEmpDetail.ibusPersonAccount.ibusOrgPlan.iclbOrgPlanMemberType.Where(i => i.icdoOrgPlanMemberType.member_type_value == lbusPlanMemberTypeCrossref.icdoPlanMemberTypeCrossref.member_type_value
                            && busGlobalFunctions.CheckDateOverlapping(ldtEffectiveDate, i.icdoOrgPlanMemberType.start_date, i.icdoOrgPlanMemberType.end_date)).FirstOrDefault().icdoOrgPlanMemberType.rhic_benefit_plan;
                            break;
                        }
                    }
                }
            }
        }


        public void LoadMemberType(DateTime adtEffectiveDate, Collection<busPersonAccount> acblPersonAccount = null)
        {
            //As discussed with satya, if the member has both DB and DC plans, sort the plan records by end date desc, start date
            if (iclbAllPersonAccountEmpDtl == null)
                LoadAllPersonAccountEmploymentDetails();
            var lenuEnrolledList = iclbAllPersonAccountEmpDtl.Where(i => i.icdoPersonAccountEmploymentDetail.election_value == busConstant.PersonAccountElectionValueEnrolled
                                                                && i.icdoPersonAccountEmploymentDetail.person_account_id > 0);
            foreach (busPersonAccountEmploymentDetail lbusPAEmpDetail in lenuEnrolledList)
            {
                 #region Performance related code change
                if (acblPersonAccount.IsNotNull() && acblPersonAccount.Count == 0)
                {
                    busPersonAccount lbusPersonAccount = acblPersonAccount.Where<busPersonAccount>(
                           lbusPerAccRet => lbusPerAccRet.icdoPersonAccount.person_account_id ==
                               lbusPAEmpDetail.icdoPersonAccountEmploymentDetail.person_account_id).FirstOrDefault();

                    if (lbusPersonAccount == null)
                    {
                        if (lbusPAEmpDetail.ibusPlan == null)
                        {
                            if (idtbPlanCacheData != null)
                                lbusPAEmpDetail.idtbPlanCacheData = idtbPlanCacheData;
                            lbusPAEmpDetail.LoadPlan();
                        }
                    if (lbusPAEmpDetail.ibusPersonAccount == null)
                        lbusPAEmpDetail.LoadPersonAccount();

                    if (lbusPAEmpDetail.ibusPersonAccount.ibusPersonAccountRetirement == null)
                        lbusPAEmpDetail.ibusPersonAccount.LoadPersonAccountRetirement();
                        lbusPAEmpDetail.ibusPersonAccount.ibusPlan = lbusPAEmpDetail.ibusPlan;

                        acblPersonAccount.Add(lbusPAEmpDetail.ibusPersonAccount);
                    }
                    else
                    {
                        lbusPAEmpDetail.ibusPersonAccount = lbusPersonAccount;
                        lbusPAEmpDetail.ibusPlan = lbusPersonAccount.ibusPlan;
                    }

                    if (lbusPAEmpDetail.ibusPersonAccount.ibusPersonAccountRetirement.ibusHistoryAsofDate == null)
                        lbusPAEmpDetail.ibusPersonAccount.ibusPersonAccountRetirement.LoadEnrolledHistoryByMonthYear(adtEffectiveDate);
                }
                else
                {
                    if (lbusPAEmpDetail.ibusPlan == null)
                    {
                        if (idtbPlanCacheData != null)
                            lbusPAEmpDetail.idtbPlanCacheData = idtbPlanCacheData;
                        lbusPAEmpDetail.LoadPlan();
                    }
                    if (lbusPAEmpDetail.ibusPersonAccount == null)
                        lbusPAEmpDetail.LoadPersonAccount();

                    if (lbusPAEmpDetail.ibusPersonAccount.ibusPersonAccountRetirement == null)
                        lbusPAEmpDetail.ibusPersonAccount.LoadPersonAccountRetirement();

                    if (lbusPAEmpDetail.ibusPersonAccount.ibusPersonAccountRetirement.ibusHistoryAsofDate == null)
                        lbusPAEmpDetail.ibusPersonAccount.ibusPersonAccountRetirement.LoadEnrolledHistoryByMonthYear(adtEffectiveDate);
                }
                 #endregion
            }

            busPersonAccountEmploymentDetail lbusPaEmpDetail = lenuEnrolledList.Where(i => (i.ibusPlan.IsDBRetirementPlan() || i.ibusPlan.IsDCRetirementPlan() || i.ibusPlan.IsHBRetirementPlan()) &&
                                    i.ibusPersonAccount.ibusPersonAccountRetirement.ibusHistoryAsofDate.icdoPersonAccountRetirementHistory.plan_participation_status_value ==
                                    busConstant.PlanParticipationStatusRetirementEnrolled)
                                    .OrderByDescending(i => i.ibusPersonAccount.icdoPersonAccount.end_date_no_null)
                                    .ThenByDescending(i => i.ibusPersonAccount.icdoPersonAccount.start_date).FirstOrDefault();
            if (lbusPaEmpDetail != null)
                LoadMemberType(adtEffectiveDate, lbusPaEmpDetail);
        }

        public bool iblnUseQueryToLoadData = true;
        public Collection<busPlanMemberTypeCrossref> iclbAllPlanMemberTypeCrossref { get; set; }
        public void LoadAllPlanMemberTypeCrossRef()
        {
            DataTable ldtbList = Select<cdoPlanMemberTypeCrossref>(new string[0] { }, new object[0] { }, null, null);
            iclbAllPlanMemberTypeCrossref = GetCollection<busPlanMemberTypeCrossref>(ldtbList, "icdoPlanMemberTypeCrossref");
        }

        public Collection<busPlanMemberTypeCrossref> iclbPlanMemberTypeCrossref { get; set; }
        public void LoadPlanMemberTypeCrossRef()
        {
            iclbPlanMemberTypeCrossref = new Collection<busPlanMemberTypeCrossref>();
            //job class value null in DB
            if (!icdoPersonEmploymentDetail.job_class_value.IsNullOrEmpty() && !icdoPersonEmploymentDetail.type_value.IsNullOrEmpty())
            {
                if (iblnUseQueryToLoadData)
                {
                    if (!icdoPersonEmploymentDetail.job_class_value.IsNullOrEmpty() && !icdoPersonEmploymentDetail.type_value.IsNullOrEmpty())
                    {
                        DataTable ldtbList = Select<cdoPlanMemberTypeCrossref>(
                          new string[2] { enmPlanMemberTypeCrossref.employment_type_value.ToString(), enmPlanMemberTypeCrossref.job_class_value.ToString() },
                          new object[2] { icdoPersonEmploymentDetail.type_value, icdoPersonEmploymentDetail.job_class_value }, null, null);
                        iclbPlanMemberTypeCrossref = GetCollection<busPlanMemberTypeCrossref>(ldtbList, "icdoPlanMemberTypeCrossref");
                    }
                }
                else
                {
                    if (iclbAllPlanMemberTypeCrossref == null)
                        LoadAllPlanMemberTypeCrossRef();

                    var lenuList = iclbAllPlanMemberTypeCrossref.Where(i => i.icdoPlanMemberTypeCrossref.employment_type_value == icdoPersonEmploymentDetail.type_value &&
                                                                            i.icdoPlanMemberTypeCrossref.job_class_value == icdoPersonEmploymentDetail.job_class_value);
                    foreach (busPlanMemberTypeCrossref lbusPMTCrossref in lenuList)
                    {
                        iclbPlanMemberTypeCrossref.Add(lbusPMTCrossref);
                    }
                }
            }
        }

        //UAT PIR 1505 - 457 Payroll Frequency Validation
        public bool IsPayrollFrequencyDifferWhenTransferEmployment()
        {
            if (iclbPersonAccountEmpDtl != null)
            {
                bool lblnEnrollingInDeffCompPlanAndNotLinked = iclbPersonAccountEmpDtl.Any(i => i.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDeferredCompensation
                                                                           && i.icdoPersonAccountEmploymentDetail.election_value == busConstant.PersonAccountElectionValueEnrolled
                                                                           && i.icdoPersonAccountEmploymentDetail.person_account_id == 0);
                if (lblnEnrollingInDeffCompPlanAndNotLinked)
                {
                    //Is transfer employment
                    if (ibusPersonEmployment == null)
                        LoadPersonEmployment();

                    if (ibusPersonEmployment.ibusPerson.IsNull())
                        ibusPersonEmployment.LoadPerson();

                    ibusPersonEmployment.ibusPerson.LoadPersonAccountByPlan(busConstant.PlanIdDeferredCompensation);
                    if (ibusPersonEmployment.ibusPerson.icolPersonAccountByPlan.Count > 0)
                    {
                        busPersonAccount lbusDeffCompAccount = ibusPersonEmployment.ibusPerson.icolPersonAccountByPlan.Where(i => (!i.IsWithDrawn())).FirstOrDefault();
                        if (lbusDeffCompAccount.IsNotNull())
                        {
                            if (lbusDeffCompAccount.iclbAccountEmploymentDetail == null)
                                lbusDeffCompAccount.LoadPersonAccountEmploymentDetails();

                            foreach (busPersonAccountEmploymentDetail lbusPAEmpDetail in lbusDeffCompAccount.iclbAccountEmploymentDetail)
                            {
                                if (lbusPAEmpDetail.ibusEmploymentDetail == null)
                                    lbusPAEmpDetail.LoadPersonEmploymentDetail();

                                if (lbusPAEmpDetail.ibusEmploymentDetail.ibusPersonEmployment == null)
                                    lbusPAEmpDetail.ibusEmploymentDetail.LoadPersonEmployment();
                            }

                            //Get the end date of previous employment
                            busPersonAccountEmploymentDetail lbusPreviousPAEmpDetail = lbusDeffCompAccount.iclbAccountEmploymentDetail
                                                            .Where(i => i.ibusEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date != DateTime.MinValue)
                                                            .OrderByDescending(i => i.ibusEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date).FirstOrDefault();

                            if (lbusPreviousPAEmpDetail != null)
                            {
                                busPersonEmployment lbusPreviousEmployment = lbusPreviousPAEmpDetail.ibusEmploymentDetail.ibusPersonEmployment;
                                if (lbusPreviousEmployment != null)
                                {
                                    int lintDiffInDays = busGlobalFunctions.DateDiffInDays(lbusPreviousEmployment.icdoPersonEmployment.end_date, ibusPersonEmployment.icdoPersonEmployment.start_date);
                                    if (lintDiffInDays >= 0 && lintDiffInDays <= 31)
                                    {
                                        //Check the Payroll Frequency
                                        if (lbusPreviousEmployment.ibusOrganization == null)
                                            lbusPreviousEmployment.LoadOrganization();

                                        if (lbusPreviousEmployment.ibusOrganization.iclbOrgPlan == null)
                                            lbusPreviousEmployment.ibusOrganization.LoadOrgPlan();

                                        busOrgPlan lbusPreviousOrgPlan = lbusPreviousEmployment.ibusOrganization.iclbOrgPlan
                                                                            .Where(i => i.icdoOrgPlan.plan_id == busConstant.PlanIdDeferredCompensation &&
                                                                            busGlobalFunctions.CheckDateOverlapping(lbusPreviousEmployment.icdoPersonEmployment.end_date,
                                                                            i.icdoOrgPlan.participation_start_date, i.icdoOrgPlan.participation_end_date)).FirstOrDefault();
                                        if (lbusPreviousOrgPlan != null)
                                        {
                                            if (ibusPersonEmployment.ibusOrganization == null)
                                                ibusPersonEmployment.LoadOrganization();

                                            if (ibusPersonEmployment.ibusOrganization.iclbOrgPlan == null)
                                                ibusPersonEmployment.ibusOrganization.LoadOrgPlan();

                                            busOrgPlan lbusOrgPlan = ibusPersonEmployment.ibusOrganization.iclbOrgPlan
                                                                         .Where(i => i.icdoOrgPlan.plan_id == busConstant.PlanIdDeferredCompensation &&
                                                                         busGlobalFunctions.CheckDateOverlapping(icdoPersonEmploymentDetail.start_date,
                                                                         i.icdoOrgPlan.participation_start_date, i.icdoOrgPlan.participation_end_date)).FirstOrDefault();
                                            if (lbusOrgPlan != null)
                                            {
                                                if (lbusPreviousOrgPlan.icdoOrgPlan.report_frequency_value != lbusOrgPlan.icdoOrgPlan.report_frequency_value)
                                                {
                                                    return true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }


        // SFN 17627
        public decimal idecMainTempEEContribution
        {
            get
            {
                return LoadMonthlyEEPercentage(busConstant.MemberTypeMainTemp, busConstant.PlanIdMain, false);
            }
        }

        // SFN 54366
        public decimal idecDCTempEEContribution
        {
            get
            {
                return LoadMonthlyEEPercentage(busConstant.MemberTypeDCTemp, busConstant.PlanIdDC, false);
            }
        }

        // SFN 54366
        public decimal idecDCTempEEContributionExcludingRHIC
        {
            get
            {
                return LoadMonthlyEEPercentage(busConstant.MemberTypeDCTemp, busConstant.PlanIdDC, true);
            }
        }

        public decimal idecEERHIC { get; set; }

        //Fixed as per Maik's mail dated 7/6/2010
        public decimal LoadMonthlyEEPercentage(string astrMemberType, int aintPlanId, bool ablnExcludeRHIC, bool ablnIsPurchaseCorr = false)
        {
            //get person account id
            if (ibusPersonEmployment.IsNull())
                LoadPersonEmployment();
            if (ibusPersonEmployment.ibusPerson.IsNull())
                ibusPersonEmployment.LoadPerson();
            ibusPersonEmployment.ibusPerson.LoadPersonAccountByPlan(aintPlanId);

            decimal ldecPercentage = 0M; idecEERHIC = 0M;

            if (ibusPersonEmployment.ibusOrganization.IsNull())
                ibusPersonEmployment.LoadOrganization();

            if (ibusPersonEmployment.ibusOrganization.iclbOrgPlan.IsNull())
                ibusPersonEmployment.ibusOrganization.LoadOrgPlan();

            var lenumPlans = ibusPersonEmployment.ibusOrganization.iclbOrgPlan.Where(lobjOP => lobjOP.icdoOrgPlan.plan_id == aintPlanId
                && busGlobalFunctions.CheckDateOverlapping(DateTime.Now, lobjOP.icdoOrgPlan.participation_start_date, lobjOP.icdoOrgPlan.participation_end_date));

            if (lenumPlans.Count() > 0)
            {
                busOrgPlan lobjOrgPlan = new busOrgPlan { icdoOrgPlan = new cdoOrgPlan() };
                lobjOrgPlan = lenumPlans.FirstOrDefault();

                if (lobjOrgPlan.iclbOrgPlanMemberType.IsNull())
                    lobjOrgPlan.LoadMemberType();

                if (lobjOrgPlan.iclbPlanRetirementRate.IsNull())
                    lobjOrgPlan.LoadEmployerRetirementRates();

                var lenumRate = lobjOrgPlan.iclbPlanRetirementRate.Where(lobjRate => lobjRate.icdoPlanRetirementRate.member_type_value == astrMemberType);

                if (lenumRate.Count() > 0)
                {
                    if (aintPlanId != busConstant.PlanIdDC && aintPlanId != busConstant.PlanIdDC2020 && aintPlanId != busConstant.PlanIdDC2025) //PIR 20232 //PIR 25920
                    {
                        ldecPercentage = lenumRate
                              .Sum(lobjRetRate => lobjRetRate.icdoPlanRetirementRate.ee_pre_tax + lobjRetRate.icdoPlanRetirementRate.ee_post_tax
                                  + lobjRetRate.icdoPlanRetirementRate.ee_emp_pickup + lobjRetRate.icdoPlanRetirementRate.ee_rhic);
                        if (ablnIsPurchaseCorr)
                        {
                            ldecPercentage += lenumRate.Sum(lobj => lobj.icdoPlanRetirementRate.er_post_tax + lobj.icdoPlanRetirementRate.er_rhic);
                            //PIR 18212
                            if (ibusPersonEmployment.ibusPerson.icdoPerson.db_addl_contrib == busConstant.Flag_Yes)
                                ldecPercentage += lenumRate.Sum(lobj => lobj.icdoPlanRetirementRate.addl_ee_pre_tax + lobj.icdoPlanRetirementRate.addl_ee_post_tax +
                                                                lobj.icdoPlanRetirementRate.addl_ee_emp_pickup);
                        }

                        ldecPercentage = Math.Round(ldecPercentage, 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        if (!ablnExcludeRHIC)
                        {
                            ldecPercentage = lenumRate
                                .Sum(lobjRetRate => lobjRetRate.icdoPlanRetirementRate.ee_pre_tax + lobjRetRate.icdoPlanRetirementRate.ee_post_tax
                                      + lobjRetRate.icdoPlanRetirementRate.ee_emp_pickup + lobjRetRate.icdoPlanRetirementRate.ee_rhic);
                            if (ablnIsPurchaseCorr)
                            {
                                ldecPercentage += lenumRate.Sum(lobj => lobj.icdoPlanRetirementRate.er_post_tax + lobj.icdoPlanRetirementRate.er_rhic);
                            }
                        }
                        else
                        {
                            ldecPercentage = lenumRate
                              .Sum(lobjRetRate => lobjRetRate.icdoPlanRetirementRate.ee_pre_tax + lobjRetRate.icdoPlanRetirementRate.ee_post_tax
                                    + lobjRetRate.icdoPlanRetirementRate.ee_emp_pickup);
                            if (ablnIsPurchaseCorr)
                            {
                                ldecPercentage += lenumRate.Sum(lobj => lobj.icdoPlanRetirementRate.er_post_tax);
                            }
                        }

                        ldecPercentage = Math.Round(ldecPercentage, 2, MidpointRounding.AwayFromZero);
                        idecEERHIC = lenumRate.Sum(lobjRetRate => lobjRetRate.icdoPlanRetirementRate.ee_rhic);
                        idecEERHIC = Math.Round(idecEERHIC, 2, MidpointRounding.AwayFromZero);
                    }
                }
            }
            return Math.Round(ldecPercentage, 2, MidpointRounding.AwayFromZero);
        }

        #region UAT PIR - 1107
        public busPersonEmploymentDetail ibusCurrentPersonEmploymentDetail { get; set; }
        public busPersonEmploymentDetail ibusNewPersonEmploymentDetail { get; set; }
        public busPersonEmploymentDetail ibusOldPersonEmploymentDetail { get; set; }

        public void LoadOldPersonEmploymentDetail()
        {
            if (ibusOldPersonEmploymentDetail.IsNull())
                ibusOldPersonEmploymentDetail = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };

            if (!icdoPersonEmploymentDetail.ihstOldValues.IsNullOrEmpty())
            {
                if (icdoPersonEmploymentDetail.ihstOldValues["start_date"].ToString().IsNotNullOrEmpty())
                {
                    DateTime ldtDateTime = DateTime.MinValue;
                    string lstrStartDate = icdoPersonEmploymentDetail.ihstOldValues["start_date"].ToString();
                    bool lblnSuccess = DateTime.TryParse(lstrStartDate, out ldtDateTime);
                    if (lblnSuccess)
                        ibusOldPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date = ldtDateTime;
                }
                if (icdoPersonEmploymentDetail.ihstOldValues["end_date"].ToString().IsNotNullOrEmpty())
                {
                    DateTime ldtDateTime = DateTime.MinValue;
                    string lstrEndDate = icdoPersonEmploymentDetail.ihstOldValues["end_date"].ToString();
                    bool lblnSuccess = DateTime.TryParse(lstrEndDate, out ldtDateTime);
                    if (lblnSuccess)
                        ibusOldPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date = ldtDateTime;
                }
                if (icdoPersonEmploymentDetail.ihstOldValues["seasonal_value"].IsNotNull())
                {
                    ibusOldPersonEmploymentDetail.icdoPersonEmploymentDetail.seasonal_value = icdoPersonEmploymentDetail.ihstOldValues["seasonal_value"].ToString();
                }
                if (icdoPersonEmploymentDetail.ihstOldValues["hourly_value"].IsNotNull())
                {
                    ibusOldPersonEmploymentDetail.icdoPersonEmploymentDetail.hourly_value = icdoPersonEmploymentDetail.ihstOldValues["hourly_value"].ToString();

                }

            }
        }

        public bool iblnIsUpdateSeasonalHourlyButtonHit { get; set; }
        //check is seasonal / hourly changed
        public bool IsSeasonalOrHourlyChanged()
        {
            if (ibusOldPersonEmploymentDetail.IsNull())
                LoadOldPersonEmploymentDetail();
            if (ibusOldPersonEmploymentDetail.icdoPersonEmploymentDetail.seasonal_value != icdoPersonEmploymentDetail.seasonal_value
                || ibusOldPersonEmploymentDetail.icdoPersonEmploymentDetail.hourly_value != icdoPersonEmploymentDetail.hourly_value)
                return true;
            return false;
        }
        #endregion

        //UAT PIR: 1589. Error should be raised for a Retirement Plan Account (for RTW) if the disability Payee Account is in Receiving status.
        public bool IsRTWDisabilityPayeeAccountinReceivingStatus()
        {
            if (iclbPersonAccountEmpDtl != null)
            {
                if (ibusPersonEmployment == null)
                    LoadPersonEmployment();

                if (ibusPersonEmployment.ibusPerson == null)
                    ibusPersonEmployment.LoadPerson();

                foreach (busPersonAccountEmploymentDetail lobjPersonAccountEmpDtl in _iclbPersonAccountEmpDtl)
                {
                    if (lobjPersonAccountEmpDtl.ibusPlan == null)
                        lobjPersonAccountEmpDtl.LoadPlan();

                    if (lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.election_value == busConstant.PlanOptionStatusValueEnrolled)
                    {
                        //Link the Person Account Only if not set
                        if (lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.person_account_id == 0)
                        {
                            if (ibusPersonEmployment.ibusPerson.icolPersonAccount == null)
                                ibusPersonEmployment.ibusPerson.LoadPersonAccount();

                            busPersonAccount lbusPersonAccount =
                                ibusPersonEmployment.ibusPerson.icolPersonAccount.Where(
                                    i =>
                                    i.icdoPersonAccount.plan_id == lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.plan_id &&
                                    (!i.IsWithDrawn())).FirstOrDefault();

                            if (lbusPersonAccount != null)
                            {
                                int lintRTWPayeeAccountID = 0;

                                if (lobjPersonAccountEmpDtl.ibusPlan.IsRetirementPlan())
                                {
                                    //to avoid getting old person account id for RTW member                              
                                    if (ibusPersonEmployment.ibusPerson.IsRTWMember(lobjPersonAccountEmpDtl.icdoPersonAccountEmploymentDetail.plan_id,
                                        busConstant.PayeeStatusForRTW.IgnoreStatus, ref lintRTWPayeeAccountID))
                                    {
                                        //UAT PIR 1589 - Link with existing account when the person has suspended plan with disability payee account.
                                        if (lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended)
                                        {
                                            busPayeeAccount lbusDisaPayeeAccount = new busPayeeAccount();
                                            lbusDisaPayeeAccount.FindPayeeAccount(lintRTWPayeeAccountID);
                                            if (lbusDisaPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                                            {
                                                lbusDisaPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                                                if (lbusDisaPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusReceiving())
                                                {
                                                    return true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public string istrEmploymentStartDateLongFormat
        {
            get
            {
                return icdoPersonEmploymentDetail.start_date.ToString(busConstant.DateFormatLongDate);
            }
        }
        //uAT PIR 2220
        //when the plan is waived
        //only retirement plans
        private void PostESSmessage()
        {
            if (iarrChangeLog.Count > 0)
            {
                var lclbPersonAccountEmploymentDetail = iarrChangeLog.OfType<cdoPersonAccountEmploymentDetail>();

                if (lclbPersonAccountEmploymentDetail.Count() > 0)
                {
                    foreach (cdoPersonAccountEmploymentDetail lobjPAED in lclbPersonAccountEmploymentDetail)
                    {
                        if (lobjPAED.election_value == busConstant.PersonAccountElectionValueWaived)
                        {
                            if (ibusPersonEmployment.ibusPerson == null)
                                ibusPersonEmployment.LoadPerson();
                            //post message to employer
                            if (ibusPersonEmployment.IsNull())
                                LoadPersonEmployment();

                            if (ibusPersonEmployment.ibusPerson.IsNull())
                                ibusPersonEmployment.LoadPerson();

                            busPlan lbusPlan = new busPlan { icdoPlan = new cdoPlan() };
                            lbusPlan.FindPlan(lobjPAED.plan_id);
                            // if (lbusPlan.IsRetirementPlan())
                            {
                                string lstrPrioityValue = string.Empty;
                                busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(31, iobjPassInfo, ref lstrPrioityValue),
                                    ibusPersonEmployment.ibusPerson.icdoPerson.FullName, ibusPersonEmployment.ibusPerson.icdoPerson.LastFourDigitsOfSSN, lbusPlan.icdoPlan.plan_name),
                                    lstrPrioityValue, aintOrgID: ibusPersonEmployment.icdoPersonEmployment.org_id,
                                        astrContactRoleValue: busConstant.OrgContactRoleAuthorizedAgent);
                            }
                            //else if (lbusPlan.IsInsurancePlan())
                            //{
                            //    if (lobjPAED.plan_id == busConstant.PlanIdGroupHealth
                            //        || lobjPAED.plan_id == busConstant.PlanIdDental
                            //        || lobjPAED.plan_id == busConstant.PlanIdVision
                            //        || lobjPAED.plan_id == busConstant.PlanIdMedicarePartD)
                            //    {
                            //        busWSSHelper.PublishESSMessage(0, 0, string.Format("{0} elected to waive participation in {1}",
                            //            ibusPersonEmployment.ibusPerson.icdoPerson.FullName, lbusPlan.icdoPlan.plan_name),
                            //            busConstant.WSS_MessageBoard_Priority_Low, aintOrgID: ibusPersonEmployment.icdoPersonEmployment.org_id,
                            //                astrContactRoleValue: busConstant.OrgContactRoleAuthorizedAgent);
                            //    }
                            //}
                            //else if (lbusPlan.IsDeferredCompPlan())
                            //{
                            //    busWSSHelper.PublishESSMessage(0, 0, string.Format("{0} elected to waive participation in {1}", ibusPerson.icdoPerson.FullName, ibusPlan.icdoPlan.plan_name),
                            //                   busConstant.WSS_MessageBoard_Priority_Low, aintOrgID: ibusPersonEmployment.icdoPersonEmployment.org_id,
                            //                       astrContactRoleValue: busConstant.OrgContactRoleAuthorizedAgent);
                            //}

                            //busWSSHelper.PublishMSSMessage(0, 0, string.Format("Your enrollment/change for the {0} has been processed.", lbusPlan.icdoPlan.plan_name),
                            //                                             busConstant.WSS_MessageBoard_Priority_High, ibusPersonEmployment.ibusPerson.icdoPerson.person_id);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// prod pir 4702 : need to post message to ESS when transfer occurs
        /// </summary>
        private void PostESSMessageForTransfer()
        {
            string lstrPrioityValue = string.Empty;
            busPersonAccount lobjPersonAccount = new busPersonAccount();
            if (iarrChangeLog.Count > 0)
            {
                var lclbPersonAccountEmploymentDetail = iarrChangeLog.OfType<cdoPersonAccountEmploymentDetail>();

                if (lclbPersonAccountEmploymentDetail.Count() > 0)
                {
                    foreach (cdoPersonAccountEmploymentDetail lobjPAED in lclbPersonAccountEmploymentDetail)
                    {
                        if (lobjPAED.election_value == busConstant.PersonAccountElectionValueEnrolled)
                        {
                            lobjPersonAccount = new busPersonAccount();
                            lobjPersonAccount.FindPersonAccount(lobjPAED.person_account_id);
                            if (lobjPersonAccount.ibusPreviousEmploymentDetail == null)
                                lobjPersonAccount.LoadPreviousEmploymentDetail();
                            if (lobjPersonAccount.ibusPreviousEmploymentDetail != null && lobjPersonAccount.ibusPreviousEmploymentDetail.icdoPersonEmploymentDetail != null &&
                                lobjPersonAccount.ibusPreviousEmploymentDetail.icdoPersonEmploymentDetail.person_employment_id != icdoPersonEmploymentDetail.person_employment_id)
                            {
                                if (ibusPersonEmployment.ibusPerson == null)
                                    ibusPersonEmployment.LoadPerson();
                                //post message to employer
                                if (ibusPersonEmployment.IsNull())
                                    LoadPersonEmployment();

                                if (ibusPersonEmployment.ibusPerson.IsNull())
                                    ibusPersonEmployment.LoadPerson();

                                busPlan lbusPlan = new busPlan { icdoPlan = new cdoPlan() };
                                lbusPlan.FindPlan(lobjPAED.plan_id);
                                if (istrIsPIR9115Enabled == busConstant.Flag_No)
                                {
                                    if (lobjPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled &&
                                        (lbusPlan.IsRetirementPlan() || lbusPlan.IsDCRetirementPlan()))
                                    {
                                        busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(30, iobjPassInfo, ref lstrPrioityValue),
                                            ibusPersonEmployment.ibusPerson.icdoPerson.FullName, ibusPersonEmployment.ibusPerson.icdoPerson.LastFourDigitsOfSSN, lbusPlan.icdoPlan.plan_name,
                                            lobjPersonAccount.icdoPersonAccount.history_change_date.ToString("d", CultureInfo.CreateSpecificCulture("en-US"))), //prod pir 6294
                                            lstrPrioityValue, aintOrgID: ibusPersonEmployment.icdoPersonEmployment.org_id,
                                                astrContactRoleValue: busConstant.OrgContactRoleAuthorizedAgent);
                                    }
                                    else if (lobjPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                                        (lbusPlan.icdoPlan.plan_id == busConstant.PlanIdDental || lbusPlan.icdoPlan.plan_id == busConstant.PlanIdVision ||
                                        lbusPlan.icdoPlan.plan_id == busConstant.PlanIdGroupHealth))
                                    {
                                        busPersonAccountGhdv lobjGHDV = new busPersonAccountGhdv();
                                        lobjGHDV.FindGHDVByPersonAccountID(lobjPersonAccount.icdoPersonAccount.person_account_id);
                                        lobjGHDV.FindPersonAccount(lobjPersonAccount.icdoPersonAccount.person_account_id);
                                        lobjGHDV.icdoPersonAccount.person_employment_dtl_id = icdoPersonEmploymentDetail.person_employment_dtl_id;
                                        lobjGHDV.LoadPlanEffectiveDate();
                                        lobjGHDV.RecalculatePremiumBasedOnPlanEffectiveDate();
                                        string lstrCoverageCodeOrLevelOfCoverage = string.Empty;
                                        if (lobjGHDV.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth
                                            || lobjGHDV.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD)
                                        {
                                            lobjGHDV.LoadCoverageCodeDescription();
                                            lstrCoverageCodeOrLevelOfCoverage = lobjGHDV.istrCoverageCode;
                                        }
                                        else
                                            lstrCoverageCodeOrLevelOfCoverage = lobjGHDV.icdoPersonAccountGhdv.level_of_coverage_description;
                                        busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(26, iobjPassInfo, ref lstrPrioityValue),
                                        ibusPersonEmployment.ibusPerson.icdoPerson.FullName, ibusPersonEmployment.ibusPerson.icdoPerson.LastFourDigitsOfSSN, lbusPlan.icdoPlan.plan_name, lstrCoverageCodeOrLevelOfCoverage,
                                        lobjGHDV.icdoPersonAccountGhdv.MonthlyPremiumAmount, lobjGHDV.icdoPersonAccount.history_change_date.ToString("d", CultureInfo.CreateSpecificCulture("en-US"))),
                                        lstrPrioityValue, aintPlanID: lbusPlan.icdoPlan.plan_id, aintOrgID: ibusPersonEmployment.icdoPersonEmployment.org_id,
                                        astrContactRoleValue: busConstant.OrgContactRoleAuthorizedAgent);
                                    }
                                    else if (lobjPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                                        lbusPlan.icdoPlan.plan_id == busConstant.PlanIdGroupLife)
                                    {
                                        busPersonAccountLife lobjLife = new busPersonAccountLife();
                                        lobjLife.FindPersonAccountLife(lobjPersonAccount.icdoPersonAccount.person_account_id);
                                        lobjLife.FindPersonAccount(lobjPersonAccount.icdoPersonAccount.person_account_id);
                                        lobjLife.icdoPersonAccount.person_employment_dtl_id = icdoPersonEmploymentDetail.person_employment_dtl_id;
                                        lobjLife.LoadLifeOptionData();
                                        lobjLife.LoadOrgPlan();
                                        lobjLife.LoadProviderOrgPlan();
                                        lobjLife.GetMonthlyPremiumAmount();
                                        string lstrMessage = string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(27, iobjPassInfo, ref lstrPrioityValue),
                                           ibusPersonEmployment.ibusPerson.icdoPerson.FullName, ibusPersonEmployment.ibusPerson.icdoPerson.LastFourDigitsOfSSN);
                                        if (lobjLife.idecLifeBasicPremiumAmt > 0.0M ||
                                            (lobjLife.idecLifeSupplementalPremiumAmount + lobjLife.idecDependentSupplementalPremiumAmt + lobjLife.idecSpouseSupplementalPremiumAmt) > 0.0M)
                                            lstrMessage += string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(28, iobjPassInfo, ref lstrPrioityValue),
                                                lobjLife.idecLifeBasicPremiumAmt, (lobjLife.idecLifeSupplementalPremiumAmount + lobjLife.idecDependentSupplementalPremiumAmt +
                                                lobjLife.idecSpouseSupplementalPremiumAmt), lobjLife.icdoPersonAccount.history_change_date.ToString("d", CultureInfo.CreateSpecificCulture("en-US")));
                                        busWSSHelper.PublishESSMessage(0, 0, lstrMessage, lstrPrioityValue, aintPlanID: busConstant.PlanIdGroupLife,
                                                aintOrgID: ibusPersonEmployment.icdoPersonEmployment.org_id,
                                                astrContactRoleValue: busConstant.OrgContactRoleAuthorizedAgent);
                                    }
                                    else if (lobjPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                                        lbusPlan.icdoPlan.plan_id == busConstant.PlanIdLTC)
                                    {
                                        busPersonAccountLtc lobjLTC = new busPersonAccountLtc();
                                        lobjLTC.FindPersonAccount(lobjPersonAccount.icdoPersonAccount.person_account_id);
                                        lobjLTC.icdoPersonAccount.person_employment_dtl_id = icdoPersonEmploymentDetail.person_employment_dtl_id;
                                        lobjLTC.LoadLtcOptionUpdateMember();
                                        lobjLTC.LoadLtcOptionUpdateSpouse();
                                        lobjLTC.LoadOrgPlan();
                                        lobjLTC.LoadProviderOrgPlan();
                                        lobjLTC.GetMonthlyPremiumAmount();
                                        string lstrMessage = string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(29, iobjPassInfo, ref lstrPrioityValue),
                                            ibusPersonEmployment.ibusPerson.icdoPerson.FullName, ibusPersonEmployment.ibusPerson.icdoPerson.LastFourDigitsOfSSN, lobjLTC.idecTotalMonthlyPremium,
                                            lobjLTC.icdoPersonAccount.history_change_date.ToString("d", CultureInfo.CreateSpecificCulture("en-US")));
                                        busWSSHelper.PublishESSMessage(0, 0, lstrMessage, lstrPrioityValue, aintPlanID: busConstant.PlanIdLTC,
                                        aintOrgID: ibusPersonEmployment.icdoPersonEmployment.org_id, astrContactRoleValue: busConstant.OrgContactRoleAuthorizedAgent);
                                    }
                                    else if (lobjPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled &&
                                        lbusPlan.icdoPlan.plan_id == busConstant.PlanIdFlex)
                                    {
                                        busPersonAccountFlexComp lobjFlexComp = new busPersonAccountFlexComp();
                                        lobjFlexComp.FindPersonAccountFlexComp(lobjPersonAccount.icdoPersonAccount.person_account_id);
                                        lobjFlexComp.FindPersonAccount(lobjPersonAccount.icdoPersonAccount.person_account_id);
                                        lobjFlexComp.icdoPersonAccount.person_employment_dtl_id = icdoPersonEmploymentDetail.person_employment_dtl_id;
                                        lobjFlexComp.LoadFlexCompOptionUpdate();
                                        lobjFlexComp.LoadFlexCompConversion();
                                        string lstrProviderList = string.Empty;
                                        foreach (busPersonAccountFlexcompConversion lobjConversion in lobjFlexComp.iclbFlexcompConversion)
                                        {
                                            if (lobjConversion.ibusProvider.IsNull())
                                                lobjConversion.LoadProvider();
                                            //there is a trim command line below, so if changing the end string need to modify there too.
                                            lstrProviderList += lobjConversion.ibusProvider.icdoOrganization.org_name + ", ";
                                        }
                                        decimal ldecMedicalSalaryPerPayPreiod = lobjFlexComp.iclbFlexCompOption.Where(lobjOption => lobjOption.icdoPersonAccountFlexCompOption.level_of_coverage_value
                                            == busConstant.FlexLevelOfCoverageMedicareSpending).FirstOrDefault().icdoPersonAccountFlexCompOption.annual_pledge_amount;
                                        DateTime ldtMedicalEffectiveDate = lobjFlexComp.iclbFlexCompOption.Where(lobjOption => lobjOption.icdoPersonAccountFlexCompOption.level_of_coverage_value
                                            == busConstant.FlexLevelOfCoverageMedicareSpending).FirstOrDefault().icdoPersonAccountFlexCompOption.effective_start_date;

                                        decimal ldecDependentSalaryPerPayPreiod = lobjFlexComp.iclbFlexCompOption.Where(lobjOption => lobjOption.icdoPersonAccountFlexCompOption.level_of_coverage_value
                                            == busConstant.FlexLevelOfCoverageDependentSpending).FirstOrDefault().icdoPersonAccountFlexCompOption.annual_pledge_amount;
                                        DateTime ldtDependentEffectiveDate = lobjFlexComp.iclbFlexCompOption.Where(lobjOption => lobjOption.icdoPersonAccountFlexCompOption.level_of_coverage_value
                                            == busConstant.FlexLevelOfCoverageDependentSpending).FirstOrDefault().icdoPersonAccountFlexCompOption.effective_start_date;
                                        string lstrMessage = string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(21, iobjPassInfo, ref lstrPrioityValue),
                                            ibusPersonEmployment.ibusPerson.icdoPerson.FullName, ibusPersonEmployment.ibusPerson.icdoPerson.LastFourDigitsOfSSN);
                                        if (ldecDependentSalaryPerPayPreiod > 0.0M)
                                        {
                                            lstrMessage += string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(22, iobjPassInfo, ref lstrPrioityValue),
                                                ldecDependentSalaryPerPayPreiod, ldtDependentEffectiveDate.ToString("d", CultureInfo.CreateSpecificCulture("en-US")));
                                        }
                                        if (ldecMedicalSalaryPerPayPreiod > 0.0M)
                                        {
                                            lstrMessage += string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(23, iobjPassInfo, ref lstrPrioityValue),
                                                ldecMedicalSalaryPerPayPreiod, ldtMedicalEffectiveDate.ToString("d", CultureInfo.CreateSpecificCulture("en-US")));
                                        }
                                        if (!string.IsNullOrEmpty(lstrProviderList))
                                        {
                                            //removing the last "," and " " before storing into message
                                            lstrProviderList = lstrProviderList.TrimEnd(new char[2] { ',', ' ' });
                                            lstrMessage += string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(24, iobjPassInfo, ref lstrPrioityValue), lstrProviderList);
                                        }
                                        busWSSHelper.PublishESSMessage(0, 0, lstrMessage,
                                                lstrPrioityValue, aintPlanID: busConstant.PlanIdFlex, aintOrgID: ibusPersonEmployment.icdoPersonEmployment.org_id,
                                                    astrContactRoleValue: busConstant.OrgContactRoleAuthorizedAgent);
                                    }
                                    else if (lobjPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompEnrolled &&
                                        (lbusPlan.icdoPlan.plan_id == busConstant.PlanIdDeferredCompensation || lbusPlan.icdoPlan.plan_id == busConstant.PlanIdOther457))
                                    {
                                        busPersonAccountDeferredComp lobjDefComp = new busPersonAccountDeferredComp();
                                        lobjDefComp.FindPersonAccountDeferredComp(lobjPersonAccount.icdoPersonAccount.person_account_id);
                                        lobjDefComp.FindPersonAccount(lobjPersonAccount.icdoPersonAccount.person_account_id);
                                        lobjDefComp.icdoPersonAccount.person_employment_dtl_id = icdoPersonEmploymentDetail.person_employment_dtl_id;
                                        lobjDefComp.LoadActivePersonAccountProviders();
                                        busPersonAccountDeferredCompProvider lobjProvider = new busPersonAccountDeferredCompProvider { icdoPersonAccountDeferredCompProvider = new cdoPersonAccountDeferredCompProvider() };
                                        if (lobjDefComp.icolPersonAccountDeferredCompProvider.Count > 0)
                                            lobjProvider = lobjDefComp.icolPersonAccountDeferredCompProvider[0];
                                        if (lobjDefComp.ibusOrgPlan == null)
                                            lobjDefComp.LoadOrgPlan();
                                        if (lobjDefComp.ibusProviderOrgPlan == null)
                                            lobjDefComp.LoadProviderOrgPlan();
                                        if (lobjDefComp.ibusProviderOrgPlan.ibusOrganization == null)
                                            lobjDefComp.ibusProviderOrgPlan.LoadOrganization();

                                        string lstrMessage = string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(20, iobjPassInfo, ref lstrPrioityValue),
                                            ibusPersonEmployment.ibusPerson.icdoPerson.FullName, ibusPersonEmployment.ibusPerson.icdoPerson.LastFourDigitsOfSSN,
                                            lobjDefComp.ibusProviderOrgPlan.ibusOrganization.icdoOrganization.org_name, lobjProvider.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt,
                                            lobjProvider.icdoPersonAccountDeferredCompProvider.start_date.ToString("d", CultureInfo.CreateSpecificCulture("en-US")));

                                        busWSSHelper.PublishESSMessage(0, 0, lstrMessage, lstrPrioityValue, aintPlanID: busConstant.PlanIdDeferredCompensation,
                                                            aintOrgID: ibusPersonEmployment.icdoPersonEmployment.org_id,
                                                            astrContactRoleValue: busConstant.OrgContactRoleAuthorizedAgent);
                                    }
                                }
                                else
                                {
                                    busGlobalFunctions.PostESSMessage(ibusPersonEmployment.icdoPersonEmployment.org_id,
                                                                lbusPlan.icdoPlan.plan_id, iobjPassInfo);
                                }
                            }
                        }
                        lobjPersonAccount = null;
                    }
                }
            }
        }
        //PIR 14656 - Add hard error when DB_ADDL_CONTRIB is Y but Member Type associated with Employer does not have values in the new fields
        public bool IsEmployerMemberTypeHasNoValuesInRateAddlFields()
        {
            if (iclbAllPersonAccountEmpDtl.IsNull()) LoadAllPersonAccountEmploymentDetails();
            foreach (busPersonAccountEmploymentDetail lbusPersonAccountEmploymentDetail in iclbAllPersonAccountEmpDtl)
            {
                if (lbusPersonAccountEmploymentDetail.ibusPlan.IsNull()) lbusPersonAccountEmploymentDetail.LoadPlan();
            }
            //Hard error need to be thrown only when db plan is enrolled.
            busPersonAccountEmploymentDetail lbusDBPersonAccountEmploymentDetail = iclbAllPersonAccountEmpDtl
                                                                                    .Where(i => (i.ibusPlan.IsDBRetirementPlan() || i.ibusPlan.IsHBRetirementPlan()) && i.icdoPersonAccountEmploymentDetail.election_value == busConstant.PersonAccountElectionValueEnrolled)
                                                                                    .FirstOrDefault();
            if (ibusPersonEmployment.IsNull()) LoadPersonEmployment();
            if (ibusPersonEmployment.ibusPerson.IsNull()) ibusPersonEmployment.LoadPerson();
            if ((!string.IsNullOrEmpty(ibusPersonEmployment.ibusPerson.icdoPerson.db_addl_contrib) &&
                ibusPersonEmployment.ibusPerson.icdoPerson.db_addl_contrib.ToUpper() == busConstant.Flag_Yes) &&
                lbusDBPersonAccountEmploymentDetail.IsNotNull())
            {
                if (ibusPersonEmployment.ibusOrganization.IsNull()) ibusPersonEmployment.LoadOrganization();
                if (ibusPersonEmployment.ibusOrganization.iclbOrgPlan.IsNull())
                    ibusPersonEmployment.ibusOrganization.LoadOrgPlan();
                DateTime leffecitvedate = DateTime.Now;
                if (icdoPersonEmploymentDetail.end_date_no_null < leffecitvedate) leffecitvedate = icdoPersonEmploymentDetail.end_date_no_null;
                busOrgPlan lbusOrgPlan = ibusPersonEmployment.ibusOrganization.
                    iclbOrgPlan.Where(i => (i.ibusPlan.IsDBRetirementPlan() || i.ibusPlan.IsHBRetirementPlan()) && 
                        busGlobalFunctions.CheckDateOverlapping(leffecitvedate, i.icdoOrgPlan.participation_start_date, i.icdoOrgPlan.participation_end_date)
                        && i.icdoOrgPlan.plan_id == lbusDBPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id).FirstOrDefault();
                if (lbusOrgPlan.IsNotNull())
                {
                    lbusOrgPlan.LoadMemberType();
                    DataTable ldtbPlanMemberTypeList = Select<cdoPlanMemberTypeCrossref>(
                          new string[3] { enmPlanMemberTypeCrossref.employment_type_value.ToString(), enmPlanMemberTypeCrossref.job_class_value.ToString(), enmPlanMemberTypeCrossref.plan_id.ToString() },
                          new object[3] { icdoPersonEmploymentDetail.type_value, icdoPersonEmploymentDetail.job_class_value, lbusDBPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id }, null, null);
                    Collection<busPlanMemberTypeCrossref> lclbPlanMemberTypeCrossref = GetCollection<busPlanMemberTypeCrossref>(ldtbPlanMemberTypeList, "icdoPlanMemberTypeCrossref");

                    Collection<busOrgPlanMemberType> lclcOrgPlanMemberType = lbusOrgPlan.iclbOrgPlanMemberType
                        .Where(i => busGlobalFunctions.CheckDateOverlapping(leffecitvedate, i.icdoOrgPlanMemberType.start_date, i.icdoOrgPlanMemberType.end_date)).ToList().ToCollection();
                    foreach (busOrgPlanMemberType lbusOrgPlanMemberType in lclcOrgPlanMemberType)
                    {
                        if (lclbPlanMemberTypeCrossref.Any(i => i.icdoPlanMemberTypeCrossref.member_type_value == lbusOrgPlanMemberType.icdoOrgPlanMemberType.member_type_value))
                        {
                            DataTable ldtbList = busBase.SelectWithOperator<cdoPlanRetirementRate>(new string[3] { enmPlanRetirementRate.member_type_value.ToString(), enmPlanRetirementRate.effective_date.ToString(), enmPlanRetirementRate.plan_id.ToString() }, 
                                                                                            new string[3] { "=", "<=", "=" },
                                                                                        new object[3] { lbusOrgPlanMemberType.icdoOrgPlanMemberType.member_type_value, leffecitvedate, lbusDBPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id }, "EFFECTIVE_DATE DESC");
                            if (ldtbList.Rows.Count > 0)
                            {
                                cdoPlanRetirementRate lcdoPlanRetirementRate = new cdoPlanRetirementRate();
                                lcdoPlanRetirementRate.LoadData(ldtbList.Rows[0]);
                                if (lcdoPlanRetirementRate.addl_ee_emp_pickup == 0.0M
                                    && lcdoPlanRetirementRate.addl_ee_post_tax == 0.0M && lcdoPlanRetirementRate.addl_ee_pre_tax == 0.0M)
                                {
                                    return true;
                                }
                            }
                            else
                                return true;
                        }
                    }
                }
                //else
                //{
                //    return true;
                //}
            }
            return false;
        }

        public void IsPersonEnrolledinhealthonPreviousEmpolyment()
        {
            busPersonEmploymentDetail lobjEmploymentDtl = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
            DataTable ldtbEmpDtl = busBase.Select("entPersonEmploymentDetail.LoadEnrolledPreviousEmploymentDetail", new object[1] { ibusPersonEmployment.icdoPersonEmployment.person_id });

            if (ldtbEmpDtl.Rows.Count > 0)
            {
                lobjEmploymentDtl.icdoPersonEmploymentDetail.LoadData(ldtbEmpDtl.Rows[0]);
                if (lobjEmploymentDtl.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent)
                {
                    if (lobjEmploymentDtl.iclbAllPersonAccountEmpDtl == null)
                        lobjEmploymentDtl.LoadAllPersonAccountEmploymentDetails(false);

                    busPersonAccountEmploymentDetail lbusPersonAccountEmploymentDetail = lobjEmploymentDtl
                                                                                        .iclbAllPersonAccountEmpDtl.Where(pa => pa.icdoPersonAccountEmploymentDetail.person_account_id > 0)
                                                                                        .FirstOrDefault(x =>
                                                                                        x.icdoPersonAccountEmploymentDetail.plan_id ==
                                                                                        busConstant.PlanIdGroupHealth ||
                                                                                        x.icdoPersonAccountEmploymentDetail.plan_id ==
                                                                                        busConstant.PlanIdDental ||
                                                                                        x.icdoPersonAccountEmploymentDetail.plan_id ==
                                                                                        busConstant.PlanIdVision ||
                                                                                        x.icdoPersonAccountEmploymentDetail.plan_id ==
                                                                                        busConstant.PlanIdFlex
                                                                                        );
                    bool lblnIsPersonEnrolledinGHDVorFlexinPreviousEmployment = false;
                    if (lbusPersonAccountEmploymentDetail.IsNotNull() && 
                        lbusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.person_account_id > 0)
                    {
                        lbusPersonAccountEmploymentDetail.LoadPersonAccount();
                        if (lbusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdFlex)
                        {
                            lbusPersonAccountEmploymentDetail.ibusPersonAccount.LoadPersonAccountFlex();
                            lbusPersonAccountEmploymentDetail.ibusPersonAccount.ibusPersonAccountFlex.LoadFlexCompHistory();

                            if (lbusPersonAccountEmploymentDetail
                                .ibusPersonAccount
                                .ibusPersonAccountFlex
                                .iclbFlexCompHistory.Any(history => history.icdoPersonAccountFlexCompHistory.plan_participation_status_value
                                                                    == busConstant.PlanParticipationStatusFlexCompEnrolled &&
                                                                    busGlobalFunctions.CheckDateOverlapping(history.icdoPersonAccountFlexCompHistory.effective_start_date,
                                                                    lobjEmploymentDtl.icdoPersonEmploymentDetail.start_date,
                                                                    lobjEmploymentDtl.icdoPersonEmploymentDetail.end_date)))
                            {
                                lblnIsPersonEnrolledinGHDVorFlexinPreviousEmployment = true;                                
                            }
                        }
                        else if(lbusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdGroupHealth ||
                                lbusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDental ||
                                lbusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdVision)
                        {
                            lbusPersonAccountEmploymentDetail.ibusPersonAccount.LoadPersonAccountGHDV();
                            lbusPersonAccountEmploymentDetail.ibusPersonAccount.ibusPersonAccountGHDV.LoadPersonAccountGHDVHistory();

                            if (lbusPersonAccountEmploymentDetail
                                .ibusPersonAccount
                                .ibusPersonAccountGHDV
                                .iclbPersonAccountGHDVHistory.Any(history => history.icdoPersonAccountGhdvHistory.plan_participation_status_value
                                                                    == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                                                                    busGlobalFunctions.CheckDateOverlapping(history.icdoPersonAccountGhdvHistory.start_date,
                                                                    lobjEmploymentDtl.icdoPersonEmploymentDetail.start_date,
                                                                    lobjEmploymentDtl.icdoPersonEmploymentDetail.end_date)))
                            {
                                lblnIsPersonEnrolledinGHDVorFlexinPreviousEmployment = true;
                            }
                        }
                        if(lblnIsPersonEnrolledinGHDVorFlexinPreviousEmployment)
                        {
                            lobjEmploymentDtl.icdoPersonEmploymentDetail.cobra_letter_status_value = busConstant.COBRALetterStatusNotSent;
                            lobjEmploymentDtl.icdoPersonEmploymentDetail.Update();
                        }
                    }
                }
            }
        }
        public void InitiateWorkFlowForServicePurchasePaymentInstallmentsEmploymentChange(int aintPersonID)
        {
            DataTable ldtActivityInstance = busBase.Select("entSolBpmActivityInstance.LoadRunningInstancesByPersonAndProcess",
                      new object[2] { aintPersonID, busConstant.Map_Service_Purchase_Payment_Installments_Employment_Change });
            if ((ldtActivityInstance.Rows.Count == 0))
            {
                busWorkflowHelper.InitiateBpmRequest(busConstant.Map_Service_Purchase_Payment_Installments_Employment_Change, aintPersonID, 0, 0, iobjPassInfo);
            }
        }

        /// <summary>
        /// return employment detail with earliest employment start with type Permanant by person id and plan id 
        /// PIR 25920 New Plan DC 2025
        /// </summary>
        public busPersonEmploymentDetail LoadEarliestPermEmploymentDetail(int aintPersonId, int aintPlanID)
        {
            busPersonAccountEmploymentDetail lobjPersonAccountEmploymentDetail = new busPersonAccountEmploymentDetail { icdoPersonAccountEmploymentDetail = new cdoPersonAccountEmploymentDetail() };
            DataTable ldtbPersonAccountEmploymentDetail = busBase.Select("entPersonAccountEmploymentDetail.GetEarliestPermEmpDtl", new object[2] { aintPersonId, aintPlanID });

            if (ldtbPersonAccountEmploymentDetail.Rows.Count > 0)
                lobjPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.LoadData(ldtbPersonAccountEmploymentDetail.Rows[0]);
            
                lobjPersonAccountEmploymentDetail.LoadPersonEmploymentDetail(true);
                //lobjEmploymentDtl.FindPersonEmploymentDetail(lobjPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.person_employment_dtl_id);
            
            return lobjPersonAccountEmploymentDetail.ibusEmploymentDetail;
        }
    }
}
