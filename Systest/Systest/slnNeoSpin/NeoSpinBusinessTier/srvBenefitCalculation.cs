#region Using directives

using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using NeoSpin.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.Common;
using System.Linq;

#endregion

namespace NeoSpin.BusinessTier
{
    public class srvBenefitCalculation : srvNeoSpin
    {
        public srvBenefitCalculation()
        {
        }
        protected override ArrayList ValidateNew(string astrFormName, Hashtable ahstParam)
        {
            ArrayList larrErrors = null;
           // iobjPassInfo.iconFramework.Open();
            try
            {
                if (astrFormName == "wfmRetirementBenefitCalculationLookup")
                {
                    busBenefitCalculationLookup lbusBenefitCalculation = new busBenefitCalculationLookup();
                    larrErrors = lbusBenefitCalculation.ValidateNew(ahstParam);
                }
                if (astrFormName == "wfmRHICCombiningLookup")
                {
                    busRHICCombiningLookup lbusBenefitRHICCombine = new busRHICCombiningLookup();
                    larrErrors = lbusBenefitRHICCombine.ValidateNew(ahstParam);
                }
                if (astrFormName == "wfmRetirementBenefitCalculationEstimateMaintenance")
                {
                    busBenefitCalculation lobjBenefitCalculation = new busBenefitCalculation();
                    larrErrors = lobjBenefitCalculation.ValidateNew(ahstParam);
                }
            }
            finally
            {
               // iobjPassInfo.iconFramework.Close();
            }
            return larrErrors;
        }

        public busBenefitCalculationLookup LoadBenefitCalculation(DataTable adtbSearchResult)
        {
            busBenefitCalculationLookup lobjBenefitCalculation = new busBenefitCalculationLookup();
            lobjBenefitCalculation.LoadBenefitCalculations(adtbSearchResult);
            return lobjBenefitCalculation;
        }

