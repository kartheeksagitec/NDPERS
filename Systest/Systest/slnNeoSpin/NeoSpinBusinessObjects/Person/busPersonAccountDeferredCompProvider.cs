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
using System.Globalization;
using System.Linq;
using NeoSpin.DataObjects;
using Sagitec.CustomDataObjects;
#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPersonAccountDeferredCompProvider : busPersonAccountDeferredCompProviderGen
    {
        private bool IsAssetsWithProviderflagChanged = false;

        // PIR 9115 functionality enable/disable property
        public string istrIsPIR9115Enabled
        {
            get
            {
                return busGlobalFunctions.GetData1ByCodeValue(52, "9115", iobjPassInfo);
            }
        }

        //PIR 12690
        private Collection<busPersonAccountDeferredCompContribution> _iclbContributions;
        public Collection<busPersonAccountDeferredCompContribution> iclbContributions
        {
            get { return _iclbContributions; }
            set { _iclbContributions = value; }
        }
        public bool iblnIsProviderExistsforSameDay { get; set; }
        public DateTime idtStartDate { get; set; } // PIR 11045

        public bool iblnStartDateValidation { get; set; }
        public bool iblnIsObjectStateInsert { get; set; }
        //PIR 25920 New Plan DC 2025
        public string istrApplyEmployerMatchingContributionOnCompany { get; set; }
        public string istrUpdateExistingEmployerFlag { get; set; }
        private Collection<busPersonAccountDeferredCompProvider> _iclbPersonAccountDeferredCompProviderByProvider;
        public Collection<busPersonAccountDeferredCompProvider> iclbPersonAccountDeferredCompProviderByProvider
        {
            get
            {
                return _iclbPersonAccountDeferredCompProviderByProvider;
            }

            set
            {
                _iclbPersonAccountDeferredCompProviderByProvider = value;
            }
        }

        public busOrganization ibusProviderOrganization { get; set; }

        //PIR 16908
        public string istrIsUnUsedSickLeaveConversion { get; set; }
        Collection<busPersonAccountDeferredCompProvider> iclbPersonAccountDeferredCompProviderUpdatedRecord = new Collection<busPersonAccountDeferredCompProvider>();//pir 20702
        int iintNewProviderIdForChangeAmount = 0; //pir 20702
        public void LoadProviderOrganization()
        {
            ibusProviderOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
            if (icdoPersonAccountDeferredCompProvider.istrProviderOrgCode.IsNotNullOrEmpty())
            {
                ibusProviderOrganization.FindOrganizationByOrgCode(icdoPersonAccountDeferredCompProvider.istrProviderOrgCode);
            }
        }

        public Collection<cdoOrganization> LoadDeferredCompProviders(bool ablnIsUpdateMode = false)
        {
            ////PIR-10616 Start 
            bool IsExistingRestrictedProvider = false;
            Collection<busOrgPlan> lclbOrgplan = new Collection<busOrgPlan>();
            if (ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider == null || ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider.Count() == 0)
                ibusPersonAccountDeferredComp.LoadPersonAccountProviders();
            //foreach()
            Collection<cdoOrganization> lclbActiveProviders = new Collection<cdoOrganization>();
            var lvarActiveProviders = ibusPersonAccountDeferredComp.LoadActiveProviders(ablnIsUpdateMode).OrderBy(lobj => lobj.org_name);
            if (ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider.Count() == 0 || ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider == null)
            {
                foreach (cdoOrganization lcdoOrg in lvarActiveProviders)
                {
                    busOrganization lbusOrganization = new busOrganization();
                    lbusOrganization.FindOrganization(lcdoOrg.org_id);
                    lbusOrganization.LoadOrgPlan();
                    if (!(lbusOrganization.iclbOrgPlan.Where(o => o.icdoOrgPlan.plan_id == busConstant.PlanIdDeferredCompensation && o.icdoOrgPlan.restriction == busConstant.Flag_Yes
                        && (o.icdoOrgPlan.participation_end_date == DateTime.MinValue || o.icdoOrgPlan.participation_end_date > DateTime.Now)).Count() > 0)
                        && !lclbActiveProviders.Contains(lcdoOrg))
                        lclbActiveProviders.Add(lcdoOrg);
                }
            }
            else
            {
                foreach (busPersonAccountDeferredCompProvider lbusPersonAccountDeferredCompProvider in ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider)
                {
                    lbusPersonAccountDeferredCompProvider.LoadProviderOrgPlan();
                    lclbOrgplan.Add(lbusPersonAccountDeferredCompProvider.ibusProviderOrgPlan);
                }
                if ((lclbOrgplan.Where(o => o.icdoOrgPlan.restriction == busConstant.Flag_Yes).Count() > 0))
                {
                    IsExistingRestrictedProvider = true;
                }

                if (IsExistingRestrictedProvider)
                {
                    if (lvarActiveProviders != null && lvarActiveProviders.Count() > 0)
                    {
                        foreach (cdoOrganization lcdoOrg in lvarActiveProviders)
                        {
                            busOrganization lobjOrganization = new busOrganization();
                            lobjOrganization.FindOrganization(lcdoOrg.org_id);
                            lobjOrganization.LoadOrgPlan();
                            var lvarDefCompOrgPlan = lobjOrganization.iclbOrgPlan.Where(o => o.icdoOrgPlan.plan_id == busConstant.PlanIdDeferredCompensation &&
                                (o.icdoOrgPlan.participation_end_date == DateTime.MinValue || o.icdoOrgPlan.participation_end_date > DateTime.Now));
                            if (lvarDefCompOrgPlan != null && lvarDefCompOrgPlan.Count() > 0)
                            {
                                foreach (busOrgPlan lbusOrgPlan in lvarDefCompOrgPlan)
                                {
                                    if (lbusOrgPlan.icdoOrgPlan.restriction == busConstant.Flag_Yes)
                                    {
                                        //Only the restricted provider where the member already has a record should be in the drop down list. PIR-10232
                                        if (lclbOrgplan.Where(o => o.icdoOrgPlan.org_plan_id == lbusOrgPlan.icdoOrgPlan.org_plan_id).Count() > 0 && !lclbActiveProviders.Contains(lcdoOrg))
                                            lclbActiveProviders.Add(lcdoOrg);
                                    }
                                    else
                                    {
                                        if (!lclbActiveProviders.Contains(lcdoOrg))
                                            lclbActiveProviders.Add(lcdoOrg);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (cdoOrganization lcdoOrg in lvarActiveProviders)
                    {
                        busOrganization lbusOrganization = new busOrganization();
                        lbusOrganization.FindOrganization(lcdoOrg.org_id);
                        lbusOrganization.LoadOrgPlan();
                        if (!(lbusOrganization.iclbOrgPlan.Where(o => o.icdoOrgPlan.plan_id == busConstant.PlanIdDeferredCompensation && o.icdoOrgPlan.restriction == busConstant.Flag_Yes
                            && (o.icdoOrgPlan.participation_end_date == DateTime.MinValue || o.icdoOrgPlan.participation_end_date > DateTime.Now)).Count() > 0)
                            && !lclbActiveProviders.Contains(lcdoOrg))
                            lclbActiveProviders.Add(lcdoOrg);
                    }
                }

            }
            return lclbActiveProviders;
            //PIR-10616 End
        }

        public Collection<cdoOrganization> LoadDeferredCompProvidersNew()
        {
            return LoadDeferredCompProviders();
        }

        // PROD PIR 7436: In New Mode, load all active employment as of today's date. In Update mode, load as of the entered start date.
        public Collection<cdoOrganization> LoadDeferredCompProvidersUpdate()
        {
            return LoadDeferredCompProviders(true);
        }

        public Collection<cdoPersonEmployment> LoadDeferredCompEmployments(bool ablnIsUpdateMode)
        {
            Collection<cdoPersonEmployment> lclcDeferredCompEmployments = new Collection<cdoPersonEmployment>();
            if (ibusPersonAccountDeferredComp.iclbEmploymentDetail == null)
                ibusPersonAccountDeferredComp.LoadAllPersonEmploymentDetails();
            foreach (busPersonEmploymentDetail lobjEmploymentDetail in ibusPersonAccountDeferredComp.iclbEmploymentDetail)
            {
                // PIR: 1904. Show Employers only in Contributing status where end date is null or future. 
                // PROD PIR 4063 : Remove Contributing Condition here.
                if ((busGlobalFunctions.CheckDateOverlapping(DateTime.Now, lobjEmploymentDetail.icdoPersonEmploymentDetail.start_date,
                    lobjEmploymentDetail.icdoPersonEmploymentDetail.end_date_no_null) || lobjEmploymentDetail.icdoPersonEmploymentDetail.start_date > DateTime.Now)
                    || ablnIsUpdateMode) // Editable dropdown visilbe to specific users only.
                {
                    if (lobjEmploymentDetail.ibusPersonEmployment == null)
                        lobjEmploymentDetail.LoadPersonEmployment();
                    if (lobjEmploymentDetail.ibusPersonEmployment.ibusOrganization == null)
                        lobjEmploymentDetail.ibusPersonEmployment.LoadOrganization();

                    lobjEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.istrOrgCodeID =
                        lobjEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.org_code;

                    lobjEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.istrOrgName =
                       lobjEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.org_name + " (" +
                       lobjEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date.ToString("MM/dd/yyyy") + " - " +
                       (lobjEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date == DateTime.MinValue ?
                       "Present" : lobjEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date.ToString("MM/dd/yyyy")) + ")";

                    var lenuList =
                        lclcDeferredCompEmployments.Where(
                            i =>
                            i.person_employment_id == lobjEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.person_employment_id);
                    bool lblnEntryFound = false;
                    if ((lenuList != null) && (lenuList.Count() > 0))
                        lblnEntryFound = true;

                    if (!lblnEntryFound)
                        lclcDeferredCompEmployments.Add(lobjEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment);
                }
            }
            return lclcDeferredCompEmployments;
        }

        public Collection<cdoPersonEmployment> LoadDeferredCompEmploymentsNew()
        {
            Collection<cdoPersonEmployment> lclbPersonEmployment = LoadDeferredCompEmployments(false);
            if (iblnIsFromPortal)
            {
                if (lclbPersonEmployment.Count == 1)
                    icdoPersonAccountDeferredCompProvider.person_employment_id = lclbPersonEmployment[0].person_employment_id;
            }
            return lclbPersonEmployment;
        }

        // PROD PIR 7436: In New Mode, load all active employment as of today's date. In Update mode, load as of the entered start date.
        public Collection<cdoPersonEmployment> LoadDeferredCompEmploymentsUpdate()
        {
            return LoadDeferredCompEmployments(true);
        }

        //PIR 9692
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

        public void LoadProviderOrgPlanByProvider()
        {
            if (ibusProviderOrganization == null)
                LoadProviderOrganization();

            if (ibusPersonAccountDeferredComp == null)
                LoadPersonAccountDeferredComp();

            ibusProviderOrgPlan = new busOrgPlan { icdoOrgPlan = new cdoOrgPlan() };

            DataTable ldtbList = Select<cdoOrgPlan>(new string[2] { "org_id", "plan_id" },
                                                    new object[2]
                                                        {
                                                            ibusProviderOrganization.icdoOrganization.org_id,
                                                            ibusPersonAccountDeferredComp.icdoPersonAccount.plan_id
                                                        }, null, null);

            Collection<busOrgPlan> lclbOrgPlan = GetCollection<busOrgPlan>(ldtbList, "icdoOrgPlan");
            foreach (busOrgPlan lobjOrgPlan in lclbOrgPlan)
            {
                if (busGlobalFunctions.CheckDateOverlapping(ibusPersonAccountDeferredComp.icdoPersonAccount.history_change_date_no_null,
                    lobjOrgPlan.icdoOrgPlan.participation_start_date, lobjOrgPlan.icdoOrgPlan.participation_end_date))
                {
                    ibusProviderOrgPlan = lobjOrgPlan;
                    break;
                }
            }
        }

        public bool IsPayPeriodAmountChanged()
        {
            if (icdoPersonAccountDeferredCompProvider.ienuObjectState == ObjectState.Update)
            {
                DateTime ltdStartDate =
                    Convert.ToDateTime(icdoPersonAccountDeferredCompProvider.start_date.ToString("MM/yyyy", CultureInfo.CreateSpecificCulture("en-US")));
                DateTime ltdSysDate =
                    Convert.ToDateTime(DateTime.Now.ToString("MM/yyyy", CultureInfo.CreateSpecificCulture("en-US")));
                if (ltdStartDate < ltdSysDate)
                {
                    return true;
                }
            }
            return false;
        }

        //Check the Amount per pay period is >= $25.00 for DeffComp Plan with Monthly frequency 
        //and >= $12.00 for SemiMonthly and Biweekly frequency
        public bool IsAmountPerPayPeriodNotValid()
        {
            bool lblnIsAmountPerPayPeriodNotValid = false;
            if ((ibusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencyBiWeekly) ||
                          (ibusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencySemiMonthly))
            {
                if (icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt < 12.50M)
                {
                    lblnIsAmountPerPayPeriodNotValid = true;
                }
            }
            else if ((ibusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencyMonthly) &&
                                                (icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt < 25.00M))
            {
                lblnIsAmountPerPayPeriodNotValid = true;
            }
            return lblnIsAmountPerPayPeriodNotValid;
        }
        public bool IsPayPeriodDeductionAmountExceedAnnualLimit()
        {
            decimal ldecAnnualLimit;
            string lstr457Limit;
            decimal ldecTotalContribution = 0.00M;
            decimal ldec457ActualLimit = 0.00M;
            decimal ldecActualAnnualLimit = 0.00M;
            bool lblnIsPayPeriodDeductionAmountExceedAnnualLimit = false;

            ldecTotalContribution = CalculateContributionLimit();
            //PIR 8915
            //lstr457Limit = busGlobalFunctions.GetData1ByCodeValue(334, ibusPersonAccountDeferredComp.icdoPersonAccountDeferredComp.limit_457_value, iobjPassInfo);
            decimal lint457Limit = ibusPersonAccountDeferredComp.GetIRSLimitAmount(ibusPersonAccountDeferredComp.icdoPersonAccountDeferredComp.limit_457_value, DateTime.Now);
            // if (lstr457Limit.IsNotNullOrEmpty())
            ldec457ActualLimit = lint457Limit - ldecTotalContribution;
            if (lint457Limit != 0)
            {
                if (icdoPersonAccountDeferredCompProvider.end_date == DateTime.MinValue)
                {
                    if ((ibusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencyBiWeekly) ||
                                  (ibusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencySemiMonthly))
                    {
                        ldecAnnualLimit = icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt * 2;
                    }
                    else if (ibusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencyMonthly)
                    {
                        ldecAnnualLimit = icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt;
                    }
                    //For Weekly
                    else
                    {
                        ldecAnnualLimit = icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt * 4;
                    }

                    if (icdoPersonAccountDeferredCompProvider.start_date.Year == DateTime.Now.Year)
                    {
                        ldecActualAnnualLimit = ldecTotalContribution + ldecAnnualLimit * (12 - icdoPersonAccountDeferredCompProvider.start_date.Month);
                        if ((icdoPersonAccountDeferredCompProvider.start_date > DateTime.Now) && (icdoPersonAccountDeferredCompProvider.start_date.Day == 1))
                            ldecActualAnnualLimit = ldecActualAnnualLimit + ldecAnnualLimit;
                    }
                    else
                        ldecActualAnnualLimit = ldecTotalContribution + ldecAnnualLimit * (12 - DateTime.Now.Month);

                    if (icdoPersonAccountDeferredCompProvider.end_date == DateTime.MinValue && ldecActualAnnualLimit > lint457Limit)
                    {
                        lblnIsPayPeriodDeductionAmountExceedAnnualLimit = true;
                    }

                    if (ldecAnnualLimit <= ldec457ActualLimit && icdoPersonAccountDeferredCompProvider.end_date == DateTime.MinValue &&
                        ldecActualAnnualLimit < lint457Limit)
                    {
                        lblnIsPayPeriodDeductionAmountExceedAnnualLimit = false;
                    }

                    if (ldecAnnualLimit > ldec457ActualLimit)
                    {
                        lblnIsPayPeriodDeductionAmountExceedAnnualLimit = true;
                    }
                }
                else
                {
                    ldecAnnualLimit = icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt;
                    ldecActualAnnualLimit = ldecTotalContribution + ldecAnnualLimit;
                    if (ldecActualAnnualLimit > lint457Limit)
                    {
                        lblnIsPayPeriodDeductionAmountExceedAnnualLimit = true;
                    }

                    if (ldecAnnualLimit <= ldec457ActualLimit && ldecActualAnnualLimit < lint457Limit)
                    {
                        lblnIsPayPeriodDeductionAmountExceedAnnualLimit = false;
                    }

                    if (ldecAnnualLimit > ldec457ActualLimit)
                    {
                        lblnIsPayPeriodDeductionAmountExceedAnnualLimit = true;
                    }

                }
            }
            return lblnIsPayPeriodDeductionAmountExceedAnnualLimit;
        }

        public bool IsMemberContributingWithin6MonthsOfHardshipWithdrawal()
        {
            if (ibusPersonAccountDeferredComp != null)
            {
                DateTime ldtHardshipWithdrawalDate = ibusPersonAccountDeferredComp.icdoPersonAccountDeferredComp.hardship_withdrawal_effective_date.AddMonths(6);
                if ((icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt != 0.00M) &&
                       (icdoPersonAccountDeferredCompProvider.start_date < ldtHardshipWithdrawalDate))
                {
                    if (ibusPersonAccountDeferredComp.icdoPersonAccountDeferredComp.hardship_withdrawal_flag == busConstant.Flag_Yes)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsUnEndDatedRecordExistsForSameProvider()
        {
            if (iclbPersonAccountDeferredCompProviderByProvider == null)
            {
                LoadAllDeferredCompProviderByProvider();
            }
            if (iclbPersonAccountDeferredCompProviderByProvider.Count > 0)
            {
                if (iclbPersonAccountDeferredCompProviderByProvider[0].icdoPersonAccountDeferredCompProvider.person_account_provider_id
                                                                    != icdoPersonAccountDeferredCompProvider.person_account_provider_id &&
                    iclbPersonAccountDeferredCompProviderByProvider[0].icdoPersonAccountDeferredCompProvider.person_employment_id
                    == icdoPersonAccountDeferredCompProvider.person_employment_id) //pir 7898
                {
                    if (iclbPersonAccountDeferredCompProviderByProvider[0].icdoPersonAccountDeferredCompProvider.end_date == DateTime.MinValue)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void LoadAllDeferredCompProviderByProvider()
        {
            DataTable ldtbList = Select<cdoPersonAccountDeferredCompProvider>(
                new string[2] { "provider_org_plan_id", "person_account_id" },
                new object[2] { icdoPersonAccountDeferredCompProvider.provider_org_plan_id, icdoPersonAccountDeferredCompProvider.person_account_id }, null, "start_date desc");
            iclbPersonAccountDeferredCompProviderByProvider = GetCollection<busPersonAccountDeferredCompProvider>(ldtbList, "icdoPersonAccountDeferredCompProvider");
        }
        //PIR 25920 New Plan DC 2025
        /// <summary>
        /// only one provider is eligible for apply employer matching contribution check/flag 
        /// </summary>
        /// <returns>return true if check flag is available on provider</returns>
        public bool IsApplyEmployerMatchingContributionAvailbale()
        {
            if (ibusPersonAccountDeferredComp.IsEmployerMatchAvailable)
            {
                if (iclbDefCompProviders == null) LoadDeferredCompProviderByProvider();
                if (iclbDefCompProviders.Any(lobjProvider => lobjProvider.icdoPersonAccountDeferredCompProvider.is_apply_employer_matching_contribution == busConstant.Flag_Yes &&
                                 icdoPersonAccountDeferredCompProvider.person_account_provider_id != lobjProvider.icdoPersonAccountDeferredCompProvider.person_account_provider_id &&
                                 busGlobalFunctions.CheckDateOverlapping(icdoPersonAccountDeferredCompProvider.start_date,
                                 lobjProvider.icdoPersonAccountDeferredCompProvider.start_date, lobjProvider.icdoPersonAccountDeferredCompProvider.end_date_no_null)))
                    istrApplyEmployerMatchingContributionOnCompany = iclbDefCompProviders.FirstOrDefault(lobjProvider => lobjProvider.icdoPersonAccountDeferredCompProvider.is_apply_employer_matching_contribution == busConstant.Flag_Yes &&
                                    icdoPersonAccountDeferredCompProvider.person_account_provider_id != lobjProvider.icdoPersonAccountDeferredCompProvider.person_account_provider_id &&
                                    busGlobalFunctions.CheckDateOverlapping(icdoPersonAccountDeferredCompProvider.start_date,
                                    lobjProvider.icdoPersonAccountDeferredCompProvider.start_date, lobjProvider.icdoPersonAccountDeferredCompProvider.end_date_no_null)).icdoPersonAccountDeferredCompProvider.company_name;
            }
            return istrApplyEmployerMatchingContributionOnCompany.IsNotNullOrEmpty() ? true : false;
        }

        //PIR 25920 DC2025 changes
        public bool IsEmployerMatchAvailable
        {
            get
            {
                if(ibusPersonAccountDeferredComp.icdoPersonAccount.plan_id == busConstant.PlanIdDeferredCompensation)
                { 
                    if (ibusPersonAccountDeferredComp.ibusPersonAccount.IsNull())
                    {
                        ibusPersonAccountDeferredComp.LoadPersonAccount();
                        ibusPersonAccountDeferredComp.icdoPersonAccount = ibusPersonAccountDeferredComp.ibusPersonAccount.icdoPersonAccount;
                    }
                    DateTime ldtStartDate = iblnIsFromPortal ? icdoPersonAccountDeferredCompProvider.start_date : icdoPersonAccountDeferredCompProvider.new_start_date;
                    if (ldtStartDate == DateTime.MinValue) { SetStartDate(); ldtStartDate = idtStartDate; }
                    return ibusPersonAccountDeferredComp.ibusPersonAccount.IsEmployerMatchAvailableWithElection(ldtStartDate);
                    //return ibusPersonAccountDeferredComp.IsEmployerMatchAvailable; // IsEmploymentLinkedWithDC25Plan && GetPersonRetirementOptionalValue() < GetMaxERPreTaxMatch();
                }
                return false;
            }
        }
        public bool IsApplyMatchWithZeroAmount
        {
            get
            {
                if (iclbDefCompProviders == null) LoadDeferredCompProviderByProvider();
                return IsEmployerMatchAvailable && icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt == 0
                    && icdoPersonAccountDeferredCompProvider.is_apply_employer_matching_contribution == busConstant.Flag_Yes
                    && (iclbDefCompProviders.Any(lobjDefCompOtherProvider => lobjDefCompOtherProvider.icdoPersonAccountDeferredCompProvider.end_date == DateTime.MinValue
                        && lobjDefCompOtherProvider.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt > 0));
            }
        }
        
        public bool IsRecordOverlapsForSameProvider()
        {
            if (iclbPersonAccountDeferredCompProviderByProvider == null)
            {
                LoadAllDeferredCompProviderByProvider();
            }

            foreach (busPersonAccountDeferredCompProvider lobjProvider in iclbPersonAccountDeferredCompProviderByProvider)
            {
                if (lobjProvider.icdoPersonAccountDeferredCompProvider.person_account_provider_id !=
                    icdoPersonAccountDeferredCompProvider.person_account_provider_id &&
                    lobjProvider.icdoPersonAccountDeferredCompProvider.person_employment_id == icdoPersonAccountDeferredCompProvider.person_employment_id) //pir 7898
                {
                    if (icdoPersonAccountDeferredCompProvider.start_date != DateTime.MinValue)
                    {
                        if (icdoPersonAccountDeferredCompProvider.start_date != lobjProvider.icdoPersonAccountDeferredCompProvider.end_date && // PIR 10253
                            busGlobalFunctions.CheckDateOverlapping(icdoPersonAccountDeferredCompProvider.start_date,
                            lobjProvider.icdoPersonAccountDeferredCompProvider.start_date, lobjProvider.icdoPersonAccountDeferredCompProvider.end_date))
                        {
                            return true;
                        }
                    }
                    if (icdoPersonAccountDeferredCompProvider.end_date != DateTime.MinValue)
                    {
                        if (busGlobalFunctions.CheckDateOverlapping(icdoPersonAccountDeferredCompProvider.end_date,
                            lobjProvider.icdoPersonAccountDeferredCompProvider.start_date, lobjProvider.icdoPersonAccountDeferredCompProvider.end_date))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool IsProviderAssociatedWithEmployer()
        {
            int lintCount =
                      Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPersonAccountDeferredCompProvider.IsProviderAssociatedToEmployer",
                                                       new object[3]
                                                       {
                                                          ibusPersonEmployment.icdoPersonEmployment.org_id,
                                                          ibusPersonAccountDeferredComp.icdoPersonAccount.plan_id,
                                                          icdoPersonAccountDeferredCompProvider.istrProviderOrgCode
                                                       }, iobjPassInfo.iconFramework,
                                                          iobjPassInfo.itrnFramework));
            if (lintCount == 0)
            {
                return true;
            }
            return false;
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            //prod pir 7176,7177,7178
            if (!iblnFromESS && !iblnIsFromPortal)
            {
                if (icdoPersonAccountDeferredCompProvider.person_account_provider_id == 0)
                {
                    icdoPersonAccountDeferredCompProvider.person_employment_id = icdoPersonAccountDeferredCompProvider.new_person_employment_id;
                    icdoPersonAccountDeferredCompProvider.istrProviderOrgCode = icdoPersonAccountDeferredCompProvider.new_istrProviderOrgCode;
                    icdoPersonAccountDeferredCompProvider.provider_agent_contact_id = icdoPersonAccountDeferredCompProvider.new_provider_agent_contact_id;
                    icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt = icdoPersonAccountDeferredCompProvider.new_per_pay_period_contribution_amt;
                    icdoPersonAccountDeferredCompProvider.mutual_fund_window_flag = icdoPersonAccountDeferredCompProvider.new_mutual_fund_window_flag;
                    icdoPersonAccountDeferredCompProvider.start_date = icdoPersonAccountDeferredCompProvider.new_start_date;
                    icdoPersonAccountDeferredCompProvider.end_date = icdoPersonAccountDeferredCompProvider.new_end_date; //PIR-8597
                } //Start PIR-8597               
                else
                {
                    DateTime ldtEnd_date_Old = Convert.ToDateTime(icdoPersonAccountDeferredCompProvider.ihstOldValues[enmPersonAccountDeferredCompProvider.end_date.ToString()]);
                    if (ldtEnd_date_Old == DateTime.MinValue && icdoPersonAccountDeferredCompProvider.end_date == DateTime.MinValue)
                        icdoPersonAccountDeferredCompProvider.end_date = icdoPersonAccountDeferredCompProvider.new_end_date;
                    else
                        icdoPersonAccountDeferredCompProvider.new_end_date = icdoPersonAccountDeferredCompProvider.end_date;
                }  //End PIR-8597  
            }
            else
            {
                //Commented for PIR 11512
                //if (iblnIsFromPortal)
                //{
                //    if (Convert.ToInt32(istrStartYear) > 0 && iintStartMonth > 0)
                //        icdoPersonAccountDeferredCompProvider.start_date = new DateTime((Convert.ToInt32(istrStartYear) > 0) ?
                //                                                            Convert.ToInt32(istrStartYear) : 1, iintStartMonth > 0 ? iintStartMonth : 1, 01);
                //}
                //PIR 11512
                if (iblnIsFromPortal)
                {
                    //PIR 12834 - added rule for start date validation
                    iblnStartDateValidation = false;
                    if (icdoPersonAccountDeferredCompProvider.start_date < idtStartDate)
                    {
                        iblnStartDateValidation = true;
                        icdoPersonAccountDeferredCompProvider.start_date = idtStartDate;
                    }
                }

                if (iintMSSContactId > 0 && icdoPersonAccountDeferredCompProvider.provider_agent_contact_id <= 0)
                {
                    icdoPersonAccountDeferredCompProvider.provider_agent_contact_id = GetProviderOrgContactId();
                    LoadProviderAgentOrgContact();
                    ibusProviderAgentOrgContact.LoadContact();
                }
                if (string.IsNullOrEmpty(icdoPersonAccountDeferredCompProvider.mutual_fund_window_flag))
                    icdoPersonAccountDeferredCompProvider.mutual_fund_window_flag = busConstant.Flag_No;
            }

            //Reload Employment based on the selection
            LoadPersonEmployment();
            //Reload the Provider Organization
            LoadProviderOrganization();

            //Loading Org Plan
            ibusPersonAccountDeferredComp.LoadOrgPlan(
                icdoPersonAccountDeferredCompProvider.start_date,
                ibusPersonEmployment.icdoPersonEmployment.org_id); //PROD PIR ID 4737

            //Reload Load Provider Org Plan
            LoadProviderOrgPlanByProvider();

            //PIR 12690
            ibusPersonAccountDeferredComp.Set457Limit();

            icdoPersonAccountDeferredCompProvider.provider_org_plan_id = ibusProviderOrgPlan.icdoOrgPlan.org_plan_id;
            //PIR 25808 Added validation when member exceed one change per provider per day 
            if (iblnIsFromPortal)
            {
                DataTable ldtbList = Select<cdoPersonAccountDeferredCompProvider>(
                                                 new string[2] { "PERSON_ACCOUNT_ID","PROVIDER_ORG_PLAN_ID" },
                                                 new object[2] { icdoPersonAccountDeferredCompProvider.person_account_id,icdoPersonAccountDeferredCompProvider.provider_org_plan_id }, null, "CREATED_DATE DESC");

                busPersonAccountDeferredCompProvider lbusPersonAccountDeferredCompProvider = new busPersonAccountDeferredCompProvider { icdoPersonAccountDeferredCompProvider = new cdoPersonAccountDeferredCompProvider() };
                if (ldtbList.IsNotNull() && ldtbList.Rows.Count > 0)
                {
                    lbusPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.LoadData(ldtbList.Rows[0]);
                    if (lbusPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.created_date.Date == DateTime.Now.Date &&
                       lbusPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.provider_org_plan_id == icdoPersonAccountDeferredCompProvider.provider_org_plan_id)
                    {
                        iblnIsProviderExistsforSameDay = true;
                    }
                }
            }

        }

        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            base.ValidateHardErrors(aenmPageMode);
            if (iblnIsFromPortal)
            {

                foreach (utlError lobjError in iarrErrors)
                    lobjError.istrErrorID = string.Empty;
            }
            //PIR-2373
            if (iblnIsFromPortal && iarrErrors.Count == 0 && !IsNoChangesMade() && !iblnChangeFlag)
                iblnChangeFlag = true;
        }

        public bool iblnIsNewMode { get; set; }
        public bool iblnIsNewModeFromAllPortal = false;//PIR 20702
        public bool iblnFieldValuesChangedInUpdateMode = false;//PIR 20702
        public DateTime idtOldEndDate { get; set; } // PIR 20702

        public override int PersistChanges()
        {
            if (iblnIsFromPortal && iblnIsUpdate)
            {
                return 1;
            }
            else
                return base.PersistChanges();
        }

        public override void BeforePersistChanges()
        {
            //pir 20702: new column added. Default Value to be Null
            if (icdoPersonAccountDeferredCompProvider.lumpsum == busConstant.Flag_No)
                icdoPersonAccountDeferredCompProvider.lumpsum = null;

            idtOldEndDate = Convert.ToDateTime(icdoPersonAccountDeferredCompProvider.ihstOldValues["end_date"]); //PIR 20702

            if (iclbDefCompAcknowledgement != null)
            {
                if (iclbDefCompAcknowledgement.Count > 0)
                {
                    for (int i = iarrChangeLog.Count - 1; i >= 0; i--)//PIR 6961
                    {
                        if (iarrChangeLog[i] is cdoWssAcknowledgement)
                        {
                            cdoWssAcknowledgement lcdoWssAcknowledgement = (cdoWssAcknowledgement)iarrChangeLog[i];
                            iarrChangeLog.Remove(lcdoWssAcknowledgement);
                        }
                    }
                }
            }
            //PIR 25920 New Plan DC 2025 // set default value to Y if no other provider apply to  Y 
            if (ibusPersonAccountDeferredComp.icdoPersonAccount.plan_id != busConstant.PlanIdOther457 && IsEmployerMatchAvailable && icdoPersonAccountDeferredCompProvider.new_end_date == DateTime.MinValue)
            {
                if (ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider == null || ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider?.Count() == 0)
                    ibusPersonAccountDeferredComp.LoadPersonAccountProviders();
                if (!ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider.Any(lobjPersonAccountDeferredCompProvider => lobjPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.is_apply_employer_matching_contribution == busConstant.Flag_Yes &&
                        lobjPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.end_date == DateTime.MinValue))
                    icdoPersonAccountDeferredCompProvider.is_apply_employer_matching_contribution = busConstant.Flag_Yes;
                if(istrUpdateExistingProviderFlag == busConstant.Flag_Yes || istrUpdateExistingEmployerFlag == busConstant.Flag_Yes)
                    icdoPersonAccountDeferredCompProvider.is_apply_employer_matching_contribution = busConstant.Flag_Yes;
            }

            // To insert person id in Audit log
            if (ibusPersonAccountDeferredComp.IsNotNull())
                icdoPersonAccountDeferredCompProvider.person_id = ibusPersonAccountDeferredComp.icdoPersonAccount.person_id;
            if (ibusPersonEmployment.IsNotNull())
                icdoPersonAccountDeferredCompProvider.org_id = ibusPersonEmployment.icdoPersonEmployment.org_id;

            icdoPersonAccountDeferredCompProvider.company_name = ibusProviderOrganization.icdoOrganization.org_name;

            if (icdoPersonAccountDeferredCompProvider.ihstOldValues.Count > 0)
            {
                //PIR 1419 : Null Handling.
                string lstrOldAssetValue = null;
                if (icdoPersonAccountDeferredCompProvider.ihstOldValues["assets_with_provider_value"] != null)
                    lstrOldAssetValue = icdoPersonAccountDeferredCompProvider.ihstOldValues["assets_with_provider_value"].ToString();
                if (lstrOldAssetValue != icdoPersonAccountDeferredCompProvider.assets_with_provider_value)
                {
                    IsAssetsWithProviderflagChanged = true;
                }
            }
            else
            {
                IsAssetsWithProviderflagChanged = true;
            }

            if (!iblnIsFromPortal) // PIR 10394 - below code should run only when value changed from internal application
            {
                //If We add new providers, and all other provider payment status are already closed, we need to set the  plan participation status to
                //Suspended.
                if ((IsAllProviderPaymentStatusClosed()) &&
                    (icdoPersonAccountDeferredCompProvider.payment_status_value != busConstant.DeferredPaymentStatusClosed))
                {
                    if ((ibusPersonAccountDeferredComp != null) && (ibusPersonAccountDeferredComp.icdoPersonAccount != null))
                    {
                        ibusPersonAccountDeferredComp.icdoPersonAccount.history_change_date = DateTime.Now;
                        ibusPersonAccountDeferredComp.IsHistoryEntryRequired = true;
                        ibusPersonAccountDeferredComp.icdoPersonAccount.status_value = busConstant.StatusValid;
                        ibusPersonAccountDeferredComp.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusDefCompSuspended;
                        ibusPersonAccountDeferredComp.icdoPersonAccount.Update();

                        ibusPersonAccountDeferredComp.ProcessHistory();
                    }
                }
            }

            // PIR ID 1603
            if (icdoPersonAccountDeferredCompProvider.ienuObjectState == ObjectState.Insert)
            {
                if (icdoPersonAccountDeferredCompProvider.istrProviderOrgCode == busGlobalFunctions.GetData1ByCodeValue(5012, busConstant.Provider_Fidelity, iobjPassInfo))
                {
                    if ((ibusPersonAccountDeferredComp.IsNotNull()) &&
                        (ibusPersonAccountDeferredComp.icdoPersonAccountDeferredComp.file_457_sent_flag != busConstant.Flag_No))
                    {
                        ibusPersonAccountDeferredComp.icdoPersonAccountDeferredComp.file_457_sent_flag = busConstant.Flag_No;
                        ibusPersonAccountDeferredComp.icdoPersonAccountDeferredComp.Update();
                    }
                }

                //UAT PIR 2373 people soft file changes
                //--Start--//
                UpdatePersonAccountPSEventFlag();
                //--End--//
                iblnIsNewMode = true;
                iblnIsNewModeFromAllPortal = true; ////PIR 20702


                // PROD PIR ID 5611
                if (icdoPersonAccountDeferredCompProvider.istrProviderOrgCode == busConstant.NDPERSCompanionPlanOrgCode)
                {
                    if (iclbPersonAccountDeferredCompProviderByProvider.IsNull())
                        LoadAllDeferredCompProviderByProvider();
                    if (iclbPersonAccountDeferredCompProviderByProvider.Count > 0)
                    {
                        busPersonAccountDeferredCompProvider lobPrevProvider = new busPersonAccountDeferredCompProvider();
                        lobPrevProvider = iclbPersonAccountDeferredCompProviderByProvider[0];
                        if (lobPrevProvider.ibusProviderOrgPlan.IsNull())
                            lobPrevProvider.LoadProviderOrgPlan();
                        if (lobPrevProvider.ibusProviderOrgPlan.ibusOrganization.IsNull())
                            lobPrevProvider.ibusProviderOrgPlan.LoadOrganization();
                        if (lobPrevProvider.ibusProviderOrgPlan.ibusOrganization.icdoOrganization.org_code == busConstant.NDPERSCompanionPlanOrgCode &&
                            lobPrevProvider.icdoPersonAccountDeferredCompProvider.mutual_fund_window_flag == busConstant.Flag_Yes &&
                            istrOverrideMutualWindowFlag != busConstant.Flag_Yes)
                            icdoPersonAccountDeferredCompProvider.mutual_fund_window_flag = lobPrevProvider.icdoPersonAccountDeferredCompProvider.mutual_fund_window_flag;
                    }
                }
            }
            else if (icdoPersonAccountDeferredCompProvider.ienuObjectState == ObjectState.Update)
            {
                if (IsFieldValuesChangedInUpdateMode())//PIR 20702
                {
                    iblnFieldValuesChangedInUpdateMode = true;
                }
                if (!iblnIsFromPortal)
                {
                    //icdoPersonAccountDeferredCompProvider.is_enrollment_report_generated = busConstant.Flag_No;//PIR 17081
                    icdoPersonAccountDeferredCompProvider.Update();
                }
                if (icdoPersonAccountDeferredCompProvider.end_date != DateTime.MinValue)
                {
                    //UAT PIR 2373 people soft file changes
                    //--Start--// 
                    UpdatePersonAccountPSEventFlag();
                    //--End--//
                    iblnIsNewMode = false;//PIR 20702 : when we create new reord and save it and remain on the same screen without going back. 
                                          //and change any field, iblnIsNewMode field is still remain true.
                                          //Because of this issue created new field iblnIsNewModeFromAllPortal
                }
                iblnIsNewModeFromAllPortal = false;//PIR 20702
            }
            

            if (icdoPersonAccountDeferredCompProvider.person_account_provider_id == 0 && icdoPersonAccountDeferredCompProvider.start_date != DateTime.MinValue
                && icdoPersonAccountDeferredCompProvider.end_date != DateTime.MinValue)
                iblnIsObjectStateInsert = true;

        }

        private void UpdatePersonAccountPSEventFlag()
        {
            if ((ibusPersonAccountDeferredComp != null) && (ibusPersonAccountDeferredComp.icdoPersonAccount != null) && ibusPersonAccountDeferredComp.iblnCheckForEnrollmentChange)
            {
                ibusPersonAccountDeferredComp.icdoPersonAccount.ps_file_change_event_value = busConstant.LevelOfCoverageChange;
                //ibusPersonAccountDeferredComp.icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                ibusPersonAccountDeferredComp.icdoPersonAccount.Update();
            }
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            LoadProviderAgentOrgContact();
            ibusProviderAgentOrgContact.LoadContact();
            LoadPersonEmployment();
            ibusPersonEmployment.LoadOrganization();

            if (IsAssetsWithProviderflagChanged)
            {
                UpdateAllRecordsIfProviderAssetFlagChange();
            }

            //UCS 93 - If all the payment status is closed, set the plan participation to withdrawn
            //Reload All Deferrred Comp Providers
            ibusPersonAccountDeferredComp.LoadPersonAccountProviders();
            if (IsAllProviderPaymentStatusClosed())
            {
                if ((ibusPersonAccountDeferredComp != null) &&
                    (ibusPersonAccountDeferredComp.icdoPersonAccount != null))
                {
                    ibusPersonAccountDeferredComp.icdoPersonAccount.history_change_date = DateTime.Now;
                    ibusPersonAccountDeferredComp.IsHistoryEntryRequired = true;
                    ibusPersonAccountDeferredComp.icdoPersonAccount.status_value = busConstant.StatusValid;
                    ibusPersonAccountDeferredComp.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusDeffCompWithDrawn;
                    ibusPersonAccountDeferredComp.icdoPersonAccount.Update();

                    ibusPersonAccountDeferredComp.ProcessHistory();
                }
            }
            if (!iblnIsFromPortal)
            {
                // UAT PIR 2220
                if (ibusPersonAccountDeferredComp == null)
                    LoadPersonAccountDeferredComp();
                PostESSMessage();

                // SYSTEST PIR ID 1543
                if (icdoPersonAccountDeferredCompProvider.ihstOldValues.Count > 0)
                {
                    if (iclbPersonAccountDeferredCompProviderByProvider == null)
                        LoadAllDeferredCompProviderByProvider();

                    // If Payee Account Status Changed
                    foreach (busPersonAccountDeferredCompProvider lobjProvider in iclbPersonAccountDeferredCompProviderByProvider)
                    {
                        if (lobjProvider.icdoPersonAccountDeferredCompProvider.person_account_provider_id != icdoPersonAccountDeferredCompProvider.person_account_provider_id)
                        {
                            if (lobjProvider.icdoPersonAccountDeferredCompProvider.provider_org_plan_id == icdoPersonAccountDeferredCompProvider.provider_org_plan_id)
                            {
                                // Modify the same payee Status to all records
                                lobjProvider.icdoPersonAccountDeferredCompProvider.payment_status_value = icdoPersonAccountDeferredCompProvider.payment_status_value;
                                lobjProvider.icdoPersonAccountDeferredCompProvider.Update();
                            }
                        }
                    }
                }
            }

            if (ibusPersonAccountDeferredComp.IsNull())
                LoadPersonAccountDeferredComp();

            // UAT PIR 2382 - Addition reason
            // PROD PIR 7885 - Commented reason
            //UpdatePlanStatusToSuspend();


            if (iblnIsFromPortal == true)
            {
                if (ibusPersonAccountDeferredComp.IsNull())
                    LoadPersonAccountDeferredComp();
                if (ibusPersonAccountDeferredComp.ibusPlan.IsNull())
                    ibusPersonAccountDeferredComp.LoadPlan();

                //LoadMSSAllProviderDetials();
                //pir 6272
                PostESSMessage();
                // busWSSHelper.PublishMSSMessage(0, 0, string.Format("Your enrollment/change for the {0} has been processed.", ibusPersonAccountDeferredComp.ibusPlan.icdoPlan.plan_name),
                //                                                 busConstant.WSS_MessageBoard_Priority_High, ibusPersonAccountDeferredComp.icdoPersonAccount.person_id);

            }


            //PIR 9692
            if (iblnIsFromPortal && (iblnIsUpdate || iblnIsAddNewProvider))
            {
                LoadPersonAccount();
                if (!iblnIsAddNewProvider)
                    UpdateDefCompProvider(icdoPersonAccountDeferredCompProvider.person_account_provider_id, icdoPersonAccountDeferredCompProvider.start_date,
                        icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt);
                iintRequestId = 0;
                cdoWssPersonAccountEnrollmentRequest lcdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest();
                LoadAcknowledgement();
                ibusPersonAccount.LoadPersonAccountEmploymentDetails();
                lcdoWssPersonAccountEnrollmentRequest.person_id = ibusPersonAccount.icdoPersonAccount.person_id;
                lcdoWssPersonAccountEnrollmentRequest.plan_id = ibusPersonAccount.icdoPersonAccount.plan_id;
                lcdoWssPersonAccountEnrollmentRequest.target_person_account_id = ibusPersonAccount.icdoPersonAccount.person_account_id;
                lcdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id = ibusPersonAccount.iclbAccountEmploymentDetail[0].icdoPersonAccountEmploymentDetail.person_employment_dtl_id;//ibusPersonAccount.icdoPersonAccount.person_employment_dtl_id;
                lcdoWssPersonAccountEnrollmentRequest.reason_value = ibusPersonAccount.icdoPersonAccount.reason_value;
                lcdoWssPersonAccountEnrollmentRequest.status_id = busConstant.MemberPortalEnrollmentRequestStatus;
                lcdoWssPersonAccountEnrollmentRequest.status_value = busConstant.StatusProcessed;
                lcdoWssPersonAccountEnrollmentRequest.enrollment_type_id = busConstant.MemberPortalEnrollmentType;
                lcdoWssPersonAccountEnrollmentRequest.enrollment_type_value = busConstant.DeferredCompensation;
                lcdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value = busConstant.PlanEnrollmentOptionValueEnroll;
                lcdoWssPersonAccountEnrollmentRequest.reason_value = busConstant.ReasonValueNotApplicable;
                lcdoWssPersonAccountEnrollmentRequest.Insert();
                iintRequestId = lcdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;
                InsertString("DCAM");
                InsertCollection(iclbDefCompAcknowledgement);
                InsertString("CONF");

                cdoWssPersonAccountDeferredComp lcdoWssPersonAccountDeferredComp = new cdoWssPersonAccountDeferredComp();
                lcdoWssPersonAccountDeferredComp.wss_person_account_enrollment_request_id = iintRequestId;
                lcdoWssPersonAccountDeferredComp.target_person_account_id = ibusPersonAccountDeferredComp.icdoPersonAccountDeferredComp.person_account_id;
                lcdoWssPersonAccountDeferredComp.catch_up_start_date = ibusPersonAccountDeferredComp.icdoPersonAccountDeferredComp.catch_up_start_date;
                lcdoWssPersonAccountDeferredComp.catch_up_end_date = ibusPersonAccountDeferredComp.icdoPersonAccountDeferredComp.catch_up_end_date;
                lcdoWssPersonAccountDeferredComp.limit_457_id = ibusPersonAccountDeferredComp.icdoPersonAccountDeferredComp.limit_457_id;
                lcdoWssPersonAccountDeferredComp.limit_457_value = ibusPersonAccountDeferredComp.icdoPersonAccountDeferredComp.limit_457_value;
                lcdoWssPersonAccountDeferredComp.hardship_withdrawal_flag = ibusPersonAccountDeferredComp.icdoPersonAccountDeferredComp.hardship_withdrawal_flag;
                lcdoWssPersonAccountDeferredComp.hardship_withdrawal_effective_date = ibusPersonAccountDeferredComp.icdoPersonAccountDeferredComp.hardship_withdrawal_effective_date;
                lcdoWssPersonAccountDeferredComp.de_minimus_distribution_flag = ibusPersonAccountDeferredComp.icdoPersonAccountDeferredComp.de_minimus_distribution_flag;
                lcdoWssPersonAccountDeferredComp.file_457_sent_flag = ibusPersonAccountDeferredComp.icdoPersonAccountDeferredComp.file_457_sent_flag;
                lcdoWssPersonAccountDeferredComp.Insert();
            }
            //PIR 6961
            if (iblnIsFromPortal)
            {
                //if (icdoPersonAccountDeferredCompProvider.start_date != DateTime.MinValue &&
                //    icdoPersonAccountDeferredCompProvider.provider_org_plan_id > 0)
                //{
                if (istrUpdateExistingProviderFlag == busConstant.Flag_Yes || istrUpdateExistingEmployerFlag == busConstant.Flag_Yes)
                    UpdatePreviousDefCompProvider();
                
                cdoWssPersonAccountDeferredCompProvider lcdoWssPersonAccountDeferredCompProvider = new cdoWssPersonAccountDeferredCompProvider();
                ibusPersonAccountDeferredComp.LoadPersonAccount();
                int aintpersonaccountid = ibusPersonAccountDeferredComp.icdoPersonAccount.person_account_id;
                DataTable dtbDeffComp = Select("cdoWssAcknowledgement.DeffComp", new object[1] { aintpersonaccountid });
                if (dtbDeffComp.Rows.Count > 0)
                {
                    int aintPersonAccountDeferredCompid = Convert.ToInt32(dtbDeffComp.Rows[0]["WSS_PERSON_ACCOUNT_DEFERRED_COMP_ID"]);

                    lcdoWssPersonAccountDeferredCompProvider.wss_person_account_deferred_comp_id = aintPersonAccountDeferredCompid;
                    lcdoWssPersonAccountDeferredCompProvider.provider_org_plan_id = icdoPersonAccountDeferredCompProvider.provider_org_plan_id;
                    lcdoWssPersonAccountDeferredCompProvider.company_name = icdoPersonAccountDeferredCompProvider.company_name;
                    lcdoWssPersonAccountDeferredCompProvider.provider_agent_contact_id = icdoPersonAccountDeferredCompProvider.provider_agent_contact_id;
                    lcdoWssPersonAccountDeferredCompProvider.per_pay_period_contribution_amt = icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt;
                    lcdoWssPersonAccountDeferredCompProvider.start_date = icdoPersonAccountDeferredCompProvider.start_date;
                    lcdoWssPersonAccountDeferredCompProvider.end_date = icdoPersonAccountDeferredCompProvider.end_date;
                    lcdoWssPersonAccountDeferredCompProvider.mutual_fund_window_flag = icdoPersonAccountDeferredCompProvider.mutual_fund_window_flag;
                    lcdoWssPersonAccountDeferredCompProvider.person_employment_id = icdoPersonAccountDeferredCompProvider.person_employment_id;
                    lcdoWssPersonAccountDeferredCompProvider.assets_with_provider_id = icdoPersonAccountDeferredCompProvider.assets_with_provider_id;
                    lcdoWssPersonAccountDeferredCompProvider.assets_with_provider_value = icdoPersonAccountDeferredCompProvider.assets_with_provider_value;
                    lcdoWssPersonAccountDeferredCompProvider.comments = icdoPersonAccountDeferredCompProvider.comments;
                    lcdoWssPersonAccountDeferredCompProvider.payment_status_id = icdoPersonAccountDeferredCompProvider.payment_status_id;
                    lcdoWssPersonAccountDeferredCompProvider.payment_status_value = icdoPersonAccountDeferredCompProvider.payment_status_value;
                    if (iblnIsAddNewProvider)
                        lcdoWssPersonAccountDeferredCompProvider.update_existing_provider = istrUpdateExistingProviderFlag;
                    lcdoWssPersonAccountDeferredCompProvider.Insert();
                }
                //}
            }

            //if(iblnIsObjectStateInsert)
            //{
            //    busPersonAccountDeferredCompProvider lbusDefWithoutEndDate = new busPersonAccountDeferredCompProvider { icdoPersonAccountDeferredCompProvider = new cdoPersonAccountDeferredCompProvider() };
            //    busPersonAccountDeferredCompProvider lbusDefWithEndDate = new busPersonAccountDeferredCompProvider { icdoPersonAccountDeferredCompProvider = new cdoPersonAccountDeferredCompProvider() };
            //    lbusDefWithoutEndDate.FindPersonAccountDeferredCompProvider(icdoPersonAccountDeferredCompProvider.person_account_provider_id);

            //    lbusDefWithEndDate.FindPersonAccountDeferredCompProvider(icdoPersonAccountDeferredCompProvider.person_account_provider_id);
            //    lbusDefWithoutEndDate.icdoPersonAccountDeferredCompProvider.end_date = DateTime.MinValue;

            //    Collection<busPersonAccountDeferredCompProvider> lclbDefProvider = new Collection<busPersonAccountDeferredCompProvider>();
            //    lclbDefProvider.Add(lbusDefWithoutEndDate);
            //    lclbDefProvider.Add(lbusDefWithEndDate);

            //    foreach(busPersonAccountDeferredCompProvider lbusDefCompProvider in lclbDefProvider)
            //    {
            //        lbusDefCompProvider.InsertIntoEnrollmentData(iblnIsObjectStateInsert, istrIsUnUsedSickLeaveConversion);
            //    }
            //}
            //else
            //{

            /* PIR 20702: Both PS File and Benefit Enrollment Report flag should be set to N for only the following scenarios 
             (all others don’t even need an entry in the Enrollment Data table):
             1.Inserting a new Def Comp Provider – already working
             2.Ending existing Def Comp Provider – already working
             3.Updating any of the following elements should also write to the table
                 a.Update Start Date
                 b.Update End Date
                 c.Update Provider
                 d.Update Amount
             4.Plan Participation changes should not be written to the table
             5.All other updates should not create an entry*/

            //added iblnIsFromPortal portal check : to fix scenario mss change amount=> add zero amount
            if (iblnIsNewModeFromAllPortal || (!iblnIsNewModeFromAllPortal && iblnFieldValuesChangedInUpdateMode) || iblnIsFromPortal)
            //&&  icdoPersonAccountDeferredCompProvider.start_date != icdoPersonAccountDeferredCompProvider.end_date) //Pir 20702
            {
                int lintCount = 0;
                DataTable ldtRetirement = DBFunction.DBSelect("entPersonAccount.IsEnrolledInDC25", new object[2]
                       { ibusPersonAccountDeferredComp?.icdoPersonAccount?.person_id, icdoPersonAccountDeferredCompProvider.start_date }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (ldtRetirement.IsNotNull() && ldtRetirement.Rows.Count > 0)
                    lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("entPersonAccount.CheckForDeferredCompEntry", new object[2]
                       { ibusPersonAccountDeferredComp?.icdoPersonAccount?.person_id, Convert.IsDBNull(ldtRetirement.Rows[0]["addl_ee_contribution_percent"]) ? 0 : Convert.ToInt32(ldtRetirement.Rows[0]["addl_ee_contribution_percent"]) }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));


                if (ldtRetirement.IsNotNull() && ldtRetirement.Rows.Count > 0 && lintCount == 0)
                {

                    InsertIntoEnrollmentData(iblnIsObjectStateInsert, icdoPersonAccountDeferredCompProvider.lumpsum);//Entry for amount changes/new enrollment
                    InsertIntoEnrollmentData(false, busConstant.Flag_No, true, Convert.IsDBNull(ldtRetirement.Rows[0]["addl_ee_contribution_percent"]) ? 0 : Convert.ToInt32(ldtRetirement.Rows[0]["addl_ee_contribution_percent"]));//Extra line for DC25
                }
                else
                    //PIR 20702 :  Added new column for Lumpsum.
                    InsertIntoEnrollmentData(iblnIsObjectStateInsert, icdoPersonAccountDeferredCompProvider.lumpsum); //PIR 20702 : added lumpsum field in database
            }
            //}

        }

        private bool IsFieldValuesChangedInUpdateMode()
        {
            /* PIR 20702: Both PS File and Benefit Enrollment Report flag should be set to N for only the following scenarios 
             (all others don’t even need an entry in the Enrollment Data table):
             3.Updating any of the following elements should also write to the table
                 a.Update Start Date
                 b.Update End Date
                 c.Update Provider
                 d.Update Amount*/
            if (icdoPersonAccountDeferredCompProvider.start_date != Convert.ToDateTime(icdoPersonAccountDeferredCompProvider.ihstOldValues["start_date"]) ||
               icdoPersonAccountDeferredCompProvider.end_date != Convert.ToDateTime(icdoPersonAccountDeferredCompProvider.ihstOldValues["end_date"]) ||
               icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt != Convert.ToDecimal(icdoPersonAccountDeferredCompProvider.ihstOldValues["per_pay_period_contribution_amt"]) ||
               icdoPersonAccountDeferredCompProvider.provider_org_plan_id != Convert.ToInt32(icdoPersonAccountDeferredCompProvider.ihstOldValues["provider_org_plan_id"]) ||
                icdoPersonAccountDeferredCompProvider.lumpsum != (icdoPersonAccountDeferredCompProvider.ihstOldValues["lumpsum"]) ||
                icdoPersonAccountDeferredCompProvider.person_employment_id != Convert.ToInt32(icdoPersonAccountDeferredCompProvider.ihstOldValues["person_employment_id"])
               )
                return true;
            else
                return false;
        }
        private void LoadObjectByDefferedComp() //PIR 20702
        {

            if (ibusPersonAccountDeferredComp.IsNull())
                LoadPersonAccountDeferredComp();

            if (ibusPersonAccountDeferredComp.ibusPersonAccount.IsNull())
            {
                ibusPersonAccountDeferredComp.LoadPersonAccount();
                ibusPersonAccountDeferredComp.icdoPersonAccount = ibusPersonAccountDeferredComp.ibusPersonAccount.icdoPersonAccount;
            }
            if (ibusPersonAccountDeferredComp.ibusHistory == null)
                ibusPersonAccountDeferredComp.LoadPreviousHistory();

            if (ibusPersonAccountDeferredComp.ibusPerson.IsNull())
                ibusPersonAccountDeferredComp.LoadPerson();

            if (ibusPersonAccountDeferredComp.ibusPersonEmploymentDetail.IsNull())
                ibusPersonAccountDeferredComp.LoadPersonEmploymentDetail();

            if (ibusPersonAccountDeferredComp.ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                ibusPersonAccountDeferredComp.ibusPersonEmploymentDetail.LoadPersonEmployment();
        }

        public void InsertIntoEnrollmentData(bool ablnIsObjectStateInsert, string astrIsUnUsedSickLeaveConversion, bool ablnIsFromDC25 = false, int aintAddlEEContriPercent = 0)
        {
            busEnrollmentData lobjEnrollmentData = new busEnrollmentData();
            lobjEnrollmentData.icdoEnrollmentData = new doEnrollmentData();

            string lstrProviderOrgCode = string.Empty;
            //DateTime ldtDeductionStartDate = new DateTime();
            string lstrProviderType = string.Empty;
            Collection<busPersonAccountDeferredCompProvider> lclbPersonAccountDeferredCompProviderTemp = new Collection<BusinessObjects.busPersonAccountDeferredCompProvider>();

            //Reload All Deferrred Comp Providers
            LoadObjectByDefferedComp();//pir 20702        
            ibusPersonAccountDeferredComp.LoadPersonAccountProviders();//pir 20702 : reloaded : change amount scenario not getting latest record

            if ((istrUpdateExistingProviderFlag == busConstant.Flag_No) || (!iblnIsFromPortal && (iblnFieldValuesChangedInUpdateMode || iblnIsNewMode)))
            {
                //pir 20702 : Mss no case, internal new and update mode scenario
                busPersonAccountDeferredCompProvider lobjPersonAccountDeferredCompProviderCurrentObject = ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider.Where(
                    i => i.icdoPersonAccountDeferredCompProvider.person_account_provider_id == this.icdoPersonAccountDeferredCompProvider.person_account_provider_id).FirstOrDefault();
                if (lobjPersonAccountDeferredCompProviderCurrentObject.IsNotNull())
                    lclbPersonAccountDeferredCompProviderTemp.Add(lobjPersonAccountDeferredCompProviderCurrentObject);
            }
            else if (istrUpdateExistingProviderFlag == busConstant.Flag_Yes)
            {
                //pir 20702 mss add new provider : Yes case
                lclbPersonAccountDeferredCompProviderTemp = iclbPersonAccountDeferredCompProviderUpdatedRecord; //ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider;

                busPersonAccountDeferredCompProvider lobjPersonAccountDeferredCompProviderCurrentObject = ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider.Where(
                    i => i.icdoPersonAccountDeferredCompProvider.person_account_provider_id == this.icdoPersonAccountDeferredCompProvider.person_account_provider_id).FirstOrDefault();
                if (lobjPersonAccountDeferredCompProviderCurrentObject.IsNotNull())
                    lclbPersonAccountDeferredCompProviderTemp.Add(lobjPersonAccountDeferredCompProviderCurrentObject);
            }
            else if (iblnIsFromPortal && !iblnIsAddNewProvider)
            {
                //pir 20702 change amount scenario
                busPersonAccountDeferredCompProvider lobjPersonAccountDeferredCompProviderCurrentObject = ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider.Where(
                    i => i.icdoPersonAccountDeferredCompProvider.person_account_provider_id == this.icdoPersonAccountDeferredCompProvider.person_account_provider_id).FirstOrDefault();
                if (lobjPersonAccountDeferredCompProviderCurrentObject.IsNotNull())
                    lclbPersonAccountDeferredCompProviderTemp.Add(lobjPersonAccountDeferredCompProviderCurrentObject);

                busPersonAccountDeferredCompProvider lobjPersonAccountDeferredCompProviderLatest = ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider.Where(
                i => i.icdoPersonAccountDeferredCompProvider.person_account_provider_id == iintNewProviderIdForChangeAmount).FirstOrDefault();
                if (lobjPersonAccountDeferredCompProviderLatest.IsNotNull())
                    lclbPersonAccountDeferredCompProviderTemp.Add(lobjPersonAccountDeferredCompProviderLatest);
            }
            else
            {
                //annual and regular enrollment through wizard
                busPersonAccountDeferredCompProvider lobjPersonAccountDeferredCompProviderCurrentObject = ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider.Where(
                   i => i.icdoPersonAccountDeferredCompProvider.person_account_provider_id == this.icdoPersonAccountDeferredCompProvider.person_account_provider_id).FirstOrDefault();
                if (lobjPersonAccountDeferredCompProviderCurrentObject.IsNotNull())
                    lclbPersonAccountDeferredCompProviderTemp.Add(lobjPersonAccountDeferredCompProviderCurrentObject);
                //lclbPersonAccountDeferredCompProviderTemp = ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider;
            }
            //PIR 21456 handle zero amount and should not print in report and PS file
            var lintMaxSourceIDIfZero = 0;
            if (lclbPersonAccountDeferredCompProviderTemp.Where(DefCompProvider => 
            DefCompProvider.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt == 0
                                    && DefCompProvider.icdoPersonAccountDeferredCompProvider.end_date == DateTime.MinValue).Any())
            {
                busPersonAccountDeferredCompProvider lbusPersonAccountMaxDeferredCompProvider = lclbPersonAccountDeferredCompProviderTemp.
                                        FirstOrDefault(DefCompProvider => DefCompProvider.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt != 0
                                      && DefCompProvider.icdoPersonAccountDeferredCompProvider.end_date != DateTime.MinValue);

                if (lbusPersonAccountMaxDeferredCompProvider.IsNotNull())
                {
                    lintMaxSourceIDIfZero = lbusPersonAccountMaxDeferredCompProvider.icdoPersonAccountDeferredCompProvider.person_account_provider_id;
                }
            }
            foreach (busPersonAccountDeferredCompProvider lobjPersonAccountDeferredCompProvider in lclbPersonAccountDeferredCompProviderTemp) //PIR 20702
            {
                ibusPersonAccountDeferredComp = lobjPersonAccountDeferredCompProvider.ibusPersonAccountDeferredComp;
                LoadObjectByDefferedComp();//pir 20702

                busDailyPersonAccountPeopleSoft lobjDailyPAPeopleSoft = new busDailyPersonAccountPeopleSoft();
                lobjDailyPAPeopleSoft.ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                lobjDailyPAPeopleSoft.ibusProvider = new busOrganization { icdoOrganization = new cdoOrganization() };
                lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };

                lobjDailyPAPeopleSoft.ibusPersonAccount.icdoPersonAccount = ibusPersonAccountDeferredComp.icdoPersonAccount;
                lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail = ibusPersonAccountDeferredComp.ibusPersonEmploymentDetail;
                lobjDailyPAPeopleSoft.iblnIsFromDeferredComp = true;//PIR 20702
                lobjDailyPAPeopleSoft.iintPersonEmpId = lobjPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.person_employment_id;//PIR 20702
                lobjPersonAccountDeferredCompProvider.LoadProviderOrgPlan();
                lobjPersonAccountDeferredCompProvider.ibusProviderOrgPlan.LoadOrganization();
                lobjDailyPAPeopleSoft.ibusProvider = lobjPersonAccountDeferredCompProvider.ibusProviderOrgPlan.ibusOrganization;

                lobjDailyPAPeopleSoft.LoadPersonEmploymentForPeopleSoft();
                //PeopleSoft record should not be created if the org group is not PeopleSoft.
                lobjDailyPAPeopleSoft.LoadPeopleSoftOrgGroupValues();
                if (!iblnIsFromPortal && iblnFieldValuesChangedInUpdateMode)
                    //PIR 20702 :benefit reprot generated flag to 'Y' for same provider with benefit report generated flag N; By source id.

                    DBFunction.DBNonQuery("cdoPersonAccount.UpdatePrevPeopleSoftNBenefitEnrlFlagDefComp", new object[2] { lobjPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.person_account_provider_id, icdoPersonAccountDeferredCompProvider.person_account_id },
                       iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                #region pplsoft org
                if ((lobjDailyPAPeopleSoft.iclbPeopleSoftOrgGroupValue.Where(i => i.code_value == lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value
                    && i.data2 == busConstant.Flag_Yes).Count() > 0) && astrIsUnUsedSickLeaveConversion != busConstant.Flag_Yes && ibusPersonAccountDeferredComp.icdoPersonAccount.plan_id != busConstant.PlanIdOther457) //PIR 20169
                {
                    //if (ibusProviderOrgPlan == null)
                    //    LoadProviderOrgPlan();
                    //if (ibusProviderOrgPlan.ibusOrganization == null)
                    //    ibusProviderOrgPlan.LoadOrganization();commented unnecessary code
                    if (!ablnIsFromDC25)
                        lstrProviderOrgCode = lobjPersonAccountDeferredCompProvider.ibusProviderOrgPlan.ibusOrganization.icdoOrganization.org_code;

                    //PIR 20702 case 3 - Start Date (E) of new within one day of prior End Date set "E"
                    if (ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider.IsNull())
                        ibusPersonAccountDeferredComp.LoadPersonAccountProviders();

                    DateTime ldtMaxDatePrevDefComp = DateTime.MinValue;
                    //PIR 20702 getting max end date with matching provider and ord id
                    if (ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider.IsNotNull())
                        ldtMaxDatePrevDefComp = ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider.Where(i => i.icdoPersonAccountDeferredCompProvider.person_employment_id == lobjPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.person_employment_id
                        && i.icdoPersonAccountDeferredCompProvider.provider_org_plan_id == lobjPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.provider_org_plan_id
                        && i.icdoPersonAccountDeferredCompProvider.end_date != null).OrderByDescending(i => i.icdoPersonAccountDeferredCompProvider.end_date).Select(i => i.icdoPersonAccountDeferredCompProvider.end_date).FirstOrDefault();

                    if (lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft == null)
                        lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft = new Collection<busDailyPersonAccountPeopleSoft>();

                        lobjDailyPAPeopleSoft.idtDeductionStartDate = lobjPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.start_date;
                        lobjDailyPAPeopleSoft.idtDeductionEndDate = lobjPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.end_date;
                        lobjEnrollmentData.icdoEnrollmentData.pay_period_amount = lobjPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt;
                        lobjDailyPAPeopleSoft.GeneratePeoplesoftEntryForDeferredComp(lstrProviderOrgCode, aintAddlEEContriPercent, ablnIsFromDC25, lobjDailyPAPeopleSoft.istrEmpTypeValue);

                    if (lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft.IsNotNull() && lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft.Count > 0)
                    {
                        foreach (busDailyPersonAccountPeopleSoft lobjDailyPeopleSoft in lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft)
                        {
                            lobjEnrollmentData.icdoEnrollmentData.source_id = lobjPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.person_account_provider_id;
                            lobjEnrollmentData.icdoEnrollmentData.plan_id = ibusPersonAccountDeferredComp.icdoPersonAccount.plan_id;
                            lobjEnrollmentData.icdoEnrollmentData.employment_type_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value;
                            lobjEnrollmentData.icdoEnrollmentData.ssn = ibusPersonAccountDeferredComp.ibusPerson.icdoPerson.ssn;
                            lobjEnrollmentData.icdoEnrollmentData.ndpers_member_id = ibusPersonAccountDeferredComp.ibusPerson.icdoPerson.person_id;
                            lobjEnrollmentData.icdoEnrollmentData.person_account_id = ibusPersonAccountDeferredComp.icdoPersonAccount.person_account_id;
                            lobjEnrollmentData.icdoEnrollmentData.peoplesoft_id = ibusPersonAccountDeferredComp.ibusPerson.icdoPerson.peoplesoft_id;
                            lobjEnrollmentData.icdoEnrollmentData.employer_org_id = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
                            lobjEnrollmentData.icdoEnrollmentData.plan_status_value = ibusPersonAccountDeferredComp.ibusHistory.icdoPersonAccountDeferredCompHistory.plan_participation_status_value;
                            lobjEnrollmentData.icdoEnrollmentData.peoplesoft_org_group_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value;
                            lobjEnrollmentData.icdoEnrollmentData.peoplesoft_change_reason_value = lobjDailyPAPeopleSoft.istrPSFileChangeEvent;
                            if (!ablnIsFromDC25)
                                lobjEnrollmentData.icdoEnrollmentData.provider_org_id = lobjPersonAccountDeferredCompProvider.ibusProviderOrgPlan.ibusOrganization.icdoOrganization.org_id;

                            lobjEnrollmentData.icdoEnrollmentData.start_date = lobjPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.start_date;
                            lobjEnrollmentData.icdoEnrollmentData.end_date = lobjPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.end_date;

                            lobjEnrollmentData.icdoEnrollmentData.coverage_code = lobjDailyPeopleSoft.istrCoverageCode;
                            lobjEnrollmentData.icdoEnrollmentData.benefit_plan = lobjDailyPeopleSoft.istrBenefitPlan;
                            lobjEnrollmentData.icdoEnrollmentData.deduction_begin_date = lobjDailyPeopleSoft.idtDeductionBeginDate;
                            lobjEnrollmentData.icdoEnrollmentData.coverage_begin_date = lobjDailyPeopleSoft.idtCoverageBeginDate;
                            lobjEnrollmentData.icdoEnrollmentData.plan_type = lobjDailyPeopleSoft.istrPlanType;

                            if (ablnIsFromDC25)
                            {
                                if (aintAddlEEContriPercent == 0)
                                    lobjEnrollmentData.icdoEnrollmentData.employee_employer_match = "Employer Match up to 3%";
                                if (aintAddlEEContriPercent == 1)
                                    lobjEnrollmentData.icdoEnrollmentData.employee_employer_match = "Employer Match up to 2%";
                                if (aintAddlEEContriPercent == 2)
                                    lobjEnrollmentData.icdoEnrollmentData.employee_employer_match = "Employer Match up to 1%";
                            }

                            //PIR 20702 case 1 - Ending def comp provider record and emp end date is null set "W"
                            if (lobjPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.end_date != DateTime.MinValue && lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date == DateTime.MinValue)
                            {
                                lobjEnrollmentData.icdoEnrollmentData.coverage_election_value = "W";
                            }
                            //PIR 20702 case 3 - Start Date (E) of new within one day of prior End Date set "E" updae W records both flag to "Y"
                            //PIR 21456 don’t update the sent flags to Y where the amount is NULL or 0.
                            else if (lobjEnrollmentData.icdoEnrollmentData.pay_period_amount > 0 && ldtMaxDatePrevDefComp != DateTime.MinValue && busGlobalFunctions.DateDiffInDays(ldtMaxDatePrevDefComp, lobjPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.start_date) == 1)
                            {
                                lobjEnrollmentData.icdoEnrollmentData.coverage_election_value = "E";
                                //update prev record where all "W" and set both PS & report flag to Y
                                DBFunction.DBNonQuery("cdoPersonAccount.UpdatePrevEnrollmentDataFlagDefComp", new object[2] { lobjPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.person_account_id,
                                                                                                                               lobjEnrollmentData.icdoEnrollmentData.plan_type},iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework); //PIR 22900
                            }
                            //PIR 20702 case 4 - Start Date (E) of new within one day of prior End Date set "E" 
                            else if (ldtMaxDatePrevDefComp != DateTime.MinValue && busGlobalFunctions.DateDiffInDays(ldtMaxDatePrevDefComp, lobjPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.start_date) > 1)
                            {
                                lobjEnrollmentData.icdoEnrollmentData.coverage_election_value = "E";
                            }
                            //PIR 20702 case 1 - Ending def comp provider record and emp end date is Present set "T"
                            else if (lobjPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.end_date != DateTime.MinValue && lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date != DateTime.MinValue)
                                lobjEnrollmentData.icdoEnrollmentData.coverage_election_value = "T";
                            else
                                lobjEnrollmentData.icdoEnrollmentData.coverage_election_value = lobjDailyPeopleSoft.istrCoverageElection;
                            lobjEnrollmentData.icdoEnrollmentData.election_date = lobjDailyPeopleSoft.idtElectionDate;
                            lobjEnrollmentData.icdoEnrollmentData.pretax_amount = lobjPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt;

                            if (ablnIsObjectStateInsert && lobjEnrollmentData.icdoEnrollmentData.coverage_election_value == "E" && astrIsUnUsedSickLeaveConversion == busConstant.Flag_Yes)
                                lobjEnrollmentData.icdoEnrollmentData.lump_sum_payout = busConstant.Flag_Yes;
                            else if (!ablnIsObjectStateInsert && astrIsUnUsedSickLeaveConversion == busConstant.Flag_Yes)
                                lobjEnrollmentData.icdoEnrollmentData.lump_sum_payout = busConstant.Flag_Yes;

                            if (lobjEnrollmentData.icdoEnrollmentData.lump_sum_payout == busConstant.Flag_Yes)
                                lobjEnrollmentData.icdoEnrollmentData.change_reason_value = busConstant.LumpSumPayout;

                            if (ablnIsObjectStateInsert && lobjEnrollmentData.icdoEnrollmentData.coverage_election_value == "E")
                                lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_No;
                            else if (ablnIsObjectStateInsert && lobjEnrollmentData.icdoEnrollmentData.coverage_election_value != "E")
                                lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_Yes;
                            else
                                lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_No;
                            //PIR 21456 update both flag to Y where the amount is NULL or 0.
                            //lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated =  busConstant.Flag_No;
                            lobjEnrollmentData.icdoEnrollmentData.peoplesoft_file_sent_flag = lobjEnrollmentData.icdoEnrollmentData.pay_period_amount > 0 ? busConstant.Flag_No : busConstant.Flag_Yes;

                            //PIR 20178
                            if ((lobjEnrollmentData.icdoEnrollmentData.coverage_election_value == "T" ||
                                lobjEnrollmentData.icdoEnrollmentData.coverage_election_value == "W") || ablnIsFromDC25) //PIR 25920 Amount for MATCHx should not be shown on PS file
                                lobjEnrollmentData.icdoEnrollmentData.pretax_amount = 0.0m;
                            //PIR 21456 handle zero amount and should not print in report and PS file
                            if (lintMaxSourceIDIfZero !=0 && lobjEnrollmentData.icdoEnrollmentData.pay_period_amount == 0 && icdoPersonAccountDeferredCompProvider.person_account_id > 0)
                            {
                                DBFunction.DBNonQuery("cdoPersonAccount.UpdateDefCompPrevRecordsIfZeroAmount", new object[2] { lintMaxSourceIDIfZero,
                                    icdoPersonAccountDeferredCompProvider.person_account_id },
                                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            }

                            //PIR 23856 - Update previous PS sent flag in Enrollment Data to Y if Person Employment is changed (transfer).
                            DBFunction.DBNonQuery("cdoPersonAccount.UpdatePSSentFlagForTransfersDeferredComp", new object[6] { ibusPersonAccountDeferredComp.icdoPersonAccount.plan_id,ibusPersonAccountDeferredComp.icdoPersonAccount.person_id,
                                                                                                  lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,
                                                                                                  lobjEnrollmentData.icdoEnrollmentData.deduction_begin_date,ibusPersonAccountDeferredComp.icdoPersonAccount.person_account_id,
                                                                                                  lobjPersonAccountDeferredCompProvider.ibusProviderOrgPlan.ibusOrganization.icdoOrganization.org_id},
                                                                                                       iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);


                            if (ablnIsFromDC25)
                            {
                                lobjEnrollmentData.icdoEnrollmentData.pay_period_amount = 0;

                                DBFunction.DBNonQuery("cdoPersonAccount.UpdateDC25ElectionForDefComp", new object[2] { ibusPersonAccountDeferredComp.icdoPersonAccount.person_account_id,
                                lobjEnrollmentData.icdoEnrollmentData.start_date }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            }

                            lobjEnrollmentData.icdoEnrollmentData.Insert();
                        }
                    }
                }
                #endregion pplsoft org
                #region nonpplsoft org
                else
                {

                    lobjEnrollmentData.icdoEnrollmentData.source_id = lobjPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.person_account_provider_id;
                    lobjEnrollmentData.icdoEnrollmentData.plan_id = ibusPersonAccountDeferredComp.icdoPersonAccount.plan_id;
                    lobjEnrollmentData.icdoEnrollmentData.employment_type_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value;
                    lobjEnrollmentData.icdoEnrollmentData.ssn = ibusPersonAccountDeferredComp.ibusPerson.icdoPerson.ssn;
                    lobjEnrollmentData.icdoEnrollmentData.ndpers_member_id = ibusPersonAccountDeferredComp.ibusPerson.icdoPerson.person_id;
                    lobjEnrollmentData.icdoEnrollmentData.person_account_id = ibusPersonAccountDeferredComp.icdoPersonAccount.person_account_id;
                    lobjEnrollmentData.icdoEnrollmentData.peoplesoft_id = ibusPersonAccountDeferredComp.ibusPerson.icdoPerson.peoplesoft_id;
                    lobjEnrollmentData.icdoEnrollmentData.employer_org_id = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
                    lobjEnrollmentData.icdoEnrollmentData.plan_status_value = ibusPersonAccountDeferredComp.ibusHistory.icdoPersonAccountDeferredCompHistory.plan_participation_status_value;
                    lobjEnrollmentData.icdoEnrollmentData.peoplesoft_org_group_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value;
                    if (!ablnIsFromDC25)
                        lobjEnrollmentData.icdoEnrollmentData.provider_org_id = lobjPersonAccountDeferredCompProvider.ibusProviderOrgPlan.ibusOrganization.icdoOrganization.org_id;
                    lobjEnrollmentData.icdoEnrollmentData.pay_period_amount = lobjPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt;

                    if (ablnIsFromDC25)
                    {
                        if (aintAddlEEContriPercent == 0)
                            lobjEnrollmentData.icdoEnrollmentData.employee_employer_match = "Employer Match up to 3%";
                        if (aintAddlEEContriPercent == 1)
                            lobjEnrollmentData.icdoEnrollmentData.employee_employer_match = "Employer Match up to 2%";
                        if (aintAddlEEContriPercent == 2)
                            lobjEnrollmentData.icdoEnrollmentData.employee_employer_match = "Employer Match up to 1%";
                    }

                    if (astrIsUnUsedSickLeaveConversion == busConstant.Flag_Yes)
                        lobjEnrollmentData.icdoEnrollmentData.lump_sum_payout = busConstant.Flag_Yes;

                    if (lobjEnrollmentData.icdoEnrollmentData.lump_sum_payout == busConstant.Flag_Yes)
                        lobjEnrollmentData.icdoEnrollmentData.change_reason_value = busConstant.LumpSumPayout;

                    lobjEnrollmentData.icdoEnrollmentData.start_date = lobjPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.start_date;
                    lobjEnrollmentData.icdoEnrollmentData.end_date = lobjPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.end_date;

                    //if (ablnIsObjectStateInsert && icdoPersonAccountDeferredCompProvider.end_date == DateTime.MinValue)
                    //    lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_No;
                    //else if (ablnIsObjectStateInsert && icdoPersonAccountDeferredCompProvider.end_date != DateTime.MinValue)
                    //    lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_Yes;
                    //else if (!ablnIsObjectStateInsert)
                    //PIR 21456 update both flag to Y where the amount is NULL or 0.
                    lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated =  busConstant.Flag_No;

                    lobjEnrollmentData.icdoEnrollmentData.peoplesoft_file_sent_flag = busConstant.Flag_Yes;
                    //PIR 21456 handle zero amount and should not print in report and PS file
                    if (lintMaxSourceIDIfZero != 0 && lobjEnrollmentData.icdoEnrollmentData.pay_period_amount == 0 && icdoPersonAccountDeferredCompProvider.person_account_id > 0 )
                    {
                        DBFunction.DBNonQuery("cdoPersonAccount.UpdateDefCompPrevRecordsIfZeroAmount", new object[2] { lintMaxSourceIDIfZero,
                            icdoPersonAccountDeferredCompProvider.person_account_id },
                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                    }
                    if (ablnIsFromDC25)
                    {
                        lobjEnrollmentData.icdoEnrollmentData.pay_period_amount = 0;

                        DBFunction.DBNonQuery("cdoPersonAccount.UpdateDC25ElectionForDefComp", new object[2] { ibusPersonAccountDeferredComp.icdoPersonAccount.person_account_id,
                           lobjEnrollmentData.icdoEnrollmentData.start_date },iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                    }
                    lobjEnrollmentData.icdoEnrollmentData.Insert();
                }
                //20702 when a record is inserted, outside of the above, and the Coverage Begin Date is different on the records
                if (this.icdoPersonAccountDeferredCompProvider.end_date != DateTime.MinValue)
                {
                    DBFunction.DBNonQuery("cdoPersonAccount.UpdateEnrollmentDataForDiffCoverageBeginDate", new object[2] { lobjPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.person_account_provider_id, icdoPersonAccountDeferredCompProvider.person_account_id },
                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                }
                #endregion nonpplsoft org
            }
        }

        //PIR 9692
        public int iintRequestId;
        private cdoPersonAccount _icdoPersonAccount;
        public cdoPersonAccount icdoPersonAccount
        {
            get
            {
                return _icdoPersonAccount;
            }
            set
            {
                _icdoPersonAccount = value;
            }
        }
        //PIR 9692
        public bool FindPersonAccount(int Aintpersonaccountid)
        {
            bool lblnResult = false;
            if (_icdoPersonAccount == null)
            {
                _icdoPersonAccount = new cdoPersonAccount();
            }
            if (_icdoPersonAccount.SelectRow(new object[1] { Aintpersonaccountid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        //PIR 9692
        public busPersonAccount ibusPersonAccount { get; set; }
        public void LoadPersonAccount()
        {
            if (ibusPersonAccount == null)
                ibusPersonAccount = new busPersonAccount();
            ibusPersonAccount.FindPersonAccount(icdoPersonAccountDeferredCompProvider.person_account_id);
        }

        //PIR 9692
        public void InsertCollection(Collection<cdoWssAcknowledgement> aclbAcknowledgement)
        {
            busWssPersonAccountEnrollmentRequestAck lobjWssPersonAccountEnrollmentRequestAck = new busWssPersonAccountEnrollmentRequestAck();
            lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck = new cdoWssPersonAccountEnrollmentRequestAck();

            foreach (cdoWssAcknowledgement lobjWssAcknowledgement in aclbAcknowledgement)
            {
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_id = lobjWssAcknowledgement.acknowledgement_id;
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.wss_person_account_enrollment_request_id = iintRequestId;
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.Insert();
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

        //PIR 9692
        public void InsertString(string astrQuersyString)
        {
            busWssPersonAccountEnrollmentRequestAck lobjWssPersonAccountEnrollmentRequestAck = new busWssPersonAccountEnrollmentRequestAck();
            lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck = new cdoWssPersonAccountEnrollmentRequestAck();

            if (astrQuersyString == "CONF")
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = istrConfirmationText;
            else if (astrQuersyString == "DCAM")  // PIR 9692
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = istrDefCompAmountMessage;
            lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.wss_person_account_enrollment_request_id = iintRequestId;
            lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.Insert();
        }

        // An deduction exists for this provider for a future date.  Please contact NDPERS to make an election prior to the already entered date.
        public bool IsStartDateInValidForDefCompProvider()
        {
            if (ibusPersonAccountDeferredComp.IsNotNull())
                LoadPersonAccountDeferredComp();
            if (ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider.IsNull())
                ibusPersonAccountDeferredComp.LoadPersonAccountProviders();

            if (ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider.IsNull() ||
                ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider.Count == 0)
                return false;

            return ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider.Where(
                        o => o.icdoPersonAccountDeferredCompProvider.start_date > icdoPersonAccountDeferredCompProvider.start_date).Any();
        }

        //update the end date of previous address of current type
        public void UpdatePreviousDefCompProvider()
        {
            if (ibusPersonAccountDeferredComp.IsNull())
                LoadPersonAccountDeferredComp();
            if (ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider.IsNull())
                ibusPersonAccountDeferredComp.LoadPersonAccountProviders();

            busPersonAccountDeferredCompProvider lobjCurrentProvider = new busPersonAccountDeferredCompProvider { icdoPersonAccountDeferredCompProvider = new cdoPersonAccountDeferredCompProvider() };
            lobjCurrentProvider.FindPersonAccountDeferredCompProvider(icdoPersonAccountDeferredCompProvider.person_account_provider_id);

            foreach (busPersonAccountDeferredCompProvider lobjProvider in ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider)
            {
                if (istrUpdateExistingProviderFlag == busConstant.Flag_No && istrUpdateExistingEmployerFlag == busConstant.Flag_Yes)
                {
                    if (lobjProvider.icdoPersonAccountDeferredCompProvider.end_date == DateTime.MinValue &&
                    lobjProvider.icdoPersonAccountDeferredCompProvider.start_date != DateTime.MinValue &&
                    lobjProvider.icdoPersonAccountDeferredCompProvider.is_apply_employer_matching_contribution == busConstant.Flag_Yes &&
                    lobjProvider.icdoPersonAccountDeferredCompProvider.person_account_provider_id != icdoPersonAccountDeferredCompProvider.person_account_provider_id)
                    {
                        if (lobjProvider.icdoPersonAccountDeferredCompProvider.start_date < lobjCurrentProvider.icdoPersonAccountDeferredCompProvider.start_date)
                            lobjProvider.icdoPersonAccountDeferredCompProvider.end_date = icdoPersonAccountDeferredCompProvider.start_date.AddDays(-1);
                        else if (lobjProvider.icdoPersonAccountDeferredCompProvider.start_date == lobjCurrentProvider.icdoPersonAccountDeferredCompProvider.start_date)
                            lobjProvider.icdoPersonAccountDeferredCompProvider.end_date = icdoPersonAccountDeferredCompProvider.start_date;
                        else if (lobjCurrentProvider.icdoPersonAccountDeferredCompProvider.start_date == DateTime.MinValue)
                        {
                            if (lobjProvider.icdoPersonAccountDeferredCompProvider.start_date < icdoPersonAccountDeferredCompProvider.start_date)
                                lobjProvider.icdoPersonAccountDeferredCompProvider.end_date = icdoPersonAccountDeferredCompProvider.start_date.AddDays(-1);
                            else if (lobjProvider.icdoPersonAccountDeferredCompProvider.start_date == icdoPersonAccountDeferredCompProvider.start_date)
                                lobjProvider.icdoPersonAccountDeferredCompProvider.end_date = icdoPersonAccountDeferredCompProvider.start_date;
                        }
                        //lobjProvider.icdoPersonAccountDeferredCompProvider.is_apply_employer_matching_contribution = busConstant.Flag_No;
                        //PIR 17081
                        //lobjProvider.icdoPersonAccountDeferredCompProvider.is_enrollment_report_generated = busConstant.Flag_No; // PIR 9115
                        lobjProvider.icdoPersonAccountDeferredCompProvider.Update();
                        if (icdoPersonAccountDeferredCompProvider.is_apply_employer_matching_contribution == busConstant.Flag_Yes)
                            InsertDefCompProviderWithEmployerMatchingn(lobjProvider);
                        iclbPersonAccountDeferredCompProviderUpdatedRecord.Add(lobjProvider); //PIR 20702 : collection contains only modified records
                    }
                }
                else
                {
                    if (lobjProvider.icdoPersonAccountDeferredCompProvider.end_date == DateTime.MinValue &&
                        lobjProvider.icdoPersonAccountDeferredCompProvider.start_date != DateTime.MinValue &&
                        lobjProvider.icdoPersonAccountDeferredCompProvider.person_account_provider_id != icdoPersonAccountDeferredCompProvider.person_account_provider_id)
                    {
                        if (lobjProvider.icdoPersonAccountDeferredCompProvider.start_date < lobjCurrentProvider.icdoPersonAccountDeferredCompProvider.start_date)
                            lobjProvider.icdoPersonAccountDeferredCompProvider.end_date = icdoPersonAccountDeferredCompProvider.start_date.AddDays(-1);
                        else if (lobjProvider.icdoPersonAccountDeferredCompProvider.start_date == lobjCurrentProvider.icdoPersonAccountDeferredCompProvider.start_date)
                            lobjProvider.icdoPersonAccountDeferredCompProvider.end_date = icdoPersonAccountDeferredCompProvider.start_date;
                        lobjProvider.icdoPersonAccountDeferredCompProvider.is_apply_employer_matching_contribution = busConstant.Flag_No;
                        //PIR 17081
                        //lobjProvider.icdoPersonAccountDeferredCompProvider.is_enrollment_report_generated = busConstant.Flag_No; // PIR 9115
                        lobjProvider.icdoPersonAccountDeferredCompProvider.Update();
                        iclbPersonAccountDeferredCompProviderUpdatedRecord.Add(lobjProvider); //PIR 20702 : collection contains only modified records
                    }
                }
            }

            //PIR 25920 Reload collection after insert/update data
            ibusPersonAccountDeferredComp.LoadPersonAccountProviders();
        }
        /// <summary>
        /// PIR 25920
        /// Insert new Def Comp Provider record with IS_APPLY_EMPLOYER_MATCHING_CONTRIBUTION = N using Start Date = new Provider Start Date 
        /// and same provider details as record we just ended => (End existing Def Comp Provider record where IS_APPLY_EMPLOYER_MATCHING_CONTRIBUTION = Y using End Date = new Provider Start Date minus 1 day)
        /// </summary>
        /// <param name="aobjProvider"></param>
        public void InsertDefCompProviderWithEmployerMatchingn(busPersonAccountDeferredCompProvider aobjProvider)
        {
            busPersonAccountDeferredCompProvider lobjDefCompProvider = new busPersonAccountDeferredCompProvider() { icdoPersonAccountDeferredCompProvider = new cdoPersonAccountDeferredCompProvider() };
            lobjDefCompProvider.icdoPersonAccountDeferredCompProvider.person_account_id = icdoPersonAccountDeferredCompProvider.person_account_id;
            lobjDefCompProvider.icdoPersonAccountDeferredCompProvider.provider_org_plan_id = aobjProvider.icdoPersonAccountDeferredCompProvider.provider_org_plan_id;
            lobjDefCompProvider.icdoPersonAccountDeferredCompProvider.company_name = aobjProvider.icdoPersonAccountDeferredCompProvider.company_name;
            lobjDefCompProvider.icdoPersonAccountDeferredCompProvider.provider_agent_contact_id = aobjProvider.icdoPersonAccountDeferredCompProvider.provider_agent_contact_id;
            lobjDefCompProvider.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt = aobjProvider.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt;
            lobjDefCompProvider.icdoPersonAccountDeferredCompProvider.start_date = icdoPersonAccountDeferredCompProvider.start_date;
            lobjDefCompProvider.icdoPersonAccountDeferredCompProvider.person_employment_id = aobjProvider.icdoPersonAccountDeferredCompProvider.person_employment_id;
            lobjDefCompProvider.icdoPersonAccountDeferredCompProvider.assets_with_provider_value = aobjProvider.icdoPersonAccountDeferredCompProvider.assets_with_provider_value;
            lobjDefCompProvider.icdoPersonAccountDeferredCompProvider.payment_status_value = aobjProvider.icdoPersonAccountDeferredCompProvider.payment_status_value;
            lobjDefCompProvider.icdoPersonAccountDeferredCompProvider.is_enrollment_report_generated = busConstant.Flag_No;
            lobjDefCompProvider.icdoPersonAccountDeferredCompProvider.lumpsum = aobjProvider.icdoPersonAccountDeferredCompProvider.lumpsum;
            lobjDefCompProvider.icdoPersonAccountDeferredCompProvider.is_apply_employer_matching_contribution = busConstant.Flag_No;
            lobjDefCompProvider.icdoPersonAccountDeferredCompProvider.Insert();
            iclbPersonAccountDeferredCompProviderUpdatedRecord.Add(lobjDefCompProvider);
        }
        public bool IsAllAcknowledgementSelected()
        {
            DataTable ldtbAckCheckDetails = Select("cdoWssAcknowledgement.SelectAck", new object[2] { DateTime.Now, busConstant.DeferredCompCheck });
            var ldtbResult = ldtbAckCheckDetails.FilterTable(busConstant.DataType.String, "SHOW_CHECK_BOX_FLAG", busConstant.Flag_Yes);
            if (iclbDefCompAcknowledgement.Count == ldtbResult.Count())
            {
                return !iclbDefCompAcknowledgement.Any(i => i.ienuObjectState != ObjectState.CheckListInsert);//iclbDefCompAcknowledgement
            }
            return false;
        }

        public Collection<cdoWssAcknowledgement> iclbDefCompAcknowledgement { get; set; }
        //PIR 6961

        public Collection<busWssAcknowledgement> iclbDefCompAcknowledgementNew { get; set; }
        public Collection<cdoWssAcknowledgement> LoadAcknowledgement()
        {
            iclbDefCompAcknowledgement = new Collection<cdoWssAcknowledgement>();
            DataTable ldtbListWSSAcknowledgement = Select("cdoWssAcknowledgement.SelectAck", new object[2] { DateTime.Now, busConstant.DeferredCompCheck });
            iclbDefCompAcknowledgement = cdoWssAcknowledgement.GetCollection<cdoWssAcknowledgement>(ldtbListWSSAcknowledgement);
            /* Load Bus object for Grid */
            iclbDefCompAcknowledgementNew = new Collection<busWssAcknowledgement>();
            iclbDefCompAcknowledgementNew = GetCollection<busWssAcknowledgement>(ldtbListWSSAcknowledgement);
            return iclbDefCompAcknowledgement;
        }

        //PIR 6961
        public string istrConfirmationText
        {
            get
            {
                string luserName = ibusPersonAccountDeferredComp.ibusPerson.icdoPerson.FullName;
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


        /// <summary>
        /// this method is used for Member portal 
        /// please do not make changes here
        /// </summary>
        public void LoadMSSAllProviderDetials()
        {
            foreach (busPersonAccountDeferredCompProvider lobjProvider in ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider)
            {
                if (lobjProvider.ibusProviderAgentOrgContact.IsNull())
                    lobjProvider.LoadProviderAgentOrgContact();
                if (lobjProvider.ibusProviderAgentOrgContact.ibusContact.IsNull())
                    lobjProvider.ibusProviderAgentOrgContact.LoadContact();
                if (lobjProvider.ibusPersonEmployment.IsNull())
                    lobjProvider.LoadPersonEmployment();
                if (lobjProvider.ibusPersonEmployment.ibusOrganization.IsNull())
                    lobjProvider.ibusPersonEmployment.LoadOrganization();
                if (lobjProvider.ibusProviderOrgPlan.IsNull())
                    lobjProvider.LoadProviderOrgPlan();
                if (lobjProvider.ibusProviderOrgPlan.ibusOrganization.IsNull())
                    lobjProvider.ibusProviderOrgPlan.LoadOrganization();
                if (lobjProvider.ibusPersonAccountDeferredComp == null)
                    lobjProvider.LoadPersonAccountDeferredComp();
                if (lobjProvider.ibusPersonAccountDeferredComp.icdoPersonAccount.person_employment_dtl_id <= 0)
                    lobjProvider.ibusPersonAccountDeferredComp.icdoPersonAccount.person_employment_dtl_id = lobjProvider.ibusPersonAccountDeferredComp.GetEmploymentDetailID();
                if (lobjProvider.ibusPersonAccountDeferredComp.ibusOrgPlan == null)
                    lobjProvider.ibusPersonAccountDeferredComp.LoadOrgPlan();
                lobjProvider.icdoPersonAccountDeferredCompProvider.istrProviderOrgCode =
                lobjProvider.ibusProviderOrgPlan.ibusOrganization.icdoOrganization.org_code;
            }
        }

        // BR-093-09 - Change Plan participation status to Withdrawn if all the Provider's payee status is Closed.
        private bool IsAllProviderPaymentStatusClosed()
        {
            // Provider Collection needs to load everytime since the Provider Count vary if new provider is entered
            if (ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider == null)
                ibusPersonAccountDeferredComp.LoadPersonAccountProviders();

            var lvar = from p in ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider
                       where (p.icdoPersonAccountDeferredCompProvider.payment_status_value == busConstant.DeferredPaymentStatusClosed)
                       select p;

            if (lvar.Count() > 0)
            {
                if (lvar.Count() == ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider.Count)
                    return true;
            }
            return false;
        }

        private void UpdateAllRecordsIfProviderAssetFlagChange()
        {
            if (iclbPersonAccountDeferredCompProviderByProvider == null)
            {
                LoadAllDeferredCompProviderByProvider();
            }
            foreach (busPersonAccountDeferredCompProvider lobjProvider in iclbPersonAccountDeferredCompProviderByProvider)
            {
                if (lobjProvider.icdoPersonAccountDeferredCompProvider.person_account_provider_id
                    != icdoPersonAccountDeferredCompProvider.person_account_provider_id)
                {
                    lobjProvider.icdoPersonAccountDeferredCompProvider.assets_with_provider_value = icdoPersonAccountDeferredCompProvider.assets_with_provider_value;
                    lobjProvider.icdoPersonAccountDeferredCompProvider.Update();
                }
            }
        }

        public string istrProviderContact { get; set; }

        public void SetProviderContact()
        {
            if (ibusProviderAgentOrgContact == null)
                LoadProviderAgentOrgContact();
            if (ibusProviderAgentOrgContact.ibusContact == null)
                ibusProviderAgentOrgContact.LoadContact();

            ibusProviderAgentOrgContact.ibusContact.LoadContactAddressForDeferredCompAgent(ibusProviderAgentOrgContact.ibusContact.icdoContact.contact_id);

            istrProviderContact += ibusProviderAgentOrgContact.ibusContact.icdoContact.ContactName + "\r\n";
            istrProviderContact += icdoPersonAccountDeferredCompProvider.company_name + "\r\n";
            if (ibusProviderAgentOrgContact.ibusContact.ibusContactPrimaryAddress.icdoOrgContactAddress != null)
            {
                istrProviderContact += ibusProviderAgentOrgContact.ibusContact.ibusContactPrimaryAddress.addr_description;
            }
        }

        #region Approaching 457 Contribution Limit Correspondence

        /// Total Contribution per Provider
        private decimal _ldclContribution;
        public decimal ldclContribution
        {
            get { return _ldclContribution; }
            set { _ldclContribution = value; }
        }

        #endregion


        # region UCS 022 Cor

        //ENR-5057
        //{plan name} – Insert ‘North Dakota 457 Deferred Compensation Plan number 64579’ if plan is Def Comp or ‘NDPERS Defined Contribution Plan number 57637’ if plan is DC. 
        public string istrPlanName
        {
            get
            {
                if (ibusPersonAccountDeferredComp.icdoPersonAccount.plan_id == busConstant.PlanIdDeferredCompensation)
                    return "North Dakota 457 Deferred Compensation Plan number 64579";
                else if (ibusPersonAccountDeferredComp.icdoPersonAccount.plan_id == busConstant.PlanIdOther457)
                    return "NDPERS Defined Contribution Plan number 57637";
                return string.Empty;
            }
        }


        //PER-0304
        public override void LoadCorresProperties(string astrTemplateName)
        {
            if (ibusProviderOrganization.IsNull())
                LoadProviderOrganization();
            if (ibusProviderAgentOrgContact.IsNull())
                LoadProviderAgentOrgContact();
            if (ibusProviderOrganization.ibusOrgPrimaryAddress.IsNull())
                ibusProviderOrganization.LoadOrgContactPrimaryAddressByContact(ibusProviderAgentOrgContact.icdoOrgContact.contact_id);

            // UAT PIR ID 2154
            if (ibusProviderAgentOrgContact.ibusContact.IsNull()) ibusProviderAgentOrgContact.LoadContact();
            if (ibusProviderAgentOrgContact.ibusContact.ibusContactPrimaryAddress.IsNull())
                ibusProviderAgentOrgContact.ibusContact.LoadContactAddressForDeferredCompAgent(ibusProviderAgentOrgContact.ibusContact.icdoContact.contact_id);

            if (ibusPersonAccountDeferredComp.IsNull())
                LoadPersonAccountDeferredComp();
            if (ibusPersonAccountDeferredComp.ibusPlan.IsNull())
                ibusPersonAccountDeferredComp.LoadPlan();
        }

        public override busBase GetCorPerson()
        {
            if (ibusPersonAccountDeferredComp.IsNull())
                LoadPersonAccountDeferredComp();
            if (ibusPersonAccountDeferredComp.ibusPerson.IsNull())
                ibusPersonAccountDeferredComp.LoadPerson();
            return ibusPersonAccountDeferredComp.ibusPerson;
        }
        public override busBase GetCorOrganization()
        {
			//PIR 25069
            if (iobjPassInfo.IsNotNull() && iobjPassInfo.idictParams.Count > 0 && (iobjPassInfo.idictParams.ContainsKey("TemplateName")) && Convert.ToString(iobjPassInfo.idictParams["TemplateName"]) == "PER-0358")
            {
                if (ibusPersonEmployment.IsNull())
                    LoadPersonEmployment();
                if (ibusPersonEmployment.ibusOrganization.IsNull())
                    ibusPersonEmployment.LoadOrganization();
                if (ibusPersonEmployment.ibusOrganization.ibusESSPrimaryOrgContact.IsNull())
                    ibusPersonEmployment.ibusOrganization.LoadESSPrimaryAuthorizedContact();
                ibusPersonEmployment.ibusOrganization.ibusOrgContact = ibusPersonEmployment.ibusOrganization.ibusESSPrimaryOrgContact;
                return ibusPersonEmployment.ibusOrganization;
            }
            else
            {
                if (ibusProviderOrganization.IsNull())
                    LoadProviderOrganization();
                if (ibusProviderAgentOrgContact.IsNull()) //UAT PIR - 2091 - cor PER-0305
                    LoadProviderAgentOrgContact();
                ibusProviderOrganization.ibusOrgContact = ibusProviderAgentOrgContact;
                return ibusProviderOrganization;
            }
        }
        # endregion

        # region UCS 24

        public string istrAuthorizationFlagChecked { get; set; }
        public bool iblnIsFromPortal { get; set; }
        public bool iblnIsUpdate { get; set; } // PIR 9692
        public bool iblnIsAddNewProvider { get; set; } // PIR 9692
        //prod pir 7176,7177,7178
        public bool iblnFromESS { get; set; }

        //public Collection<busPersonAccountDeferredCompProvider> iclbAllDeferredCompProviders { get; set; }
        //public void LoadAllDeferredCompProviders()
        //{
        //    DataTable ldtbList = Select<cdoPersonAccountDeferredCompProvider>(
        //        new string[1] { "person_account_id" },
        //        new object[1] { icdoPersonAccountDeferredCompProvider.person_account_id }, null, "start_date desc");
        //    iclbAllDeferredCompProviders = GetCollection<busPersonAccountDeferredCompProvider>(ldtbList, "icdoPersonAccountDeferredCompProvider");
        //}

        //This method called when we click Hand Icon. That will reload the object and display the details
        public ArrayList ReloadHandButtonData()
        {
            if (ibusProviderAgentOrgContact.IsNotNull() && ibusProviderAgentOrgContact.ibusContact.IsNotNull())
                ibusProviderAgentOrgContact.ibusContact.icdoContact = new cdoContact();
            ArrayList larrList = new ArrayList();
            if (iintMSSContactId != 0)
            {
                icdoPersonAccountDeferredCompProvider.provider_agent_contact_id = GetProviderOrgContactId();
                LoadProviderAgentOrgContact();
                ibusProviderAgentOrgContact.LoadContact();
            }
            larrList.Add(this);
            return larrList;
        }

        public int iintMSSContactId { get; set; }
        //load provider org contact id
        public int GetProviderOrgContactId()
        {
            int lintResult = 0;

            if (!String.IsNullOrEmpty(icdoPersonAccountDeferredCompProvider.istrProviderOrgCode))
            {
                ibusProviderOrganization = new busOrganization();
                ibusProviderOrganization.FindOrganizationByOrgCode(icdoPersonAccountDeferredCompProvider.istrProviderOrgCode);
                ibusProviderOrganization.LoadOrgContactByContactID(iintMSSContactId);

                var lDefCompOrgContact = ibusProviderOrganization.iclbOrgContact.Where(lobjOC => lobjOC.icdoOrgContact.status_value == busConstant.StatusActive);

                if (lDefCompOrgContact.Count() > 0)
                {
                    return lDefCompOrgContact.FirstOrDefault().icdoOrgContact.org_contact_id;
                }
            }
            return lintResult;
        }

        #endregion


        //UAT PIR 2220
        public void PostESSMessage()
        {
            string lstrPrioityValue = string.Empty;
            if (ibusPersonAccountDeferredComp == null)
                LoadPersonAccountDeferredComp();

            if (ibusPersonAccountDeferredComp.icdoPersonAccount.person_employment_dtl_id == 0)
            {
                ibusPersonAccountDeferredComp.icdoPersonAccount.plan_id = ibusPersonAccountDeferredComp.icdoPersonAccount.plan_id == 0 ?
                    busConstant.PlanIdDeferredCompensation : ibusPersonAccountDeferredComp.icdoPersonAccount.plan_id;
                ibusPersonAccountDeferredComp.icdoPersonAccount.person_account_id = ibusPersonAccountDeferredComp.icdoPersonAccount.person_account_id == 0 ?
                    ibusPersonAccountDeferredComp.icdoPersonAccountDeferredComp.person_account_id : ibusPersonAccountDeferredComp.icdoPersonAccount.person_account_id;
                ibusPersonAccountDeferredComp.icdoPersonAccount.person_employment_dtl_id =
                    ibusPersonAccountDeferredComp.GetEmploymentDetailID(busConstant.PlanIdDeferredCompensation, ibusPersonAccountDeferredComp.icdoPersonAccountDeferredComp.person_account_id);
            }

            //pir 20702 : commented unncessary code
            //if (ibusPersonAccountDeferredComp.ibusPersonEmploymentDetail.IsNull())
            //    ibusPersonAccountDeferredComp.LoadPersonEmploymentDetail();

            //if (ibusPersonAccountDeferredComp.ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
            //    ibusPersonAccountDeferredComp.ibusPersonEmploymentDetail.LoadPersonEmployment();


            if (ibusPersonAccountDeferredComp.ibusPerson.IsNull())
                ibusPersonAccountDeferredComp.LoadPerson();

            //PIR 14335 -- added or codition i.e. plan_participation_status_value = 'SPN1'
            if (ibusPersonAccountDeferredComp.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompEnrolled
             || ibusPersonAccountDeferredComp.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompSuspended)
            {
                if (ibusProviderOrgPlan == null)
                    LoadProviderOrgPlan();
                if (ibusProviderOrgPlan.ibusOrganization == null)
                    ibusProviderOrgPlan.LoadOrganization();
                if (iblnIsNewMode)
                {
                    // PIR 9115
                    if (istrIsPIR9115Enabled == busConstant.Flag_Yes)
                    {
                        //PIR 20702: corrected org id
                        busGlobalFunctions.PostESSMessage(icdoPersonAccountDeferredCompProvider.org_id,
                                                            busConstant.PlanIdDeferredCompensation, iobjPassInfo);
                    }
                    else
                    {
                        string lstrMessage = string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(19, iobjPassInfo, ref lstrPrioityValue),
                            ibusPersonAccountDeferredComp.ibusPerson.icdoPerson.FullName, ibusPersonAccountDeferredComp.ibusPerson.icdoPerson.LastFourDigitsOfSSN,
                            ibusProviderOrgPlan.ibusOrganization.icdoOrganization.org_name, icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt,
                            icdoPersonAccountDeferredCompProvider.start_date.ToString("d", CultureInfo.CreateSpecificCulture("en-US")));
                        if (icdoPersonAccountDeferredCompProvider.org_id > 0)//PIR 20702: corrected org id
                        {
                            busWSSHelper.PublishESSMessage(0, 0, lstrMessage, lstrPrioityValue, aintPlanID: busConstant.PlanIdDeferredCompensation,
                                                 aintOrgID: icdoPersonAccountDeferredCompProvider.org_id,
                                                astrContactRoleValue: busConstant.OrgContactRoleAuthorizedAgent);
                        }
                    }
                }
                else if (icdoPersonAccountDeferredCompProvider.end_date != DateTime.MinValue && !iblnIsNewMode)
                {
                    // PIR 9115
                    if (istrIsPIR9115Enabled == busConstant.Flag_Yes)
                    {
                        busGlobalFunctions.PostESSMessage(icdoPersonAccountDeferredCompProvider.org_id,
                                                             busConstant.PlanIdDeferredCompensation, iobjPassInfo);
                    }
                    else
                    {
                        string lstrMessage = string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(20, iobjPassInfo, ref lstrPrioityValue),
                            ibusPersonAccountDeferredComp.ibusPerson.icdoPerson.FullName, ibusPersonAccountDeferredComp.ibusPerson.icdoPerson.LastFourDigitsOfSSN,
                                               ibusProviderOrgPlan.ibusOrganization.icdoOrganization.org_name, icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt,
                                               icdoPersonAccountDeferredCompProvider.end_date.ToString("d", CultureInfo.CreateSpecificCulture("en-US")));
                        if (icdoPersonAccountDeferredCompProvider.org_id > 0) //PIR 20702: corrected org id
                        {
                            busWSSHelper.PublishESSMessage(0, 0, lstrMessage, lstrPrioityValue, aintPlanID: busConstant.PlanIdDeferredCompensation,
                                                aintOrgID: icdoPersonAccountDeferredCompProvider.org_id,
                                                astrContactRoleValue: busConstant.OrgContactRoleAuthorizedAgent);
                        }
                    }
                }
            }
        }

        //UAT PIR - 2382
        private void UpdatePlanStatusToSuspend()
        {
            //always reload
            ibusPersonAccountDeferredComp.LoadPersonAccountProviders();

            if (ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider.IsNotNull())
            {
                var lOpenProviderRecordCount = ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider.Where(lobjPAD => lobjPAD.icdoPersonAccountDeferredCompProvider.end_date == DateTime.MinValue).Count();

                if (lOpenProviderRecordCount == 0)
                {
                    //If the current Employment Detail is also end dated only, suspend the plan
                    if (ibusPersonAccountDeferredComp.icdoPersonAccount.person_employment_dtl_id == 0)
                    {
                        ibusPersonAccountDeferredComp.icdoPersonAccount.plan_id = ibusPersonAccountDeferredComp.icdoPersonAccount.plan_id == 0 ?
                            busConstant.PlanIdDeferredCompensation : ibusPersonAccountDeferredComp.icdoPersonAccount.plan_id;
                        ibusPersonAccountDeferredComp.icdoPersonAccount.person_account_id = ibusPersonAccountDeferredComp.icdoPersonAccount.person_account_id == 0 ?
                            ibusPersonAccountDeferredComp.icdoPersonAccountDeferredComp.person_account_id : ibusPersonAccountDeferredComp.icdoPersonAccount.person_account_id;
                        ibusPersonAccountDeferredComp.icdoPersonAccount.person_employment_dtl_id =
                            ibusPersonAccountDeferredComp.GetEmploymentDetailID(busConstant.PlanIdDeferredCompensation, ibusPersonAccountDeferredComp.icdoPersonAccountDeferredComp.person_account_id);
                    }

                    if (ibusPersonAccountDeferredComp.ibusPersonEmploymentDetail.IsNull())
                        ibusPersonAccountDeferredComp.LoadPersonEmploymentDetail();

                    if ((ibusPersonAccountDeferredComp.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusDefCompSuspended)
                        && (ibusPersonAccountDeferredComp.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date != DateTime.MinValue))
                    {
                        ibusPersonAccountDeferredComp.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusDefCompSuspended;
                        ibusPersonAccountDeferredComp.icdoPersonAccountDeferredComp.ienuObjectState = ObjectState.Update;
                        ibusPersonAccountDeferredComp.BeforeValidate(utlPageMode.Update);
                        ibusPersonAccountDeferredComp.BeforePersistChanges();
                        ibusPersonAccountDeferredComp.PersistChanges();
                        ibusPersonAccountDeferredComp.AfterPersistChanges();
                    }
                }
            }
        }

        public Collection<busPersonAccountDeferredCompProvider> iclbDefCompProviders { get; set; }

        public void LoadDeferredCompProviderByProvider(bool ablnLoadOtherObjects = false)
        {
            if (iclbDefCompProviders.IsNull()) iclbDefCompProviders = new Collection<busPersonAccountDeferredCompProvider>();
            DataTable ldtbList = Select<cdoPersonAccountDeferredCompProvider>(
                new string[1] { "person_account_id" },
                new object[1] { icdoPersonAccountDeferredCompProvider.person_account_id }, null, "start_date desc");
            iclbDefCompProviders = GetCollection<busPersonAccountDeferredCompProvider>(ldtbList, "icdoPersonAccountDeferredCompProvider");
            if (ablnLoadOtherObjects)
            {
                foreach (busPersonAccountDeferredCompProvider lobjProvider in iclbDefCompProviders)
                {
                    if (lobjProvider.ibusProviderOrgPlan.IsNull())
                        lobjProvider.LoadProviderOrgPlan();
                    if (lobjProvider.ibusProviderOrgPlan.ibusOrganization.IsNull())
                        lobjProvider.ibusProviderOrgPlan.LoadOrganization();
                }
            }
        }

        public bool IsOverrideMutualWindowFlagVisible()
        {
            if (iclbDefCompProviders.IsNull())
                LoadDeferredCompProviderByProvider(true);
            if (iclbDefCompProviders.Count > 0 &&
                iclbDefCompProviders[0].icdoPersonAccountDeferredCompProvider.mutual_fund_window_flag == busConstant.Flag_Yes &&
                iclbDefCompProviders[0].ibusProviderOrgPlan.ibusOrganization.icdoOrganization.org_code == busConstant.NDPERSCompanionPlanOrgCode)
                return true;
            return false;
        }

        public string istrOverrideMutualWindowFlag { get; set; }

        //pir 6321 : need to set start date based on BR-24-40
        public void SetStartDate()
        {
            if (ibusPersonAccountDeferredComp != null && ibusPersonAccountDeferredComp.ibusOrgPlan != null)
            {
                DateTime ldtStartDate = DateTime.Today > ibusPersonAccountDeferredComp.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date ?
                    DateTime.Today : ibusPersonAccountDeferredComp.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date;
                //Commented for PIR 11512
                //Commented for PIR 11512
                //if (ibusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencyMonthly)
                //{
                //    icdoPersonAccountDeferredCompProvider.start_date = ldtStartDate.Day <= 15 ?
                //        ldtStartDate.GetFirstDayofNextMonth() : ldtStartDate.AddMonths(1).GetFirstDayofNextMonth();
                //}
                //else if (ibusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencySemiMonthly ||
                //    ibusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencyBiWeekly ||
                //    ibusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencyWeekly)
                //{
                //    icdoPersonAccountDeferredCompProvider.start_date = ldtStartDate.Day <= 15 ?
                //        ldtStartDate.GetFirstDayofNextMonth() : ldtStartDate.GetFirstDayofNextMonth().AddDays(15);
                //}


                if (ibusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.day_of_month > 1)
                    icdoPersonAccountDeferredCompProvider.start_date = ldtStartDate.GetFirstDayofNextMonth().AddDays(ibusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.day_of_month); //PIR 14377
                else
                    icdoPersonAccountDeferredCompProvider.start_date = ldtStartDate.GetFirstDayofNextMonth();// PIR 11512
                idtStartDate = icdoPersonAccountDeferredCompProvider.start_date; // PIR 11045

                //PIR 14377
                if (ibusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencyBiWeekly ||
                    ibusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencyWeekly ||
                    ibusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencyMonthly ||
                    ibusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencySemiMonthly)
                {
                    icdoPersonAccountDeferredCompProvider.start_date = LoadEPHPayNextPeriodStartDate(ibusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.org_id, 
                                                                                                ibusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.report_frequency_value,
                                                                                                ibusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.day_of_month);
                    if(ldtStartDate != DateTime.MinValue)
                        idtStartDate = icdoPersonAccountDeferredCompProvider.start_date;
                }
            }
        }
        //PIR 14377
        public DateTime LoadEPHPayNextPeriodStartDate(int aintOrgId, string astrFrequency, int aintDayOfMonth)
        {
            if (aintDayOfMonth == 0)
                aintDayOfMonth = 1;
            DateTime ldtStartDate = DateTime.MinValue;
            DateTime ldtLastPayrollEndDate = DateTime.MinValue;
            DateTime ldtLastPayrollStartDate = DateTime.MinValue;
            busEmployerPayrollHeader lbusEmployerPayrollHeader = new busEmployerPayrollHeader { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader() };
            DateTime ldtSystemStartDate = busGlobalFunctions.GetSysManagementBatchDate();
            DateTime ldtDefalutStartDate = ldtSystemStartDate;//.GetFirstDayofNextMonth();
            
            //checking employer header dates 
            DataTable ldtbList = Select<cdoEmployerPayrollHeader>(new string[3] { "ORG_ID", "HEADER_TYPE_VALUE", "STATUS_VALUE" },
                                 new object[3] { aintOrgId, busConstant.PayrollHeaderBenefitTypeDefComp,
                                 busConstant.PayrollHeaderStatusPosted }, null, "PAY_PERIOD_END_DATE desc");
            if (ldtbList.IsNotNull() && ldtbList.Rows.Count > 0)
            {
                lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.LoadData(ldtbList.Rows[0]);
                if (lbusEmployerPayrollHeader.IsNotNull() && lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period_end_date != DateTime.MinValue)
                {
                    //PIR 14377 rework - if the pay period end date is less than system date (batch date) then provider default date should be system date. 
                    ldtLastPayrollEndDate = lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period_end_date;
                    ldtLastPayrollStartDate = lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period_start_date;
                    //if(astrFrequency == busConstant.DeffCompFrequencyBiWeekly)
                    //      ldtLastPayrollEndDate = ldtLastPayrollEndDate.AddDays(1);
                    //set defalut start date to payroll end date if system date is less than payroll start date
                    if (ldtLastPayrollEndDate != DateTime.MinValue)
                        ldtDefalutStartDate = ldtDefalutStartDate > ldtLastPayrollEndDate ?
                        ldtDefalutStartDate : ldtLastPayrollEndDate;
                    do
                    {
                        switch (astrFrequency)
                        {
                            case busConstant.DeffCompFrequencyMonthly:
                                if (aintDayOfMonth == 1)
                                    ldtLastPayrollEndDate = ldtLastPayrollEndDate.AddMonths(1).GetLastDayofMonth();
                                else
                                    ldtLastPayrollEndDate = ldtLastPayrollEndDate.AddMonths(1);
                                break;
                            case busConstant.DeffCompFrequencySemiMonthly:
                                if (ldtLastPayrollEndDate.Day < 16)
                                {
                                    ldtLastPayrollStartDate = ldtLastPayrollEndDate.AddDays(1);
                                    ldtLastPayrollEndDate = ldtLastPayrollEndDate.AddDays(15);
                                }
                                else
                                {
                                    ldtLastPayrollEndDate = ldtLastPayrollStartDate.AddMonths(1).AddDays(-1);
                                    ldtLastPayrollStartDate = ldtLastPayrollStartDate.AddDays(15);
                                }
                                break;
                            case busConstant.DeffCompFrequencyBiWeekly:
                                ldtLastPayrollEndDate = ldtLastPayrollEndDate.AddDays(14);
                                break;
                            case busConstant.DeffCompFrequencyWeekly:
                                ldtLastPayrollEndDate = ldtLastPayrollEndDate.AddDays(7);
                                break;
                        }
                    } while (ldtLastPayrollEndDate < ldtDefalutStartDate);
                    ldtDefalutStartDate = ldtLastPayrollEndDate;
                }
            }
            if (ldtDefalutStartDate == DateTime.MinValue) ldtDefalutStartDate = ldtSystemStartDate;
            if (ldtLastPayrollEndDate == DateTime.MinValue) ldtLastPayrollEndDate = ldtDefalutStartDate;

            

            if (ldtDefalutStartDate != DateTime.MinValue)
                {
                    switch (astrFrequency)
                    {
                        case busConstant.DeffCompFrequencyMonthly:
                            //‘Day Of Month’ (NULL = 1) + 1 month would be the first default date.  
                            //I.e. User enters new Def Comp amount on 4/28, default Start Date to 5/1 (If ‘Day Of Month’ is set to 11 it should be 5/11). 
                            //The user can only enter records effective on the ‘Day Of Month’ for any month in the future.

                            if (aintDayOfMonth == 1)
                            {
                                ldtStartDate = ldtDefalutStartDate.AddMonths(1);
                                ldtStartDate = ldtStartDate.GetFirstDayofCurrentMonth();
                            }
                            else
                            {
                                //if (DateTime.Now.Day > ldtDefalutStartDate.Day)
                                //{
                                    ldtStartDate = new DateTime(ldtDefalutStartDate.Year, ldtDefalutStartDate.Month, aintDayOfMonth);
                                    ldtStartDate = ldtStartDate.AddMonths(1);
                                //}
                                //else
                                //{
                                //    ldtStartDate = new DateTime(ldtDefalutStartDate.Year, ldtDefalutStartDate.Month, aintDayOfMonth);
                                //    ldtStartDate = ldtStartDate.AddMonths(2);
                                //}
                            }
                            break;

                        case busConstant.DeffCompFrequencySemiMonthly:
                            //‘Day Of Month’ (NULL = 1) + 1 month would be the first default date (Typically, pay periods go from 1-15 and 16-30/31 but can vary based on ‘Day of Month’ like Org 400163).
                            //I.e. User enters new Def Comp amount on 5/2, default Start Date to 6/1 (If ‘Day Of Month’ is set to 11 it should be 6/11, 6/25, 7/11, etc.)

                            if (aintDayOfMonth == 1)
                            {
                                //if (DateTime.Now.Day > ldtDefalutStartDate.Day)
                                //{
                                    ldtStartDate = ldtDefalutStartDate.AddMonths(1);
                                    ldtStartDate = ldtStartDate.GetFirstDayofCurrentMonth();
                                //}
                                //else
                                //{
                                //    ldtStartDate = ldtDefalutStartDate.AddMonths(2);
                                //    ldtStartDate = ldtStartDate.GetFirstDayofCurrentMonth();
                                //}
                            }
                            else
                            {
                                //if (DateTime.Now.Day > ldtDefalutStartDate.Day)
                                //{
                                    ldtStartDate = new DateTime(ldtDefalutStartDate.Year, ldtDefalutStartDate.Month, aintDayOfMonth);
                                    ldtStartDate = ldtStartDate.AddMonths(1);
                                //}
                                //else
                                //{
                                //    ldtStartDate = new DateTime(ldtDefalutStartDate.Year, ldtDefalutStartDate.Month, aintDayOfMonth);
                                //    ldtStartDate = ldtStartDate.AddMonths(2);
                                //}
                            }

                            break;

                        case busConstant.DeffCompFrequencyWeekly:
                        //logic is the same as Bi-Weekly with 7 day periods instead of 14.
                        //skipping multiple weeks
                        //eg. User makes election on 6/6 and prior pay period end date 6/2/23 now default start date will be 7/1
                            int lintCountIterration = 1;
                            while (ldtDefalutStartDate.Month == ldtDefalutStartDate.AddDays(7 * lintCountIterration).Month)
                            {
                                ldtStartDate = ldtDefalutStartDate.AddDays(7 * lintCountIterration);
                                lintCountIterration++;
                            }
                            if (ldtStartDate.Month == ldtDefalutStartDate.Month)
                                ldtStartDate = ldtStartDate.AddDays(7);
                        ldtStartDate = ldtStartDate.AddDays(1);
                        break;
                        case busConstant.DeffCompFrequencyBiWeekly:
                            //14 day periods so we’ll have to count in 14 day increments.
                            //I.e. User enters new Def Comp amount on 4/28, and prior report ended 4/16 default Start Date to 5/1 (future dates should be 5/15, 5/29, 6/12, etc.).
                            //If user makes election on 5/15 the default would be 6/12.

                            if (ldtDefalutStartDate.Month == ldtDefalutStartDate.AddDays(15).Month)
                                ldtStartDate = ldtDefalutStartDate.AddDays(29);
                            else
                                ldtStartDate = ldtDefalutStartDate.AddDays(15);
                            break;
                    }
                }
            
            return ldtStartDate;
        }

        //pir 6322 : validation to check deduction start date same as employment start date
        public bool IsDeductionStartDateSameAsEmploymentStartDate()
        {
            bool lblResult = false;

            if (ibusPersonEmployment != null && ibusPersonEmployment.icdoPersonEmployment != null)
            {
                if (icdoPersonAccountDeferredCompProvider.start_date != DateTime.MinValue &&
                    icdoPersonAccountDeferredCompProvider.start_date <= ibusPersonEmployment.icdoPersonEmployment.start_date)
                    lblResult = true;
            }

            return lblResult;
        }

        public int iintStartMonth { get; set; }

        public string istrStartYear { get; set; }

        public string istrIsPopupValidation { get; set; }

        public string istrUpdateExistingProviderFlag { get; set; }
       
        public string istrIsApplyEmployerMatchingContribution { get; set; }

        public bool IsNoChangesMade()
        {
            if (iblnIsFromPortal && icdoPersonAccountDeferredCompProvider.person_account_provider_id != 0)
            {
                if (icdoPersonAccountDeferredCompProvider.ihstOldValues.Count > 0 &&
                   icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt ==
                   Convert.ToDecimal(icdoPersonAccountDeferredCompProvider.ihstOldValues["per_pay_period_contribution_amt"]))
                    return true;
            }
            return false;
        }

        public bool IsSameProviderSelected()
        {
            if (iblnIsFromPortal)
            {
                if (ibusPersonAccountDeferredComp.IsNotNull())
                    LoadPersonAccountDeferredComp();
                if (ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider.IsNull())
                    ibusPersonAccountDeferredComp.LoadPersonAccountProviders();
                foreach (busPersonAccountDeferredCompProvider lobjProvider in ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider)
                {
                    if (lobjProvider.icdoPersonAccountDeferredCompProvider.end_date == DateTime.MinValue &&
                        lobjProvider.icdoPersonAccountDeferredCompProvider.start_date != DateTime.MinValue &&
                        icdoPersonAccountDeferredCompProvider.person_account_provider_id == 0)
                    {
                        if (lobjProvider.icdoPersonAccountDeferredCompProvider.provider_org_plan_id == icdoPersonAccountDeferredCompProvider.provider_org_plan_id &&
                            istrUpdateExistingProviderFlag == busConstant.Flag_No)
                            return true;
                    }
                }
            }
            return false;
        }

        //PIR 9692
        public ArrayList UpdateDefCompProvider(int aintProviderID, DateTime adteStartDate, decimal adecPayPeriodAmount)
        {
            ArrayList larrResult = new ArrayList();
            // End Existing Provider
            busPersonAccountDeferredCompProvider lobjCurrentProvider = new busPersonAccountDeferredCompProvider { icdoPersonAccountDeferredCompProvider = new cdoPersonAccountDeferredCompProvider() };
            lobjCurrentProvider.FindPersonAccountDeferredCompProvider(aintProviderID);
            if (adteStartDate != DateTime.MinValue)
            {
                bool lblnFlag = false;
                //PIR 17081
                //lobjCurrentProvider.icdoPersonAccountDeferredCompProvider.is_enrollment_report_generated = busConstant.Flag_No; // PIR 9115
                if (lobjCurrentProvider.icdoPersonAccountDeferredCompProvider.start_date < adteStartDate)
                {
                    lobjCurrentProvider.icdoPersonAccountDeferredCompProvider.end_date = adteStartDate.AddDays(-1);
                    lblnFlag = true;

                }
                else if (lobjCurrentProvider.icdoPersonAccountDeferredCompProvider.start_date == adteStartDate)
                {
                    lobjCurrentProvider.icdoPersonAccountDeferredCompProvider.end_date = adteStartDate;
                    lblnFlag = true;

                }
                else if (ibusPersonAccountDeferredComp.icdoPersonAccount.start_date <= adteStartDate &&// PIR 10253
                    lobjCurrentProvider.icdoPersonAccountDeferredCompProvider.start_date > adteStartDate)
                {
                    lobjCurrentProvider.icdoPersonAccountDeferredCompProvider.start_date = adteStartDate;
                }
                lobjCurrentProvider.icdoPersonAccountDeferredCompProvider.Update();

                if (lblnFlag)
                {
                    // Add new Provider
                    busPersonAccountDeferredCompProvider lobjNewProvider = new busPersonAccountDeferredCompProvider { icdoPersonAccountDeferredCompProvider = new cdoPersonAccountDeferredCompProvider() };
                    lobjNewProvider = lobjCurrentProvider;
                    lobjNewProvider.icdoPersonAccountDeferredCompProvider.start_date = adteStartDate;
                    lobjNewProvider.icdoPersonAccountDeferredCompProvider.end_date = DateTime.MinValue;
                    lobjNewProvider.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt = adecPayPeriodAmount;
					//PIR 25808 defect fix of created date carry forward
                    lobjNewProvider.icdoPersonAccountDeferredCompProvider.created_date = DateTime.Now;
                    //PIR 17081
                    //lobjNewProvider.icdoPersonAccountDeferredCompProvider.is_enrollment_report_generated = busConstant.Flag_No; // PIR 9115
                    lobjNewProvider.icdoPersonAccountDeferredCompProvider.Insert();
                    iintNewProviderIdForChangeAmount = lobjNewProvider.icdoPersonAccountDeferredCompProvider.person_account_provider_id; //PIR 20702 : new provider id in change amount scenario
                    if (istrIsPIR9115Enabled == busConstant.Flag_Yes) // PIR 9115
                    {
                        DataTable ldtbEmployment = busBase.Select<cdoPersonEmployment>(new string[1] { "person_employment_id" }, new object[1] {
                                                                            lobjNewProvider.icdoPersonAccountDeferredCompProvider.person_employment_id }, null, null);


                        busGlobalFunctions.PostESSMessage(Convert.ToInt32(ldtbEmployment.Rows[0]["org_id"]), busConstant.PlanIdDeferredCompensation, iobjPassInfo);
                    }
                }
            }
            larrResult.Add("Success");
            return larrResult;
        }

        // PIR 10253
        public bool IsMSSProviderStartDateLessThanPlanStartDate()
        {
            if (iblnIsFromPortal && icdoPersonAccountDeferredCompProvider.start_date != DateTime.MinValue &&
                icdoPersonAccountDeferredCompProvider.start_date < ibusPersonAccountDeferredComp.icdoPersonAccount.start_date &&
                ibusPersonAccountDeferredComp.icdoPersonAccount.person_account_id > 0)
                return true;
            return false;
        }

        //PIR 12690
        public void LoadContributionsForPerson(DateTime adtStartDate, DateTime adtEndDate)
        {
            DataTable ldtbContribution = Select("cdoPersonAccountDeferredComp.ContrbutionSummaryByPayCheckDate",
                                               new object[3] { icdoPersonAccountDeferredCompProvider.person_account_id, adtEndDate, adtStartDate });
            iclbContributions = GetCollection<busPersonAccountDeferredCompContribution>(ldtbContribution, "icdoPersonAccountDeferredCompContribution");
        }

        public decimal CalculateContributionLimit()
        {
            decimal ldecSumOfContributionAmount = 0.00M;
            decimal ldecPerPayPeriodamount = 0.00M;
            decimal ldecSumOfPerPayPeriodamount = 0.00M;
            decimal ldecTotalAmount = 0.00M;
            DateTime ldtDateCriteria = DateTime.MinValue;

            DateTime ldtStartDate = DateTime.Now;
            DateTime ldtContributionStartDate = new DateTime(ldtStartDate.Year, 1, 1);
            DateTime ldtContributionEndDate = new DateTime(ldtStartDate.Year, ldtStartDate.Month, 1);

            if ((icdoPersonAccountDeferredCompProvider.ienuObjectState == ObjectState.Update) && (icdoPersonAccountDeferredCompProvider.end_date == DateTime.MinValue))
                ldtDateCriteria = DateTime.Now;
            else if ((icdoPersonAccountDeferredCompProvider.ienuObjectState == ObjectState.Update) && (icdoPersonAccountDeferredCompProvider.end_date != DateTime.MinValue))
                ldtDateCriteria = icdoPersonAccountDeferredCompProvider.end_date;
            else
                ldtDateCriteria = icdoPersonAccountDeferredCompProvider.start_date;

            bool lblContributionForCurrentMonthIsPosted = false;

            ibusPersonAccountDeferredComp.icdoPersonAccount = new cdoPersonAccount();
            ibusPersonAccountDeferredComp.LoadPersonAccount();
            LoadPersonAccountDeferredComp();
            ibusPersonAccountDeferredComp.Set457Limit();
            ibusPersonAccountDeferredComp.LoadActivePersonAccountProviders();
            LoadContributionsForPerson(ldtContributionStartDate, ldtContributionEndDate);

            foreach (busPersonAccountDeferredCompContribution lobjPersonAccountDeferredCompContribution in iclbContributions)
            {
                if ((lobjPersonAccountDeferredCompContribution.icdoPersonAccountDeferredCompContribution.paid_date.Month == ldtStartDate.Month) &&
                        (lobjPersonAccountDeferredCompContribution.icdoPersonAccountDeferredCompContribution.paid_date.Year == ldtStartDate.Year) &&
                        (lobjPersonAccountDeferredCompContribution.icdoPersonAccountDeferredCompContribution.transaction_type_value == busConstant.TransactionTypeRegularPayroll))
                {
                    lblContributionForCurrentMonthIsPosted = true;
                }
                if (ldtDateCriteria.Month >= lobjPersonAccountDeferredCompContribution.icdoPersonAccountDeferredCompContribution.paid_date.Month)
                    ldecSumOfContributionAmount += lobjPersonAccountDeferredCompContribution.icdoPersonAccountDeferredCompContribution.pay_period_contribution_amount;
            }

            if (!lblContributionForCurrentMonthIsPosted)
                ldtStartDate = DateTime.Now.GetFirstDayofCurrentMonth();
            else
                ldtStartDate = DateTime.Now.GetFirstDayofNextMonth();
            //For Single open provider
            if (ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider.Count == 1 && icdoPersonAccountDeferredCompProvider.end_date != DateTime.MinValue)
            {
                foreach (busPersonAccountDeferredCompProvider lobjPersonAccountDefCompProvider in ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider)
                {
                    while ((icdoPersonAccountDeferredCompProvider.end_date.Year == DateTime.Today.Year) && (ldtStartDate.Month <= icdoPersonAccountDeferredCompProvider.end_date.Month)
                        && ldtStartDate.Year <= icdoPersonAccountDeferredCompProvider.end_date.Year)
                    {
                        if (busGlobalFunctions.CheckDateOverlapping(ldtStartDate,
                                                                    lobjPersonAccountDefCompProvider.icdoPersonAccountDeferredCompProvider.start_date,
                                                                    lobjPersonAccountDefCompProvider.icdoPersonAccountDeferredCompProvider.end_date))
                        {
                            if ((ibusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencyBiWeekly) ||
                                (ibusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencySemiMonthly))
                            {
                                ldecPerPayPeriodamount =
                                    lobjPersonAccountDefCompProvider.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt * 2;
                            }
                            else if ((ibusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencyWeekly))
                            {
                                ldecPerPayPeriodamount =
                                    lobjPersonAccountDefCompProvider.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt * 4;
                            }
                            else
                            {
                                ldecPerPayPeriodamount =
                                    lobjPersonAccountDefCompProvider.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt;
                            }
                            ldecSumOfPerPayPeriodamount += ldecPerPayPeriodamount;

                        }
                        ldtStartDate = ldtStartDate.AddMonths(1);
                    }
                }
            }
            else if (ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider.Count > 1)
            {
                foreach (busPersonAccountDeferredCompProvider lobjPersonAccountDefComp in ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider)
                {

                    while ((ldtStartDate.Year == DateTime.Today.Year) && (ldtStartDate.Month <= 12))
                    {
                        if (busGlobalFunctions.CheckDateOverlapping(ldtStartDate,
                                                                    lobjPersonAccountDefComp.icdoPersonAccountDeferredCompProvider.start_date,
                                                                    lobjPersonAccountDefComp.icdoPersonAccountDeferredCompProvider.end_date))
                        {
                            if ((ibusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencyBiWeekly) ||
                                (ibusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencySemiMonthly))
                            {
                                ldecPerPayPeriodamount =
                                    lobjPersonAccountDefComp.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt * 2;
                            }
                            else if ((ibusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencyWeekly))
                            {
                                ldecPerPayPeriodamount =
                                    lobjPersonAccountDefComp.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt * 4;
                            }
                            else
                            {
                                ldecPerPayPeriodamount =
                                    lobjPersonAccountDefComp.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt;
                            }
                            ldecSumOfPerPayPeriodamount += ldecPerPayPeriodamount;

                        }
                        ldtStartDate = ldtStartDate.AddMonths(1);

                    }
                }
            }
            else if (ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider.Count == 1 && icdoPersonAccountDeferredCompProvider.end_date == DateTime.MinValue)
            {
                foreach (busPersonAccountDeferredCompProvider lobjPersonAccountDefCompProvider in ibusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider)
                {
                    while ((icdoPersonAccountDeferredCompProvider.start_date.Year == DateTime.Today.Year) && (ldtStartDate.Month <= icdoPersonAccountDeferredCompProvider.start_date.Month) &&
                        ldtStartDate.Year <= icdoPersonAccountDeferredCompProvider.start_date.Year) //PIR 13585
                    {
                        if (busGlobalFunctions.CheckDateOverlapping(ldtStartDate,
                                                                    lobjPersonAccountDefCompProvider.icdoPersonAccountDeferredCompProvider.start_date,
                                                                    lobjPersonAccountDefCompProvider.icdoPersonAccountDeferredCompProvider.end_date))
                        {
                            if ((ibusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencyBiWeekly) ||
                                (ibusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencySemiMonthly))
                            {
                                ldecPerPayPeriodamount =
                                    lobjPersonAccountDefCompProvider.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt * 2;
                            }
                            else if ((ibusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencyWeekly))
                            {
                                ldecPerPayPeriodamount =
                                    lobjPersonAccountDefCompProvider.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt * 4;
                            }
                            else
                            {
                                ldecPerPayPeriodamount =
                                    lobjPersonAccountDefCompProvider.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt;
                            }
                            ldecSumOfPerPayPeriodamount += ldecPerPayPeriodamount;

                        }
                        ldtStartDate = ldtStartDate.AddMonths(1);
                    }
                }
            }

            ldecTotalAmount = ldecSumOfContributionAmount + ldecSumOfPerPayPeriodamount;
            return ldecTotalAmount;
        }
    }
}
