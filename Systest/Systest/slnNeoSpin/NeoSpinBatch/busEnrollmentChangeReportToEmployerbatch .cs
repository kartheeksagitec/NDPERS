using System;
using System.Collections.ObjectModel;
using System.Data;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Linq;

namespace NeoSpinBatch
{
    class busEnrollmentChangeReportToEmployerbatch : busNeoSpinBatch
    {
        DataTable idtResultTable = new DataTable();
        public busDBCacheData ibusDBCacheData { get; set; }
        public Collection<busOrgPlan> iclbProviderOrgPlan { get; set; }
        public DataTable idtbPALifeOption { get; set; }
        public DataTable idtbGHDVHistory { get; set; }
        public DataTable idtbLifeHistory { get; set; }

        public void GenerateReports()
        {
            istrProcessName = "Generating Enrollment Change Report To Employer";
            idlgUpdateProcessLog("Generating Enrollment Change Report To Employer", "INFO", istrProcessName);

            LoadEnrollmentChangeDatatable();

            if (idtResultTable.Rows.Count > 0)
            {
                CreateReport("rptEnrollmentChangeReportToEmployer.rpt", idtResultTable);
                idlgUpdateProcessLog("Generated Enrollment Change Report To Employer successfully", "INFO", istrProcessName);
            }
            else
                idlgUpdateProcessLog("No Enrollment Change Report To Employer generated", "INFO", istrProcessName);
        }

        private void LoadEnrollmentChangeDatatable()
        {
            DateTime ldtLastBatchRunDate = DateTime.MinValue;
            string lstrStepParameter = iobjBatchSchedule.step_parameters;
            if (lstrStepParameter.IsNullOrEmpty())
                lstrStepParameter = DateTime.MinValue.ToString();
            bool IsLastDateExists = DateTime.TryParse(lstrStepParameter, out ldtLastBatchRunDate);
            if (IsLastDateExists)
            {
                if (ldtLastBatchRunDate == DateTime.MinValue)
                    ldtLastBatchRunDate = new DateTime(1900, 1, 1);
                DataTable ldtEnrollmentRequest = busBase.Select("cdoWssPersonAccountEnrollmentRequest.rptEnrollmentChangeReportToEmployer",
                    new object[1] { ldtLastBatchRunDate });
                //ldtEnrollmentRequest.DataSet.Tables.RemoveAt(0);
                idtResultTable = new DataTable();
                idtResultTable.TableName = busConstant.ReportTableName;
                idtResultTable.Columns.Add("Employer_Org", Type.GetType("System.String"));
                idtResultTable.Columns.Add("Employer_Org_Code", Type.GetType("System.String"));
                idtResultTable.Columns.Add("Perslink_id", Type.GetType("System.Int32"));
                idtResultTable.Columns.Add("Person_Name", Type.GetType("System.String"));
                idtResultTable.Columns.Add("Plan_Name", Type.GetType("System.String"));
                idtResultTable.Columns.Add("Level_Of_Coverage", Type.GetType("System.String"));
                //idtResultTable.Columns.Add("Coverage_Code", Type.GetType("System.String"));
                idtResultTable.Columns.Add("Premium_Amount", Type.GetType("System.Decimal"));
                idtResultTable.Columns.Add("Effective_Change_Date", Type.GetType("System.DateTime"));

                foreach (DataRow ldrRow in ldtEnrollmentRequest.Rows)
                    SetFieldsbyPlan(ldrRow);

            }
        }

