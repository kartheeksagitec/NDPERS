using System;
using System.Data;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Linq;
using System.Collections.ObjectModel;

namespace NeoSpinBatch
{
    class busDisabilitytoNormalRuleConversionbatch : busNeoSpinBatch
    {
        public busDBCacheData ibusDBCacheData { get; set; }

        public void ConvertDisabilityToNormalBasedOnRuleEligibility()
        {
            istrProcessName = "Disability to Normal Conversion - Rule Based Batch ";
            DateTime ldtBatchStartDate = iobjSystemManagement.icdoSystemManagement.batch_date;
            DateTime ldtBatchEndDate = new DateTime(ldtBatchStartDate.AddMonths(2).Year, ldtBatchStartDate.AddMonths(2).Month, 1);
            //ldtBatchEndDate = ldtBatchEndDate.AddDays(-1); //PIR 19145

            //Load benefit provision eligibility data
            if (ibusDBCacheData == null)
                ibusDBCacheData = new busDBCacheData();
            if (ibusDBCacheData.idtbCachedBenefitProvisionEligibility == null)
                ibusDBCacheData.idtbCachedBenefitProvisionEligibility = busGlobalFunctions.LoadBenefitProvisionEligibilityCacheData(iobjPassInfo);

            idlgUpdateProcessLog("Load all Person applied for Disability and having Payee Account in recieving status", "INFO", istrProcessName);
            //get all disability record with receiving status
            DataTable ldtGetDisablePersonRecords = busBase.Select("cdoBenefitCalculation.LoadPayeeAccountForRuleConversion", new object[0] { });

            foreach (DataRow dr in ldtGetDisablePersonRecords.Rows)
            {
                // Load all objects related to Payee account 
                busPayeeAccount lobjPayeeAccount = LoadObjectsReturnPayeeAccount(dr);

                int lintBenefitProvisionId = lobjPayeeAccount.ibusApplication.ibusPersonAccount.ibusPlan.icdoPlan.benefit_provision_id;

                DateTime ldtDateOfBirth = lobjPayeeAccount.ibusApplication.ibusPersonAccount.ibusPerson.icdoPerson.date_of_birth;

                idlgUpdateProcessLog("For Person Id :" + lobjPayeeAccount.ibusApplication.icdoBenefitApplication.member_person_id +
                                    "For Plan " + lobjPayeeAccount.ibusApplication.ibusPersonAccount.ibusPlan.icdoPlan.plan_name, "INFO", istrProcessName);

                if ((lobjPayeeAccount.ibusApplication.ibusPersonAccount.icdoPersonAccount.plan_id != busConstant.PlanIdJobService)
                    && (lobjPayeeAccount.ibusApplication.ibusPersonAccount.icdoPersonAccount.plan_id != busConstant.PlanIdDC &&
                    lobjPayeeAccount.ibusApplication.ibusPersonAccount.icdoPersonAccount.plan_id != busConstant.PlanIdDC2020 && //PIR 20232
                    lobjPayeeAccount.ibusApplication.ibusPersonAccount.icdoPersonAccount.plan_id != busConstant.PlanIdDC2025)) //PIR 25920
                {
                    //PIR 14646 - Maik's mail dated 08/12/2015
                    string lstrBenefitTierValue = string.Empty;
                    if (lobjPayeeAccount.ibusApplication.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMain)//PIR 20232
                    {
                        if (lobjPayeeAccount.ibusApplication.ibusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value == busConstant.MainBenefit2016Tier)
                            lstrBenefitTierValue = busConstant.MainBenefit2016Tier;
                        else
                            lstrBenefitTierValue = busConstant.MainBenefit1997Tier;
                    }
                    //PIR 26282
                    else if (lobjPayeeAccount.ibusApplication.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdBCILawEnf)
                    {
                        if (lobjPayeeAccount.ibusApplication.ibusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value == busConstant.BCIBenefit2023Tier)
                            lstrBenefitTierValue = busConstant.BCIBenefit2023Tier;
                        else
                            lstrBenefitTierValue = busConstant.BCIBenefit2011Tier;
                    }
                    //PIR 26544
                    else if (lobjPayeeAccount.ibusApplication.ibusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value.IsNotNullOrEmpty())
                    {
                        lstrBenefitTierValue = lobjPayeeAccount.ibusApplication.ibusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value;
                    }
                    //get eligibility Age for this person based on the plan and                  
                    int lintAgeplusService = 0;
                    var lobjResult = from a in ibusDBCacheData.idtbCachedBenefitProvisionEligibility.AsEnumerable()
                                     where a.Field<int>("benefit_provision_id") == lintBenefitProvisionId
                                      && a.Field<string>("benefit_account_type_value") == busConstant.ApplicationBenefitTypeRetirement
                                      && a.Field<string>("ELIGIBILITY_TYPE_VALUE") == busConstant.BenefitProvisionEligibilityNormal
                                      && a.Field<DateTime>("Effective_date") <= ldtBatchEndDate
                                      && a.Field<string>("Benefit_tier_Value") == (string.IsNullOrEmpty(lstrBenefitTierValue) ? null : lstrBenefitTierValue) //For New Benefit Tier for main
                                     select a; // PIR 14646
                    cdoBenefitProvisionEligibility lcdoBenefitProvisionEligibility = new cdoBenefitProvisionEligibility();
                    lcdoBenefitProvisionEligibility.LoadData(lobjResult.FirstOrDefault());
                    lintAgeplusService = lcdoBenefitProvisionEligibility.age_plus_service;
                    DateTime ldtRetirementDate = lobjPayeeAccount.ibusApplication.icdoBenefitApplication.retirement_date;

                    lobjPayeeAccount.ibusApplication.ibusPersonAccount.ibusPerson.icdoPerson.person_id = lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id;
                    //get totoal TVSC
                    int lintTotalTVSC = Convert.ToInt32(Math.Round(lobjPayeeAccount.ibusApplication.GetRoundedTVSC()));

                    int lintNumberOfMonthsToBeAdded = (lintAgeplusService * 12) - (lintTotalTVSC);

                    // add no of years obtained to get Normal Eligibility
                    DateTime ldtNormalEligibilityDate = ldtDateOfBirth.AddMonths(lintNumberOfMonthsToBeAdded).AddMonths(1).GetFirstDayofCurrentMonth(); //PIR 19145
                    int lintMembersAgeInMonthsAsOnRule = 0;
                    decimal ldecMemberAgeBasedOnRule = 0.0M;
                    int lintMemberAgeYearPart = 0;
                    int lintMemberAgeMonthPart = 0;
                    busPersonBase.CalculateAge(ldtDateOfBirth, ldtNormalEligibilityDate,
                                ref lintMembersAgeInMonthsAsOnRule, ref ldecMemberAgeBasedOnRule, 2, ref lintMemberAgeYearPart, ref lintMemberAgeMonthPart);
                    if ((busGlobalFunctions.CheckDateOverlapping(ldtNormalEligibilityDate, ldtBatchStartDate, ldtBatchEndDate)) ||
                        (ldtNormalEligibilityDate < ldtBatchStartDate))
                    {
                        //PIR 14646
                        if (((lobjPayeeAccount.ibusApplication.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMain) &&
                            lstrBenefitTierValue == busConstant.MainBenefit2016Tier &&
                            lintMemberAgeYearPart >= lcdoBenefitProvisionEligibility.minimum_age)
                            || ((lobjPayeeAccount.ibusApplication.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMain)
                            && lstrBenefitTierValue != busConstant.MainBenefit2016Tier) ||
                            (lobjPayeeAccount.ibusApplication.ibusPersonAccount.icdoPersonAccount.plan_id != busConstant.PlanIdMain)) //PIR 20232
                        {
                            // inititate workflow
                            InitializeWorkFlow(busConstant.Map_Process_Disability_to_Normal_Rule_Conversion,
                                lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id, lobjPayeeAccount.icdoPayeeAccount.payee_account_id,
                                busConstant.WorkflowProcessStatus_UnProcessed, busConstant.WorkflowProcessSource_Batch);

                            //set workflow age conversion flag as yes
                            lobjPayeeAccount.icdoPayeeAccount.workflow_rule_conversion_flag = busConstant.Flag_Yes;
                            lobjPayeeAccount.icdoPayeeAccount.Update();
                        }
                    }
                    else
                    {
                        idlgUpdateProcessLog("No Workflow Initialized for Person Id :"
                            + lobjPayeeAccount.ibusApplication.icdoBenefitApplication.member_person_id, "INFO", istrProcessName);
                    }
                }
            }
            if (ldtGetDisablePersonRecords.Rows.Count.Equals(0))
            {
                idlgUpdateProcessLog("No records found for conversion", "INFO", istrProcessName);
            }
        }

