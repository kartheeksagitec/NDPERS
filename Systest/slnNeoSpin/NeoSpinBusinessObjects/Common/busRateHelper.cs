using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using System.Reflection;
using Sagitec.CustomDataObjects;

namespace NeoSpin.BusinessObjects
{
    public static class busRateHelper
    {
        public static decimal GetHMOPremiumAmount(int aintOrgPlanID, string astrEmpCategoryValue,
            string astrHMOInsuranceTypeValue, string astrLevelOfCoverageValue, DateTime adtEffectiveDate,
            DataTable adtbCachedHMORate, utlPassInfo aobjPassInfo)
        {
            decimal ldecHMOPremiumAmount = 0;

            //If the Organization Object Not Set, return it with the ZERO Amount.
            if (string.IsNullOrEmpty(astrEmpCategoryValue)) return ldecHMOPremiumAmount;

            if ((astrHMOInsuranceTypeValue != null) &&
                (astrLevelOfCoverageValue != null))
            {

                if (adtbCachedHMORate == null)
                    adtbCachedHMORate = busGlobalFunctions.LoadHMORateCacheData(aobjPassInfo);

                var lenuList = from row in adtbCachedHMORate.AsEnumerable()
                               where row.Field<int>("org_plan_id") == aintOrgPlanID &&
                                     row.Field<string>("hmo_insurance_type_value") == astrHMOInsuranceTypeValue &&
                                     row.Field<string>("emp_category_value") == astrEmpCategoryValue &&
                                     row.Field<string>("level_of_coverage_value") == astrLevelOfCoverageValue &&
                                     row.Field<DateTime>("effective_date") <= adtEffectiveDate
                               select row;
                DataTable ldtbResult = lenuList.AsDataTable();
                if (ldtbResult.Rows.Count > 0)
                    ldecHMOPremiumAmount = ldtbResult.Rows[0].Field<decimal>("premium_amt");
            }
            return ldecHMOPremiumAmount;
        }
        public static decimal GetVisionPremiumAmount(int aintOrgPlanID,
            string astrVisionInsTypeValue, string astrLevelOfCoverageValue,
            DateTime adtEffectiveDate, DataTable adtbCachedVisionRate, utlPassInfo aobjPassInfo)
        {
            decimal ldecVisionPremiumAmount = 0;

            if ((astrVisionInsTypeValue != null) &&
                (astrLevelOfCoverageValue != null))
            {
                if (adtbCachedVisionRate == null)
                    adtbCachedVisionRate = busGlobalFunctions.LoadVisionRateCacheData(aobjPassInfo);

                var lenuList = from row in adtbCachedVisionRate.AsEnumerable()
                               where row.Field<int>("org_plan_id") == aintOrgPlanID &&
                                     row.Field<string>("vision_insurance_type_value") == astrVisionInsTypeValue &&
                                     row.Field<string>("level_of_coverage_value") == astrLevelOfCoverageValue &&
                                     row.Field<DateTime>("effective_date") <= adtEffectiveDate
                               select row;
                DataTable ldtbResult = lenuList.AsDataTable();
                if (ldtbResult.Rows.Count > 0)
                    ldecVisionPremiumAmount = ldtbResult.Rows[0].Field<decimal>("premium_amt");
            }
            return ldecVisionPremiumAmount;
        }


        public static decimal GetDentalPremiumAmount(int aintOrgPlanID, string astrDentalInsTypeValue, string astrLevelOfCoverageValue,
            DateTime adtEffectiveDate, DataTable adtbCachedDentalRate, utlPassInfo aobjPassInfo)
        {
            decimal ldecDentalPremiumAmount = 0;

            if ((astrDentalInsTypeValue != null) &&
                (astrLevelOfCoverageValue != null))
            {
                if (adtbCachedDentalRate == null)
                    adtbCachedDentalRate = busGlobalFunctions.LoadDentalRateCacheData(aobjPassInfo);

                var lenuList = from row in adtbCachedDentalRate.AsEnumerable()
                               where row.Field<int>("org_plan_id") == aintOrgPlanID &&
                                     row.Field<string>("dental_insurance_type_value") == astrDentalInsTypeValue &&
                                     row.Field<string>("level_of_coverage_value") == astrLevelOfCoverageValue &&
                                     row.Field<DateTime>("effective_date") <= adtEffectiveDate
                               select row;
                DataTable ldtbResult = lenuList.AsDataTable();
                if (ldtbResult.Rows.Count > 0)
                    ldecDentalPremiumAmount = ldtbResult.Rows[0].Field<decimal>("premium_amt");
            }
            return ldecDentalPremiumAmount;
        }

