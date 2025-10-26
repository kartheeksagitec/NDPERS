using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.CustomDataObjects;
using System.Data;
using System.Collections.ObjectModel;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;
using Sagitec.Common;
using System.Collections;                                     

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busMSSPersonAccountRetirementWeb : busPersonAccountRetirement
    {
        public int iintEnrollmentRequestID { get; set; }
        public bool iblnIsRetirementAccountAvailable { get; set; }
        public string istrIsDBRetirementEnrollment { get; set; }
        public string istrIsDBOptionalEnrollment { get; set; }
        public string istrIsDBElectedOfficialEnrollment { get; set; }
        public string istrIsDCOptionalEnrollment { get; set; }
        public string istrIsDCRetirementEnrollment { get; set; }
        public string istrIsDCEligible { get; set; }

        public decimal MemberContribution { get; set; }
        public decimal VestedEmployerContriubtion { get; set; }
        public decimal TotalMemberAccountBalance { get; set; }
        public decimal TotalTaxableAmount { get; set; }
        public decimal TotalNonTaxableAmount { get; set; }
        //public int iintAddlEEContributionPercent { get; set; }
        public Collection<busPersonEmploymentDetail> iclbMSSPersonEmploymentDetail { get; set; }

        //load contributions for Retirement DB/DC plan        
        public busPersonAccountRetirement ibusMSSContributionSummaryForRetirementPlans { get; set; }
        public void LoadMSSContributionSummaryForRetirementPlans()
        {
            ibusMSSContributionSummaryForRetirementPlans = new busPersonAccountRetirement { icdoPersonAccountRetirement = new cdoPersonAccountRetirement() };

            DataTable ldtbContributionList = Select("cdoPerson.LoadContribtuionSummaryForDB", new object[3] { DateTime.Now, icdoPersonAccount.person_id, icdoPersonAccount.plan_id });
            if (ldtbContributionList.Rows.Count > 0)
            {
                ibusMSSContributionSummaryForRetirementPlans.icdoPersonAccountRetirement.LoadData(ldtbContributionList.Rows[0]);
            }
            
            //pir 7946
            if (ibusMSSContributionSummaryForRetirementPlans.icdoPersonAccount.IsNull())
                ibusMSSContributionSummaryForRetirementPlans.icdoPersonAccount = new cdoPersonAccount();
            ibusMSSContributionSummaryForRetirementPlans.icdoPersonAccount.person_account_id = icdoPersonAccount.person_account_id;
            ibusMSSContributionSummaryForRetirementPlans.LoadLTDSummary();
            //end

            // PIR 9967
            MemberContribution = ibusMSSContributionSummaryForRetirementPlans.post_tax_ee_amount_ltd +
                                 ibusMSSContributionSummaryForRetirementPlans.pre_tax_ee_amount_ltd +
                                 ibusMSSContributionSummaryForRetirementPlans.ee_er_pickup_amount_ltd +
                                 ibusMSSContributionSummaryForRetirementPlans.post_tax_ee_ser_pur_cont_ltd +
                                 ibusMSSContributionSummaryForRetirementPlans.pre_tax_ee_ser_pur_cont_ltd +
                                 ibusMSSContributionSummaryForRetirementPlans.ee_rhic_ser_pur_cont_ltd;
            VestedEmployerContriubtion = ibusMSSContributionSummaryForRetirementPlans.er_vested_amount_ltd;
            TotalMemberAccountBalance = MemberContribution + VestedEmployerContriubtion +
                                        ibusMSSContributionSummaryForRetirementPlans.interest_amount_ltd +
                                        ibusMSSContributionSummaryForRetirementPlans.ee_rhic_amount_ltd;
            TotalNonTaxableAmount = ibusMSSContributionSummaryForRetirementPlans.post_tax_ee_amount_ltd + 
                                  ibusMSSContributionSummaryForRetirementPlans.ee_rhic_amount_ltd +
                                  ibusMSSContributionSummaryForRetirementPlans.ee_rhic_ser_pur_cont_ltd +
                                  ibusMSSContributionSummaryForRetirementPlans.post_tax_ee_ser_pur_cont_ltd;
            TotalTaxableAmount = ibusMSSContributionSummaryForRetirementPlans.pre_tax_ee_amount_ltd +
                                 ibusMSSContributionSummaryForRetirementPlans.ee_er_pickup_amount_ltd +
                                 ibusMSSContributionSummaryForRetirementPlans.er_vested_amount_ltd +
                                 ibusMSSContributionSummaryForRetirementPlans.interest_amount_ltd +
                                 ibusMSSContributionSummaryForRetirementPlans.pre_tax_ee_ser_pur_cont_ltd;
        }

        public Collection<busPersonBeneficiary> iclbMSSPersonAccountBeneficiaries { get; set; }
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

        public ArrayList btnUpdateMutualFundFlag_Click()
        {
            ArrayList larrList = new ArrayList();
            if (icdoPersonAccountRetirement.person_account_id > 0)
            {
                icdoPersonAccountRetirement.mutual_fund_window_flag = busConstant.Flag_Yes;
                icdoPersonAccountRetirement.Update();
            }
            larrList.Add(this);
            return larrList;
        }
        /// <summary>
        /// update the additional ee contribution percentage
        /// PIR 25920 New Plan DC 2025
        /// </summary>
        /// <returns></returns>
        public ArrayList btnUpdatePersonAccountADECPercentage_Click()
        {
            ArrayList larrList = new ArrayList();
            if (icdoPersonAccountRetirement.person_account_id > 0)
            {
                //icdoPersonAccount.addl_ee_contribution_percent = iintAddlEEContributionPercent;
                icdoPersonAccount.modified_by = iobjPassInfo.istrUserID;
                icdoPersonAccount.modified_date = DateTime.Now;
                icdoPersonAccount.Update();
            }
            larrList.Add(this);
            return larrList;
        }
        public void LoadAdditionalContributionPercentage()
        {
            if (ibusPersonAccount.IsNull()) LoadPersonAccount();
            if (ibusPersonEmploymentDetail.IsNull()) LoadPersonEmploymentDetail();
            if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                iintAddlEEContributionPercent = ibusPersonAccount.icdoPersonAccount.addl_ee_contribution_percent_temp;
            else
                iintAddlEEContributionPercent = ibusPersonAccount.icdoPersonAccount.addl_ee_contribution_percent;
        }
        /// <summary>
        /// return true if contribution is null or employtment detail start date within limit of 30 days 
        /// PIR 25920 New Plan DC 2025
        /// </summary>
        public bool lblnIsVisibleADECAmountLink
        {
            get
            {
                if (ibusPersonEmploymentDetail.IsNull()) LoadPersonEmploymentDetail();
                if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.IsNull()) LoadPersonEmploymentDetail();
                if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull()) ibusPersonEmploymentDetail.LoadPersonEmployment();
                //busPersonEmploymentDetail lbusValidateDateRangePersonEmploymentDetail = new busPersonEmploymentDetail();
                //lbusValidateDateRangePersonEmploymentDetail = ibusPersonEmploymentDetail.LoadEarliestPermEmploymentDetail(ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.person_id, icdoPersonAccount.plan_id);

                //int lintEmploymentDaysDiff = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary ?
                //                    busGlobalFunctions.DateDiffInDays(ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date, DateTime.Today) :
                //                    busGlobalFunctions.DateDiffInDays(lbusValidateDateRangePersonEmploymentDetail.icdoPersonEmploymentDetail.start_date, DateTime.Today);
                int lintDaysDiffByEmploymentype = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary ? 180 : 30;
                
                bool lblnReturnTrue = false;
                
                if (lblnIsVisibleADECAmountList)
                {
                    bool lblnIsAddlSelectionNotDone = true;
                    if ((icdoPersonAccountRetirement != null && icdoPersonAccountRetirement.person_account_id > 0))
                    {                        
                        //int lintperson_account_retirement_history_id = 0;
                        //ibusPersonAccount.LoadPersonAccountRetirement();
                        //ibusPersonAccount.ibusPersonAccountRetirement.LoadPreviousHistory();
                        //lintperson_account_retirement_history_id = ibusPersonAccount.ibusPersonAccountRetirement.ibusHistory.icdoPersonAccountRetirementHistory.person_account_retirement_history_id;

                        //DataTable ldtLatestHistory = Select<cdoPersonAccountRetirementHistory>(new string[1] { enmPersonAccountRetirementHistory.person_account_retirement_history_id.ToString() },
                        //                   new object[1] { lintperson_account_retirement_history_id }, null, null);
                        DataTable ldtPersonAccount = Select<cdoPersonAccount>(new string[1] { enmPersonAccount.person_account_id.ToString() },
                                           new object[1] { ibusPersonAccount.icdoPersonAccount.person_account_id }, null, null);
                        if (ldtPersonAccount.IsNotNull() && ldtPersonAccount.Rows.Count > 0)
                        {
                            if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary
                                    && ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent_temp.ToString()] != DBNull.Value)
                            {
                                lblnIsAddlSelectionNotDone = false;
                            }
                            else if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent
                                && ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent.ToString()] != DBNull.Value)
                            {
                                lblnIsAddlSelectionNotDone = false;
                            }
                        }
                    }
                    if (lblnIsAddlSelectionNotDone)
                    {
                        //Special condition if perm emploment and prevoius is temp and not had a ADEC value then allow to selection
                        //if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent && IsMemberAbleTochangeADECTempExtend())
                        //{
                        //    lintEmploymentDaysDiff = busGlobalFunctions.DateDiffInDays(ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date, DateTime.Today);
                        //}
                        //if (lintEmploymentDaysDiff < lintDaysDiffByEmploymentype)
                        //    return true;
                        int lintEmploymentDaysDiff;
                        DateTime ldtAddlEEContributionPercentEndDate = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary ?
                                                icdoPersonAccount.addl_ee_contribution_percent_temp_end_date : icdoPersonAccount.addl_ee_contribution_percent_end_date;
                        if (ldtAddlEEContributionPercentEndDate == DateTime.MinValue)
                        {
                            lintEmploymentDaysDiff = busGlobalFunctions.DateDiffInDays(ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date, busGlobalFunctions.GetSysManagementBatchDate());
                            lblnReturnTrue = lintEmploymentDaysDiff < iintDaysDiffByEmploymentype;
                        }
                        else
                        {
                            lblnReturnTrue = busGlobalFunctions.GetSysManagementBatchDate() < ldtAddlEEContributionPercentEndDate;
                        }
                    }
                    
                }
                return lblnReturnTrue;
            }
        }
        public bool IsMemberAbleTochangeADECTempExtend()
        {
            bool lblnIsMemberAbleTochangeADECTempExtend = false;
            GetAllPersonEmploymentDetail();

            if (ibusPersonEmploymentDetail.ibusPersonEmployment.icolPersonEmploymentDetail.Any(lobjPersonEmplDtl => lobjPersonEmplDtl.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary))
            {
                int lintEmpNumber = 0;
                //current emp is Perm and previous is Temp then return true
                foreach (busPersonEmploymentDetail lobjPersonEmploymentDetail in ibusPersonEmploymentDetail.ibusPersonEmployment.icolPersonEmploymentDetail)
                {
                    if (lintEmpNumber == 1 && lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary
                        && ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent)
                    {
                        lblnIsMemberAbleTochangeADECTempExtend = true;
                        break;
                    }
                    lintEmpNumber++;
                }
            }
            return lblnIsMemberAbleTochangeADECTempExtend;
        }
        public void GetAllPersonEmploymentDetail()
        {
            if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull()) ibusPersonEmploymentDetail.LoadPersonEmployment();
            
            DataTable ldtbList = Select<cdoPersonEmploymentDetail>(
                new string[1] { "person_employment_id" },
                new object[1] { ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.person_employment_id }, null, "start_date desc, end_date desc");
            ibusPersonEmploymentDetail.ibusPersonEmployment.icolPersonEmploymentDetail = GetCollection<busPersonEmploymentDetail>(ldtbList, "icdoPersonEmploymentDetail");
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

        //load methods to set visibility
        public void SetVisibilityNewRequestButtons()
        {
            if (ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDB || ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueHB)       //PIR 25920  New DC plan
            {
                if (busWSSHelper.IsDBElectedRetirementEnrollment(icdoPersonAccount.plan_id, icdoPersonAccount.person_account_id, icdoPersonAccount.person_employment_dtl_id))
                    istrIsDBElectedOfficialEnrollment = busConstant.Flag_Yes;
                if (busWSSHelper.IsDBRetirementOptional(icdoPersonAccount.plan_id, icdoPersonAccount.person_employment_dtl_id))
                    istrIsDBOptionalEnrollment = busConstant.Flag_Yes;

                if (istrIsDBElectedOfficialEnrollment != busConstant.Flag_Yes
                && istrIsDBOptionalEnrollment != busConstant.Flag_Yes)
                    istrIsDBRetirementEnrollment = busConstant.Flag_Yes;
            }
            else if (ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDC)
            {
                if (busWSSHelper.IsDCOptionalRetirementEnrollment(icdoPersonAccount.plan_id, icdoPersonAccount.person_account_id, icdoPersonAccount.person_employment_dtl_id))
                    istrIsDCOptionalEnrollment = busConstant.Flag_Yes;

                if (istrIsDCOptionalEnrollment != busConstant.Flag_Yes)
                    istrIsDCRetirementEnrollment = busConstant.Flag_Yes;
            }
        }
        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            SetVisibilityNewRequestButtons();
        }

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
            bool lblnProceedNext = false;
            if (ibusPlan.IsNull())
                LoadPlan();
            if (ibusPlan.IsDBRetirementPlan() || ibusPlan.IsHBRetirementPlan())
            {
                if (!SetUpdateButtonVisibilityForDCEnrollment())
                {
                    lblnProceedNext = true;
                }
            }
            else
                lblnProceedNext = true;

            if (lblnProceedNext)
            {
                if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled)
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
            }
            return false;
        }

        //if the member is eligible in DC then dont show the DB update button
        //show update button in DC only
        public bool SetUpdateButtonVisibilityForDCEnrollment()
        {
            if (ibusPersonEmploymentDetail.IsNull())
                LoadPersonEmploymentDetail();

            //if (ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl.IsNull())
            //    ibusPersonEmploymentDetail.LoadEnrolledPersonAccountEmploymentDetails();

            //foreach (busPersonAccountEmploymentDetail lbusPAEmpDetail in ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl)
            //{
            //    if (lbusPAEmpDetail.ibusPlan == null)
            //        lbusPAEmpDetail.LoadPlan();
            //}

            if (ibusPerson.IsNull())
                LoadPerson();

            //check if the member is eligible for DC update                            
            ibusPersonEmploymentDetail.LoadAllPersonAccountEmploymentDetails();
            foreach (busPersonAccountEmploymentDetail lbusPAEmpDetail in ibusPersonEmploymentDetail.iclbAllPersonAccountEmpDtl)
            {
                if (lbusPAEmpDetail.ibusPlan == null)
                    lbusPAEmpDetail.LoadPlan();
            }

            bool lblnIsEmployerOfferDC = ibusPersonEmploymentDetail.iclbAllPersonAccountEmpDtl.Any(i => i.ibusPlan.IsDCRetirementPlan());

            bool lblnIsDBAccountExists = ibusPerson.IsDBPersonAccountExists();
            bool lblnIsDCAccountExists = ibusPerson.IsDCPersonAccountExists();

            if ((lblnIsDBAccountExists) && (!lblnIsDCAccountExists))
            {
                if (lblnIsEmployerOfferDC)
                {
                    if (ibusPerson.icolPersonAccount == null)
                        ibusPerson.LoadPersonAccount();
                    foreach (busPersonAccount lbusPersonAccount in ibusPerson.icolPersonAccount)
                    {
                        if (lbusPersonAccount.ibusPlan == null)
                            lbusPersonAccount.LoadPlan();

                        if ((lbusPersonAccount.ibusPlan.IsDBRetirementPlan() || lbusPersonAccount.ibusPlan.IsHBRetirementPlan()) && (!lbusPersonAccount.IsWithDrawn()))
                        {
                            if (lbusPersonAccount.ibusPersonAccountRetirement == null)
                                lbusPersonAccount.LoadPersonAccountRetirement();
                            if (lbusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccountRetirement.dc_eligibility_no_null > DateTime.Now.Date)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        //this is to navigate from the retirement pension plan maintenance to DC enrollment if DC is eligible
        public void SetDCEnrollmentType()
        {

            if (busWSSHelper.IsDCOptionalRetirementEnrollment(busConstant.PlanIdDC, icdoPersonAccount.person_account_id, icdoPersonAccount.person_employment_dtl_id)
                || busWSSHelper.IsDCOptionalRetirementEnrollment(busConstant.PlanIdDC2025, icdoPersonAccount.person_account_id, icdoPersonAccount.person_employment_dtl_id) //PIR 25920
                || busWSSHelper.IsDCOptionalRetirementEnrollment(busConstant.PlanIdDC2020, icdoPersonAccount.person_account_id, icdoPersonAccount.person_employment_dtl_id)) //PIR 20232
                istrIsDCOptionalEnrollment = busConstant.Flag_Yes;

            if (istrIsDCOptionalEnrollment != busConstant.Flag_Yes)
                istrIsDCRetirementEnrollment = busConstant.Flag_Yes;
        }

        // PIR 10695 - to show current enrollment in 'Plan Details' panel
        public DateTime istrStartDate { get; set; }
        public DateTime istrEndDate { get; set; }
        public string istrPlanParticipation { get; set; }

        // PIR 10695 new collection
        public Collection<busPersonAccountRetirementHistory> iclbMSSHistory { get; set; }

        public void LoadMSSHistory()
        {
            int iintIter = 0;
            if (icolPersonAccountRetirementHistory.IsNull())
                LoadPersonAccountRetirementHistory();

            iclbMSSHistory = new Collection<busPersonAccountRetirementHistory>();
            foreach (busPersonAccountRetirementHistory lobjHistory in icolPersonAccountRetirementHistory)
            {
                if (iintIter == 0)
                {
                    istrStartDate = lobjHistory.icdoPersonAccountRetirementHistory.start_date;
                    istrEndDate = lobjHistory.icdoPersonAccountRetirementHistory.end_date;
                    istrPlanParticipation = lobjHistory.icdoPersonAccountRetirementHistory.plan_participation_status_description;
                    iclbMSSHistory.Add(lobjHistory);        //pir 25920 DC 2025 changes shown all history line 
                }
                else
                {
                    if ((lobjHistory.icdoPersonAccountRetirementHistory.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled)
                        && (lobjHistory.icdoPersonAccountRetirementHistory.start_date != lobjHistory.icdoPersonAccountRetirementHistory.end_date))
                    {
                        iclbMSSHistory.Add(lobjHistory);
                        //break;        //pir 25920 DC 2025 changes shown all history line 
                    }
                }
                iintIter++;
            }
        }

        //wfmDefault.aspx file code conversion - btn_OpenPDF method (Instead of "btn_OpenMssForms" method, we will use "btn_OpenPDF" method due to redundant code)
        public string istrDownloadFileName
        {
            get
            {
                DataTable ldtbPathData = utlPassInfo.iobjPassInfo.isrvDBCache.GetCacheData("sgs_system_paths", "path_code = 'MSSHELP'");
                return ldtbPathData.Rows[0]["path_value"].ToString() + "SFN-2560.pdf";
            }
        }
    }
}