        /// <summary>
        /// this method returns a payee account
        /// after loading all objects required for this batch
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        private busPayeeAccount LoadObjectsReturnPayeeAccount(DataRow dr)
        {
            busPayeeAccount lobjPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
            lobjPayeeAccount.icdoPayeeAccount.LoadData(dr);

            lobjPayeeAccount.ibusBenefitCalculaton = new busBenefitCalculation { icdoBenefitCalculation = new cdoBenefitCalculation() };
            lobjPayeeAccount.ibusBenefitCalculaton.icdoBenefitCalculation.LoadData(dr);

            lobjPayeeAccount.ibusApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            lobjPayeeAccount.ibusApplication.icdoBenefitApplication.LoadData(dr);

            lobjPayeeAccount.ibusApplication.ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            lobjPayeeAccount.ibusApplication.ibusPersonAccount.icdoPersonAccount.LoadData(dr);

            lobjPayeeAccount.ibusApplication.ibusPersonAccount.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lobjPayeeAccount.ibusApplication.ibusPersonAccount.ibusPerson.icdoPerson.LoadData(dr);

            lobjPayeeAccount.ibusApplication.ibusPersonAccount.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
            lobjPayeeAccount.ibusApplication.ibusPersonAccount.ibusPlan.icdoPlan.LoadData(dr);

            lobjPayeeAccount.ibusApplication.ibusPersonAccount.ibusPersonAccountRetirement = new busPersonAccountRetirement { icdoPersonAccountRetirement = new cdoPersonAccountRetirement() };
            lobjPayeeAccount.ibusApplication.ibusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccountRetirement.LoadData(dr);

            return lobjPayeeAccount;
        }
    }
}