        public static decimal GetLifePremiumAmount(string astrLifeInsuranceType, string astrLevelofCoverage,
            decimal adecCoverageAmount, int aintInsuranceAge, int aintOrgPlanId, DateTime adtEffectiveDate, ref decimal adecEmployerPremiumAmount,
            DataTable adtbCachedLifeRate, utlPassInfo aobjPassInfo, ref decimal adecADAndDBasicRate, ref decimal adecADAndDSuppRate)
        {
            decimal ldecLifePremiumAmount = 0;

            if (adtbCachedLifeRate == null)
                adtbCachedLifeRate = busGlobalFunctions.LoadLifeRateCacheData(aobjPassInfo);

            DataRow ldrOrgPlanLifeRate = null;

            var lenuList = from row in adtbCachedLifeRate.AsEnumerable()
                           where row.Field<int>("org_plan_id") == aintOrgPlanId &&
                                 row.Field<string>("life_insurance_type_value") == astrLifeInsuranceType &&
                                 row.Field<string>("level_of_coverage_value") == astrLevelofCoverage &&
                                 aintInsuranceAge >= row.Field<int>("min_age_yrs") &&
                                 aintInsuranceAge <= row.Field<int>("max_age_yrs") &&
                                 row.Field<DateTime>("effective_date") <= adtEffectiveDate
                           select row;
            DataTable ldtbResult = lenuList.AsDataTable();
            if (ldtbResult.Rows.Count > 0)
            {
                ldrOrgPlanLifeRate = ldtbResult.Rows[0];
            }

            if (ldrOrgPlanLifeRate != null)
            {
                //Jeeva : Logic added for PER Coverage Also.
                if (ldrOrgPlanLifeRate.Field<string>("supplemental_rate_criteria_value") == busConstant.Per_Thousand)
                {
                    // Karthik : PIR ID 209 
                    //ldecLifePremiumAmount = ldrOrgPlanLifeRate.Field<decimal>("basic_employee_premium_amt") +
                    //    (ldrOrgPlanLifeRate.Field<decimal>("supplemental_premium_rate_amt") *
                    //     (adecCoverageAmount / 1000)) +
                    //     ldrOrgPlanLifeRate.Field<decimal>("basic_employer_premium_amt");

                    // UAT PIR ID 1081 - 
                    // As per discussion with Satya and client, the 'basic_employer_premium_amt' & 'supplemental_premium_rate_amt' from orgplanlife rate table
                    // is not added in the Supplemental Premium Calculation.

                    //UAT PIR: 1733
                    // As per discussion with Satya, the Basic Employee Premium Amount need not be added in the Life Premium Amount other than basic Istself
                    if (astrLevelofCoverage == busConstant.LevelofCoverage_Basic)
                    {
                        ldecLifePremiumAmount = ldrOrgPlanLifeRate.Field<decimal>("basic_employee_premium_amt") +
                                                    (ldrOrgPlanLifeRate.Field<decimal>("supplemental_premium_rate_amt") * (adecCoverageAmount / 1000));
                    }
                    else
                    {
                        ldecLifePremiumAmount = (ldrOrgPlanLifeRate.Field<decimal>("supplemental_premium_rate_amt") * (adecCoverageAmount / 1000));
                    }

                    adecEmployerPremiumAmount = ldrOrgPlanLifeRate.Field<decimal>("basic_employer_premium_amt");
                }
                else if (ldrOrgPlanLifeRate.Field<string>("supplemental_rate_criteria_value") == busConstant.Per_Coverage)
                {
                    foreach (DataRow ldrRow in ldtbResult.Rows)
                    {
                        if (ldrRow.Field<decimal>("supplemental_coverage_amt") == adecCoverageAmount)
                        {
                            ldecLifePremiumAmount = ldrRow.Field<decimal>("supplemental_premium_amt");
                            adecEmployerPremiumAmount = ldrRow.Field<decimal>("basic_employer_premium_amt");
                            break;
                        }
                    }
                }
                adecADAndDBasicRate = ldrOrgPlanLifeRate["ad_and_d_basic_premium_rate"] == DBNull.Value ? 0.0000M : Convert.ToDecimal(ldrOrgPlanLifeRate["ad_and_d_basic_premium_rate"]);
                adecADAndDSuppRate = ldrOrgPlanLifeRate["ad_and_d_supplemental_premium_rate"] == DBNull.Value ? 0.0000M : Convert.ToDecimal(ldrOrgPlanLifeRate["ad_and_d_supplemental_premium_rate"]);
            }

            return ldecLifePremiumAmount;
        }

        public static decimal GetLTCPremiumAmount(int aintOrgPlanID, string astrLtcInsuranceType, string astrLevelofCoverage, DateTime adtEffectiveDate,
            int aintLTCInsuranceAge, DataTable adtbCachedLtcRate, utlPassInfo aobjPassInfo)
        {
            decimal ldecLtcPremiumAmount = 0;
            if ((astrLtcInsuranceType != null) &&
                (astrLevelofCoverage != null))
            {
                if (adtbCachedLtcRate == null)
                    adtbCachedLtcRate = busGlobalFunctions.LoadLTCRateCacheData(aobjPassInfo);

                var lenuList = from row in adtbCachedLtcRate.AsEnumerable()
                               where row.Field<int>("org_plan_id") == aintOrgPlanID &&
                                     row.Field<string>("ltc_insurance_type_value") == astrLtcInsuranceType &&
                                     row.Field<string>("level_of_coverage_value") == astrLevelofCoverage &&
                                     aintLTCInsuranceAge >= row.Field<int>("min_age_yrs") &&
                                     aintLTCInsuranceAge <= row.Field<int>("max_age_yrs") &&
                                     row.Field<DateTime>("effective_date") <= adtEffectiveDate
                               select row;
                DataTable ldtbResult = lenuList.AsDataTable();
                if (ldtbResult.Rows.Count > 0)
                    ldecLtcPremiumAmount = ldtbResult.Rows[0].Field<decimal>("premium_amt");
            }
            return ldecLtcPremiumAmount;
        }