        private void SetFieldsbyPlan(DataRow adrRow)
        {
            //load all objects
            busWssPersonAccountEnrollmentRequest lobjWSSPersonAccountEnrollmentRequest = new busWssPersonAccountEnrollmentRequest { icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest() };
            lobjWSSPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.LoadData(adrRow);

            //busPersonAccount lobjPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            //lobjPersonAccount.icdoPersonAccount.LoadData(adrRow);

            busPerson lbusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lbusPerson.icdoPerson.LoadData(adrRow);

            busPlan lbusPlan = new busPlan { icdoPlan = new cdoPlan() };
            lbusPlan.icdoPlan.LoadData(adrRow);

            idlgUpdateProcessLog("Processing for person - " + lbusPerson.icdoPerson.FullName + " and plan " + lbusPlan.icdoPlan.plan_name, "INFO", iobjBatchSchedule.step_name);

            //Get Premium Amount
            int lintPlanId = lbusPlan.icdoPlan.plan_id;
            DateTime ldtEffectiveChangeDate = DateTime.MinValue;
            decimal ldecPremiumAmount = 0.00M;

            decimal ldecLifeBasicPremiumAmt = 0.00M;
            decimal ldecLifeSuppPremiumAmt = 0.00M;
            decimal ldecLifeSpouseSuppPremiumAmt = 0.00M;
            decimal ldecLifeDepSuppPremiumAmt = 0.00M;

            lobjWSSPersonAccountEnrollmentRequest.LoadPersonAccount();
            lobjWSSPersonAccountEnrollmentRequest.LoadPersonEmploymentDetail();
            lobjWSSPersonAccountEnrollmentRequest.ibusPersonEmploymentDetail.LoadPersonEmployment();
            lobjWSSPersonAccountEnrollmentRequest.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();


            if (lbusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeInsurance)
            {
                ldtEffectiveChangeDate = Convert.ToDateTime(adrRow["Effective_Change_Date"]);

                //Loading DB Cache (optimization)
                idlgUpdateProcessLog("Loading DB Cache Data", "INFO", istrProcessName);
                LoadDBCacheData();

                idlgUpdateProcessLog("Loading All Active Providers", "INFO", iobjBatchSchedule.step_name);
                //Loading Complete Activte Provider Org Plan List (Optimization Purpose)
                LoadActiveProviders(ldtEffectiveChangeDate);

                ldecPremiumAmount = GetPremiums(ldtEffectiveChangeDate, lobjWSSPersonAccountEnrollmentRequest, ref ldecLifeBasicPremiumAmt,
                   ref ldecLifeSuppPremiumAmt, ref ldecLifeSpouseSuppPremiumAmt, ref ldecLifeDepSuppPremiumAmt);

                if (lintPlanId != busConstant.PlanIdGroupLife)
                {

                    lobjWSSPersonAccountEnrollmentRequest.LoadMSSGHDV();
                    lobjWSSPersonAccountEnrollmentRequest.LoadPersonAccountGHDV();

                    if (lintPlanId == busConstant.PlanIdGroupHealth)
                        lobjWSSPersonAccountEnrollmentRequest.LoadCoverageCodeDescription();

                    DataRow dr = idtResultTable.NewRow();
                    dr["Employer_Org"] = adrRow["Employer_Org"].ToString();
                    dr["Employer_Org_Code"] = adrRow["Employer_Org_Code"].ToString();
                    dr["Person_Name"] = adrRow["Person_Name"].ToString();
                    dr["Perslink_id"] = Convert.ToInt32(adrRow["Perslink_id"]);
                    dr["Plan_Name"] = adrRow["Plan_Name"].ToString();
                    dr["Effective_Change_Date"] = ldtEffectiveChangeDate;
                    if (!String.IsNullOrEmpty(adrRow["Level_Of_Coverage"].ToString()))
                        dr["Level_Of_Coverage"] = adrRow["Level_Of_Coverage"].ToString();
                    else
                        dr["Level_Of_Coverage"] = lobjWSSPersonAccountEnrollmentRequest.ibusMSSPersonAccountGHDV.istrCoverageCode;
                    dr["Premium_Amount"] = ldecPremiumAmount;
                    idtResultTable.Rows.Add(dr);
                }
                else
                {
                    if (lintPlanId == busConstant.PlanIdGroupLife)
                    {
                        lobjWSSPersonAccountEnrollmentRequest.LoadMSSLifeOptions();

                        if (lobjWSSPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.coverage_amount != 0.00M
                            && ldecLifeBasicPremiumAmt != 0.00M)
                        {
                            DataRow dr = idtResultTable.NewRow();
                            dr["Employer_Org"] = adrRow["Employer_Org"].ToString();
                            dr["Employer_Org_Code"] = adrRow["Employer_Org_Code"].ToString();
                            dr["Person_Name"] = adrRow["Person_Name"].ToString();
                            dr["Perslink_id"] = Convert.ToInt32(adrRow["Perslink_id"]);
                            dr["Plan_Name"] = adrRow["Plan_Name"].ToString();
                            dr["Effective_Change_Date"] = ldtEffectiveChangeDate;
                            dr["Level_Of_Coverage"] = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(408, busConstant.LevelofCoverage_Basic);
                            dr["Premium_Amount"] = ldecLifeBasicPremiumAmt;
                            idtResultTable.Rows.Add(dr);
                        }
                        if (lobjWSSPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount != 0.00M
                            && ldecLifeSuppPremiumAmt != 0.00M)
                        {
                            DataRow dr = idtResultTable.NewRow();
                            dr["Employer_Org"] = adrRow["Employer_Org"].ToString();
                            dr["Employer_Org_Code"] = adrRow["Employer_Org_Code"].ToString();
                            dr["Person_Name"] = adrRow["Person_Name"].ToString();
                            dr["Perslink_id"] = Convert.ToInt32(adrRow["Perslink_id"]);
                            dr["Plan_Name"] = adrRow["Plan_Name"].ToString();
                            dr["Effective_Change_Date"] = ldtEffectiveChangeDate;
                            dr["Level_Of_Coverage"] = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(408, busConstant.LevelofCoverage_Supplemental);
                            dr["Premium_Amount"] = ldecLifeSuppPremiumAmt;
                            idtResultTable.Rows.Add(dr);
                        }
                        if (lobjWSSPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount != 0.00M
                            && ldecLifeSpouseSuppPremiumAmt != 0.00M)
                        {
                            DataRow dr = idtResultTable.NewRow();
                            dr["Employer_Org"] = adrRow["Employer_Org"].ToString();
                            dr["Employer_Org_Code"] = adrRow["Employer_Org_Code"].ToString();
                            dr["Person_Name"] = adrRow["Person_Name"].ToString();
                            dr["Perslink_id"] = Convert.ToInt32(adrRow["Perslink_id"]);
                            dr["Plan_Name"] = adrRow["Plan_Name"].ToString();
                            dr["Effective_Change_Date"] = ldtEffectiveChangeDate;
                            dr["Level_Of_Coverage"] = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(408, busConstant.LevelofCoverage_SpouseSupplemental);
                            dr["Premium_Amount"] = ldecLifeSpouseSuppPremiumAmt;

                            idtResultTable.Rows.Add(dr);
                        }
                        if (!String.IsNullOrEmpty(lobjWSSPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_coverage_option_value)
                            && ldecLifeDepSuppPremiumAmt != 0.00M)
                        {
                            DataRow dr = idtResultTable.NewRow();
                            dr["Employer_Org"] = adrRow["Employer_Org"].ToString();
                            dr["Employer_Org_Code"] = adrRow["Employer_Org_Code"].ToString();
                            dr["Person_Name"] = adrRow["Person_Name"].ToString();
                            dr["Perslink_id"] = Convert.ToInt32(adrRow["Perslink_id"]);
                            dr["Plan_Name"] = adrRow["Plan_Name"].ToString();
                            dr["Effective_Change_Date"] = ldtEffectiveChangeDate;
                            dr["Level_Of_Coverage"] = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(408, busConstant.LevelofCoverage_DependentSupplemental);
                            dr["Premium_Amount"] = ldecLifeDepSuppPremiumAmt;

                            idtResultTable.Rows.Add(dr);
                        }
                    }
                }
                if (lbusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeFlex)
                {
                    if (lintPlanId == busConstant.PlanIdFlex)
                    {
                        lobjWSSPersonAccountEnrollmentRequest.LoadMSSFlexComp();

                        lobjWSSPersonAccountEnrollmentRequest.LoadMssFlexCompOption();

                        if (lobjWSSPersonAccountEnrollmentRequest.icdoMSSFlexCompOption.dependent_annual_pledge_amount != 0.00M)
                        {
                            DataRow dr = idtResultTable.NewRow();
                            dr["Employer_Org"] = adrRow["Employer_Org"].ToString();
                            dr["Employer_Org_Code"] = adrRow["Employer_Org_Code"].ToString();
                            dr["Person_Name"] = adrRow["Person_Name"].ToString();
                            dr["Perslink_id"] = Convert.ToInt32(adrRow["Perslink_id"]);
                            dr["Plan_Name"] = adrRow["Plan_Name"].ToString();
                            dr["Effective_Change_Date"] = ldtEffectiveChangeDate;
                            dr["Level_Of_Coverage"] = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(408, busConstant.FlexLevelOfCoverageDependentSpending);
                            dr["Premium_Amount"] = 0.00M;

                            idtResultTable.Rows.Add(dr);
                        }
                        if (lobjWSSPersonAccountEnrollmentRequest.icdoMSSFlexCompOption.medical_annual_pledge_amount != 0.00M)
                        {
                            DataRow dr = idtResultTable.NewRow();
                            dr["Employer_Org"] = adrRow["Employer_Org"].ToString();
                            dr["Employer_Org_Code"] = adrRow["Employer_Org_Code"].ToString();
                            dr["Person_Name"] = adrRow["Person_Name"].ToString();
                            dr["Perslink_id"] = Convert.ToInt32(adrRow["Perslink_id"]);
                            dr["Plan_Name"] = adrRow["Plan_Name"].ToString();
                            dr["Effective_Change_Date"] = ldtEffectiveChangeDate;
                            dr["Level_Of_Coverage"] = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(408, busConstant.FlexLevelOfCoverageMedicareSpending);
                            dr["Premium_Amount"] = 0.00M;

                            idtResultTable.Rows.Add(dr);
                        }
                    }
                }
            }
            else
            {
                bool IsSuccess = false;
                string lstrEffectiveFromdate = adrRow["EFFECTIVE_FROM_DATE"].ToString();
                string lstrAcknowledgementCdate = adrRow["ACKNOLWEDGEMENT_PART_C_DATE"].ToString();
                IsSuccess = DateTime.TryParse(lstrEffectiveFromdate, out ldtEffectiveChangeDate);
                if (!IsSuccess)
                    IsSuccess = DateTime.TryParse(lstrAcknowledgementCdate, out ldtEffectiveChangeDate);


                DataRow dr = idtResultTable.NewRow();
                dr["Employer_Org"] = adrRow["Employer_Org"].ToString();
                dr["Employer_Org_Code"] = adrRow["Employer_Org_Code"].ToString();
                dr["Person_Name"] = adrRow["Person_Name"].ToString();
                dr["Perslink_id"] = Convert.ToInt32(adrRow["Perslink_id"]);
                dr["Plan_Name"] = adrRow["Plan_Name"].ToString();
                dr["Effective_Change_Date"] = ldtEffectiveChangeDate;

                idtResultTable.Rows.Add(dr);
            }
        }