        public busRetirementBenefitCalculation NewBenefitCalculationEstimate(int aintBenefitCalculationID, int aintPersonID, int aintPlanID, string astrBenefitType)
        {
            busRetirementBenefitCalculation lobjRetirementBenefitCalculation = new busRetirementBenefitCalculation();
            if (aintBenefitCalculationID == 0)
            {
                lobjRetirementBenefitCalculation.icdoBenefitCalculation = new cdoBenefitCalculation();
                lobjRetirementBenefitCalculation.IsESTMFromLOB = true; // PIR 23878
                lobjRetirementBenefitCalculation.icdoBenefitCalculation.person_id = aintPersonID;
                lobjRetirementBenefitCalculation.icdoBenefitCalculation.plan_id = aintPlanID;
                lobjRetirementBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value = astrBenefitType;
                lobjRetirementBenefitCalculation.icdoBenefitCalculation.benefit_account_type_description =
                    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1904, astrBenefitType);
                lobjRetirementBenefitCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeEstimate;
                lobjRetirementBenefitCalculation.icdoBenefitCalculation.calculation_type_description =
                    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2303, busConstant.CalculationTypeEstimate);
                lobjRetirementBenefitCalculation.icdoBenefitCalculation.status_value = busConstant.ApplicationStatusReview;
                lobjRetirementBenefitCalculation.icdoBenefitCalculation.status_description =
                    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1902, busConstant.ApplicationStatusReview);
                lobjRetirementBenefitCalculation.icdoBenefitCalculation.action_status_value = busConstant.CalculationStatusPendingApproval;
                lobjRetirementBenefitCalculation.icdoBenefitCalculation.action_status_description =
                    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2302, lobjRetirementBenefitCalculation.icdoBenefitCalculation.action_status_value);
                lobjRetirementBenefitCalculation.icdoBenefitCalculation.created_date = DateTime.Now;

                lobjRetirementBenefitCalculation.LoadMember();
                lobjRetirementBenefitCalculation.LoadPlan();
                lobjRetirementBenefitCalculation.LoadPersonAccount();
                lobjRetirementBenefitCalculation.LoadBenefitServicePurchase();
                lobjRetirementBenefitCalculation.LoadBenefitCalculationPayeeForNewMode();
                if (lobjRetirementBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                    lobjRetirementBenefitCalculation.LoadOtherDisabilityBenefits();
                lobjRetirementBenefitCalculation.iblnIsBenefitPayeeTabForNewModeVisible = true;
                lobjRetirementBenefitCalculation.icdoBenefitCalculation.normal_retirement_date = lobjRetirementBenefitCalculation.GetNormalRetirementDateBasedOnNormalEligibility(lobjRetirementBenefitCalculation.ibusPlan.icdoPlan.plan_code);
            }
            else
            {
                if (lobjRetirementBenefitCalculation.FindBenefitCalculation(aintBenefitCalculationID))
                {
                    lobjRetirementBenefitCalculation.icdoBenefitCalculation.benefit_calculation_id = 0;
                    lobjRetirementBenefitCalculation.LoadMember();
                    lobjRetirementBenefitCalculation.LoadPlan();
                    lobjRetirementBenefitCalculation.LoadPersonAccount();
                    lobjRetirementBenefitCalculation.LoadBenefitServicePurchase();
                    lobjRetirementBenefitCalculation.LoadBenefitCalculationPayeeForNewMode();
                    lobjRetirementBenefitCalculation.LoadOtherDisabilityBenefits();
                    lobjRetirementBenefitCalculation.CalculateMemberAge();
                    lobjRetirementBenefitCalculation.iblnIsBenefitPayeeTabForNewModeVisible = true;
                    lobjRetirementBenefitCalculation.iarrChangeLog.Add(lobjRetirementBenefitCalculation.icdoBenefitCalculation);
                    lobjRetirementBenefitCalculation.icdoBenefitCalculation.ienuObjectState = ObjectState.Insert;
                    lobjRetirementBenefitCalculation.icdoBenefitCalculation.created_date = DateTime.Now;
                    //Set Values for Calculation properties
                    if (lobjRetirementBenefitCalculation.icdoBenefitCalculation.plan_id != busConstant.PlanIdJobService)
                    {
                        lobjRetirementBenefitCalculation.icdoBenefitCalculation.early_retirement_percentage_decrease =
                            (lobjRetirementBenefitCalculation.icdoBenefitCalculation.early_reduced_months *
                            lobjRetirementBenefitCalculation.icdoBenefitCalculation.early_reduction_factor);
                    }
                    else
                    {
                        if (lobjRetirementBenefitCalculation.icdoBenefitCalculation.early_reduction_factor != 0M)
                            lobjRetirementBenefitCalculation.icdoBenefitCalculation.early_retirement_percentage_decrease =
                                1 - lobjRetirementBenefitCalculation.icdoBenefitCalculation.early_reduction_factor;
                    }
                }
            }

            // UCS-060 - Return to Work
            lobjRetirementBenefitCalculation.LoadPersonPlanAccounts();
            if (lobjRetirementBenefitCalculation.icdoBenefitCalculation.is_return_to_work_member)
            {
                if (lobjRetirementBenefitCalculation.icdoBenefitCalculation.rtw_refund_election_value.IsNullOrEmpty())
                {
                    lobjRetirementBenefitCalculation.icdoBenefitCalculation.rtw_refund_election_value = busConstant.Flag_No_Value.ToUpper();
                }

                //PIR 19594
                busBenefitCalculation lobjPrevBenefitCal = lobjRetirementBenefitCalculation.LoadPrevRTWBenefitCalculation();
                if (astrBenefitType == busConstant.ApplicationBenefitTypeRetirement && 
                    lobjPrevBenefitCal.icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
                {
                    lobjRetirementBenefitCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeEstimateSubsequent;
                    lobjRetirementBenefitCalculation.icdoBenefitCalculation.calculation_type_description =
                        iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2303, busConstant.CalculationTypeEstimateSubsequent);

                    lobjRetirementBenefitCalculation.icdoBenefitCalculation.benefit_option_value =
                                                                            lobjPrevBenefitCal.icdoBenefitCalculation.benefit_option_value;
                }

            }
            //  UAT - PIR - 857 - Defaulting the Termination date in Estimate.
            DateTime ldteTerminationDate = new DateTime();
            DateTime ldteCheckTerminationDate = new DateTime(2020, 01, 01);
            lobjRetirementBenefitCalculation.GetOrgIdAsLatestEmploymentOrgId(
                                lobjRetirementBenefitCalculation.ibusPersonAccount.icdoPersonAccount.person_account_id,
                                busConstant.ApplicationBenefitTypeRetirement,
                                ref ldteTerminationDate);
            lobjRetirementBenefitCalculation.icdoBenefitCalculation.termination_date = ldteTerminationDate;
            //PIR 23048
            if (ldteTerminationDate != DateTime.MinValue && ldteTerminationDate < ldteCheckTerminationDate)
                lobjRetirementBenefitCalculation.iblnIsActualTerminationDate = true;
            lobjRetirementBenefitCalculation.CalculateQDROAmount(true);
            lobjRetirementBenefitCalculation.EvaluateInitialLoadRules();
            lobjRetirementBenefitCalculation.ibusPersonAccount.LoadPersonAccountRetirement(); //PIR 16343
            return lobjRetirementBenefitCalculation;
        }

        public busRetirementBenefitCalculation FindBenefitCalculationEstimate(int aintBenefitCalculationID)
        {
            busRetirementBenefitCalculation lobjRetirementBenefitCalculation = new busRetirementBenefitCalculation();
            if (lobjRetirementBenefitCalculation.FindBenefitCalculation(aintBenefitCalculationID))
            {
                lobjRetirementBenefitCalculation.LoadMember();
                lobjRetirementBenefitCalculation.LoadPlan();
                lobjRetirementBenefitCalculation.LoadPersonAccount();
                if (lobjRetirementBenefitCalculation.ibusPersonAccount != null) // Block added to display 'Benefit Tier value' for main plans Estimate calculation
                {
                    if (lobjRetirementBenefitCalculation.ibusPersonAccount.icdoPersonAccount != null && lobjRetirementBenefitCalculation.ibusPersonAccount.icdoPersonAccount.plan_id == 1)
                        lobjRetirementBenefitCalculation.ibusPersonAccount.LoadPersonAccountRetirement();
                }
                lobjRetirementBenefitCalculation.CalculateMemberAge();
                lobjRetirementBenefitCalculation.LoadBenefitServicePurchase();
                lobjRetirementBenefitCalculation.LoadFASMonths();
                lobjRetirementBenefitCalculation.LoadFASMonths2019();
                lobjRetirementBenefitCalculation.LoadFASMonths2020();
                lobjRetirementBenefitCalculation.LoadBenefitCalculationPayee();
                lobjRetirementBenefitCalculation.CalculateAnnuitantAge();
                if (lobjRetirementBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                    lobjRetirementBenefitCalculation.LoadOtherDisabilityBenefits();
                lobjRetirementBenefitCalculation.LoadBenefitCalculationOptions();
                lobjRetirementBenefitCalculation.LoadBenefitRHICOption();
                lobjRetirementBenefitCalculation.LoadBenefitMultiplier();
                lobjRetirementBenefitCalculation.icdoBenefitCalculation.calculation_final_average_salary =
                    lobjRetirementBenefitCalculation.icdoBenefitCalculation.computed_final_average_salary;
                //if (lobjRetirementBenefitCalculation.icdoBenefitCalculation.indexed_final_average_salary > 0)
                //{
                //    lobjRetirementBenefitCalculation.icdoBenefitCalculation.calculation_final_average_salary =
                //        lobjRetirementBenefitCalculation.icdoBenefitCalculation.indexed_final_average_salary;
                //}

                if (lobjRetirementBenefitCalculation.icdoBenefitCalculation.overridden_final_average_salary > 0)
                {
                    lobjRetirementBenefitCalculation.icdoBenefitCalculation.calculation_final_average_salary =
                        lobjRetirementBenefitCalculation.icdoBenefitCalculation.overridden_final_average_salary;
                }
                //Set Values for Calculation properties
                if (lobjRetirementBenefitCalculation.icdoBenefitCalculation.plan_id != busConstant.PlanIdJobService)
                {
                    lobjRetirementBenefitCalculation.icdoBenefitCalculation.early_retirement_percentage_decrease =
                        (lobjRetirementBenefitCalculation.icdoBenefitCalculation.early_reduced_months *
                        lobjRetirementBenefitCalculation.icdoBenefitCalculation.early_reduction_factor);
                }
                else
                {
                    if (lobjRetirementBenefitCalculation.icdoBenefitCalculation.early_reduction_factor != 0M)
                        lobjRetirementBenefitCalculation.icdoBenefitCalculation.early_retirement_percentage_decrease =
                            1 - lobjRetirementBenefitCalculation.icdoBenefitCalculation.early_reduction_factor;
                }

                //loaded for correspondence         
                lobjRetirementBenefitCalculation.LoadBenefitProvision();
                lobjRetirementBenefitCalculation.LoadBenefitProvisionMultiplier();
                lobjRetirementBenefitCalculation.LoadRHICAmountForMember();
                lobjRetirementBenefitCalculation.LoadBenefitAmount();
                lobjRetirementBenefitCalculation.LoadErrors();

                // UCS-060 Return to Work
                if (lobjRetirementBenefitCalculation.icdoBenefitCalculation.is_return_to_work_member)
                {
                    if (lobjRetirementBenefitCalculation.icdoBenefitCalculation.rtw_refund_election_value.IsNullOrEmpty())
                    {
                        lobjRetirementBenefitCalculation.icdoBenefitCalculation.rtw_refund_election_value = busConstant.Flag_No_Value.ToUpper();
                    }
                    lobjRetirementBenefitCalculation.LoadPreRTWPayeeAccount();
                    lobjRetirementBenefitCalculation.LoadBenefitCalculationPersonAccount();
                }
                lobjRetirementBenefitCalculation.SetAdjustedMonthlySingleLifeBenefitAmount();
                lobjRetirementBenefitCalculation.CalculateMinimumGuaranteedMemberAccount();
                //
                if (lobjRetirementBenefitCalculation.icdoBenefitCalculation.plan_id == busConstant.PlanIdDC ||
                    lobjRetirementBenefitCalculation.icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2020 || //PIR 20232
                    lobjRetirementBenefitCalculation.icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2025)
                {
                    string astrBenefitSubType = string.Empty;
                    decimal ldecMemberAgeMontAndYear = 0.0M;

                    lobjRetirementBenefitCalculation.CalculatePersonAge(lobjRetirementBenefitCalculation.ibusMember.icdoPerson.date_of_birth, lobjRetirementBenefitCalculation.icdoBenefitCalculation.retirement_date,
                    ref ldecMemberAgeMontAndYear, 4);
                    lobjRetirementBenefitCalculation.IsMemberEligibleForDCPlan(ldecMemberAgeMontAndYear, ref astrBenefitSubType);
                    if (astrBenefitSubType.IsNullOrEmpty())
                    {
                        lobjRetirementBenefitCalculation.iblnIsMemberRestrictedForRHIC = true;
                    }
                }

                if (lobjRetirementBenefitCalculation.icdoBenefitCalculation.is_created_from_portal == busConstant.Flag_Yes)
                {
                    // PROD PIR ID 6232
                    lobjRetirementBenefitCalculation.CalculateConsolidatedPSC();
                    lobjRetirementBenefitCalculation.CalculateConsolidatedVSC();
                }

                lobjRetirementBenefitCalculation.EvaluateInitialLoadRules();
                lobjRetirementBenefitCalculation.LoadDeduction();
                //PIR 23048
                DateTime ldteCheckTerminationDate = new DateTime(2020, 01, 01);
                if (lobjRetirementBenefitCalculation.icdoBenefitCalculation.termination_date < ldteCheckTerminationDate)
                    lobjRetirementBenefitCalculation.iblnIsActualTerminationDate = true;
            }
            return lobjRetirementBenefitCalculation;
        }

        public busRetirementBenefitCalculation NewBenefitCalculationFinal
                (int aintBenefitApplicationID, int aintDisabilityPayeeAccountID, int aintPlanID)
        {
            busRetirementBenefitCalculation lobjBenefitCalculation = new busRetirementBenefitCalculation();
            busRetirementDisabilityApplication lobjRetirementApplication = new busRetirementDisabilityApplication();
            if (lobjRetirementApplication.FindBenefitApplication(aintBenefitApplicationID))
            {
                lobjBenefitCalculation.icdoBenefitCalculation = new cdoBenefitCalculation();

                if (aintDisabilityPayeeAccountID == 0)
                {
                    lobjBenefitCalculation.GetCalculationByApplication(lobjRetirementApplication);

                    //PIR 18053 - If RTW then set calculation type as Subsequent.
                    if (lobjRetirementApplication.icdoBenefitApplication.retirement_date >= busConstant.RTWEffectiveDate && 
                        lobjRetirementApplication.icdoBenefitApplication.pre_rtw_payeeaccount_id > 0)
                    {
                        lobjRetirementApplication.LoadPreRTWPayeeAccount();
                        busRetirementDisabilityApplication lobjRetrApp = new busRetirementDisabilityApplication();
                        if (lobjRetrApp.FindBenefitApplication(lobjRetirementApplication.ibusPreRTWPayeeAccount.icdoPayeeAccount.application_id))
                        {
                            if (lobjRetrApp.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
                            {
                                if (aintPlanID == -999)
                                {
                                    lobjBenefitCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeSubsequentAdjustment;
                                    lobjBenefitCalculation.icdoBenefitCalculation.calculation_type_description =
                                            iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2303, busConstant.CalculationTypeSubsequentAdjustment);
                                }
                                else
                                {
                                    lobjBenefitCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeSubsequent;
                                    lobjBenefitCalculation.icdoBenefitCalculation.calculation_type_description =
                                            iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2303, busConstant.CalculationTypeSubsequent);
                                }
                            }  
                            else
                            {
                                //When Recalculate button clicked fro Payee Account and its not a Retirement to Retirement application 
                                if (aintPlanID == -999)
                                {
                                    lobjBenefitCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeAdjustments;
                                    lobjBenefitCalculation.icdoBenefitCalculation.calculation_type_description =
                                        iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2303, busConstant.CalculationTypeAdjustments);
                                }
                                else
                                {
                                    //Normal benefit calculation steps
                                    lobjBenefitCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeFinal;
                                    lobjBenefitCalculation.icdoBenefitCalculation.calculation_type_description =
                                        iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2303, busConstant.CalculationTypeFinal);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (aintPlanID == -999)
                        {
                            //When recalculate button is clicked from Payee Account (aintPlanID is hardcoded to -999)
                            lobjBenefitCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeAdjustments;
                            lobjBenefitCalculation.icdoBenefitCalculation.calculation_type_description =
                                iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2303, busConstant.CalculationTypeAdjustments);
                        }
                        else
                        {
                            //Normal benefit calculation steps
                            lobjBenefitCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeFinal;
                            lobjBenefitCalculation.icdoBenefitCalculation.calculation_type_description =
                                iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2303, busConstant.CalculationTypeFinal);
                        }
                    }
                }
                else
                {
                    lobjBenefitCalculation.GetRetirementCalculationByDisabilityPayeeAccount(
                                            lobjRetirementApplication, aintDisabilityPayeeAccountID, aintPlanID);
                }
                lobjBenefitCalculation.icdoBenefitCalculation.created_date = DateTime.Now;
                lobjBenefitCalculation.LoadJointAnnuitant();
                lobjBenefitCalculation.LoadPersonAccount();
                lobjBenefitCalculation.CalculateMemberAge();
                lobjBenefitCalculation.CalculateAnnuitantAge();
                lobjBenefitCalculation.LoadBenefitCalculationPayeeForNewMode();
                if (lobjBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                    lobjBenefitCalculation.LoadOtherDisabilityBenefitsFromApplication(aintBenefitApplicationID);
                lobjBenefitCalculation.iblnIsBenefitPayeeTabForNewModeVisible = true;
                lobjBenefitCalculation.SetBenefitSubType();
                //PIRs 15600
                lobjBenefitCalculation.LoadRHICEffectiveDate();
                lobjBenefitCalculation.CalculateRetirementBenefit();
                //Backlog PIR 1869 start
                if (lobjBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability
                        && lobjBenefitCalculation.icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal
                        && aintDisabilityPayeeAccountID == 0)
                {
                    if (lobjBenefitCalculation.ibusDBCacheData == null)
                        lobjBenefitCalculation.ibusDBCacheData = new busDBCacheData();
                    if (lobjBenefitCalculation.ibusDBCacheData.idtbCachedBenefitProvisionEligibility == null)
                        lobjBenefitCalculation.ibusDBCacheData.idtbCachedBenefitProvisionEligibility = busGlobalFunctions.LoadBenefitProvisionEligibilityCacheData(iobjPassInfo);
                    var lobjResult = (from a in lobjBenefitCalculation.ibusDBCacheData.idtbCachedBenefitProvisionEligibility.AsEnumerable()
                                      where a.Field<int>("benefit_provision_id") == lobjBenefitCalculation.ibusPlan.icdoPlan.benefit_provision_id
                                       && a.Field<string>("benefit_account_type_value") == busConstant.ApplicationBenefitTypeRetirement
                                       && a.Field<string>("ELIGIBILITY_TYPE_VALUE") == busConstant.BenefitProvisionEligibilityNormal
                                       && a.Field<DateTime>("Effective_date") <= DateTime.Today
                                      select a.Field<int>("age"));
                    int lintAgetobeAdded = Convert.ToInt32(lobjResult.FirstOrDefault());
                    if (lobjBenefitCalculation.ibusMember.IsNull())
                        lobjBenefitCalculation.LoadMember();
                    DateTime ldtNormalEligibilityDate = lobjBenefitCalculation.ibusMember.icdoPerson.date_of_birth.AddYears(lintAgetobeAdded);
                    if (lintAgetobeAdded > 0 && lobjBenefitCalculation.icdoBenefitCalculation.retirement_date >= ldtNormalEligibilityDate)
                    {
                        busRetirementBenefitCalculation lbusRetirementBenefitCalculation = new busRetirementBenefitCalculation { icdoBenefitCalculation = new cdoBenefitCalculation() };
                        lbusRetirementBenefitCalculation.icdoBenefitCalculation = busGlobalFunctions.DeepCopy<cdoBenefitCalculation>(lobjBenefitCalculation.icdoBenefitCalculation);
                        lbusRetirementBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypeRetirement;
                        lbusRetirementBenefitCalculation.icdoBenefitCalculation.benefit_account_sub_type_value = busConstant.ApplicationBenefitSubTypeDisabilitytoNormal;
                        lbusRetirementBenefitCalculation.LoadBenefitCalculationPayeeForNewMode();
                        lbusRetirementBenefitCalculation.CalculateRetirementBenefit();
                        if (lbusRetirementBenefitCalculation.icdoBenefitCalculation.final_monthly_benefit >= lobjBenefitCalculation.icdoBenefitCalculation.final_monthly_benefit)
                        {

                            lobjBenefitCalculation.icdoBenefitCalculation = lbusRetirementBenefitCalculation.icdoBenefitCalculation;
                            lobjBenefitCalculation.icdoBenefitCalculation.benefit_account_type_description = busGlobalFunctions.GetDescriptionByCodeValue(1904, busConstant.ApplicationBenefitTypeRetirement, iobjPassInfo);
                            lobjBenefitCalculation.iblnIsDisabilityToNormalFromStart = true;
                            lobjBenefitCalculation.BeforeValidate(utlPageMode.New);
                            lobjBenefitCalculation.CalculateRetirementBenefit();
                        }
                    }
                }
                lobjBenefitCalculation.ibusPersonAccount.LoadPersonAccountRetirement(); //PIR 16343
                //Backlog PIR 1869 end
                lobjBenefitCalculation.EvaluateInitialLoadRules();
                if (lobjBenefitCalculation.icdoBenefitCalculation.benefit_calculation_id == 0 && lobjBenefitCalculation.icdoBenefitCalculation.ienuObjectState != ObjectState.Insert)
                    lobjBenefitCalculation.icdoBenefitCalculation.ienuObjectState = ObjectState.Insert; //F/W Upgrade PIR - 15588 - Recalculate Benefits, Needs to create always new calc.

                //PIR 23048
                DateTime ldteCheckTerminationDate = new DateTime(2020, 01, 01);
                if (lobjBenefitCalculation.icdoBenefitCalculation.termination_date < ldteCheckTerminationDate)
                    lobjBenefitCalculation.iblnIsActualTerminationDate = true;
            }
            return lobjBenefitCalculation;
        }

        public busRetirementBenefitCalculation FindBenefitCalculationFinal(int aintBenefitCalculationID)
        {
            busRetirementBenefitCalculation lobjRetirementBenefitCalculation = new busRetirementBenefitCalculation();
            if (lobjRetirementBenefitCalculation.FindBenefitCalculation(aintBenefitCalculationID))
            {
                if (lobjRetirementBenefitCalculation.icdoBenefitCalculation.benefit_application_id != 0)
                {
                    busRetirementDisabilityApplication lobjRetirementApplication = new busRetirementDisabilityApplication();
                    if (lobjRetirementBenefitCalculation.icdoBenefitCalculation.disability_payee_account_id == 0)
                        if (lobjRetirementApplication.FindBenefitApplication(lobjRetirementBenefitCalculation.icdoBenefitCalculation.benefit_application_id))
                            if (lobjRetirementBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value == lobjRetirementApplication.icdoBenefitApplication.benefit_account_type_value) //Backlog PIR 1869 start
                                lobjRetirementBenefitCalculation.GetCalculationByApplication(lobjRetirementApplication);
                    if (lobjRetirementBenefitCalculation.icdoBenefitCalculation.is_calculation_visible_flag == busConstant.Flag_No)
                    {
                        lobjRetirementBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypeRetirement;
                        lobjRetirementBenefitCalculation.icdoBenefitCalculation.benefit_account_type_description =
                                    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1904, busConstant.ApplicationBenefitTypeRetirement);
                    }
                }
                lobjRetirementBenefitCalculation.LoadMember();
                lobjRetirementBenefitCalculation.LoadJointAnnuitant();
                lobjRetirementBenefitCalculation.LoadPlan();
                lobjRetirementBenefitCalculation.LoadPersonAccount();
                if (lobjRetirementBenefitCalculation.ibusPersonAccount != null)// Block added to display 'Benefit Tier value' for main plans Final calculation
                {
                    if (lobjRetirementBenefitCalculation.ibusPersonAccount.icdoPersonAccount != null && lobjRetirementBenefitCalculation.ibusPersonAccount.icdoPersonAccount.plan_id == 1)
                        lobjRetirementBenefitCalculation.ibusPersonAccount.LoadPersonAccountRetirement();
                }
                lobjRetirementBenefitCalculation.CalculateMemberAge();
                lobjRetirementBenefitCalculation.LoadBenefitCalculationOptions();
                lobjRetirementBenefitCalculation.LoadBenefitRHICOption();
                //PIR:2190. Final Calculation should not recalculate after the Calculation is Approved
                // Do not Calculate Retirement Benefit if the Action Status is Approved or Cancelled.
                //if ((lobjRetirementBenefitCalculation.icdoBenefitCalculation.action_status_value != busConstant.BenefitActionStatusApproved) &&
                //    (lobjRetirementBenefitCalculation.icdoBenefitCalculation.action_status_value != busConstant.BenefitActionStatusCancelled))
                //{
                //    lobjRetirementBenefitCalculation.SetBenefitSubType();
                //    // UAT PIR ID 1509 - Earlier the calculation will be performed iff the status is other than Approved/Cancelled.
                //    lobjRetirementBenefitCalculation.CalculateRetirementBenefit();
                //}
                //else
                //{
                lobjRetirementBenefitCalculation.LoadBenefitCalculationPayee();
                lobjRetirementBenefitCalculation.LoadBenefitMultiplier();
                lobjRetirementBenefitCalculation.LoadFASMonths();
                lobjRetirementBenefitCalculation.LoadFASMonths2019();
                lobjRetirementBenefitCalculation.LoadFASMonths2020();
                lobjRetirementBenefitCalculation.icdoBenefitCalculation.calculation_final_average_salary =
                lobjRetirementBenefitCalculation.icdoBenefitCalculation.computed_final_average_salary;
                //if (lobjRetirementBenefitCalculation.icdoBenefitCalculation.indexed_final_average_salary > 0)
                //{
                //    lobjRetirementBenefitCalculation.icdoBenefitCalculation.calculation_final_average_salary =
                //        lobjRetirementBenefitCalculation.icdoBenefitCalculation.indexed_final_average_salary;
                //}

                if (lobjRetirementBenefitCalculation.icdoBenefitCalculation.overridden_final_average_salary > 0)
                {
                    lobjRetirementBenefitCalculation.icdoBenefitCalculation.calculation_final_average_salary =
                        lobjRetirementBenefitCalculation.icdoBenefitCalculation.overridden_final_average_salary;
                }
                //Set Values for Calculation properties
                if (lobjRetirementBenefitCalculation.icdoBenefitCalculation.plan_id != busConstant.PlanIdJobService)
                {
                    lobjRetirementBenefitCalculation.icdoBenefitCalculation.early_retirement_percentage_decrease =
                        (lobjRetirementBenefitCalculation.icdoBenefitCalculation.early_reduced_months *
                        lobjRetirementBenefitCalculation.icdoBenefitCalculation.early_reduction_factor);
                }
                else
                {
                    if (lobjRetirementBenefitCalculation.icdoBenefitCalculation.early_reduction_factor != 0M)
                        lobjRetirementBenefitCalculation.icdoBenefitCalculation.early_retirement_percentage_decrease =
                            1 - lobjRetirementBenefitCalculation.icdoBenefitCalculation.early_reduction_factor;
                }

                lobjRetirementBenefitCalculation.SetAdjustedMonthlySingleLifeBenefitAmount();
                //Calculate Min Guarantee is called here since it will be called inside calculate Retirement benefit
                lobjRetirementBenefitCalculation.CalculateMinimumGuaranteedMemberAccount();
                //}
                lobjRetirementBenefitCalculation.CalculateAnnuitantAge();
                if (lobjRetirementBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                    lobjRetirementBenefitCalculation.LoadOtherDisabilityBenefits();
                lobjRetirementBenefitCalculation.LoadErrors();


                if (lobjRetirementBenefitCalculation.icdoBenefitCalculation.disability_payee_account_id != 0)
                {
                    lobjRetirementBenefitCalculation.LoadDisabilityPayeeAccount();
                    lobjRetirementBenefitCalculation.ibusDisabilityPayeeAccount.LoadGrossBenefitAmount();
                    lobjRetirementBenefitCalculation.icdoBenefitCalculation.disability_gross_benefit_amount =
                        lobjRetirementBenefitCalculation.ibusDisabilityPayeeAccount.idecGrossBenfitAmount;

                }

                //PIR 23048
                DateTime ldteCheckTerminationDate = new DateTime(2020, 01, 01);
                if (lobjRetirementBenefitCalculation.icdoBenefitCalculation.termination_date < ldteCheckTerminationDate)
                    lobjRetirementBenefitCalculation.iblnIsActualTerminationDate = true;
            }
            return lobjRetirementBenefitCalculation;
        }

        public busBenefitCalculation LoadBenefitCalculationFinalPCD(int aintBenefitCalculationID)
        {
            busBenefitCalculation lobjBenefitCalculation = new busBenefitCalculation();
            lobjBenefitCalculation.FindBenefitCalculation(aintBenefitCalculationID);
            lobjBenefitCalculation.LoadMember();
            lobjBenefitCalculation.LoadPlan();
            lobjBenefitCalculation.LoadPersonAccount();
            lobjBenefitCalculation.LoadBenefitProvisionBenefitType();
            lobjBenefitCalculation.LoadEmployerPayrollDetailForFAS();
            lobjBenefitCalculation.LoadBenefitCalculationPCD();
            return lobjBenefitCalculation;
        }

        public busRHICCombiningLookup LoadRHICCombining(DataTable adtbSearchResult)
        {
            busRHICCombiningLookup lobjRHICCombining = new busRHICCombiningLookup();
            lobjRHICCombining.LoadBenefitRHICCombining(adtbSearchResult);
            return lobjRHICCombining;
        }

        public busBenefitRhicCombine FindBenefitRhicCombine(int aintBenefitRHICcombineID)
        {
            busBenefitRhicCombine lobjBenefitRhicCombine = new busBenefitRhicCombine();
            if (lobjBenefitRhicCombine.FindBenefitRhicCombine(aintBenefitRHICcombineID))
            {
                lobjBenefitRhicCombine.LoadPerson();
                lobjBenefitRhicCombine.LoadRHICDonorDetails(ablnReloadFromPayeeAccount: false);
                lobjBenefitRhicCombine.LoadEstimatedRHICDonorDetails(ablnReloadFromAccount: false);
                lobjBenefitRhicCombine.LoadErrors();
            }
            return lobjBenefitRhicCombine;
        }

        public busBenefitRhicCombine NewBenefitRHICCombine(int aintPersonID)
        {
            busBenefitRhicCombine lobjBenefitRHICCombine = new busBenefitRhicCombine();
            lobjBenefitRHICCombine.icdoBenefitRhicCombine = new cdoBenefitRhicCombine();
            lobjBenefitRHICCombine.iblnIsFromScreenNewMode = true;
            lobjBenefitRHICCombine.icdoBenefitRhicCombine.person_id = aintPersonID;
            lobjBenefitRHICCombine.icdoBenefitRhicCombine.request_date = DateTime.Now;
            lobjBenefitRHICCombine.LoadPerson();
            lobjBenefitRHICCombine.LoadRHICDonorDetails();
            lobjBenefitRHICCombine.LoadEstimatedRHICDonorDetails();
            lobjBenefitRHICCombine.icdoBenefitRhicCombine.action_status_value = busConstant.RHICActionStatusPendingApproval;
            lobjBenefitRHICCombine.icdoBenefitRhicCombine.action_status_description =
            iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2306, lobjBenefitRHICCombine.icdoBenefitRhicCombine.action_status_value);
            lobjBenefitRHICCombine.EvaluateInitialLoadRules();

            return lobjBenefitRHICCombine;
        }

        public busBenefitCalculationPayee FindBenefitCalculationPayee(int aintBenefitCalculationPayeeID)
        {
            busBenefitCalculationPayee lobjBenefitCalculationPayee = new busBenefitCalculationPayee();
            if (lobjBenefitCalculationPayee.FindBenefitCalculationPayee(aintBenefitCalculationPayeeID))
            {
                lobjBenefitCalculationPayee.LoadBenefitCalculation();
                lobjBenefitCalculationPayee.LoadMember();
                lobjBenefitCalculationPayee.LoadPayeeNames();
            }
            return lobjBenefitCalculationPayee;
        }

        public busBenefitCalculationPayee NewBenefitCalculationPayee(int aintBenefitCalculationID)
        {
            busBenefitCalculationPayee lobjBenefitCalculationPayee = new busBenefitCalculationPayee();
            lobjBenefitCalculationPayee.icdoBenefitCalculationPayee = new cdoBenefitCalculationPayee();
            lobjBenefitCalculationPayee.icdoBenefitCalculationPayee.benefit_calculation_id = aintBenefitCalculationID;
            lobjBenefitCalculationPayee.LoadBenefitCalculation();
            lobjBenefitCalculationPayee.LoadMember();
            return lobjBenefitCalculationPayee;
        }

        public busDeductionCalculation FindDeductionCalculation(int aintBenefitCalculationID, int aintDeductionSummaryID, int aintBenefitOptionID)
        {
            busDeductionCalculation lobjDeductionCalculation = new busDeductionCalculation();
            lobjDeductionCalculation.FindDeductionCalculation(aintBenefitCalculationID);
            lobjDeductionCalculation.ibusBenefitDeductionSummary = new busBenefitDeductionSummary();
            lobjDeductionCalculation.ibusBenefitDeductionSummary.FindBenefitDeductionSummary(aintDeductionSummaryID);
            lobjDeductionCalculation.ibusBenefitCalculationOptions = new busBenefitCalculationOptions();
            lobjDeductionCalculation.ibusBenefitCalculationOptions.FindBenefitCalculationOptions(aintBenefitOptionID);
            lobjDeductionCalculation.ibusBenefitCalculationOptions.LoadBenefitProvisionOption();
            lobjDeductionCalculation.icdoBenefitCalculation.benefit_calculation_id = aintBenefitCalculationID;
            lobjDeductionCalculation.LoadBenefitCalculation();
            lobjDeductionCalculation.LoadPerson();
            lobjDeductionCalculation.ibusPerson.LoadPersonEmployment();
            lobjDeductionCalculation.LoadPlan();
            lobjDeductionCalculation.LoadBenefitApplication();
            lobjDeductionCalculation.LoadBenefitHealthDeduction();
            lobjDeductionCalculation.LoadBenefitDentalDeduction();
            lobjDeductionCalculation.LoadBenefitVisionDeduction();
            lobjDeductionCalculation.LoadBenefitLifeDeductions();
            lobjDeductionCalculation.LoadBenefitLtcMemberDeductions();
            lobjDeductionCalculation.LoadBenefitLtcSpouseDeductions();
            lobjDeductionCalculation.LoadBenefitPayeeFedTaxWithholding();
            lobjDeductionCalculation.LoadBenefitPayeeStateTaxWithholding();
            lobjDeductionCalculation.LoadCurrentCoverageLifeOptions(); //PIR 1730
            lobjDeductionCalculation.LoadBenefitDeductionSummary();

            lobjDeductionCalculation.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.gross_monthly_benefit_amount =
                lobjDeductionCalculation.ibusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_option_amount;
            if (lobjDeductionCalculation.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.rhic_overridden_amount == 0M)
                lobjDeductionCalculation.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.rhic_overridden_amount =
                     Math.Round(((1 - lobjDeductionCalculation.icdoBenefitCalculation.rhic_early_reduction_factor) *
                     lobjDeductionCalculation.icdoBenefitCalculation.unreduced_rhic_amount), 2,
                     MidpointRounding.AwayFromZero);
            if (lobjDeductionCalculation.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.health_overridden_amount > 0)
                lobjDeductionCalculation.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.net_health_insurance_premium_amount =
                    lobjDeductionCalculation.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.health_overridden_amount;
            // PIR 15271 and 20269 : Removing rhic_overridden_amount        
            //- lobjDeductionCalculation.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.rhic_overridden_amount; 
            lobjDeductionCalculation.CalculateNetMonthlyPensionBenefit(); // Net benefit calculation

            //Start -- methods for correspondence
            lobjDeductionCalculation.ibusBenefitCalculation.CalculateMemberAge();
            lobjDeductionCalculation.ibusBenefitCalculation.CalculateFAS();
            lobjDeductionCalculation.ibusBenefitCalculation.LoadPlan();
            lobjDeductionCalculation.ibusBenefitCalculation.LoadBenefitProvision();
            lobjDeductionCalculation.ibusBenefitCalculation.LoadBenefitProvisionMultiplier();
            lobjDeductionCalculation.ibusBenefitCalculation.LoadBenefitAmount();
            lobjDeductionCalculation.ibusBenefitCalculation.LoadRHICAmountForMember();
            //End -- methods for correspondence
            return lobjDeductionCalculation;
        }

        public busBenefitServicePurchase FindBenefitServicePurchase(int aintBenefitServicePurchaseID)
        {
            busBenefitServicePurchase lobjBenefitServicePurchase = new busBenefitServicePurchase();
            if (lobjBenefitServicePurchase.FindBenefitServicePurchase(aintBenefitServicePurchaseID))
            {
            }
            return lobjBenefitServicePurchase;
        }

        public busBenefitRefundCalculation NewBenefitRefundCalculation(int aintBenefitApplicationID, int aintSourceID = 0)
        {
            busBenefitRefundCalculation lobjBenefitRefundCalculation = new busBenefitRefundCalculation();
            lobjBenefitRefundCalculation.icdoBenefitCalculation = new cdoBenefitCalculation();
            lobjBenefitRefundCalculation.icdoBenefitRefundCalculation = new cdoBenefitRefundCalculation();
            lobjBenefitRefundCalculation.icdoBenefitCalculation.benefit_application_id = aintBenefitApplicationID;
            lobjBenefitRefundCalculation.LoadRefundBenefitApplication();
            //Backlog PIR 11946
            if (aintSourceID == -999)
                lobjBenefitRefundCalculation.LoadDefaultValues(false);
            else
                lobjBenefitRefundCalculation.LoadDefaultValues(true);
            lobjBenefitRefundCalculation.icdoBenefitCalculation.person_id = lobjBenefitRefundCalculation.ibusRefundBenefitApplication.icdoBenefitApplication.member_person_id;
            lobjBenefitRefundCalculation.LoadMember();
            lobjBenefitRefundCalculation.icdoBenefitCalculation.plan_id = lobjBenefitRefundCalculation.ibusRefundBenefitApplication.icdoBenefitApplication.plan_id;
            lobjBenefitRefundCalculation.LoadBenefitCalculationPersonAccount();
            lobjBenefitRefundCalculation.LoadPlan();
            lobjBenefitRefundCalculation.LoadPersonAccount();
            lobjBenefitRefundCalculation.CalculateRefund();
            lobjBenefitRefundCalculation.CalculateRequiredMinimumDistributionAmount();
            if (!lobjBenefitRefundCalculation.IsBenefitOptionRegularRefundOrAutoRefund())
            {
                lobjBenefitRefundCalculation.CalculateInterestForERPreTaxAmount();
            }
            if ((lobjBenefitRefundCalculation.IsBenefitOptionRegularRefundOrAutoRefund()))
            {
                lobjBenefitRefundCalculation.CalculateTotalRefundAmountForRegulaOrAutoRefund();
            }
            else
            {
                lobjBenefitRefundCalculation.CalculateTotalAmountForTransferOptions();
            }
            lobjBenefitRefundCalculation.LoadBenefitCalculationPayeeForNewMode();
            lobjBenefitRefundCalculation.EvaluateInitialLoadRules();
            //PIR 23048
            DateTime ldteCheckTerminationDate = new DateTime(2020, 01, 01);
            if (lobjBenefitRefundCalculation.icdoBenefitCalculation.termination_date < ldteCheckTerminationDate)
                lobjBenefitRefundCalculation.iblnIsActualTerminationDate = true;
            return lobjBenefitRefundCalculation;
        }

        public busBenefitRefundCalculation FindBenefitRefundCalculation(int aintBenefitCalculationID)
        {
            busBenefitRefundCalculation lobjBenefitRefundCalculation = new busBenefitRefundCalculation();
            if (lobjBenefitRefundCalculation.FindBenefitRefundCalculation(aintBenefitCalculationID))
            {
                lobjBenefitRefundCalculation.icdoBenefitRefundCalculation.person_id = lobjBenefitRefundCalculation.icdoBenefitCalculation.person_id;
                lobjBenefitRefundCalculation.LoadBenefitApplication();
                lobjBenefitRefundCalculation.LoadMember();
                lobjBenefitRefundCalculation.LoadPlan();
                lobjBenefitRefundCalculation.LoadPersonAccount();
                lobjBenefitRefundCalculation.LoadBenefitCalculationPayee();
                if ((lobjBenefitRefundCalculation.IsBenefitOptionRegularRefundOrAutoRefund()))
                {
                    lobjBenefitRefundCalculation.CalculateTotalRefundAmountForRegulaOrAutoRefund();
                }
                else
                {
                    lobjBenefitRefundCalculation.CalculateTotalAmountForTransferOptions();
                }
                lobjBenefitRefundCalculation.CalculateRequiredMinimumDistributionAmount();
                //Start -- Method for correspondence             
                lobjBenefitRefundCalculation.ibusMember.LoadLastEmployment(); //PIR-10361  
                lobjBenefitRefundCalculation.LoadRefundBenefitApplication();
                lobjBenefitRefundCalculation.LoadPayeeAccountID();
                lobjBenefitRefundCalculation.LoadErrors();
                //End -- Method for correspondence
                //PIR 23048
                DateTime ldteCheckTerminationDate = new DateTime(2020, 01, 01);
                if (lobjBenefitRefundCalculation.icdoBenefitCalculation.termination_date < ldteCheckTerminationDate)
                    lobjBenefitRefundCalculation.iblnIsActualTerminationDate = true;
            }
            return lobjBenefitRefundCalculation;
        }

        public busPreRetirementDeathBenefitCalculation NewPreRetirementDeathBenefitCalculationEstimate(int aintBenefitCalculationID, int aintPersonID, int aintPlanID,
                                                                                string astrBenefitType)
        {
            busPreRetirementDeathBenefitCalculation lobjPreRetirementDeathBenefitCalculation = new busPreRetirementDeathBenefitCalculation();
            if (aintBenefitCalculationID == 0)
            {
                lobjPreRetirementDeathBenefitCalculation.iblnIsNewMode = true;
                lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation = new cdoBenefitCalculation();
                lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.person_id = aintPersonID;
                lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.plan_id = aintPlanID;
                lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value = astrBenefitType;
                lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.benefit_account_type_description =
                    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1904, astrBenefitType);
                lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeEstimate;
                lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.calculation_type_description =
                    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2303, busConstant.CalculationTypeEstimate);
                lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.status_value = busConstant.ApplicationStatusReview;
                lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.status_description =
                    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1902, busConstant.ApplicationStatusReview);
                lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.action_status_value = busConstant.CalculationStatusPendingApproval;
                lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.action_status_description =
                    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2302, lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.action_status_value);
                lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.created_date = DateTime.Now;

                lobjPreRetirementDeathBenefitCalculation.LoadMember();
                lobjPreRetirementDeathBenefitCalculation.LoadPlan();
                lobjPreRetirementDeathBenefitCalculation.LoadPersonAccount();
                lobjPreRetirementDeathBenefitCalculation.LoadBenefitServicePurchase();
                lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.date_of_death = lobjPreRetirementDeathBenefitCalculation.ibusMember.icdoPerson.date_of_death; // PROD PIR ID 6743
                lobjPreRetirementDeathBenefitCalculation.LoadBenefitCalculationPayeeForNewMode();
                lobjPreRetirementDeathBenefitCalculation.CalculateMemberAge();
                if (lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                    lobjPreRetirementDeathBenefitCalculation.LoadOtherDisabilityBenefits();
                lobjPreRetirementDeathBenefitCalculation.iblnIsBenefitPayeeTabForNewModeVisible = true;

                //  UAT - PIR - 876 - Defaulting the Date of death & Termination date in Estimate too.
                DateTime ldteTerminationDate = new DateTime();
                lobjPreRetirementDeathBenefitCalculation.GetOrgIdAsLatestEmploymentOrgId(
                                    lobjPreRetirementDeathBenefitCalculation.ibusPersonAccount.icdoPersonAccount.person_account_id,
                                    busConstant.ApplicationBenefitTypePreRetirementDeath,
                                    ref ldteTerminationDate);
                lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.termination_date = ldteTerminationDate;
                lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.date_of_death =
                            lobjPreRetirementDeathBenefitCalculation.ibusMember.icdoPerson.date_of_death;
            }
            else
            {
                if (lobjPreRetirementDeathBenefitCalculation.FindBenefitCalculation(aintBenefitCalculationID))
                {
                    lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.benefit_calculation_id = 0;
                    lobjPreRetirementDeathBenefitCalculation.LoadMember();
                    lobjPreRetirementDeathBenefitCalculation.LoadPlan();
                    lobjPreRetirementDeathBenefitCalculation.LoadPersonAccount();
                    lobjPreRetirementDeathBenefitCalculation.LoadBenefitServicePurchase();
                    lobjPreRetirementDeathBenefitCalculation.LoadBenefitCalculationPayeeForNewMode();
                    lobjPreRetirementDeathBenefitCalculation.LoadOtherDisabilityBenefits();
                    lobjPreRetirementDeathBenefitCalculation.CalculateMemberAge();
                    lobjPreRetirementDeathBenefitCalculation.iblnIsBenefitPayeeTabForNewModeVisible = true;
                    lobjPreRetirementDeathBenefitCalculation.iarrChangeLog.Add(lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation);
                    lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.ienuObjectState = ObjectState.Insert;
                    lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.created_date = DateTime.Now;
                }
            }
            lobjPreRetirementDeathBenefitCalculation.CalculateQDROAmount(true);
            lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.is_member_vested =
                    lobjPreRetirementDeathBenefitCalculation.IsMemberVested() ? busConstant.Flag_Yes : busConstant.Flag_No;
            lobjPreRetirementDeathBenefitCalculation.EvaluateInitialLoadRules();

            //PIR 23048
            DateTime ldteCheckTerminationDate = new DateTime(2020, 01, 01);
            if (lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.termination_date < ldteCheckTerminationDate)
                lobjPreRetirementDeathBenefitCalculation.iblnIsActualTerminationDate = true;
            return lobjPreRetirementDeathBenefitCalculation;
        }

        public busPreRetirementDeathBenefitCalculation FindPreRetirementDeathBenefitCalculationEstimate(int aintBenefitCalculationID)
        {
            busPreRetirementDeathBenefitCalculation lobjPreRetirementDeathBenefitCalculation = new busPreRetirementDeathBenefitCalculation();
            if (lobjPreRetirementDeathBenefitCalculation.FindBenefitCalculation(aintBenefitCalculationID))
            {
                lobjPreRetirementDeathBenefitCalculation.LoadMember();
                lobjPreRetirementDeathBenefitCalculation.LoadPlan();
                lobjPreRetirementDeathBenefitCalculation.LoadPersonAccount();
                lobjPreRetirementDeathBenefitCalculation.CalculateMemberAge();
                lobjPreRetirementDeathBenefitCalculation.LoadBenefitServicePurchase();
                lobjPreRetirementDeathBenefitCalculation.LoadFASMonths();
                lobjPreRetirementDeathBenefitCalculation.LoadFASMonths2019();
                lobjPreRetirementDeathBenefitCalculation.LoadFASMonths2020();
                lobjPreRetirementDeathBenefitCalculation.LoadBenefitCalculationPayee();
                lobjPreRetirementDeathBenefitCalculation.CalculateAnnuitantAge();
                lobjPreRetirementDeathBenefitCalculation.SetPayeeBenefitOption();
                lobjPreRetirementDeathBenefitCalculation.SetNormalRetirementDate();
                if (lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                    lobjPreRetirementDeathBenefitCalculation.LoadOtherDisabilityBenefits();
                lobjPreRetirementDeathBenefitCalculation.LoadBenefitCalculationOptions();
                lobjPreRetirementDeathBenefitCalculation.LoadBenefitRHICOption();
                lobjPreRetirementDeathBenefitCalculation.LoadBenefitMultiplier();
                lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.calculation_final_average_salary =
                    lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.computed_final_average_salary;
                //if (lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.indexed_final_average_salary > 0)
                //{
                //    lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.calculation_final_average_salary =
                //        lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.indexed_final_average_salary;
                //}

                if (lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.overridden_final_average_salary > 0)
                {
                    lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.calculation_final_average_salary =
                        lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.overridden_final_average_salary;
                }

                lobjPreRetirementDeathBenefitCalculation.LoadErrors();
            }
            lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.is_member_vested =
                lobjPreRetirementDeathBenefitCalculation.IsMemberVested() ? busConstant.Flag_Yes : busConstant.Flag_No;
            lobjPreRetirementDeathBenefitCalculation.EvaluateInitialLoadRules();

            //PIR 23048
            DateTime ldteCheckTerminationDate = new DateTime(2020, 01, 01);
            if (lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.termination_date < ldteCheckTerminationDate)
                lobjPreRetirementDeathBenefitCalculation.iblnIsActualTerminationDate = true;

            return lobjPreRetirementDeathBenefitCalculation;
        }

        public busPreRetirementDeathBenefitCalculation NewPreRetirementDeathBenefitCalculationFinal(int aintSource, int aintBenefitApplicationID)
        {
            busPreRetirementDeathBenefitCalculation lobjPreRetirementDeathBenefitCalculation = new busPreRetirementDeathBenefitCalculation();
            busPreRetirementDeathApplication lobjPreRetirementApplication = new busPreRetirementDeathApplication();
            if (lobjPreRetirementApplication.FindBenefitApplication(aintBenefitApplicationID))
            {
                lobjPreRetirementDeathBenefitCalculation.iblnIsNewMode = true;
                lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation = new cdoBenefitCalculation();
                lobjPreRetirementDeathBenefitCalculation.GetCalculationByApplication(lobjPreRetirementApplication);
                //UCS - 079 : if coming from Recalculate benefits in Payee account
                if (aintSource == -999)
                {
                    lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeAdjustments;
                    lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.calculation_type_description =
                        iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2303, busConstant.CalculationTypeAdjustments);
                }
                else
                {
                    lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeFinal;
                    lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.calculation_type_description =
                        iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2303, busConstant.CalculationTypeFinal);
                }

                lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.action_status_value = busConstant.CalculationStatusPendingApproval;
                lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.action_status_description =
                    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2302, lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.action_status_value);
                //PIRs 15600
                lobjPreRetirementDeathBenefitCalculation.LoadRHICEffectiveDate();

                lobjPreRetirementDeathBenefitCalculation.LoadMember();
                lobjPreRetirementDeathBenefitCalculation.LoadPlan();
                lobjPreRetirementDeathBenefitCalculation.LoadJointAnnuitant();
                lobjPreRetirementDeathBenefitCalculation.LoadPersonAccount();
                lobjPreRetirementDeathBenefitCalculation.CalculateMemberAge();
                lobjPreRetirementDeathBenefitCalculation.CalculateAnnuitantAge();
                lobjPreRetirementDeathBenefitCalculation.LoadBenefitCalculationPayeeForNewMode();
                lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.created_date = DateTime.Now;
                if (lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                    lobjPreRetirementDeathBenefitCalculation.LoadOtherDisabilityBenefits();
                lobjPreRetirementDeathBenefitCalculation.iblnIsBenefitPayeeTabForNewModeVisible = true;
                lobjPreRetirementDeathBenefitCalculation.EvaluateInitialLoadRules();
                lobjPreRetirementDeathBenefitCalculation.CalculatePreRetirementDeathBenefit();
                if (lobjPreRetirementApplication.icdoBenefitApplication.plan_id != busConstant.PlanIdDC &&
                    lobjPreRetirementApplication.icdoBenefitApplication.plan_id != busConstant.PlanIdDC2020 &&
                    lobjPreRetirementApplication.icdoBenefitApplication.plan_id != busConstant.PlanIdDC2025) //PIR 20232 PIR 25920
                    lobjPreRetirementDeathBenefitCalculation.SetNormalRetirementDate();
                lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.is_member_vested =
                    lobjPreRetirementDeathBenefitCalculation.IsMemberVested() ? busConstant.Flag_Yes : busConstant.Flag_No;
                if (lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.benefit_calculation_id == 0 && lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.ienuObjectState != ObjectState.Insert)
                    lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.ienuObjectState = ObjectState.Insert;
                //PIR 23048
                DateTime ldteCheckTerminationDate = new DateTime(2020, 01, 01);
                if (lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.termination_date < ldteCheckTerminationDate)
                    lobjPreRetirementDeathBenefitCalculation.iblnIsActualTerminationDate = true;
            }
            return lobjPreRetirementDeathBenefitCalculation;
        }

        public busPreRetirementDeathBenefitCalculation FindPreRetirementDeathBenefitCalculationFinal(int aintBenefitCalculationID)
        {
            busPreRetirementDeathBenefitCalculation lobjPreRetirementDeathBenefitCalculation = new busPreRetirementDeathBenefitCalculation();
            if (lobjPreRetirementDeathBenefitCalculation.FindBenefitCalculation(aintBenefitCalculationID))
            {
                if (lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.benefit_application_id != 0)
                {
                    busPreRetirementDeathApplication lobjPreRetirementDeathApplication = new busPreRetirementDeathApplication();
                    if (lobjPreRetirementDeathApplication.FindBenefitApplication(lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.benefit_application_id))
                        lobjPreRetirementDeathBenefitCalculation.GetCalculationByApplication(lobjPreRetirementDeathApplication);
                }
                lobjPreRetirementDeathBenefitCalculation.LoadMember();
                lobjPreRetirementDeathBenefitCalculation.LoadJointAnnuitant();
                lobjPreRetirementDeathBenefitCalculation.LoadPlan();
                lobjPreRetirementDeathBenefitCalculation.LoadPersonAccount();
                lobjPreRetirementDeathBenefitCalculation.LoadBenefitCalculationPayeeForNewMode();
                lobjPreRetirementDeathBenefitCalculation.LoadBenefitCalculationOptions();
                lobjPreRetirementDeathBenefitCalculation.CalculateMemberAge();
                lobjPreRetirementDeathBenefitCalculation.SetNormalRetirementDate();
                lobjPreRetirementDeathBenefitCalculation.CalculateAnnuitantAge();
                if (lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                    lobjPreRetirementDeathBenefitCalculation.LoadOtherDisabilityBenefits();
                lobjPreRetirementDeathBenefitCalculation.LoadErrors();

                //// Do not Calculate Death Benefit if Action Status is Approved or Cancelled
                //if ((lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.action_status_value != busConstant.BenefitActionStatusApproved) &&
                //    (lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.action_status_value != busConstant.BenefitActionStatusCancelled))
                //    lobjPreRetirementDeathBenefitCalculation.CalculatePreRetirementDeathBenefit(); // UAT PIR ID 1475
                //else
                //{
                lobjPreRetirementDeathBenefitCalculation.LoadFASMonths();
                lobjPreRetirementDeathBenefitCalculation.LoadFASMonths2019();
                lobjPreRetirementDeathBenefitCalculation.LoadFASMonths2020();
                lobjPreRetirementDeathBenefitCalculation.LoadBenefitMultiplier();
                lobjPreRetirementDeathBenefitCalculation.LoadBenefitRHICOption();
                lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.calculation_final_average_salary =
                    lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.computed_final_average_salary;
                if (lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.overridden_final_average_salary > 0)
                {
                    lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.calculation_final_average_salary =
                        lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.overridden_final_average_salary;
                }
                lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.reduced_monthly_after_plso_deduction =
                    lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.unreduced_benefit_amount -
                    lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.qdro_amount;
                lobjPreRetirementDeathBenefitCalculation.CalculateMinimumGuaranteedMemberAccount();
                //}
            }
            //PIR 23048
            DateTime ldteCheckTerminationDate = new DateTime(2020, 01, 01);
            if (lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.termination_date < ldteCheckTerminationDate)
                lobjPreRetirementDeathBenefitCalculation.iblnIsActualTerminationDate = true;
            lobjPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.is_member_vested =
                lobjPreRetirementDeathBenefitCalculation.IsMemberVested() ? busConstant.Flag_Yes : busConstant.Flag_No;
            lobjPreRetirementDeathBenefitCalculation.EvaluateInitialLoadRules();
            return lobjPreRetirementDeathBenefitCalculation;
        }

        public busPostRetirementDeathBenefitCalculation NewPostRetirementDeathBenefitCalculationFinal(int aintSource, int aintBenefitApplicationID)
        {
            busPostRetirementDeathBenefitCalculation lobjPostRetirementDeathBenefitCalculation = new busPostRetirementDeathBenefitCalculation();
            busPostRetirementDeathApplication lobjPostRetirementApplication = new busPostRetirementDeathApplication();
            if (lobjPostRetirementApplication.FindBenefitApplication(aintBenefitApplicationID))
            {
                lobjPostRetirementDeathBenefitCalculation.iblnIsNewMode = true;
                lobjPostRetirementDeathBenefitCalculation.icdoBenefitCalculation = new cdoBenefitCalculation();
                lobjPostRetirementDeathBenefitCalculation.GetCalculationByApplication(lobjPostRetirementApplication);
                //26061 : if coming from Recalculate benefits in Payee account
                if (aintSource == -999)
                {
                    lobjPostRetirementDeathBenefitCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeAdjustments;
                    lobjPostRetirementDeathBenefitCalculation.icdoBenefitCalculation.calculation_type_description =
                        iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2303, busConstant.CalculationTypeAdjustments);
                }
                else
                {
                    lobjPostRetirementDeathBenefitCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeFinal;
                    lobjPostRetirementDeathBenefitCalculation.icdoBenefitCalculation.calculation_type_description =
                        iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2303, busConstant.CalculationTypeFinal);
                }
                lobjPostRetirementDeathBenefitCalculation.icdoBenefitCalculation.status_value = busConstant.ApplicationStatusReview;
                lobjPostRetirementDeathBenefitCalculation.icdoBenefitCalculation.status_description =
                    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1902, busConstant.ApplicationStatusReview);

                lobjPostRetirementDeathBenefitCalculation.icdoBenefitCalculation.action_status_value = busConstant.CalculationStatusPendingApproval;
                lobjPostRetirementDeathBenefitCalculation.icdoBenefitCalculation.action_status_description =
                    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2302, lobjPostRetirementDeathBenefitCalculation.icdoBenefitCalculation.action_status_value);

                lobjPostRetirementDeathBenefitCalculation.LoadOriginatingPayeeAccount();
                lobjPostRetirementDeathBenefitCalculation.ibusOriginatingPayeeAccount.LoadApplication();
                lobjPostRetirementDeathBenefitCalculation.LoadBenefitApplication();
                lobjPostRetirementDeathBenefitCalculation.ibusBenefitApplication.LoadRecipient();
                lobjPostRetirementDeathBenefitCalculation.LoadPersonAccount();
                lobjPostRetirementDeathBenefitCalculation.CalculateMemberAge();
                lobjPostRetirementDeathBenefitCalculation.CalculateSpouseAge();
                lobjPostRetirementDeathBenefitCalculation.icdoBenefitCalculation.created_date = DateTime.Now;
                lobjPostRetirementDeathBenefitCalculation.iblnIsBenefitPayeeTabForNewModeVisible = true;
                lobjPostRetirementDeathBenefitCalculation.EvaluateInitialLoadRules();
                if (lobjPostRetirementDeathBenefitCalculation.icdoBenefitCalculation.originating_payee_account_id != 0)
                    lobjPostRetirementDeathBenefitCalculation.CalculatePostRetirementDeathBenefit();
                lobjPostRetirementDeathBenefitCalculation.LoadMemberTerminationDate();
                //F/W Upgrade PIR 16067 - When no changes were done and clicking save, record is not getting saved.
                if(lobjPostRetirementDeathBenefitCalculation.icdoBenefitCalculation.benefit_calculation_id == 0 && lobjPostRetirementDeathBenefitCalculation.icdoBenefitCalculation.ienuObjectState != ObjectState.Insert)
                {
                    lobjPostRetirementDeathBenefitCalculation.icdoBenefitCalculation.ienuObjectState = ObjectState.Insert;
                    lobjPostRetirementDeathBenefitCalculation.iarrChangeLog.Add(lobjPostRetirementDeathBenefitCalculation.icdoBenefitCalculation);
                }
                //PIR 23048
                DateTime ldteCheckTerminationDate = new DateTime(2020, 01, 01);
                if (lobjPostRetirementDeathBenefitCalculation.icdoBenefitCalculation.termination_date < ldteCheckTerminationDate)
                    lobjPostRetirementDeathBenefitCalculation.iblnIsActualTerminationDate = true;
                
                //PIR 25662 Load org details, to show applicant org details on screen
                lobjPostRetirementDeathBenefitCalculation.ibusBenefitApplication.LoadApplicantOrganization();
            }
            return lobjPostRetirementDeathBenefitCalculation;
        }

        public busPostRetirementDeathBenefitCalculation FindPostRetirementDeathBenefitCalculationFinal(int aintBenefitCalculationID)
        {
            busPostRetirementDeathBenefitCalculation lobjPostRetirementDeathBenefitCalculation = new busPostRetirementDeathBenefitCalculation();
            if (lobjPostRetirementDeathBenefitCalculation.FindBenefitCalculation(aintBenefitCalculationID))
            {
                if (lobjPostRetirementDeathBenefitCalculation.icdoBenefitCalculation.benefit_application_id != 0)
                {
                    busPostRetirementDeathApplication lobjPostRetirementDeathApplication = new busPostRetirementDeathApplication();
                    if (lobjPostRetirementDeathApplication.FindBenefitApplication(lobjPostRetirementDeathBenefitCalculation.icdoBenefitCalculation.benefit_application_id))
                        lobjPostRetirementDeathBenefitCalculation.GetCalculationByApplication(lobjPostRetirementDeathApplication);
                }

                lobjPostRetirementDeathBenefitCalculation.LoadOriginatingPayeeAccount();
                lobjPostRetirementDeathBenefitCalculation.ibusOriginatingPayeeAccount.LoadApplication();
                lobjPostRetirementDeathBenefitCalculation.LoadBenefitApplication();
                lobjPostRetirementDeathBenefitCalculation.ibusBenefitApplication.LoadRecipient();
                lobjPostRetirementDeathBenefitCalculation.LoadPersonAccount();
                lobjPostRetirementDeathBenefitCalculation.LoadBenefitCalculationOptions();
                lobjPostRetirementDeathBenefitCalculation.CalculateMemberAge();
                lobjPostRetirementDeathBenefitCalculation.CalculateSpouseAge();
                lobjPostRetirementDeathBenefitCalculation.LoadErrors();
                // Do not Calculate Death Benefit if Action Status is Approved or Cancelled
                //if ((lobjPostRetirementDeathBenefitCalculation.icdoBenefitCalculation.action_status_value != busConstant.BenefitActionStatusApproved) ||
                //    (lobjPostRetirementDeathBenefitCalculation.icdoBenefitCalculation.action_status_value != busConstant.BenefitActionStatusCancelled))
                //    lobjPostRetirementDeathBenefitCalculation.CalculatePostRetirementDeathBenefit();
                lobjPostRetirementDeathBenefitCalculation.LoadBenefitCalculationPayeeApplicationInfo();
                lobjPostRetirementDeathBenefitCalculation.LoadBenefitCalculationOptions();
                lobjPostRetirementDeathBenefitCalculation.LoadBenefitRHICOption();
                lobjPostRetirementDeathBenefitCalculation.LoadMemberTerminationDate();
                //PIR 23048
                DateTime ldteCheckTerminationDate = new DateTime(2020, 01, 01);
                if (lobjPostRetirementDeathBenefitCalculation.icdoBenefitCalculation.termination_date < ldteCheckTerminationDate)
                    lobjPostRetirementDeathBenefitCalculation.iblnIsActualTerminationDate = true;
                //PIR 25662 Load org details, to show applicant org details on screen
                lobjPostRetirementDeathBenefitCalculation.ibusBenefitApplication.LoadApplicantOrganization();

                //PIR 26161
                if (lobjPostRetirementDeathBenefitCalculation.icdoBenefitCalculation.status_value == busConstant.CalculationStatusProcessed &&
                    lobjPostRetirementDeathBenefitCalculation.icdoBenefitCalculation.action_status_value == busConstant.CalculationStatusPendingApproval)
                    lobjPostRetirementDeathBenefitCalculation.icdoBenefitCalculation.status_value = busConstant.ApplicationStatusValid;
            }
            lobjPostRetirementDeathBenefitCalculation.EvaluateInitialLoadRules();
            return lobjPostRetirementDeathBenefitCalculation;
        }

        public busBenefitRhicCombineHealthSplit FindBenefitRhicCombineHealthSplit(int aintbenefitrhiccombinehealthsplitid)
        {
            busBenefitRhicCombineHealthSplit lobjBenefitRhicCombineHealthSplit = new busBenefitRhicCombineHealthSplit();
            if (lobjBenefitRhicCombineHealthSplit.FindBenefitRhicCombineHealthSplit(aintbenefitrhiccombinehealthsplitid))
            {
                lobjBenefitRhicCombineHealthSplit.LoadBenefitRhicCombine();
                lobjBenefitRhicCombineHealthSplit.LoadPersonAccount();
                lobjBenefitRhicCombineHealthSplit.LoadPayeeAccountRetroPayment();
            }

            return lobjBenefitRhicCombineHealthSplit;
        }
    }
}