        /// <summary>
        /// We dont need the History Object here to get the premium, because, we already passing provider org id here.
        /// </summary>
        /// <param name="aintOrgPlanID"></param>
        /// <param name="adtEffectiveDate"></param>
        /// <returns></returns>
        public static decimal GetEAPPremiumAmount(int aintOrgPlanID, DateTime adtEffectiveDate,
            DataTable adtbCachedEapRate, utlPassInfo aobjPassInfo)
        {
            decimal ldecEAPPremiumAmount = 0;
            if (adtbCachedEapRate == null)
                adtbCachedEapRate = busGlobalFunctions.LoadEAPRateCacheData(aobjPassInfo);

            var lenuList = from row in adtbCachedEapRate.AsEnumerable()
                           where row.Field<int>("org_plan_id") == aintOrgPlanID &&
                                 row.Field<string>("eap_insurance_type_value") == busConstant.EAPInsuranceTypeValueRegular &&
                                 row.Field<DateTime>("effective_date") <= adtEffectiveDate
                           select row;

            DataTable ldtbResult = lenuList.AsDataTable();
            if (ldtbResult.Rows.Count > 0)
                ldecEAPPremiumAmount = ldtbResult.Rows[0].Field<decimal>("premium_amt");
            return ldecEAPPremiumAmount;
        }