        private decimal GetPremiums(DateTime adtEffectiveChangeDate, busWssPersonAccountEnrollmentRequest aobjEnrollmentrequest, ref decimal adecLifeBasicPremiumAmt,
                           ref decimal adecLifeSuppPremiumAmt, ref decimal adecLifeSpouseSuppPremiumAmt, ref decimal adecLifeDepSuppPremiumAmt)
        {
            bool lblnErrorFound = false;

            busBase lbusBase = new busBase();

            var lbusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            lbusPersonAccount = aobjEnrollmentrequest.ibusPersonAccount;

            lbusPersonAccount.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lbusPersonAccount.ibusPerson = aobjEnrollmentrequest.ibusPerson;

            lbusPersonAccount.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
            lbusPersonAccount.ibusPlan = aobjEnrollmentrequest.ibusPlan;

            decimal ldecMemberPremiumAmt = 0.00M;

            if ((lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth) ||
                (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental) ||
               (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision) ||
               (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD))
            {
                string lstrCoverageCode = string.Empty;
                decimal ldecGroupHealthFeeAmt = 0.00M;
                decimal ldecBuydownAmt = 0.00M;
                decimal ldecRHICAmt = 0.00M;
                decimal ldecOthrRHICAmount = 0.00M;
                decimal ldecJSRHICAmount = 0.00M;
                decimal ldecPremiumAmt = 0.00M;
                decimal ldecTotalPremiumAmt = 0.00M;
                decimal ldecProviderPremiumAmt = 0.00M;
                decimal ldecMedicarePartD = 0.00M;

                int lintGHDVHistoryID = 0;
                string lstrGroupNumber = string.Empty;

                var lobjGhdv = new busPersonAccountGhdv { icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };
                lobjGhdv.icdoPersonAccount = aobjEnrollmentrequest.ibusPersonAccount.icdoPersonAccount;
                lobjGhdv.FindGHDVByPersonAccountID(aobjEnrollmentrequest.ibusPersonAccount.icdoPersonAccount.person_account_id);
                lobjGhdv.LoadHistoryByDate(adtEffectiveChangeDate);
                lobjGhdv.ibusPerson = lbusPersonAccount.ibusPerson;
                lobjGhdv.ibusPlan = lbusPersonAccount.ibusPlan;

                lobjGhdv.idtbCachedCoverageRef = ibusDBCacheData.idtbCachedCoverageRef;
                lobjGhdv.idtbCachedDentalRate = ibusDBCacheData.idtbCachedDentalRate;
                lobjGhdv.idtbCachedHealthRate = ibusDBCacheData.idtbCachedHealthRate;
                lobjGhdv.idtbCachedHmoRate = ibusDBCacheData.idtbCachedHMORate;
                lobjGhdv.idtbCachedRateRef = ibusDBCacheData.idtbCachedRateRef;
                lobjGhdv.idtbCachedRateStructureRef = ibusDBCacheData.idtbCachedRateStructureRef;
                lobjGhdv.idtbCachedVisionRate = ibusDBCacheData.idtbCachedVisionRate;

                //Get the GHDV History Object By Billing Month Year
                busPersonAccountGhdvHistory lobjPAGhdvHistory = lobjGhdv.LoadHistoryByDate(adtEffectiveChangeDate);
                if (lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id == 0)
                {
                    idlgUpdateProcessLog(
                        "Error : No History Record Found for Person Account = " +
                        lbusPersonAccount.icdoPersonAccount.person_account_id, "INFO", istrProcessName);
                    lblnErrorFound = true;
                }

                if (!lblnErrorFound)
                {
                    if (iclbProviderOrgPlan != null)
                    {
                        busOrgPlan lbusProviderOrgPlan = iclbProviderOrgPlan.FirstOrDefault(i => i.icdoOrgPlan.plan_id == lobjGhdv.icdoPersonAccount.plan_id);
                        if (lbusProviderOrgPlan != null)
                        {
                            lobjGhdv.ibusProviderOrgPlan = lbusProviderOrgPlan;
                        }
                        else
                        {
                            lobjGhdv.LoadActiveProviderOrgPlan(adtEffectiveChangeDate);
                        }
                    }
                    else
                    {
                        lobjGhdv.LoadActiveProviderOrgPlan(adtEffectiveChangeDate);
                    }

                    if (IsHealthOrMedicare(lbusPersonAccount.icdoPersonAccount.plan_id))
                    {
                        lobjGhdv = lobjPAGhdvHistory.LoadGHDVObject(lobjGhdv);

                        lintGHDVHistoryID = lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id;
                        lstrGroupNumber = lobjGhdv.GetGroupNumber();

                        if (lobjGhdv.ibusPerson == null)
                            lobjGhdv.LoadPerson();

                        if (lobjGhdv.ibusPlan == null)
                            lobjGhdv.LoadPlan();
                        //Initialize the Org Object to Avoid the NULL error
                        lobjGhdv.InitializeObjects();
                        lobjGhdv.idtPlanEffectiveDate = adtEffectiveChangeDate;

                        if (lobjGhdv.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                        {
                            lobjGhdv.LoadRateStructureForUserStructureCode();
                        }
                        else
                        {
                            lobjGhdv.LoadHealthParticipationDate();
                            //To Get the Rate Structure Code (Derived Field)
                            lobjGhdv.LoadRateStructure(adtEffectiveChangeDate);
                        }

                        //Get the Coverage Ref ID
                        lobjGhdv.LoadCoverageRefID();

                        //Get the Premium Amount
                        lobjGhdv.GetMonthlyPremiumAmountByRefID(adtEffectiveChangeDate);

                        if (lobjGhdv.icdoPersonAccountGhdv.Coverage_Ref_ID == 0)
                        {
                            idlgUpdateProcessLog(
                                "Error : Invalid Coverage Ref ID for Person Account = " +
                                lbusPersonAccount.icdoPersonAccount.person_account_id, "INFO", istrProcessName);
                            lblnErrorFound = true;
                        }
                        if (!lblnErrorFound)
                        {
                            lstrCoverageCode =
                                GetGroupHealthCoverageCodeDescription(lobjGhdv.icdoPersonAccountGhdv.Coverage_Ref_ID);

                            ldecPremiumAmt = lobjGhdv.icdoPersonAccountGhdv.PremiumExcludingFeeAmount;
                            ldecGroupHealthFeeAmt = lobjGhdv.icdoPersonAccountGhdv.FeeAmount;
                            ldecBuydownAmt = lobjGhdv.icdoPersonAccountGhdv.BuydownAmount;
                            ldecMedicarePartD = lobjGhdv.icdoPersonAccountGhdv.MedicarePartDAmount;//PIR 14271

                            ldecRHICAmt = lobjGhdv.icdoPersonAccountGhdv.total_rhic_amount;
                            /* UAT PIR 476, Including other and JS RHIC Amount */
                            ldecOthrRHICAmount = lobjGhdv.icdoPersonAccountGhdv.other_rhic_amount;
                            ldecJSRHICAmount = lobjGhdv.icdoPersonAccountGhdv.js_rhic_amount;
                            /* UAT PIR 476 ends here */
                            ldecMemberPremiumAmt = ldecPremiumAmt + ldecGroupHealthFeeAmt - ldecRHICAmt - ldecBuydownAmt + ldecMedicarePartD;//PIR 14271
                            ldecTotalPremiumAmt = ldecPremiumAmt + ldecGroupHealthFeeAmt - ldecBuydownAmt + ldecMedicarePartD;//PIR 14271
                            ldecProviderPremiumAmt = ldecPremiumAmt;
                        }
                    }
                    else if (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental)
                    {
                        ldecPremiumAmt =
                            busRateHelper.GetDentalPremiumAmount(
                                lobjGhdv.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                                lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.dental_insurance_type_value,
                                lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value,
                                adtEffectiveChangeDate,
                                ibusDBCacheData.idtbCachedDentalRate, iobjPassInfo);

                        ldecMemberPremiumAmt = ldecPremiumAmt;
                        ldecTotalPremiumAmt = ldecPremiumAmt;
                        ldecProviderPremiumAmt = ldecPremiumAmt;
                        lstrCoverageCode = lobjGhdv.icdoPersonAccountGhdv.level_of_coverage_description;
                    }
                    else if (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision)
                    {
                        ldecPremiumAmt =
                            busRateHelper.GetVisionPremiumAmount(
                                lobjGhdv.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                                lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.vision_insurance_type_value,
                                lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value,
                                adtEffectiveChangeDate,
                                ibusDBCacheData.idtbCachedVisionRate, iobjPassInfo);

                        ldecMemberPremiumAmt = ldecPremiumAmt;
                        ldecTotalPremiumAmt = ldecPremiumAmt;
                        ldecProviderPremiumAmt = ldecPremiumAmt;
                        lstrCoverageCode = lobjGhdv.icdoPersonAccountGhdv.level_of_coverage_description;
                    }
                }
            }

            if (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife)
            {
                var lobjLife = new busPersonAccountLife { icdoPersonAccountLife = new cdoPersonAccountLife() };
                lobjLife.icdoPersonAccount = aobjEnrollmentrequest.ibusPersonAccount.icdoPersonAccount;
                lobjLife.FindPersonAccountLife(aobjEnrollmentrequest.ibusPersonAccount.icdoPersonAccount.person_account_id);
                lobjLife.ibusPerson = lbusPersonAccount.ibusPerson;
                lobjLife.ibusPlan = lbusPersonAccount.ibusPlan;
                lobjLife.LoadLifeOptionData();
                lobjLife.LoadHistory();

                lobjLife.idtbCachedLifeRate = ibusDBCacheData.idtbCachedLifeRate;

                //Get the Provider Org ID from History
                busPersonAccountLifeHistory lobjPALifeHistory = new busPersonAccountLifeHistory();
                lobjPALifeHistory.icdoPersonAccountLifeHistory = new cdoPersonAccountLifeHistory();
                foreach (busPersonAccountLifeOption lobjPALifeOption in lobjLife.iclbLifeOption)
                {
                    if (lobjPALifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Basic)
                    {
                        lobjPALifeHistory = lobjLife.LoadHistoryByDate(lobjPALifeOption, adtEffectiveChangeDate);
                        break;
                    }
                }

                if (iclbProviderOrgPlan != null)
                {
                    busOrgPlan lbusProviderOrgPlan = iclbProviderOrgPlan.FirstOrDefault(i => i.icdoOrgPlan.plan_id == lobjLife.icdoPersonAccount.plan_id);
                    if (lbusProviderOrgPlan != null)
                    {
                        lobjLife.ibusProviderOrgPlan = lbusProviderOrgPlan;
                    }
                    else
                    {
                        lobjLife.LoadActiveProviderOrgPlan(adtEffectiveChangeDate);
                    }
                }
                else
                {
                    lobjLife.LoadActiveProviderOrgPlan(adtEffectiveChangeDate);
                }

                lobjLife.LoadMemberAge(adtEffectiveChangeDate);
                lobjLife.GetMonthlyPremiumAmount(adtEffectiveChangeDate);

                adecLifeBasicPremiumAmt = 0.00M;
                adecLifeSuppPremiumAmt = 0.00M;
                adecLifeSpouseSuppPremiumAmt = 0.00M;
                adecLifeDepSuppPremiumAmt = 0.00M;


                adecLifeBasicPremiumAmt = lobjLife.idecLifeBasicPremiumAmt;
                adecLifeSuppPremiumAmt = lobjLife.idecLifeSupplementalPremiumAmount;
                adecLifeSpouseSuppPremiumAmt = lobjLife.idecSpouseSupplementalPremiumAmt;
                adecLifeDepSuppPremiumAmt = lobjLife.idecDependentSupplementalPremiumAmt;
            }

            return ldecMemberPremiumAmt;
        }


        public void LoadDBCacheData()
        {
            if (ibusDBCacheData == null)
                ibusDBCacheData = new busDBCacheData();
            ibusDBCacheData.idtbCachedRateRef = busGlobalFunctions.LoadHealthRateRefCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedRateStructureRef = busGlobalFunctions.LoadHealthRateStructureCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedCoverageRef = busGlobalFunctions.LoadHealthCoverageRefCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedHealthRate = busGlobalFunctions.LoadHealthRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedLifeRate = busGlobalFunctions.LoadLifeRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedDentalRate = busGlobalFunctions.LoadDentalRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedHMORate = busGlobalFunctions.LoadHMORateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedLtcRate = busGlobalFunctions.LoadLTCRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedVisionRate = busGlobalFunctions.LoadVisionRateCacheData(iobjPassInfo);
        }

        public void LoadActiveProviders(DateTime adtEffectiveChangeDate)
        {
            DataTable ldtbActiveProviders = busNeoSpinBase.Select("cdoIbsHeader.LoadAllActiveProviders", new object[1] { adtEffectiveChangeDate });
            iclbProviderOrgPlan = new busBase().GetCollection<busOrgPlan>(ldtbActiveProviders, "icdoOrgPlan");
        }

        //public void LoadLifeOptionData(DateTime adtEffectiveChangeDate)
        //{
        //    idtbPALifeOption =
        //        busNeoSpinBase.Select("cdoIbsHeader.LoadLifeOption", new object[1] { adtEffectiveChangeDate });
        //}

        //public void LoadGHDVHistory(DateTime adtEffectiveChangeDate)
        //{
        //    idtbGHDVHistory =
        //        busNeoSpinBase.Select("cdoIbsHeader.LoadGHDVHistory", new object[1] { adtEffectiveChangeDate });
        //}

        //public void LoadLifeHistory(DateTime adtEffectiveChangeDate)
        //{
        //    idtbLifeHistory =
        //        busNeoSpinBase.Select("cdoIbsHeader.LoadLifeHistory", new object[1] { adtEffectiveChangeDate });
        //}

        public busOrgPlan LoadProviderOrgPlanByProviderOrgId(int aintProviderOrgId, int aintPlanId, DateTime adtEffectiveChangeDate)
        {
            busOrgPlan lbusOrgPlanToReturn = new busOrgPlan { icdoOrgPlan = new cdoOrgPlan() };
            foreach (var lbusOrgPlan in iclbProviderOrgPlan)
            {
                if ((lbusOrgPlan.icdoOrgPlan.org_id == aintProviderOrgId) && (lbusOrgPlan.icdoOrgPlan.plan_id == aintPlanId))
                {
                    if (busGlobalFunctions.CheckDateOverlapping(adtEffectiveChangeDate,
                        lbusOrgPlan.icdoOrgPlan.participation_start_date,
                        lbusOrgPlan.icdoOrgPlan.participation_end_date))
                    {
                        lbusOrgPlanToReturn = lbusOrgPlan;
                        break;
                    }
                }
            }
            return lbusOrgPlanToReturn;
        }
        public string GetGroupHealthCoverageCodeDescription(int AintCoverageRefID)
        {
            if (AintCoverageRefID > 0)
            {
                DataTable ldtbCoverageCode = busNeoSpinBase.Select("cdoIbsHeader.GetCoverageCodeDescription",
                                                                                            new object[1] { AintCoverageRefID });
                if (ldtbCoverageCode.Rows.Count > 0)
                {
                    string lstrCoverageCodeDescription = ldtbCoverageCode.Rows[0]["CLIENT_DESCRIPTION"].ToString();
                    return lstrCoverageCodeDescription;
                }
            }
            return string.Empty;
        }

        private bool IsHealthOrMedicare(int aintPlanID)
        {
            bool lblnResult = false;
            if ((aintPlanID == busConstant.PlanIdGroupHealth) || (aintPlanID == busConstant.PlanIdMedicarePartD))
                lblnResult = true;
            return lblnResult;
        }
    }
}
