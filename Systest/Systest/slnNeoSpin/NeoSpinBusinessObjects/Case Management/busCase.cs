#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using Sagitec.Common;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Linq;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busCase : busCaseGen
    {
        busDBCacheData idtbDBCache = new busDBCacheData();

        /// <summary>
        /// Load Step Details New mode
        /// Case management details are loaded in the DB cache 
        /// from cache only those step details will be loaded based on the case type
        /// for benefit appeal only it will further based on the appeal type
        /// </summary>
        public void LoadStepDetailsNewMode()
        {
            if (idtbDBCache.idtbCachedCaseManagementStepDetails == null)
                idtbDBCache.idtbCachedCaseManagementStepDetails = busGlobalFunctions.LoadCaseManagementCacheData(iobjPassInfo);

            DateTime ldtEndDate = DateTime.Now;

            var lobj = from o in idtbDBCache.idtbCachedCaseManagementStepDetails.AsEnumerable()
                       where (o["case_type_value"].ToString() == icdoCase.case_type_value)
                       && (busGlobalFunctions.CheckDateOverlapping(DateTime.Now, Convert.ToDateTime(o["start_date"]),
                       (((o["end_date"]) == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(o["end_date"])))))
                       orderby Convert.ToInt32(o["sort_order"]) ascending
                       select o;

            iclbCaseStepDetail = GetCollection<busCaseStepDetail>(lobj.AsDataTable(), "icdoCaseStepDetail");
            if (icdoCase.case_type_value == busConstant.CaseTypeBenefitAppeal)
            {
                var lobjCaseDetail = lobj.Where(o => o["appeal_type_value"].ToString() == icdoCase.appeal_type_value);
                iclbCaseStepDetail = GetCollection<busCaseStepDetail>(lobjCaseDetail.AsDataTable(), "icdoCaseStepDetail");
            }

            AssignCollectionByCaseType(iclbCaseStepDetail);
            SetDefaultValuesForEachStepInNewMode(iclbCaseStepDetail);
        }

        // Set step details in New mode
        // set target date as start date plus no of days required to complete the case step
        // set next start date       
        public void SetDefaultValuesForEachStepInNewMode(Collection<busCaseStepDetail> aclbCaseDetails)
        {
            int lintFirstRecord = 0;
            int lintCount = 0;
            DateTime ldtPreviousStepEndDate = DateTime.MinValue;

            //set default values
            foreach (busCaseStepDetail lobjStepDetail in aclbCaseDetails)
            {
                lobjStepDetail.icdoCaseStepDetail.step_name_description =
                    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(busConstant.CodeIDCaseStep, lobjStepDetail.icdoCaseStepDetail.step_name_value);

                if (lintFirstRecord == 0)
                {
                    lintFirstRecord = 1;
                    lobjStepDetail.icdoCaseStepDetail.start_date = DateTime.Now;
                    lobjStepDetail.icdoCaseStepDetail.target_date =
                        lobjStepDetail.icdoCaseStepDetail.start_date.AddDays(lobjStepDetail.icdoCaseStepDetail.number_of_days);
                    lobjStepDetail.icdoCaseStepDetail.status_value = busConstant.CaseStepStatusValuePending;
                }
                else
                {
                    lobjStepDetail.icdoCaseStepDetail.start_date = ldtPreviousStepEndDate;
                    lobjStepDetail.icdoCaseStepDetail.target_date =
                        lobjStepDetail.icdoCaseStepDetail.start_date.AddDays(lobjStepDetail.icdoCaseStepDetail.number_of_days);
                    lobjStepDetail.icdoCaseStepDetail.status_value = busConstant.CaseStepStatusValuePending;
                }

                ldtPreviousStepEndDate = (lobjStepDetail.icdoCaseStepDetail.end_date == DateTime.MinValue) ?
                                                                      lobjStepDetail.icdoCaseStepDetail.target_date : lobjStepDetail.icdoCaseStepDetail.end_date;
                lobjStepDetail.icdoCaseStepDetail.case_step_detail_id = (lobjStepDetail.icdoCaseStepDetail.case_step_detail_id != 0) ?
                                                                         lobjStepDetail.icdoCaseStepDetail.case_step_detail_id : lintCount;
                lintCount--;
            }
            SetOverAllTime(aclbCaseDetails);
            AssignCollectionByCaseType(aclbCaseDetails);
        }

        //this  will assign the common collection to respective collection based on the case type
        public void AssignCollectionByCaseType(Collection<busCaseStepDetail> aclbCaseDetails)
        {
            switch (icdoCase.case_type_value)
            {
                case busConstant.CaseTypeBenefitAppeal:
                    iclbCaseStepDetailBenefitAppeal = aclbCaseDetails;
                    break;
                case busConstant.CaseTypeFinancialHardship:
                    iclbCaseStepDetailFinancialHardship = aclbCaseDetails;
                    break;
                case busConstant.CaseTypeDisabilityRecertification:
                case busConstant.CaseTypePre1991DisabilityRecertification:
                    iclbCaseStepDetailDisability = aclbCaseDetails;
                    break;
                default:
                    break;
            }
        }

        //this  will assign the common collection to respective collection based on the case type
        public void AssignCollectionByCaseTypeBeforePersist()
        {
            switch (icdoCase.case_type_value)
            {
                case busConstant.CaseTypeBenefitAppeal:
                    iclbCaseStepDetail = iclbCaseStepDetailBenefitAppeal;
                    break;
                case busConstant.CaseTypeFinancialHardship:
                    iclbCaseStepDetail = iclbCaseStepDetailFinancialHardship;
                    break;
                case busConstant.CaseTypeDisabilityRecertification:
                case busConstant.CaseTypePre1991DisabilityRecertification:
                    iclbCaseStepDetail = iclbCaseStepDetailDisability;
                    break;
                default:
                    break;
            }
        }

        //setting the step details in the Update mode.
        //if there is any change in the end date or start date, rest of the step start dates will be changed.
        public void SetDefaultValuesForEachStepInUpdateMode(Collection<busCaseStepDetail> aclbCaseDetails)
        {
            int lintFirstRecord = 0;
            DateTime ldtPreviousStepEndDate = DateTime.MinValue;

            //set default values
            foreach (busCaseStepDetail lobjStepDetail in aclbCaseDetails)
            {
                if (lintFirstRecord == 0)
                {
                    lintFirstRecord = 1;
                }
                else
                {
                    lobjStepDetail.icdoCaseStepDetail.start_date = ldtPreviousStepEndDate;
                }
                lobjStepDetail.icdoCaseStepDetail.step_name_description =
                    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(busConstant.CodeIDCaseStep, lobjStepDetail.icdoCaseStepDetail.step_name_value);

                lobjStepDetail.icdoCaseStepDetail.target_date =
                    lobjStepDetail.icdoCaseStepDetail.start_date.AddDays(lobjStepDetail.icdoCaseStepDetail.number_of_days);

                ldtPreviousStepEndDate = (lobjStepDetail.icdoCaseStepDetail.end_date == DateTime.MinValue) ?
                    lobjStepDetail.icdoCaseStepDetail.target_date : lobjStepDetail.icdoCaseStepDetail.end_date;
            }

            AssignCollectionByCaseType(aclbCaseDetails);
        }

        // this method sets the over all time to complete case
        public void SetOverAllTime(Collection<busCaseStepDetail> aclbCaseDetails)
        {
            //PIR: 1502 Overall Days should be calculated to be between the first step Start Date and the last populated End Date; 
            //which may not always be the last step. I.e. The first 2 steps out of total 4 steps are completed I should be able to 
            //see how many Overall Days have passed.
            int lintLastRecord = aclbCaseDetails.Count;
            if (lintLastRecord > 0)
            {
                DateTime ldtFirstStepEndDate = aclbCaseDetails[0].icdoCaseStepDetail.start_date;

                DateTime ldtLastPopulatedEndDate = DateTime.MinValue;
                foreach (busCaseStepDetail lobjstepdetail in aclbCaseDetails)
                {
                    if (lobjstepdetail.icdoCaseStepDetail.end_date != DateTime.MinValue)
                    {
                        ldtLastPopulatedEndDate = lobjstepdetail.icdoCaseStepDetail.end_date;
                    }
                }

                if ((ldtFirstStepEndDate != DateTime.MinValue)
                    && (ldtLastPopulatedEndDate != DateTime.MinValue))
                {
                    icdoCase.overall_time = busGlobalFunctions.DateDiffInDays(ldtFirstStepEndDate, ldtLastPopulatedEndDate);
                }
            }
        }

        //load employment detail checkif it is hourly based
        //if yes, then set comparable earnings as FAs
        //else set as Salary
        public void SetComparableEarnings()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusBenefitCalculaton == null)
                ibusPayeeAccount.LoadBenefitCalculation();
            if (ibusPayeeAccount.ibusBenefitAccount == null)
                ibusPayeeAccount.LoadBenfitAccount();
            if (ibusPerson == null)
                LoadPerson();
            if (ibusPerson.icolPersonEmployment == null)
                ibusPerson.LoadPersonEmployment();

            ibusPayeeAccount.ibusBenefitCalculaton.LoadFASMonths();

            busPersonEmployment lobjPersonEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };

            var lobj = from o in ibusPerson.icolPersonEmployment.AsEnumerable()
                       where o.icdoPersonEmployment.org_id == ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.retirement_org_id
                       select o;

            lobjPersonEmployment = lobj.FirstOrDefault();
            if ((lobjPersonEmployment.IsNotNull()) && (lobjPersonEmployment.icdoPersonEmployment.person_employment_id > 0))
            {
                if (lobjPersonEmployment.icolPersonEmploymentDetail == null)
                    lobjPersonEmployment.LoadPersonEmploymentDetail();

                var lobjCount = (from o in lobjPersonEmployment.icolPersonEmploymentDetail
                                 where o.icdoPersonEmploymentDetail.hourly_value == busConstant.Flag_Yes_Value
                                 select o).Count();
                // if employee is hourly
                if (lobjCount > 0)
                {
                    icdoCase.comparable_earnings_amount = ibusPayeeAccount.ibusBenefitCalculaton.icdoBenefitCalculation.overridden_final_average_salary == 0 ?
                                                                ibusPayeeAccount.ibusBenefitCalculaton.icdoBenefitCalculation.computed_final_average_salary :
                                                                ibusPayeeAccount.ibusBenefitCalculaton.icdoBenefitCalculation.overridden_final_average_salary;
                }
                else
                {
                    if (ibusPayeeAccount.icdoPayeeAccount.calculation_id == 0)
                    {
                        if (ibusPayeeAccount.ibusApplication.IsNull())
                            ibusPayeeAccount.LoadApplication();
                        ibusPayeeAccount.ibusBenefitCalculaton.icdoBenefitCalculation.retirement_date = ibusPayeeAccount.ibusApplication.icdoBenefitApplication.retirement_date;
                        ibusPayeeAccount.ibusBenefitCalculaton.icdoBenefitCalculation.plan_id = ibusPayeeAccount.ibusApplication.icdoBenefitApplication.plan_id;
                        ibusPayeeAccount.ibusBenefitCalculaton.icdoBenefitCalculation.benefit_account_type_value = ibusPayeeAccount.ibusApplication.icdoBenefitApplication.benefit_account_type_value;
                        ibusPayeeAccount.ibusBenefitCalculaton.LoadBenefitProvisionBenefitType();
                        Collection<busPersonAccountRetirementContribution> lclbPARetirementCont = new Collection<busPersonAccountRetirementContribution>();
                        lclbPARetirementCont = busPersonBase.GetSalaryRecords(
                              ibusPayeeAccount.ibusApplication.icdoBenefitApplication.termination_date,
                              ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id,
                              ibusPayeeAccount.ibusBenefitCalculaton.icdoBenefitCalculation.plan_id,
                              ibusPayeeAccount.ibusBenefitCalculaton.ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_no_periods,
                              ibusPayeeAccount.ibusBenefitCalculaton.ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_no_periods_range, 0);
                        lclbPARetirementCont = busGlobalFunctions.Sort<busPersonAccountRetirementContribution>
                                                ("icdoPersonAccountRetirementContribution.pay_period_year desc," +
                                                 "icdoPersonAccountRetirementContribution.pay_period_month desc",
                                                lclbPARetirementCont);
                        if (lclbPARetirementCont.Count > 0)
                        {
                            icdoCase.comparable_earnings_amount = lclbPARetirementCont[0].icdoPersonAccountRetirementContribution.salary_amount;
                            if (lclbPARetirementCont.Count > 1)
                            {
                                if (icdoCase.comparable_earnings_amount < lclbPARetirementCont[1].icdoPersonAccountRetirementContribution.salary_amount)
                                    icdoCase.comparable_earnings_amount = lclbPARetirementCont[1].icdoPersonAccountRetirementContribution.salary_amount;
                            }
                        }
                    }
                    else
                    {
                        if ((ibusPayeeAccount.ibusBenefitCalculaton.iclbBenefitCalculationFASMonths != null)
                            && (ibusPayeeAccount.ibusBenefitCalculaton.iclbBenefitCalculationFASMonths.Count > 0))
                        {
                            icdoCase.comparable_earnings_amount =
                                ibusPayeeAccount.ibusBenefitCalculaton.iclbBenefitCalculationFASMonths[0].icdoBenefitCalculationFasMonths.salary_amount;
                            if (ibusPayeeAccount.ibusBenefitCalculaton.iclbBenefitCalculationFASMonths.Count > 1)
                            {
                                if (icdoCase.comparable_earnings_amount < ibusPayeeAccount.ibusBenefitCalculaton.iclbBenefitCalculationFASMonths[1].icdoBenefitCalculationFasMonths.salary_amount)
                                    icdoCase.comparable_earnings_amount = ibusPayeeAccount.ibusBenefitCalculaton.iclbBenefitCalculationFASMonths[1].icdoBenefitCalculationFasMonths.salary_amount;
                            }
                        }
                    }
                }
            }
        }

        //load deferred comp details for case type Finanacial Hardship
        //Load Deferred Comp Person Account 
        //check status is either suspended or Enrolled
        //and load all provider detail with assest with provider value as member
        //
        public void LoadDefCompProviderDetails()
        {
            if (ibusPerson == null)
                LoadPerson();
            if (ibusPerson.icolPersonAccount == null)
                ibusPerson.LoadPersonAccount();

            int lintCount = 0;
            if (iclbCaseFinancialHardshipProviderDetail == null)
            {
                iclbCaseFinancialHardshipProviderDetail = new Collection<busCaseFinancialHardshipProviderDetail>();

                foreach (busPersonAccount lobjPersonAccount in ibusPerson.icolPersonAccount)
                {
                    if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDeferredCompensation)
                    {
                        if ((lobjPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompEnrolled)
                            || (lobjPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompSuspended))
                        {
                            if (lobjPersonAccount.ibusPersonDeferredComp == null)
                            {
                                lobjPersonAccount.ibusPersonDeferredComp = new busPersonAccountDeferredComp();
                                lobjPersonAccount.ibusPersonDeferredComp.FindPersonAccountDeferredComp(lobjPersonAccount.icdoPersonAccount.person_account_id);
                            }
                            if (lobjPersonAccount.ibusPersonDeferredComp.icolPersonAccountDeferredCompProvider == null)
                                lobjPersonAccount.ibusPersonDeferredComp.LoadActivePersonAccountProviders();

                            var lobjDefCompProviders = from o in lobjPersonAccount.ibusPersonDeferredComp.icolPersonAccountDeferredCompProvider
                                                       where o.icdoPersonAccountDeferredCompProvider.assets_with_provider_value == busConstant.AssetsWithProviderMember
                                                       select o;

                            foreach (busPersonAccountDeferredCompProvider lobjDefCompProvider in lobjDefCompProviders)
                            {
                                //lobjDefCompProvider.LoadProviderEmployment();
                                //lobjDefCompProvider.LoadEmployer();
                                busCaseFinancialHardshipProviderDetail lobjFinancialHardshipProviderDetail = new busCaseFinancialHardshipProviderDetail
                                {
                                    icdoCaseFinancialHardshipProviderDetail = new cdoCaseFinancialHardshipProviderDetail()
                                };
                                lobjFinancialHardshipProviderDetail.icdoCaseFinancialHardshipProviderDetail.person_account_provider_id =
                                    lobjDefCompProvider.icdoPersonAccountDeferredCompProvider.person_account_provider_id;
                                lobjFinancialHardshipProviderDetail.icdoCaseFinancialHardshipProviderDetail.per_pay_period_contribution_amount =
                                    lobjDefCompProvider.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt;
                                lobjFinancialHardshipProviderDetail.icdoCaseFinancialHardshipProviderDetail.start_date =
                                    lobjDefCompProvider.icdoPersonAccountDeferredCompProvider.start_date;
                                lobjFinancialHardshipProviderDetail.icdoCaseFinancialHardshipProviderDetail.istrProviderName =
                                    lobjDefCompProvider.icdoPersonAccountDeferredCompProvider.company_name;
                                lobjFinancialHardshipProviderDetail.icdoCaseFinancialHardshipProviderDetail.case_financial_hardship_provider_detail_id =
                                    (lobjFinancialHardshipProviderDetail.icdoCaseFinancialHardshipProviderDetail.case_financial_hardship_provider_detail_id != 0) ?
                                    lobjFinancialHardshipProviderDetail.icdoCaseFinancialHardshipProviderDetail.case_financial_hardship_provider_detail_id : lintCount;
                                iclbCaseFinancialHardshipProviderDetail.Add(lobjFinancialHardshipProviderDetail);
                                lintCount--;
                            }
                            break;
                        }
                    }
                }
            }
        }

        public override void LoadCaseFinancialHardshipProviderDetails()
        {
            if (iclbCaseFinancialHardshipProviderDetail == null)
                base.LoadCaseFinancialHardshipProviderDetails();

            foreach (busCaseFinancialHardshipProviderDetail lobjDeferredComp in iclbCaseFinancialHardshipProviderDetail)
            {
                busPersonAccountDeferredCompProvider lobjDeferredCompProvider = new busPersonAccountDeferredCompProvider
                {
                    icdoPersonAccountDeferredCompProvider = new cdoPersonAccountDeferredCompProvider()
                };
                if (lobjDeferredCompProvider.FindPersonAccountDeferredCompProvider(
                    lobjDeferredComp.icdoCaseFinancialHardshipProviderDetail.person_account_provider_id))
                {
                    //lobjDeferredCompProvider.LoadProviderEmployment();
                    //lobjDeferredCompProvider.LoadEmployer();
                    lobjDeferredComp.icdoCaseFinancialHardshipProviderDetail.istrProviderName =
                        lobjDeferredCompProvider.icdoPersonAccountDeferredCompProvider.company_name;
                    lobjDeferredComp.icdoCaseFinancialHardshipProviderDetail.per_pay_period_contribution_amount =
                        lobjDeferredCompProvider.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt;
                    lobjDeferredComp.icdoCaseFinancialHardshipProviderDetail.start_date =
                        lobjDeferredCompProvider.icdoPersonAccountDeferredCompProvider.start_date;
                }
            }
        }

        //load Income verification for case type Disability
        public void LoadIncomeVerfication()
        {
            iclbCaseDisabilityIncomeVerificationDetail = new Collection<busCaseDisabilityIncomeVerificationDetail>();
            DateTime ldtTempDate = icdoCase.recertification_date.AddMonths(-1);
            DateTime ldtDateToCompare = icdoCase.recertification_date.AddMonths(-18);
            while (ldtDateToCompare <= ldtTempDate)
            {
                busCaseDisabilityIncomeVerificationDetail lobjIncomeVerification = new busCaseDisabilityIncomeVerificationDetail
                {
                    icdoCaseDisabilityIncomeVerificationDetail = new cdoCaseDisabilityIncomeVerificationDetail()
                };
                lobjIncomeVerification.icdoCaseDisabilityIncomeVerificationDetail.month = ldtTempDate.Month;
                lobjIncomeVerification.icdoCaseDisabilityIncomeVerificationDetail.year = ldtTempDate.Year;
                iclbCaseDisabilityIncomeVerificationDetail.Add(lobjIncomeVerification);
                ldtTempDate = ldtTempDate.AddMonths(-1);
            }
            iclbCaseDisabilityIncomeVerificationDetail = busGlobalFunctions.Sort<busCaseDisabilityIncomeVerificationDetail>
                ("icdoCaseDisabilityIncomeVerificationDetail.idtTempDate asc", iclbCaseDisabilityIncomeVerificationDetail);
        }

        //set payee account id, plan for Case type Disability and Pre 1991.
        //payee account status should not be completed or cancelled
        public void SetPayeeAccountIDAndPlan()
        {
            DataTable ldtbDisabilityPayeeAccountCount = busBase.Select("cdoPayeeAccount.GetPayeeAccountForDisability",
                                                                        new object[1] { icdoCase.person_id });

            var ldtGetPayeeAccount = from o in ldtbDisabilityPayeeAccountCount.AsEnumerable()
                                     where ((o["payment_status_value"].ToString() != busConstant.PayeeAccountStatusDisabilityPaymentCompleted)
                                       && (o["payment_status_value"].ToString() != busConstant.PayeeAccountStatusDisabilityCancelled))
                                     select o;
            {
                if (ldtGetPayeeAccount.Count() > 0)
                {
                    icdoCase.payee_account_id = ldtGetPayeeAccount.AsDataTable().Rows[0].Field<int>("payee_account_id");
                    icdoCase.istrPlanName = ldtGetPayeeAccount.AsDataTable().Rows[0].Field<string>("plan_name");
                    icdoCase.iintPlanId = ldtGetPayeeAccount.AsDataTable().Rows[0].Field<int>("iintPlanId");
                }
            }
        }

        # region Button Click
        /// <summary>
        /// this method is to refresh the grid based on the date entered
        /// it will reload the data from file net based on the date entered
        /// and then again insert it into the table.
        /// </summary>
        /// <returns></returns>
        public ArrayList btnRefreshImageData_Click()
        {
            ArrayList larr = new ArrayList();

            if (icdoCase.filenet_date_from != DateTime.MinValue)
            {
                icdoCase.filenet_date_to = Convert.IsDBNull(icdoCase.filenet_date_to) ? DateTime.Now : icdoCase.filenet_date_to;
                {

                    LoadCaseFileDetails();

                    foreach (busCaseFileDetail lobjFileDetail in iclbCaseFileDetail)
                    {
                        lobjFileDetail.icdoCaseFileDetail.Delete();
                    }

                    LoadFileNetImages();
                    InsertOrUpdateFileDetails();
                    larr.Add(this);
                }
            }
            return larr;
        }

        //this button click will show all the file net details irrespective 
        //of persistent checkbox value.
        public ArrayList btnShowAll_Click()
        {
            ArrayList larr = new ArrayList();
            LoadFilenetImagesFromDB(true);
            larr.Add(this);
            return larr;
        }

        # endregion

        # region Override functions

        public override void BeforePersistChanges()
        {
            if (icdoCase.case_id == 0)
                icdoCase.Insert();
            else
                icdoCase.Update();

            //this must be uploaded only at the time of persist changess
            if (iclbCaseFileDetail == null)
                LoadFileNetImages();

            AssignCollectionByCaseTypeBeforePersist();
            InsertOrUpdateStepDetails();

            if (icdoCase.case_type_value == busConstant.CaseTypeDisabilityRecertification)
            {
                SetIsComparableEarningExceedFlag();
                InsertOrUpdateIncomeVerification();
            }
            if (icdoCase.case_type_value == busConstant.CaseTypeFinancialHardship)
            {
                InsertOrUpdateFinancialDetails();
            }
            if (icdoCase.isImageDataCollectionChanged)
            {
                InsertOrUpdateFileDetails();
            }

            if ((icdoCase.case_type_value == busConstant.CaseTypeDisabilityRecertification)
                || (icdoCase.case_type_value == busConstant.CaseTypePre1991DisabilityRecertification))
            {
                //set the payee account recertification date
                if (icdoCase.case_status_value == busConstant.CaseStatusValueApproved)
                {
                    if (ibusPayeeAccount == null)
                        LoadPayeeAccount();

                    ibusPayeeAccount.icdoPayeeAccount.recertification_date = icdoCase.next_recertification_date;
                    ibusPayeeAccount.icdoPayeeAccount.case_recertification_date = icdoCase.next_recertification_date;
                    ibusPayeeAccount.icdoPayeeAccount.is_disability_batch_letter_sent_flag = busConstant.Flag_No;

                    ibusPayeeAccount.icdoPayeeAccount.Update();
                }
            }
            base.BeforePersistChanges();
        }

        //insert into Financial deferred Comp Details
        private void InsertOrUpdateFinancialDetails()
        {
            foreach (busCaseFinancialHardshipProviderDetail lobjDefCompDetails in iclbCaseFinancialHardshipProviderDetail)
            {
                lobjDefCompDetails.icdoCaseFinancialHardshipProviderDetail.case_id = icdoCase.case_id;
                lobjDefCompDetails.icdoCaseFinancialHardshipProviderDetail.ienuObjectState = ObjectState.Insert;
                if (lobjDefCompDetails.icdoCaseFinancialHardshipProviderDetail.case_financial_hardship_provider_detail_id > 0)
                {
                    lobjDefCompDetails.icdoCaseFinancialHardshipProviderDetail.ienuObjectState = ObjectState.Insert;
                }
            }
        }

        //insert into Income verification - Disability Case type: Made as Public since used in batch also.
        public void InsertOrUpdateIncomeVerification()
        {
            foreach (busCaseDisabilityIncomeVerificationDetail lobjIncomeVerification in iclbCaseDisabilityIncomeVerificationDetail)
            {
                lobjIncomeVerification.icdoCaseDisabilityIncomeVerificationDetail.case_id = icdoCase.case_id;
                if (lobjIncomeVerification.icdoCaseDisabilityIncomeVerificationDetail.case_disability_income_verification_detail_id == 0)
                {
                    lobjIncomeVerification.icdoCaseDisabilityIncomeVerificationDetail.Insert();
                }
                else
                {
                    lobjIncomeVerification.icdoCaseDisabilityIncomeVerificationDetail.Update();
                }
            }
        }

        //insert into Step details table: Made as Public since used in batch also.
        public void InsertOrUpdateStepDetails()
        {
            SetOverAllTime(iclbCaseStepDetail);
            SetDefaultValuesForEachStepInUpdateMode(iclbCaseStepDetail);

            foreach (busCaseStepDetail lobjStepDetail in iclbCaseStepDetail)
            {
                lobjStepDetail.icdoCaseStepDetail.case_id = icdoCase.case_id;
                lobjStepDetail.icdoCaseStepDetail.ienuObjectState = ObjectState.Insert;
                if (lobjStepDetail.icdoCaseStepDetail.case_step_detail_id > 0)
                {
                    lobjStepDetail.icdoCaseStepDetail.ienuObjectState = ObjectState.Update;
                }
                iarrChangeLog.Add(lobjStepDetail.icdoCaseStepDetail);
            }

        }

        //insert into File details
        private void InsertOrUpdateFileDetails()
        {
            foreach (busCaseFileDetail lobjFileDetail in iclbCaseFileDetail)
            {
                lobjFileDetail.icdoCaseFileDetail.case_id = icdoCase.case_id;

                if (lobjFileDetail.icdoCaseFileDetail.case_file_detail_id != 0)
                    lobjFileDetail.icdoCaseFileDetail.Update();
                else
                    lobjFileDetail.icdoCaseFileDetail.Insert();
            }
        }

        //Load the filenet details related to the person 
        //where intitated date is between the filenet date from and file net date to
        public void LoadFileNetImages()
        {
            if (icdoCase.filenet_date_from != DateTime.MinValue)
            {
                DateTime ldtEndDate = (icdoCase.filenet_date_to == DateTime.MinValue) ? DateTime.Now : icdoCase.filenet_date_to;
                iclbCaseFileDetail = new Collection<busCaseFileDetail>();
                var lclbWfImageData = new Collection<busSolBpmProcessInstanceAttachments>();
                lclbWfImageData = busFileNetHelper.LoadFileNetImagesByPersonDate(icdoCase.person_id, icdoCase.filenet_date_from, ldtEndDate);

                foreach (busSolBpmProcessInstanceAttachments lobjWfImageData in lclbWfImageData)
                {
                    busCaseFileDetail lobjCaseFileDetail = new busCaseFileDetail { icdoCaseFileDetail = new cdoCaseFileDetail() };

                    lobjCaseFileDetail.icdoCaseFileDetail.case_id = icdoCase.case_id;
                    lobjCaseFileDetail.icdoCaseFileDetail.document_id = lobjWfImageData.document_id;

                    lobjCaseFileDetail.icdoCaseFileDetail.document_description = lobjWfImageData.subject_title;

                    lobjCaseFileDetail.icdoCaseFileDetail.document_title = lobjWfImageData.document_title;

                    lobjCaseFileDetail.icdoCaseFileDetail.version_series_id = lobjWfImageData.version_series_id;

                    lobjCaseFileDetail.icdoCaseFileDetail.document_code = lobjWfImageData.short_name;

                    lobjCaseFileDetail.icdoCaseFileDetail.filenet_received_date = lobjWfImageData.initiated_date;

                    lobjCaseFileDetail.icdoCaseFileDetail.is_file_visible_flag = busConstant.Flag_Yes;

                    lobjCaseFileDetail.icdoCaseFileDetail.istrObjectStore = NeoSpin.Common.ApplicationSettings.Instance.OBJECT_STORE;
                    iclbCaseFileDetail.Add(lobjCaseFileDetail);
                }
                icdoCase.isImageDataCollectionChanged = true;
            }
        }

        /// <summary>
        /// collection must be refreshed after each save
        /// </summary>
        /// <param name="ablnShowAll"></param>
        public void LoadFilenetImagesFromDB(Boolean ablnShowAll)
        {
            if (!ablnShowAll)
            {
                if (iclbCaseFileDetail != null)
                {
                    var lobjFileNetImages = from o in iclbCaseFileDetail
                                            where o.icdoCaseFileDetail.is_file_visible_flag == busConstant.Flag_Yes
                                            orderby o.icdoCaseFileDetail.filenet_received_date descending
                                            select o;
                    iclbCaseFileDetail = new Collection<busCaseFileDetail>();
                    foreach (busCaseFileDetail lobj in lobjFileNetImages)
                        iclbCaseFileDetail.Add(lobj);
                }
            }
            else
            {
                iclbCaseFileDetail = new Collection<busCaseFileDetail>();
                LoadCaseFileDetails();
            }
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();

            //reload Step Details
            LoadCaseStepDetails();
            LoadFilenetImagesFromDB(false);

            if (icdoCase.case_type_value == busConstant.CaseTypeDisabilityRecertification)
            {
                LoadCaseDisabilityIncomeVerificationDetails();
                SetIsComparableEarningExceedFlag();
            }

            if (icdoCase.case_type_value == busConstant.CaseTypeFinancialHardship)
                LoadCaseFinancialHardshipProviderDetails();
        }
        # endregion

        # region business logic

        //check if the recertification date is entered
        // check if the recertification date is first of month
        // for disability and Pre 1991
        public bool IsRecertificationDateIsFirstOfMonth
        {
            get
            {
                if (icdoCase.recertification_date != DateTime.MinValue)
                    return (icdoCase.recertification_date.Day == 1) ? true : false;
                return true;
            }
        }

        //check if the next recertification date is entered
        // check if the recertification date is first of month
        // for disability and Pre 1991
        public bool IsNextRecertificationDateIsFirstOfMonth
        {
            get
            {
                if (icdoCase.next_recertification_date != DateTime.MinValue)
                    return (icdoCase.next_recertification_date.Day == 1) ? true : false;
                return true;
            }
        }

        //check next recertification date is 18 months more than recertification
        public bool IsNextRecertificationDateIsvalid()
        {
            if ((icdoCase.next_recertification_date != DateTime.MinValue)
                && (icdoCase.recertification_date != DateTime.MinValue))
            {
                if (icdoCase.next_recertification_date > icdoCase.recertification_date.AddMonths(18))
                    return false;
            }
            return true;
        }

        //check for comparable earnings are withn the limit for plan
        public void SetIsComparableEarningExceedFlag()
        {
            decimal ldecComparableEarning = 0.00M;
            ldecComparableEarning = (icdoCase.iintPlanId == busConstant.PlanIdJobService) ? (icdoCase.comparable_earnings_amount * 0.80M)
                                                                                            : (icdoCase.comparable_earnings_amount * 0.70M);

            foreach (busCaseDisabilityIncomeVerificationDetail lobjIncomeVerification in iclbCaseDisabilityIncomeVerificationDetail)
            {
                lobjIncomeVerification.icdoCaseDisabilityIncomeVerificationDetail.idecTotal =
                    lobjIncomeVerification.icdoCaseDisabilityIncomeVerificationDetail.earnings_amount1 +
                    lobjIncomeVerification.icdoCaseDisabilityIncomeVerificationDetail.earnings_amount2 +
                    lobjIncomeVerification.icdoCaseDisabilityIncomeVerificationDetail.earnings_amount3;
                //PIR: 1694. The Flag is set only when total is greater than comparable earnings. Equal to Condition Removed.
                //When the total is lesser than or equal to then the flag is set as No.
                if (lobjIncomeVerification.icdoCaseDisabilityIncomeVerificationDetail.idecTotal > ldecComparableEarning)
                    lobjIncomeVerification.icdoCaseDisabilityIncomeVerificationDetail.is_exceeds_comparable_earnings_flag = busConstant.Flag_Yes;
                else
                    lobjIncomeVerification.icdoCaseDisabilityIncomeVerificationDetail.is_exceeds_comparable_earnings_flag = busConstant.Flag_No;
            }
        }

        //validation to check if the first step start is null        
        public bool IsStartDateNotEntered()
        {
            if (iclbCaseStepDetail == null)
                LoadCaseStepDetails();
            if (iclbCaseStepDetail.Count > 0)
            {
                if (iclbCaseStepDetail[0].icdoCaseStepDetail.start_date == DateTime.MinValue)
                {
                    return false;
                }
            }
            return true;
        }

        //and step start date is greater than step end date
        public bool IsStartDateEndDateValid()
        {
            if (iclbCaseStepDetail == null)
                LoadCaseStepDetails();

            if ((from o in iclbCaseStepDetail
                 where o.icdoCaseStepDetail.start_date > DateTime.MinValue
                 && o.icdoCaseStepDetail.end_date > DateTime.MinValue
                 && o.icdoCaseStepDetail.start_date > o.icdoCaseStepDetail.end_date
                 select o).Count() > 0)
                return false;

            return true;
        }
        # endregion

        #region Correspondence

        public override busBase GetCorPerson()
        {
            if (ibusPerson == null)
                LoadPerson();

            if (iclbEndDatedStepDetails == null)
                LoadEndDatedStepDetails();
            return ibusPerson;
        }

        public void CheckMemberEnrollment()
        {
            if (ibusPerson.IsMemberEnrolledInPlan(busConstant.PlanIdGroupHealth))
                istrIsMemberEnrolledInHealth = busConstant.Flag_Yes;
            if (ibusPerson.IsMemberEnrolledInPlan(busConstant.PlanIdDental))
                istrIsMemberEnrolledInDental = busConstant.Flag_Yes;
            if (ibusPerson.IsMemberEnrolledInPlan(busConstant.PlanIdVision))
                istrMemberEnrolledInVision = busConstant.Flag_Yes;
            if (ibusPerson.IsMemberEnrolledInPlan(busConstant.PlanIdGroupLife))
                istrMemberEnrolledInLife = busConstant.Flag_Yes;

            //Set The First and Last Income Verification Details.
            if (icdoCase.case_type_value == busConstant.CaseTypeDisabilityRecertification)
            {
                DateTime ldtTempStartDate = DateTime.MinValue;
                DateTime ldtTempEndDate = DateTime.MinValue;

                if (iclbCaseDisabilityIncomeVerificationDetail.IsNull())
                {
                    LoadCaseDisabilityIncomeVerificationDetails();
                }
                if (iclbCaseDisabilityIncomeVerificationDetail.IsNotNull() &&
                    (iclbCaseDisabilityIncomeVerificationDetail.Count() > 0))
                {
                    ldtTempStartDate = iclbCaseDisabilityIncomeVerificationDetail.FirstOrDefault().icdoCaseDisabilityIncomeVerificationDetail.idtTempDate;
                    ldtTempEndDate = iclbCaseDisabilityIncomeVerificationDetail.LastOrDefault().icdoCaseDisabilityIncomeVerificationDetail.idtTempDate;

                    istrIncomeVerificationStartDate = ldtTempStartDate.ToString("MMM") + " " + ldtTempStartDate.Year.ToString();
                    istrIncomeVerificationEndDate = ldtTempEndDate.ToString("MMM") + " " + ldtTempEndDate.Year.ToString();
                }
            }
            if (ibusPerson.icolPersonAccount != null)
            {
                foreach (busPersonAccount lobjPersonAccount in ibusPerson.icolPersonAccount)
                {
                    if (lobjPersonAccount.ibusPlan != null)
                    {
                        if (lobjPersonAccount.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeInsurance)
                        {
                            istrMemberEnrolledInInsurancePlan = busConstant.Flag_Yes;
                            break;
                        }
                    }
                }
            }
        }

        public string istrIsMemberEnrolledInHealth { get; set; }

        public string istrIsMemberEnrolledInDental { get; set; }

        public string istrMemberEnrolledInVision { get; set; }

        public string istrMemberEnrolledInLife { get; set; }

        public string istrMemberEnrolledInInsurancePlan { get; set; }

        public string istrIncomeVerificationStartDate { get; set; }

        public string istrIncomeVerificationEndDate { get; set; }

        public int iintNotificationIdentifier { get; set; }
        //Systest PIR: 2275
        public string istrNotificationIdentifier
        {
            get
            {
                if (icdoCase.first_notification_flag.IsNull())
                {
                    return Convert.ToString(1);
                }
                else if (icdoCase.first_notification_flag.IsNotNull() && icdoCase.second_notification_flag.IsNotNull() && icdoCase.third_notification_flag.IsNull())
                {
                    return Convert.ToString(2);
                }
                else if (icdoCase.first_notification_flag.IsNotNull() && icdoCase.second_notification_flag.IsNotNull() && icdoCase.third_notification_flag.IsNotNull())
                {
                    return Convert.ToString(3);
                }
                return Convert.ToString(0);
            }
        }
        //Systest PIR: 2275
        public string istrIsSecondorFinalNotification
        {
            get
            {
                if (icdoCase.first_notification_flag.IsNotNull())
                    return busConstant.Flag_Yes;
                return busConstant.Flag_No;
            }
        }
        public string istrIsDisability
        {
            get
            {
                if (icdoCase.case_type_value == busConstant.CaseTypeDisabilityRecertification)
                {
                    return "Y";
                }
                else if (icdoCase.case_type_value == busConstant.CaseTypePre1991DisabilityRecertification)
                {
                    return "N";
                }
                return string.Empty;
            }
        }

        public string istrdte60DaysEarlierToRecertificationDate
        {
            get
            {
                return idte60DaysEarlierToRecertificationDate.ToString(busConstant.DateFormatLongDate);
            }
        }

        public string istrRecertificationDate
        {
            get
            {
                return icdoCase.recertification_date.ToString(busConstant.DateFormatLongDate);
            }
        }
        // changed the Property to return date 45 days prior to Recertification date.
        public DateTime idte60DaysEarlierToRecertificationDate
        {
            get
            {
                if (icdoCase.recertification_date != DateTime.MinValue)
                    return icdoCase.recertification_date.AddDays(-45);
                return icdoCase.recertification_date;
            }
        }

        public decimal idecComparableEarningPercentage { get; set; }

        public decimal idecComparateEarningAmount { get; set; }

        public Collection<busPerson> iclbMedicalConsultantRecordsPurge { get; set; }

        public busRetirementDisabilityApplication ibusRetirementDisabilityApplication { get; set; }

        public Collection<busCaseStepDetail> iclbEndDatedStepDetails { get; set; }

        public void LoadEndDatedStepDetails()
        {
            if (iclbEndDatedStepDetails == null)
                iclbEndDatedStepDetails = new Collection<busCaseStepDetail>();
            if (iclbCaseStepDetail == null)
                LoadCaseStepDetails();
            foreach (busCaseStepDetail lobjStep in iclbCaseStepDetail)
            {
                if (lobjStep.icdoCaseStepDetail.end_date != DateTime.MinValue)
                    iclbEndDatedStepDetails.Add(lobjStep);
            }
        }

        public string istrCreatedBy
        {
            get
            {
                int lintUserId = busGlobalFunctions.GetUserSerialIdFromUserId(icdoCase.created_by);
                busUser lbususer = busGlobalFunctions.GetUserObjectFromUserSerialId(lintUserId);
                return lbususer.icdoUser.first_name + " " + lbususer.icdoUser.last_name;
            }
        }

        public string istrPetitionForReviewType
        {
            get
            {
                if (icdoCase.appeal_type_value == busConstant.PlanBenefitTypeInsurance || icdoCase.appeal_type_value == busConstant.PlanBenefitTypeFlex)
                    return "AppealProcess";
                else if (icdoCase.appeal_type_value == busConstant.BenefitAppealTypeRetirement || icdoCase.appeal_type_value == busConstant.BenefitAppealTypeQDRO || icdoCase.appeal_type_value == busConstant.BenefitOptionDisability || icdoCase.appeal_type_value == busConstant.ApplicationActionStatusDeferred)
                    return "FormalReviewProcedure";
                else
                    return "";
            }
        }
        #endregion
		//PIR 15831 - APP 7007 template to be associated with case management screen too
        public void LoadRetDisAppForCorres()
        {
            if (ibusRetirementDisabilityApplication.IsNull()) ibusRetirementDisabilityApplication = new busRetirementDisabilityApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            DataTable ldtbRetDisApplications = busBase.Select("cdoCase.GetRetDisAppsForCorres",
                                                                        new object[1] { icdoCase.person_id });
            if (ldtbRetDisApplications.Rows.Count > 0)
                ibusRetirementDisabilityApplication.icdoBenefitApplication.LoadData(ldtbRetDisApplications.Rows[0]);
        }
    }
}