        public static decimal GetInsurancePremiumAmount(busOrganization aobjOrganization,
            DateTime adtEffectiveDate, int aintPersonAccountId, int aintPlanId,
            ref decimal adecFeeAmount, ref decimal adecBuydownAmount,ref decimal adecMedicarePartDAmount ,ref decimal adecRHICAmount, ref decimal adecOthrRHICAmt, ref decimal adecJSRHICAmt, ref decimal adecHSAAmt , ref decimal adecHSAVendorAmt,
            busPersonAccountLife abusPersonAccountLife,
            busPersonAccountGhdv abusPersonAccountGhdv,
            busPersonAccountLtc abusPersonAccountLtc,
            busPersonAccountEAP abusPersonAccountEap,
            busPersonAccountMedicarePartDHistory abusPersonAccountMedicare,
            utlPassInfo aobjPassInfo, busDBCacheData abusDBCacheData, ref int aintGHDVHistoryID, ref string astrGroupNumber, ref string astrCoverageCodeValue, ref string astrRateStructureCode,
            bool ablnIgnorePremiumForIBS = false)
        {
            decimal ldecPremiumAmount = 0.00M;
            int lintGHDVHistoryID = 0;
            string lstrGroupNumber = string.Empty, lstrCoverageCodeValue = string.Empty, lstrRateStructureCode = string.Empty;

            /* UAT PIR 476, Including other and JS RHIC Amount */
            //Reference Parameters
            adecOthrRHICAmt = 0.00M;
            adecJSRHICAmt = 0.00M;
            /* UAT PIR 476 ends here */

            //PIR 7705
            adecHSAAmt = 0.00M;
            adecHSAVendorAmt = 0.00M;

            //To Avoid Null Exception
            if (abusDBCacheData == null)
                abusDBCacheData = new busDBCacheData();

            if (aintPersonAccountId > 0)
            {
				//PIR 15434
                if ((aintPlanId == busConstant.PlanIdGroupHealth) ||
                    (aintPlanId == busConstant.PlanIdHMO) || (aintPlanId == busConstant.PlanIdDental) ||
                    (aintPlanId == busConstant.PlanIdVision))
                {
                    if (abusPersonAccountGhdv == null)
                    {
                        abusPersonAccountGhdv = new busPersonAccountGhdv();
                        abusPersonAccountGhdv.FindGHDVByPersonAccountID(aintPersonAccountId);
                        abusPersonAccountGhdv.FindPersonAccount(aintPersonAccountId);

                        busPersonAccountGhdvHistory lbusHistory = abusPersonAccountGhdv.LoadHistoryByDate(adtEffectiveDate);
                        abusPersonAccountGhdv = lbusHistory.LoadGHDVObject(abusPersonAccountGhdv);
                    }
                    //uat pir 1429 : post ghdv_history id and group number
                    busPersonAccountGhdvHistory lobjGHDVHistory = abusPersonAccountGhdv.LoadHistoryByDate(adtEffectiveDate);
                    lintGHDVHistoryID = lobjGHDVHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id;
                    lstrCoverageCodeValue = lobjGHDVHistory.icdoPersonAccountGhdvHistory.coverage_code;
                    lstrRateStructureCode = !string.IsNullOrEmpty(lobjGHDVHistory.icdoPersonAccountGhdvHistory.overridden_structure_code) ?
                                    lobjGHDVHistory.icdoPersonAccountGhdvHistory.overridden_structure_code : lobjGHDVHistory.icdoPersonAccountGhdvHistory.rate_structure_code;
                    if (abusPersonAccountGhdv.ibusPaymentElection == null)
                        abusPersonAccountGhdv.LoadPaymentElection();

                    //We must reload the provider org plan here because provider is getting changed in 2011 but no enrollment change.
                    //so if we consider the provider org id at the enrollment level would pick up the old provider which is an issue.
                    if (abusPersonAccountGhdv.ibusProviderOrgPlan == null)
                    {
                        if ((abusPersonAccountGhdv.ibusOrgPlan != null) && (abusPersonAccountGhdv.ibusOrgPlan.icdoOrgPlan.org_plan_id > 0))
                        {
                            abusPersonAccountGhdv.LoadProviderOrgPlan(adtEffectiveDate);
                        }
                        else
                        {
                            //RGroup case
                            abusPersonAccountGhdv.LoadActiveProviderOrgPlan(adtEffectiveDate);
                        }
                    }

                    if (abusPersonAccountGhdv.icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeRetiree ||
                        abusPersonAccountGhdv.icdoPersonAccountGhdv.is_health_cobra || (abusPersonAccountGhdv.IsGroupNumber06ConditionSatisfied()))
                    {
                        lstrGroupNumber = abusPersonAccountGhdv.GetGroupNumber();
                    }
                    //PROD PIR 5308
                    //to get the group number for Superior Vision provider
                    else if (abusPersonAccountGhdv.icdoPersonAccount.plan_id == busConstant.PlanIdVision)
                    {
                        if (busGlobalFunctions.GetOrgCodeFromOrgId(abusPersonAccountGhdv.ibusProviderOrgPlan.icdoOrgPlan.org_id) ==
                                     busGlobalFunctions.GetData1ByCodeValue(1213, busConstant.SuperiorVisionProviderCodeValue, abusPersonAccountGhdv.iobjPassInfo))
                        {
                            lstrGroupNumber = abusPersonAccountGhdv.GetGroupNumber();
                        }
                        //6077 : removal of person account ghdv history id
                        else
                        {
                            lstrGroupNumber = abusPersonAccountGhdv.GetHIPAAReferenceID();
                        }
                    }
                    else if (abusPersonAccountGhdv.icdoPersonAccount.plan_id == busConstant.PlanIdDental)
                    {
                        if (busGlobalFunctions.GetOrgCodeFromOrgId(abusPersonAccountGhdv.ibusProviderOrgPlan.icdoOrgPlan.org_id) ==
                                     busGlobalFunctions.GetData1ByCodeValue(1213, busConstant.DELTAProviderCodeValue, abusPersonAccountGhdv.iobjPassInfo))
                        {
                            lstrGroupNumber = abusPersonAccountGhdv.GetGroupNumber(); // PIR 10448 - new provider
                        }
                        else
                        {
                            lstrGroupNumber = abusPersonAccountGhdv.GetBranch();
                            lstrGroupNumber += abusPersonAccountGhdv.GetBenefitOptionCode();
                        }
                    }
                    
                    if (aintPlanId == busConstant.PlanIdHMO)
                    {
                        if (abusPersonAccountGhdv.icdoPersonAccountGhdv.person_account_ghdv_id > 0)
                        {
                            string lstrEmpCategory = string.Empty;
                            if (aobjOrganization != null)
                                lstrEmpCategory = aobjOrganization.icdoOrganization.emp_category_value;
                            if (abusPersonAccountGhdv.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                            {
                                ldecPremiumAmount =
                                    GetHMOPremiumAmount(abusPersonAccountGhdv.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                                                        lstrEmpCategory,
                                                        abusPersonAccountGhdv.icdoPersonAccountGhdv.hmo_insurance_type_value,
                                                        abusPersonAccountGhdv.icdoPersonAccountGhdv.level_of_coverage_value,
                                                        adtEffectiveDate, abusDBCacheData.idtbCachedHMORate, aobjPassInfo);
                            }
                        }

                    }
                    if (aintPlanId == busConstant.PlanIdDental)
                    {
                        if (abusPersonAccountGhdv.icdoPersonAccountGhdv.person_account_ghdv_id > 0)
                        {
                            //uat pir 2056
                            aintGHDVHistoryID = lintGHDVHistoryID;
                            //prod pir 6076
                            astrCoverageCodeValue = lstrCoverageCodeValue;
                            astrRateStructureCode = lstrRateStructureCode;
                            astrGroupNumber = lstrGroupNumber;
                            if (abusPersonAccountGhdv.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                            {
                                ldecPremiumAmount =
                                    GetDentalPremiumAmount(abusPersonAccountGhdv.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                                                           abusPersonAccountGhdv.icdoPersonAccountGhdv.dental_insurance_type_value,
                                                           abusPersonAccountGhdv.icdoPersonAccountGhdv.level_of_coverage_value,
                                                           adtEffectiveDate, abusDBCacheData.idtbCachedDentalRate, aobjPassInfo);
                            }
                        }
                    }
                    if (aintPlanId == busConstant.PlanIdVision)
                    {
                        if (abusPersonAccountGhdv.icdoPersonAccountGhdv.person_account_ghdv_id > 0)
                        {
                            //uat pir 2056
                            aintGHDVHistoryID = lintGHDVHistoryID;
                            //PROD PIR 5308
                            //to get the group number for Superior Vision provider
                            astrGroupNumber = lstrGroupNumber;
                            //prod pir 6076
                            astrCoverageCodeValue = lstrCoverageCodeValue;
                            astrRateStructureCode = lstrRateStructureCode;

                            if (abusPersonAccountGhdv.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                            {
                                ldecPremiumAmount =
                                    GetVisionPremiumAmount(abusPersonAccountGhdv.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                                                           abusPersonAccountGhdv.icdoPersonAccountGhdv.vision_insurance_type_value,
                                                           abusPersonAccountGhdv.icdoPersonAccountGhdv.level_of_coverage_value,
                                                           adtEffectiveDate, abusDBCacheData.idtbCachedVisionRate, aobjPassInfo);
                            }
                        }
                    }
					//PIR 15434
                    if (aintPlanId == busConstant.PlanIdGroupHealth)
                    {
                        if (abusPersonAccountGhdv.icdoPersonAccountGhdv.person_account_ghdv_id > 0)
                        {
                            //Loading the DB Cache Date If Available (Performance Optimization)
                            if (abusDBCacheData != null)
                            {
                                abusPersonAccountGhdv.idtbCachedRateStructureRef = abusDBCacheData.idtbCachedRateStructureRef;
                                abusPersonAccountGhdv.idtbCachedRateRef = abusDBCacheData.idtbCachedRateRef;
                                abusPersonAccountGhdv.idtbCachedCoverageRef = abusDBCacheData.idtbCachedCoverageRef;
                                abusPersonAccountGhdv.idtbCachedHealthRate = abusDBCacheData.idtbCachedHealthRate;
                            }

                            if (abusPersonAccountGhdv.ibusOrgPlan == null)
                                abusPersonAccountGhdv.LoadOrgPlan(adtEffectiveDate, aobjOrganization.icdoOrganization.org_id);

                            abusPersonAccountGhdv.idtPlanEffectiveDate = adtEffectiveDate;
                            if (abusPersonAccountGhdv.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                            {
                                abusPersonAccountGhdv.LoadRateStructureForUserStructureCode();
                            }
                            else
                            {
                                abusPersonAccountGhdv.LoadHealthParticipationDate();
                                abusPersonAccountGhdv.LoadRateStructure(adtEffectiveDate);
                            }
                            abusPersonAccountGhdv.LoadCoverageRefID();

                            if (abusPersonAccountGhdv.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                            {
                                abusPersonAccountGhdv.GetMonthlyPremiumAmountByRefID(adtEffectiveDate);
                            }

                            ldecPremiumAmount = abusPersonAccountGhdv.icdoPersonAccountGhdv.PremiumExcludingFeeAmount;
                            adecFeeAmount = abusPersonAccountGhdv.icdoPersonAccountGhdv.FeeAmount;
                            adecBuydownAmount = abusPersonAccountGhdv.icdoPersonAccountGhdv.BuydownAmount;
                            adecMedicarePartDAmount = abusPersonAccountGhdv.icdoPersonAccountGhdv.MedicarePartDAmount;//PIR 14271
                            //pir 7705
                            adecHSAAmt = abusPersonAccountGhdv.icdoPersonAccountGhdv.idecHSAAmount;
                            adecHSAVendorAmt = abusPersonAccountGhdv.icdoPersonAccountGhdv.idecHSAVendorAmount;
                            //Get the RHIC Amount
                            adecRHICAmount = abusPersonAccountGhdv.icdoPersonAccountGhdv.total_rhic_amount;
                            adecJSRHICAmt = abusPersonAccountGhdv.icdoPersonAccountGhdv.js_rhic_amount;
                            adecOthrRHICAmt = abusPersonAccountGhdv.icdoPersonAccountGhdv.other_rhic_amount;
                            //uat pir 1429
                            aintGHDVHistoryID = lintGHDVHistoryID;
                            astrGroupNumber = lstrGroupNumber;
                            //prod pir 6076
                            astrCoverageCodeValue = abusPersonAccountGhdv.icdoPersonAccountGhdv.coverage_code;
                            astrRateStructureCode = !string.IsNullOrEmpty(abusPersonAccountGhdv.icdoPersonAccountGhdv.overridden_structure_code) ?
                                    abusPersonAccountGhdv.icdoPersonAccountGhdv.overridden_structure_code : abusPersonAccountGhdv.icdoPersonAccountGhdv.rate_structure_code;

                            if (ablnIgnorePremiumForIBS)
                            {
                                if (abusPersonAccountGhdv.ibusPaymentElection == null)
                                    abusPersonAccountGhdv.LoadPaymentElection();

                                if ((abusPersonAccountGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes) &&
                                    (abusPersonAccountGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date >= new DateTime(adtEffectiveDate.Year, adtEffectiveDate.Month, 1)))
                                {
                                    ldecPremiumAmount = 0.00M;
                                    adecFeeAmount = 0.00M;
                                }
                            }
                        }
                    }
                }
				//PIR 15434
                if (aintPlanId == busConstant.PlanIdMedicarePartD)
                {
                    if (abusPersonAccountMedicare == null)
                    {
                        abusPersonAccountMedicare = new busPersonAccountMedicarePartDHistory();
                        abusPersonAccountMedicare.FindMedicareByPersonAccountIDAndEffectiveDate(aintPersonAccountId, adtEffectiveDate);
                        abusPersonAccountMedicare.FindPersonAccount(aintPersonAccountId);
                    }

                    abusPersonAccountMedicare.LoadActiveProviderOrgPlan(adtEffectiveDate);

                    if (abusPersonAccountMedicare.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                    {
                        abusPersonAccountMedicare.GetMonthlyPremiumAmountForMedicarePartD(adtEffectiveDate);
                        ldecPremiumAmount = abusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.MonthlyPremiumAmount;

                        if (ablnIgnorePremiumForIBS)
                        {
                            if (abusPersonAccountMedicare.ibusPaymentElection == null)
                                abusPersonAccountMedicare.LoadPaymentElection();

                            if ((abusPersonAccountMedicare.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes) &&
                                (abusPersonAccountMedicare.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date >= new DateTime(adtEffectiveDate.Year, adtEffectiveDate.Month, 1)))
                            {
                                ldecPremiumAmount = 0.00M;
                            }
                        }
                    }
                }

                if (aintPlanId == busConstant.PlanIdEAP)
                {
                    if (abusPersonAccountEap == null)
                    {
                        abusPersonAccountEap = new busPersonAccountEAP();
                        abusPersonAccountEap.FindPersonAccount(aintPersonAccountId);

                        busPersonAccountEAPHistory lbusHistory = abusPersonAccountEap.LoadHistoryByDate(adtEffectiveDate);
                        abusPersonAccountEap = lbusHistory.LoadEAPObject(abusPersonAccountEap);
                    }

                    //For EAP, we must choose always the selected provider.
                    if (abusPersonAccountEap.ibusProviderOrgPlan == null)
                        abusPersonAccountEap.LoadProviderOrgPlanByProviderOrgID(abusPersonAccountEap.icdoPersonAccount.provider_org_id, adtEffectiveDate);

                    if (abusPersonAccountEap.idtbCachedEapRate == null)
                        abusPersonAccountEap.idtbCachedEapRate = abusDBCacheData.idtbCachedEapRate;
                    if (abusPersonAccountEap.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                    {
                        ldecPremiumAmount =
                            GetEAPPremiumAmount(abusPersonAccountEap.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                                                adtEffectiveDate, abusDBCacheData.idtbCachedEapRate, aobjPassInfo);

                        if (ablnIgnorePremiumForIBS)
                        {
                            if (abusPersonAccountEap.ibusPaymentElection == null)
                                abusPersonAccountEap.LoadPaymentElection();

                            if ((abusPersonAccountEap.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes) &&
                                (abusPersonAccountEap.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date >= new DateTime(adtEffectiveDate.Year, adtEffectiveDate.Month, 1)))
                            {
                                ldecPremiumAmount = 0.00M;
                            }
                        }
                    }
                }

                if (aintPlanId == busConstant.PlanIdLTC)
                {
                    if (abusPersonAccountLtc == null)
                    {
                        abusPersonAccountLtc = new busPersonAccountLtc();
                        abusPersonAccountLtc.FindPersonAccount(aintPersonAccountId);
                        //Note : For LTC and Life , History Object will get loaded at the time MonthlyPremiumAmount Calculation and that checks the Participation Status
                    }

                    if (abusPersonAccountLtc.ibusProviderOrgPlan == null)
                    {
                        if ((abusPersonAccountLtc.ibusOrgPlan != null) && (abusPersonAccountLtc.ibusOrgPlan.icdoOrgPlan.org_plan_id > 0))
                        {
                            abusPersonAccountLtc.LoadProviderOrgPlan(adtEffectiveDate);
                        }
                        else
                        {
                            //RGroup case
                            abusPersonAccountLtc.LoadActiveProviderOrgPlan(adtEffectiveDate);
                        }
                    }

                    if (abusPersonAccountLtc.idtbCachedLtcRate == null)
                        abusPersonAccountLtc.idtbCachedLtcRate = abusDBCacheData.idtbCachedLtcRate;

                    abusPersonAccountLtc.LoadMemberAge(adtEffectiveDate);
                    abusPersonAccountLtc.LoadLtcOptionUpdateMemberFromHistory(adtEffectiveDate);
                    abusPersonAccountLtc.LoadLtcOptionUpdateSpouseFromHistory(adtEffectiveDate);
                    if (abusPersonAccountLtc.iintSpousePERSLinkID != 0)
                    {
                        abusPersonAccountLtc.LoadSpouseAge(abusPersonAccountLtc.iintSpousePERSLinkID);
                    }
                    abusPersonAccountLtc.GetMonthlyPremiumAmount(adtEffectiveDate);
                    ldecPremiumAmount = abusPersonAccountLtc.idecTotalMonthlyPremium;

                    if (ablnIgnorePremiumForIBS)
                    {
                        if (abusPersonAccountLtc.ibusPaymentElection == null)
                            abusPersonAccountLtc.LoadPaymentElection();

                        if ((abusPersonAccountLtc.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes) &&
                            (abusPersonAccountLtc.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date >= new DateTime(adtEffectiveDate.Year, adtEffectiveDate.Month, 1)))
                        {
                            ldecPremiumAmount = 0.00M;
                        }
                    }
                }

                if (aintPlanId == busConstant.PlanIdGroupLife)
                {
                    if (abusPersonAccountLife == null)
                    {
                        abusPersonAccountLife = new busPersonAccountLife();
                        abusPersonAccountLife.FindPersonAccountLife(aintPersonAccountId);
                        abusPersonAccountLife.FindPersonAccount(aintPersonAccountId);
                        //Note : For LTC and Life , History Object will get loaded at the time MonthlyPremiumAmount Calculation and that checks the Participation Status
                    }

                    if (abusPersonAccountLife.ibusProviderOrgPlan == null)
                    {
                        if ((abusPersonAccountLife.ibusOrgPlan != null) && (abusPersonAccountLife.ibusOrgPlan.icdoOrgPlan.org_plan_id > 0))
                        {
                            abusPersonAccountLife.LoadProviderOrgPlan(adtEffectiveDate);
                        }
                        else
                        {
                            //RGroup case
                            abusPersonAccountLife.LoadActiveProviderOrgPlan(adtEffectiveDate);
                        }
                    }

                    if (abusPersonAccountLife.idtbCachedLifeRate == null)
                        abusPersonAccountLife.idtbCachedLifeRate = abusDBCacheData.idtbCachedLifeRate;

                    if (abusPersonAccountLife.iclbLifeOption == null)
                        abusPersonAccountLife.LoadLifeOptionDataFromHistory(adtEffectiveDate);

                    abusPersonAccountLife.LoadMemberAge(adtEffectiveDate);

                    abusPersonAccountLife.GetMonthlyPremiumAmount(adtEffectiveDate);
                    ldecPremiumAmount = abusPersonAccountLife.idecTotalMonthlyPremium;

                    if (ablnIgnorePremiumForIBS)
                    {
                        if (abusPersonAccountLife.ibusPaymentElection == null)
                            abusPersonAccountLife.LoadPaymentElection();

                        if ((abusPersonAccountLife.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes) &&
                            (abusPersonAccountLife.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date >= new DateTime(adtEffectiveDate.Year, adtEffectiveDate.Month, 1)))
                        {
                            ldecPremiumAmount = 0.00M;
                        }
                    }
                }
            }
            return ldecPremiumAmount;
        }

        public static busPlanRetirementRate GetRatesForMemberTypeAndEffectiveDate(string astrMemberType, DateTime adtEffectiveDate, int aintPlanID)
        {
            busPlanRetirementRate lobjPlanRetirement = new busPlanRetirementRate();
            lobjPlanRetirement.icdoPlanRetirementRate = new cdoPlanRetirementRate();
            DataTable ldtGetRatesForEffectiveDate = busBase.SelectWithOperator<cdoPlanRetirementRate>
                                 (
                                    new string[3] { "member_type_value", "effective_date", "plan_id" },
                                    new string[3] { "=", "<=", "=" },
                                    new object[3] {astrMemberType
                                        , adtEffectiveDate, aintPlanID}, "effective_date desc"
                                 );
            if (ldtGetRatesForEffectiveDate != null)
            {
                if (ldtGetRatesForEffectiveDate.Rows.Count > 0)
                {
                    // get rates for fisrt record
                    lobjPlanRetirement.icdoPlanRetirementRate.LoadData(ldtGetRatesForEffectiveDate.Rows[0]);
                }
            }
            return lobjPlanRetirement;
        }

        //Used only in Deduction Screen (Special Cases)
        public static decimal GetDentalVisionPremiumAmount(string astrSql, string astrDentalInsuranceType, string astrLevelofCoverage, int aintOrgPlanid, DateTime adtEffectiveDate)
        {
            busBase lobjBase = new busBase();
            return Convert.ToDecimal(DBFunction.DBExecuteScalar(astrSql,
                                                        new object[4] { astrDentalInsuranceType, astrLevelofCoverage, aintOrgPlanid, adtEffectiveDate },
                                                        lobjBase.iobjPassInfo.iconFramework, lobjBase.iobjPassInfo.itrnFramework));
        }

        public static decimal GetHealthPremiumAmount(int aintCoveragRefID, DateTime adtEffectiveDate, decimal adecLowIncomeCredit,
                   ref decimal adecFeeAmount, ref decimal adecBuydownAmount, ref decimal adecMedicarePartDAmount,ref decimal adecHealthSavingsAmount, ref decimal adecHSAVendorAmount,
                   DataTable adtbCachedHealthRate, utlPassInfo iobjPassInfo)
        {
            decimal ldecHealthPremiumAmount = 0;
            if (adtbCachedHealthRate == null)
            {
                adtbCachedHealthRate = busGlobalFunctions.LoadHealthRateCacheData(iobjPassInfo);
            }

            var lenuList = from row in adtbCachedHealthRate.AsEnumerable()
                           where
                               row.Field<int>("org_plan_group_health_medicare_part_d_coverage_ref_id") ==
                               aintCoveragRefID &&
                               busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate,
                                                                       row.Field<DateTime>("premium_period_start_date"),
                                                                       row.Field<DateTime>("premium_period_end_date")) &&
                                row.Field<DateTime>("effective_date") <= adtEffectiveDate
                           select row;

            DataTable ldtbResult = lenuList.AsDataTable();
            if (ldtbResult.Rows.Count > 0)
            {
                ldecHealthPremiumAmount = ldtbResult.Rows[0].Field<decimal>("provider_premium_amt");
                adecFeeAmount = ldtbResult.Rows[0].Field<decimal>("fee_amt");
                adecBuydownAmount = ldtbResult.Rows[0].Field<decimal>("buydown_amount");
                adecMedicarePartDAmount = ldtbResult.Rows[0].Field<decimal>("medicare_part_d_amt");
                if(!ldtbResult.Rows[0].Field<decimal?>("health_savings_amount").IsNull())
                    adecHealthSavingsAmount = ldtbResult.Rows[0].Field<decimal>("health_savings_amount");
                if (!ldtbResult.Rows[0].Field<decimal?>("health_savings_vendor_amount").IsNull())
                    adecHSAVendorAmount = ldtbResult.Rows[0].Field<decimal>("health_savings_vendor_amount");
            }

            ///As per PIR 1758 , the low income credit prorated by RAS is not always been consistent. 
            ///Hence a new table added to get the amount for lowincomecredit across effective dates
            ///Code ID 367 will no longer be used.
            if (adecLowIncomeCredit > 0)
            {
                Decimal ldecLowIncomeCreditAmount = 0; 
                DataTable adtbCachedLowIncomeCreditRef = busGlobalFunctions.LoadLowIncomeCreditRefCacheData(iobjPassInfo);
                var lenumList = adtbCachedLowIncomeCreditRef.AsEnumerable().Where(i => i.Field<Decimal>("low_income_credit") == adecLowIncomeCredit).OrderByDescending(i => i.Field<DateTime>("effective_date"));
                foreach (DataRow dr in lenumList)
                {
                    if (Convert.ToDateTime(dr["effective_date"]).Date <= adtEffectiveDate.Date)
                    {
                        ldecLowIncomeCreditAmount = Convert.ToDecimal(dr["amount"]);
                        break;
                    }
                }
                ldecHealthPremiumAmount = ldecHealthPremiumAmount - ldecLowIncomeCreditAmount;
                //adecMedicarePartDAmount = adecMedicarePartDAmount - ldecLowIncomeCreditAmount; //PIR 14271 Commented as low income credit amount was being deduced twice.
            }

            /*
            if (adecLowIncomeCredit > 0)
            {
                //Load the Low Income Credit Based on the Effecitve Date
                DataTable ldtbList = iobjPassInfo.isrvDBCache.GetCodeValues(367);
                decimal ldecCredit = 0;
                DateTime ldtPreviousEffectiveDate = DateTime.MinValue;
                foreach (DataRow ldtRow in ldtbList.Rows)
                {
                    if ((!Convert.IsDBNull((ldtRow["Data1"]))) && (!Convert.IsDBNull((ldtRow["Data2"]))))
                    {
                        DateTime ldtEffectiveDate = Convert.ToDateTime(ldtRow["Data2"].ToString());
                        if (ldtEffectiveDate > adtEffectiveDate) continue;

                        if (ldtEffectiveDate > ldtPreviousEffectiveDate)
                        {
                            ldtPreviousEffectiveDate = ldtEffectiveDate;
                            ldecCredit = Convert.ToDecimal(ldtRow["Data1"].ToString());
                        }
                    }
                }
                //UAT PIR 1627
                decimal ldecLowIncomeCredit = 0.00M;
                if (adecLowIncomeCredit - decimal.Truncate(adecLowIncomeCredit) > 0)
                {
                    // PROD PIR ID 5839
                    ldecLowIncomeCredit = Math.Round(adecLowIncomeCredit * ldecCredit, 1, MidpointRounding.AwayFromZero);
                }
                else
                {
                    ldecLowIncomeCredit = adecLowIncomeCredit * ldecCredit;
                }

                ldecHealthPremiumAmount = ldecHealthPremiumAmount - ldecLowIncomeCredit;
                adecMedicarePartDAmount = adecMedicarePartDAmount - ldecLowIncomeCredit;
            }*/
            return ldecHealthPremiumAmount;
        }

        public static decimal GetMedicarePartDPremiumAmount(DateTime adtEffectiveDate, decimal adecLowIncomeCredit,
                   DataTable adtbCachedMedicarePartDRate, utlPassInfo iobjPassInfo)
        {
            decimal ldecHealthPremiumAmount = 0;
            if (adtbCachedMedicarePartDRate == null)
            {
                adtbCachedMedicarePartDRate = busGlobalFunctions.LoadMedicarePartDRateCacheData(iobjPassInfo);
            }

            var lenuList = from row in adtbCachedMedicarePartDRate.AsEnumerable().Where(i=>i.Field<DateTime>("Effective_Date") <= adtEffectiveDate) //PIR 15683
                           select row;

            DataTable ldtbResult = lenuList.AsDataTable();
            if (ldtbResult.Rows.Count > 0)
                ldecHealthPremiumAmount = ldtbResult.Rows[0].Field<decimal>("medicare_part_d_rate");

            if (adecLowIncomeCredit > 0)
            {
                Decimal ldecLowIncomeCreditAmount = 0;
                DataTable adtbCachedLowIncomeCreditRef = busGlobalFunctions.LoadLowIncomeCreditRefCacheData(iobjPassInfo);
                var lenumList = adtbCachedLowIncomeCreditRef.AsEnumerable().Where(i => i.Field<Decimal>("low_income_credit") == adecLowIncomeCredit).OrderByDescending(i => i.Field<DateTime>("effective_date"));
                foreach (DataRow dr in lenumList)
                {
                    if (Convert.ToDateTime(dr["effective_date"]).Date <= adtEffectiveDate.Date)
                    {
                        ldecLowIncomeCreditAmount = Convert.ToDecimal(dr["amount"]);
                        break;
                    }
                }
                ldecHealthPremiumAmount = ldecHealthPremiumAmount - ldecLowIncomeCreditAmount;
            }
            return ldecHealthPremiumAmount;
        }

        //This method is used to insert the premium amount from Ref table without adding LEP and subtracting LIS into SGT_PROVIDER_REPORT_DATA_MEDICARE_PART_D. 
        public static decimal GetMedicarePartDPremiumAmountFromRef(DateTime adtEffectiveDate,DataTable adtbCachedMedicarePartDRate, utlPassInfo iobjPassInfo)
        {
            decimal ldecHealthPremiumAmount = 0;
            if (adtbCachedMedicarePartDRate == null)
            {
                adtbCachedMedicarePartDRate = busGlobalFunctions.LoadMedicarePartDRateCacheData(iobjPassInfo);
            }

            var lenuList = from row in adtbCachedMedicarePartDRate.AsEnumerable().Where(i => i.Field<DateTime>("Effective_Date") <= adtEffectiveDate) //PIR 15786
                           select row;

            DataTable ldtbResult = lenuList.AsDataTable();
            if (ldtbResult.Rows.Count > 0)
                ldecHealthPremiumAmount = ldtbResult.Rows[0].Field<decimal>("medicare_part_d_rate");

            return ldecHealthPremiumAmount;
        }
    }
}
