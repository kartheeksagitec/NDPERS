using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using NeoSpin.BusinessObjects;
using System.IO;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.DataObjects;
using Sagitec.DBUtility;
using NeoSpin.CustomDataObjects;
using Sagitec.ExceptionPub;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busMAS : busNeoSpinBase
    {
        public DataSet idsMemberAnnualStatement { get; set; }

        public DataTable ldtbAllMASSelection { get; set; }

        public DataTable ldtbAllMASPerson { get; set; }

        public DataTable ldtbAllMASDBPersonPlan { get; set; }

        public DataTable ldtbAllMASDCPersonPlan { get; set; }
		
		//PIR 14875
        public DataTable ldtbAllMASPersonPlan { get; set; }

        public DataTable ldtbAllMASBenDependent { get; set; }

        public DataTable ldtbAllMASDefCompProvider { get; set; }

        public DataTable ldtbAllMASFlexOptions { get; set; }

        public DataTable ldtbAllMASFlexConversion { get; set; }

        public DataTable ldtbAllMASLifeOptions { get; set; }

        public DataTable ldtbAllPersonCalculation { get; set; }

        public DataTable ldtbBatchRequest { get; set; }


        /// Filter the DataTable by FilterString, Returns the DataTable
        public static DataTable FilterTable<T>(DataTable source, busConstant.DataType dataType, string filterFieldName, T filterFieldValue)
        {
            DataTable ldtbResuls = new DataTable();
            string lstrFilterString = string.Empty;
            if (dataType == busConstant.DataType.String)
                lstrFilterString = filterFieldName + "= '" + filterFieldValue + "'";
            else
                lstrFilterString = filterFieldName + "=" + filterFieldValue;
            source.DefaultView.RowFilter = lstrFilterString;
            ldtbResuls = source.DefaultView.ToTable();
            return ldtbResuls;
        }

        public void FetchAllReportData(int aintBatchRequestID, int aintMASSelectionID)
        {
            ldtbAllMASPerson = new DataTable();
            ldtbAllMASDBPersonPlan = new DataTable();
            ldtbAllMASDCPersonPlan = new DataTable();
			//PIR 14875
            ldtbAllMASPersonPlan = new DataTable();
            ldtbAllMASBenDependent = new DataTable();
            ldtbAllMASDefCompProvider = new DataTable();
            ldtbAllMASFlexOptions = new DataTable();
            ldtbAllMASFlexConversion = new DataTable();
            ldtbAllMASLifeOptions = new DataTable();
            ldtbAllPersonCalculation = new DataTable();

            ldtbBatchRequest = busBase.Select("cdoMASBatchRequest.LoadBatchRequestWithUserID",
                                new object[1] { aintBatchRequestID });

            ldtbAllMASPerson = busBase.Select("cdoMASBatchRequest.Fetch_MAS_Person",
                                new object[2] { aintBatchRequestID, aintMASSelectionID });
            ldtbAllMASDBPersonPlan = busBase.Select("cdoMASBatchRequest.Select_MAS_DB_Person_Plan",
                                new object[3] { aintBatchRequestID, busConstant.Flag_Yes, aintMASSelectionID });
            ldtbAllMASDCPersonPlan = busBase.Select("cdoMASBatchRequest.Select_MAS_DC_Person_Plan",
                                new object[3] { aintBatchRequestID, busConstant.Flag_Yes, aintMASSelectionID });
            //PIR 14875
			ldtbAllMASPersonPlan = busBase.Select("cdoMASBatchRequest.Select_MAS_All_Person_Plan",
                                new object[3] { aintBatchRequestID, busConstant.Flag_Yes, aintMASSelectionID });
            ldtbAllMASBenDependent = busBase.Select("cdoMASBatchRequest.Select_MAS_Person_Plan_Beneficiary",
                                new object[2] { aintBatchRequestID, aintMASSelectionID });
            ldtbAllMASDefCompProvider = busBase.Select("cdoMASBatchRequest.Select_MAS_Deff_Comp_Provider",
                                new object[2] { aintBatchRequestID, aintMASSelectionID });
            ldtbAllMASFlexOptions = busBase.Select("cdoMASBatchRequest.Select_MAS_Flex_Option",
                                new object[2] { aintBatchRequestID, aintMASSelectionID });
            ldtbAllMASFlexConversion = busBase.Select("cdoMASBatchRequest.Select_MAS_Flex_Conversion",
                                new object[2] { aintBatchRequestID, aintMASSelectionID });
            ldtbAllMASLifeOptions = busBase.Select("cdoMASBatchRequest.Select_MAS_Life_Options",
                                new object[2] { aintBatchRequestID, aintMASSelectionID });
            ldtbAllPersonCalculation = busBase.Select("cdoMASBatchRequest.Select_MAS_Person_Calculation",
                                new object[2] { aintBatchRequestID, aintMASSelectionID });
        }

        public DataSet FilterReportData(int aintMASSelectionID)
        {
            idsMemberAnnualStatement = new DataSet();
            DataTable ldtbMASPerson = FilterTable(ldtbAllMASPerson, busConstant.DataType.Numeric, "MAS_SELECTION_ID",
                                        aintMASSelectionID);
            ldtbMASPerson.TableName = busConstant.ReportTableName;
            idsMemberAnnualStatement.Tables.Add(ldtbMASPerson.Copy());

            

            if (ldtbMASPerson.Rows.Count > 0)
            {
                busMASPerson lobjMASPerson = new busMASPerson { icdoMasPerson = new cdoMasPerson() };
                lobjMASPerson.icdoMasPerson.LoadData(ldtbMASPerson.Rows[0]);

                // PIR 8837 - PRE-TAXED INSURANCE PREMIUMS
                int lintPersonId = Convert.ToInt32(ldtbMASPerson.Rows[0]["PERSON_ID"]);
                string lstrIsEligibleForFlex = ldtbMASPerson.Rows[0]["IS_ELIGIBLE_FOR_FLEX"].ToString();
                
                if (lstrIsEligibleForFlex == busConstant.Flag_Yes)
                {
                    DataTable ldtbPremiumConvPlans = new DataTable();
                    ldtbPremiumConvPlans = busBase.Select("cdoPersonAccount.LoadPremiumConversionPlans",
                    new object[2] { lintPersonId, Convert.ToDateTime(ldtbBatchRequest.Rows[0]["STATEMENT_EFFECTIVE_DATE"]) });

                    ldtbPremiumConvPlans.TableName = busConstant.ReportTableName32;
                    idsMemberAnnualStatement.Tables.Add(ldtbPremiumConvPlans.Copy());
                }

                // PIR 8837
                DataTable ldtMasAnnualPledgeAmount = busBase.Select("cdoPersonAccount.rptAnnualPledgeAmount", new object[1] { lobjMASPerson.icdoMasPerson.mas_person_id });
                ldtMasAnnualPledgeAmount.TableName = busConstant.ReportTableName33;
                idsMemberAnnualStatement.Tables.Add(ldtMasAnnualPledgeAmount.Copy());


                // MAS PERSON PLAN
                DataTable ldtbMASPersonPlan = new DataTable();
				//PIR 14875
                if (ldtbAllMASPersonPlan.Rows.Count > 0)
                {
                    ldtbMASPersonPlan = FilterTable(ldtbAllMASPersonPlan, busConstant.DataType.Numeric, "MAS_PERSON_ID",
                                                lobjMASPerson.icdoMasPerson.mas_person_id);
                }
                else //PIR-17769 person who enrolled only in insurance plans was throwing error, Because ldtbMASPersonPlan is used in rptSummaryMemberStatement report and its null in this case. 
                {
                    ldtbMASPersonPlan = ldtbAllMASPersonPlan.Clone();
                }
                //else
                //{
                //    ldtbMASPersonPlan = FilterTable(ldtbAllMASDCPersonPlan, busConstant.DataType.Numeric, "MAS_PERSON_ID",
                //                                lobjMASPerson.icdoMasPerson.mas_person_id);
                //}
                ldtbMASPersonPlan.TableName = busConstant.ReportTableName02;
                idsMemberAnnualStatement.Tables.Add(ldtbMASPersonPlan.Copy());

                // MAS PERSON BENEFICIARY DEPENDENT
                DataTable ldtbMASBenDependent = FilterTable(ldtbAllMASBenDependent, busConstant.DataType.Numeric, "MAS_PERSON_ID",
                                            lobjMASPerson.icdoMasPerson.mas_person_id);
                DataTable ldtbMASRetrBeneficiary = ldtbMASBenDependent.AsEnumerable().Where(o => o.Field<string>("IS_BENEFICIARY_FLAG") == "Y"
                    && o.Field<int>("MAS_PERSON_ID") == lobjMASPerson.icdoMasPerson.mas_person_id
                    && (o.Field<int>("PLAN_ID") == busConstant.PlanIdDC || o.Field<int>("PLAN_ID") == busConstant.PlanIdMain ||
                    o.Field<int>("PLAN_ID") == busConstant.PlanIdDC2020 || o.Field<int>("PLAN_ID") == busConstant.PlanIdMain2020 || //PIR 20232
                       o.Field<int>("PLAN_ID") == busConstant.PlanIdLE || o.Field<int>("PLAN_ID") == busConstant.PlanIdNG ||
                       o.Field<int>("PLAN_ID") == busConstant.PlanIdHP || o.Field<int>("PLAN_ID") == busConstant.PlanIdJudges ||
                       o.Field<int>("PLAN_ID") == busConstant.PlanIdJobService || o.Field<int>("PLAN_ID") == busConstant.PlanIdLEWithoutPS ||
                       o.Field<int>("PLAN_ID") == busConstant.PlanIdJobService3rdPartyPayor || o.Field<int>("PLAN_ID") == busConstant.PlanIdPriorJudges ||
                       o.Field<int>("PLAN_ID") == 25 ||
                       o.Field<int>("PLAN_ID") == busConstant.PlanIdStatePublicSafety || //PIR 25729
                       o.Field<int>("PLAN_ID") == busConstant.PlanIdBCILawEnf ||
                       o.Field<int>("PLAN_ID") == busConstant.PlanIdDC2025 //PIR 25920
                       )).AsDataTable(); // pir 7943
                if (ldtbMASRetrBeneficiary.Rows.Count == 0)
                    ldtbMASRetrBeneficiary = ldtbAllMASBenDependent.Clone();
                ldtbMASRetrBeneficiary.TableName = busConstant.ReportTableName03;
                idsMemberAnnualStatement.Tables.Add(ldtbMASRetrBeneficiary.Copy());

                // MAS DEF COMP PROVIDER
                DataTable ldtbMASDefCompProvider = FilterTable(ldtbAllMASDefCompProvider, busConstant.DataType.Numeric, "MAS_PERSON_ID",
                                            lobjMASPerson.icdoMasPerson.mas_person_id);
                ldtbMASDefCompProvider.TableName = busConstant.ReportTableName04;
                idsMemberAnnualStatement.Tables.Add(ldtbMASDefCompProvider.Copy());

                // MAS FLEX OPTIONS
                DataTable ldtbMASFlexOptions = FilterTable(ldtbAllMASFlexOptions, busConstant.DataType.Numeric, "MAS_PERSON_ID",
                                            lobjMASPerson.icdoMasPerson.mas_person_id);
                ldtbMASFlexOptions.TableName = busConstant.ReportTableName05;
                idsMemberAnnualStatement.Tables.Add(ldtbMASFlexOptions.Copy());


                // MAS FLEX CONVERSION
                DataTable ldtbMASFlexConversion = FilterTable(ldtbAllMASFlexConversion, busConstant.DataType.Numeric, "MAS_PERSON_ID",
                                            lobjMASPerson.icdoMasPerson.mas_person_id);
                ldtbMASFlexConversion.TableName = busConstant.ReportTableName06;
                idsMemberAnnualStatement.Tables.Add(ldtbMASFlexConversion.Copy());

                // MAS LIFE OPTIONS
                DataTable ldtbMASLifeOptions = FilterTable(ldtbAllMASLifeOptions, busConstant.DataType.Numeric, "MAS_PERSON_ID",
                                            lobjMASPerson.icdoMasPerson.mas_person_id);
                ldtbMASLifeOptions.TableName = busConstant.ReportTableName07;
                idsMemberAnnualStatement.Tables.Add(ldtbMASLifeOptions.Copy());

                // MAS BATCH REQUEST
                ldtbBatchRequest.TableName = busConstant.ReportTableName08;
                idsMemberAnnualStatement.Tables.Add(ldtbBatchRequest.Copy());

                // MAS PERSON CALCULATION
                DataTable ldtbMASPersonCalculation = FilterTable(ldtbAllPersonCalculation, busConstant.DataType.Numeric, "MAS_PERSON_ID",
                                            lobjMASPerson.icdoMasPerson.mas_person_id);

                // DB PERSON CALCULATION
                DataTable ldtbMASDBPersonCalculation = ldtbMASPersonCalculation.AsEnumerable().Where(o => (o.Field<int>("PLAN_ID") != busConstant.PlanIdDC && o.Field<int>("PLAN_ID") != busConstant.PlanIdDC2020 && o.Field<int>("PLAN_ID") != busConstant.PlanIdDC2025)).AsDataTable(); //PIR 20232 //PIR 25920
                ldtbMASDBPersonCalculation.TableName = busConstant.ReportTableName09;
                if (ldtbMASDBPersonCalculation.Rows.Count == 0)
                    ldtbMASDBPersonCalculation = ldtbMASPersonCalculation.Clone();
                idsMemberAnnualStatement.Tables.Add(ldtbMASDBPersonCalculation.Copy());

                // DC PERSON CALCULATION
                DataTable ldtbMASDCDisability = ldtbMASPersonCalculation.AsEnumerable().Where(o => (o.Field<int>("PLAN_ID") == busConstant.PlanIdDC || o.Field<int>("PLAN_ID") == busConstant.PlanIdDC2020 || o.Field<int>("PLAN_ID") == busConstant.PlanIdDC2025) && //PIR 20232 //PIR 25920
                                                                o.Field<string>("BENEFIT_ACCOUNT_TYPE_VALUE") == busConstant.ApplicationBenefitTypeDisability).AsDataTable();
                ldtbMASDCDisability.TableName = busConstant.ReportTableName23;
                idsMemberAnnualStatement.Tables.Add(ldtbMASDCDisability);

                // UAT PIR ID 1978
                DataTable ldtbMASDCRetirement = ldtbMASPersonCalculation.AsEnumerable().Where(o => (o.Field<int>("PLAN_ID") == busConstant.PlanIdDC || o.Field<int>("PLAN_ID") == busConstant.PlanIdDC2020 || o.Field<int>("PLAN_ID") == busConstant.PlanIdDC2025) && //PIR 20232 //PIR 25920
                                               o.Field<string>("BENEFIT_ACCOUNT_TYPE_VALUE") == busConstant.ApplicationBenefitTypeRetirement).AsDataTable();
                DataTable ldtbMASDCRestPC = new DataTable(); ldtbMASDCRestPC = ldtbMASPersonCalculation.Clone();
                int lintFlag = 0;
                foreach (DataRow ldtrRow in ldtbMASDCRetirement.Rows)
                {
                    if (lintFlag != 0)
                        ldtbMASDCRestPC.ImportRow(ldtrRow);
                    else
                        lintFlag += 1;
                } 
                ldtbMASDCRestPC.TableName = busConstant.ReportTableName31;
                idsMemberAnnualStatement.Tables.Add(ldtbMASDCRestPC.Copy());

                // MAS PERSON DC PLAN
                DataTable ldtbMASPersonPlanDC = FilterTable(ldtbAllMASDCPersonPlan, busConstant.DataType.Numeric, "MAS_PERSON_ID",
                                            lobjMASPerson.icdoMasPerson.mas_person_id);
                ldtbMASPersonPlanDC.TableName = busConstant.ReportTableName10;
                idsMemberAnnualStatement.Tables.Add(ldtbMASPersonPlanDC.Copy());

                // MAS HEALTH DEPENDENT
                DataTable ldtbMASBeneficiaryHealthDep = ldtbAllMASBenDependent.AsEnumerable().Where(o => o.Field<string>("IS_BENEFICIARY_FLAG") == "Y"
                     //&& o.Field<int>("PLAN_ID") == busConstant.PlanIdGroupHealth
                     && o.Field<int>("MAS_PERSON_ID") == lobjMASPerson.icdoMasPerson.mas_person_id
                    ).AsDataTable();
                if (ldtbMASBeneficiaryHealthDep.Rows.Count == 0)
                    ldtbMASBeneficiaryHealthDep = ldtbAllMASBenDependent.Clone();
                ldtbMASBeneficiaryHealthDep.TableName = busConstant.ReportTableName11;
                idsMemberAnnualStatement.Tables.Add(ldtbMASBeneficiaryHealthDep.Copy());

                // MAS LIFE DEPENDENT
                DataTable ldtbMASBeneficiaryLifeBen = ldtbAllMASBenDependent.AsEnumerable().Where(o => o.Field<string>("IS_BENEFICIARY_FLAG") == "Y"
                                     && o.Field<int>("PLAN_ID") == busConstant.PlanIdGroupLife
                                     && o.Field<int>("MAS_PERSON_ID") == lobjMASPerson.icdoMasPerson.mas_person_id
                                    ).AsDataTable();
                if (ldtbMASBeneficiaryLifeBen.Rows.Count == 0)
                    ldtbMASBeneficiaryLifeBen = ldtbAllMASBenDependent.Clone();
                ldtbMASBeneficiaryLifeBen.TableName = busConstant.ReportTableName12;
                idsMemberAnnualStatement.Tables.Add(ldtbMASBeneficiaryLifeBen.Copy());

                // MAS DENTAL DEPENDENT
                DataTable ldtbMASBeneficiaryDentalDep = ldtbAllMASBenDependent.AsEnumerable().Where(o => o.Field<string>("IS_BENEFICIARY_FLAG") == "N"
                                     && o.Field<int>("PLAN_ID") == busConstant.PlanIdDental
                                     && o.Field<int>("MAS_PERSON_ID") == lobjMASPerson.icdoMasPerson.mas_person_id
                                    ).AsDataTable();
                if (ldtbMASBeneficiaryDentalDep.Rows.Count == 0)
                    ldtbMASBeneficiaryDentalDep = ldtbAllMASBenDependent.Clone();
                ldtbMASBeneficiaryDentalDep.TableName = busConstant.ReportTableName13;
                idsMemberAnnualStatement.Tables.Add(ldtbMASBeneficiaryDentalDep.Copy());

                // MAS VISION DEPENDENT
                DataTable ldtbMASBeneficiaryVisionDep = ldtbAllMASBenDependent.AsEnumerable().Where(o => o.Field<string>("IS_BENEFICIARY_FLAG") == "N"
                                                     && o.Field<int>("PLAN_ID") == busConstant.PlanIdVision
                                                     && o.Field<int>("MAS_PERSON_ID") == lobjMASPerson.icdoMasPerson.mas_person_id
                                                    ).AsDataTable();
                if (ldtbMASBeneficiaryVisionDep.Rows.Count == 0)
                    ldtbMASBeneficiaryVisionDep = ldtbAllMASBenDependent.Clone();
                ldtbMASBeneficiaryVisionDep.TableName = busConstant.ReportTableName14;
                idsMemberAnnualStatement.Tables.Add(ldtbMASBeneficiaryVisionDep.Copy());

                // DB EMPLOYER VESTING CONTRIBUTION
                DataTable ldtbDBEEVestingCont = new DataTable(busConstant.ReportTableName15);
                DataColumn ldrDB = new DataColumn("VESTED_EMPLOYER_CONT_PERCENT", Type.GetType("System.Decimal"));
                ldtbDBEEVestingCont.Columns.Add(ldrDB);
                DataRow ldtr = ldtbDBEEVestingCont.NewRow();
                DataTable ldtbDBResult = ldtbMASPersonPlan.AsEnumerable().Where(o => o.Field<string>("IS_DB_PLAN") == "Y").AsDataTable();
                if (ldtbDBResult.Rows.Count > 0)
                {
                    if (!Convert.IsDBNull(ldtbDBResult.Rows[0]["VESTED_EMPLOYER_CONT_PERCENT"]))
                        ldtr["VESTED_EMPLOYER_CONT_PERCENT"] = Convert.ToDecimal(ldtbDBResult.Rows[0]["VESTED_EMPLOYER_CONT_PERCENT"]) * 100M;
                    else
                        ldtr["VESTED_EMPLOYER_CONT_PERCENT"] = 0.00M;
                    ldtbDBEEVestingCont.Rows.Add(ldtr);
                }
                idsMemberAnnualStatement.Tables.Add(ldtbDBEEVestingCont.Copy());

                // DC EMPLOYER VESTING CONTRIBUTION
                DataTable ldtbDCEEVestingCont = new DataTable(busConstant.ReportTableName16);
                DataColumn ldrDC = new DataColumn("VESTED_EMPLOYER_CONT_PERCENT", Type.GetType("System.Decimal"));
                ldtbDCEEVestingCont.Columns.Add(ldrDC);
                DataRow ldtr2 = ldtbDCEEVestingCont.NewRow();
                DataTable ldtbDCResult = ldtbAllMASDCPersonPlan.AsEnumerable().Where(o => o.Field<string>("IS_DC_PLAN") == "Y").AsDataTable();
                if (ldtbDCResult.Rows.Count > 0)
                {
                    if (!Convert.IsDBNull(ldtbDCResult.Rows[0]["VESTED_EMPLOYER_CONT_PERCENT"]))
                        ldtr2["VESTED_EMPLOYER_CONT_PERCENT"] = Convert.ToDecimal(ldtbDCResult.Rows[0]["VESTED_EMPLOYER_CONT_PERCENT"]) * 100M;
                    else
                        ldtr2["VESTED_EMPLOYER_CONT_PERCENT"] = 0.00M;
                    ldtbDCEEVestingCont.Rows.Add(ldtr2);
                }
                idsMemberAnnualStatement.Tables.Add(ldtbDCEEVestingCont.Copy());

                // MAS Disability Benefits
                DataTable ldtbMASDisabilityBenefit = FilterTable(ldtbMASPersonCalculation, busConstant.DataType.String, "BENEFIT_ACCOUNT_TYPE_VALUE",
                                            busConstant.ApplicationBenefitTypeDisability);
                ldtbMASDisabilityBenefit.TableName = busConstant.ReportTableName17;
                idsMemberAnnualStatement.Tables.Add(ldtbMASDisabilityBenefit.Copy());

                // MAS Retirement Benefits
                DataTable ldtbMemberRetirementBenefits = FilterTable(ldtbMASPersonCalculation, busConstant.DataType.String, "BENEFIT_ACCOUNT_TYPE_VALUE",
                                            busConstant.ApplicationBenefitTypeRetirement);
                DataTable ldtbTempRetirementBenefits = new DataTable();
                if (ldtbMemberRetirementBenefits.AsEnumerable().Where(o => o.Field<DateTime>("RETIREMENT_DATE") != DateTime.MinValue).Any())
                {
                    ldtbTempRetirementBenefits = ldtbMemberRetirementBenefits.AsEnumerable().OrderBy(i => i.Field<int>("PLAN_ID")).
                                                                                             ThenBy(i => i.Field<DateTime>("RETIREMENT_DATE")).AsDataTable();
                }

                if (ldtbTempRetirementBenefits.Rows.Count == 0)
                    ldtbTempRetirementBenefits = ldtbMemberRetirementBenefits.Clone();

                // 8824 - to exclude past age data from RHID grid in Online report
                DataTable ldtbRetirementBenefitsExcludePastAge = new DataTable();
                ldtbRetirementBenefitsExcludePastAge = ldtbTempRetirementBenefits.Clone();

                decimal ldecAge = CalculateAgeBasedOnStatementEffectiveDate(Convert.ToDateTime(ldtbMASPerson.Rows[0]["DATE_OF_BIRTH"]), Convert.ToDateTime(ldtbBatchRequest.Rows[0]["STATEMENT_EFFECTIVE_DATE"]));

                decimal ldecMasCalcAge = 0.00M;

                int rowInd = 0;
                foreach (DataRow ldtrRow in ldtbTempRetirementBenefits.Rows)
                {
                    string lstrAge = ldtrRow["AGE_DESCRIPTION"].ToString();
                    string[] lastrAge = lstrAge.Split(' ');
                    if (lstrAge.Contains("Age"))
                    {
                        ldecMasCalcAge = Convert.ToDecimal(lastrAge[1]);
                    }
                    else
                    {
                        ldecMasCalcAge = Convert.ToDecimal(ldtrRow["AGE_DESCRIPTION"]);
                    }
                    if (ldecMasCalcAge >= ldecAge)
                    {
                        ldtbRetirementBenefitsExcludePastAge.ImportRow(ldtbTempRetirementBenefits.Rows[rowInd]);
                    }
                    rowInd++;                
                }

                ldtbRetirementBenefitsExcludePastAge.TableName = busConstant.ReportTableName26;
                idsMemberAnnualStatement.Tables.Add(ldtbRetirementBenefitsExcludePastAge.Copy());

                // MAS Surviving Spouse Retirement Benefits Other Than Judge
                DataTable ldtbOtherthanJudge = ldtbMASPersonCalculation.AsEnumerable().Where(o =>
                                                o.Field<string>("BENEFIT_ACCOUNT_TYPE_VALUE") == busConstant.ApplicationBenefitTypePreRetirementDeath &&
                                                o.Field<int>("PLAN_ID") != 5).AsDataTable();
                DataTable ldtTempTable = ldtbMASPersonCalculation.Clone();
                if (ldtbOtherthanJudge.AsEnumerable().Where(row => !Convert.IsDBNull(row["MONTHLY_BENEFIT"])).Any())// UAT PIR ID 1771
                {
                    ldtbOtherthanJudge.TableName = busConstant.ReportTableName18;
                    ldtTempTable.TableName = busConstant.ReportTableName29;
                    // PIR 9652 - add if and code which was removed earlier
                    if (Convert.ToDateTime(ldtbBatchRequest.Rows[0]["STATEMENT_EFFECTIVE_DATE"]).Year <= Convert.ToInt32(busGlobalFunctions.GetData1ByCodeValue(52, "SEYR", iobjPassInfo)))
                    {
                        if (idsMemberAnnualStatement.Tables[busConstant.ReportTableName] != null && idsMemberAnnualStatement.Tables[busConstant.ReportTableName].Rows.Count > 0)
                            idsMemberAnnualStatement.Tables[busConstant.ReportTableName].Rows[0]["IS_VESTED_IN_DB"] = busConstant.Flag_Yes;
                    }
                }
                else
                {
                    ldtbOtherthanJudge.TableName = busConstant.ReportTableName29;
                    ldtTempTable.TableName = busConstant.ReportTableName18;
                    // PIR 9652 - add if and code which was removed earlier 
                    if (Convert.ToDateTime(ldtbBatchRequest.Rows[0]["STATEMENT_EFFECTIVE_DATE"]).Year <= Convert.ToInt32(busGlobalFunctions.GetData1ByCodeValue(52, "SEYR", iobjPassInfo)))
                    {
                        if (idsMemberAnnualStatement.Tables[busConstant.ReportTableName] != null && idsMemberAnnualStatement.Tables[busConstant.ReportTableName].Rows.Count > 0)
                        {
                            if (idsMemberAnnualStatement.Tables[busConstant.ReportTableName].Rows[0]["is_db_suspended"] != DBNull.Value &&
                                idsMemberAnnualStatement.Tables[busConstant.ReportTableName].Rows[0]["is_db_suspended"].ToString() == busConstant.Flag_Yes)
                            {
                                idsMemberAnnualStatement.Tables[busConstant.ReportTableName].Rows[0]["IS_VESTED_IN_DB"] = busConstant.Flag_No;
                            }
                            else
                                idsMemberAnnualStatement.Tables[busConstant.ReportTableName].Rows[0]["IS_VESTED_IN_DB"] = busConstant.Flag_Yes;
                        }
                    }
                }
               
                idsMemberAnnualStatement.Tables.Add(ldtbOtherthanJudge.Copy());
                idsMemberAnnualStatement.Tables.Add(ldtTempTable.Copy());

                // PIR 9652 - add if
                if (Convert.ToDateTime(ldtbBatchRequest.Rows[0]["STATEMENT_EFFECTIVE_DATE"]).Year > Convert.ToInt32(busGlobalFunctions.GetData1ByCodeValue(52, "SEYR", iobjPassInfo)))
                {
                    // PIR 9481,9482,8824 - Member's Retirement Section visibility
                    if ((idsMemberAnnualStatement.Tables[busConstant.ReportTableName].Rows[0]["IS_VESTED_IN_DB"].ToString() == busConstant.Flag_Yes)
                        && ((idsMemberAnnualStatement.Tables[busConstant.ReportTableName].Rows[0]["IS_DB_PLAN_ENROLED"].ToString() == busConstant.Flag_Yes)
                        || (idsMemberAnnualStatement.Tables[busConstant.ReportTableName].Rows[0]["IS_DC_PLAN_ENROLED"].ToString() == busConstant.Flag_Yes)
                        || (idsMemberAnnualStatement.Tables[busConstant.ReportTableName].Rows[0]["IS_DB_SUSPENDED"].ToString() == busConstant.Flag_Yes)))
                    {
                        idsMemberAnnualStatement.Tables[busConstant.ReportTableName].Rows[0]["IS_VESTED_IN_DB"] = busConstant.Flag_Yes;
                    }
                    else
                    {
                        idsMemberAnnualStatement.Tables[busConstant.ReportTableName].Rows[0]["IS_VESTED_IN_DB"] = busConstant.Flag_No;
                    }
                }

                // MAS Surviving Spouse Retirement Benefits for Judge
                DataTable ldtbJudges = ldtbMASPersonCalculation.AsEnumerable().Where(o =>
                                                o.Field<string>("BENEFIT_ACCOUNT_TYPE_VALUE") == busConstant.ApplicationBenefitTypePreRetirementDeath &&
                                                o.Field<int>("PLAN_ID") == 5).AsDataTable();
                if (ldtbJudges.AsEnumerable().Where(row => !Convert.IsDBNull(row["MONTHLY_BENEFIT"])).Any()) // UAT PIR ID 1771
                    ldtbJudges.TableName = busConstant.ReportTableName25;
                else
                    ldtbJudges.TableName = busConstant.ReportTableName30;
                idsMemberAnnualStatement.Tables.Add(ldtbJudges.Copy());

                // MAS Pre-Retirement Benefits
                DataTable ldtbMASPreRetirementBenefit = FilterTable(ldtbMASPersonCalculation, busConstant.DataType.String, "BENEFIT_ACCOUNT_TYPE_VALUE",
                                            busConstant.ApplicationBenefitTypePreRetirementDeath);
                if (ldtbMASPreRetirementBenefit.Rows.Count > 0 && ldtbMASPreRetirementBenefit.Rows[0]["MEMBER_ACCOUNT_BALANCE"] != DBNull.Value) //PROD PIR ID 7650
                {
                    ldtbMASPreRetirementBenefit.TableName = busConstant.ReportTableName19;
                    idsMemberAnnualStatement.Tables.Add(ldtbMASPreRetirementBenefit.Copy());
                }

                // Estimated Monthly Benefit Main, LE, NG or JobService
                DataTable ldtbMASEstMonthlyDBBenefit = ldtbMASPersonPlan.AsEnumerable().Where(row => row.Field<int>("PLAN_ID") == busConstant.PlanIdMain ||
                                                            row.Field<int>("PLAN_ID") == busConstant.PlanIdMain2020 ||//PIR 20232                                            
                                                            row.Field<int>("PLAN_ID") == busConstant.PlanIdLE ||
                                                            row.Field<int>("PLAN_ID") == busConstant.PlanIdNG ||
                                                            row.Field<int>("PLAN_ID") == busConstant.PlanIdJobService ||
                                                            row.Field<int>("PLAN_ID") == busConstant.PlanIdStatePublicSafety || //PIR 25729
                                                            row.Field<int>("PLAN_ID") == busConstant.PlanIdBCILawEnf
                                                            ).AsDataTable(); //pir 7943
                ldtbMASEstMonthlyDBBenefit.TableName = busConstant.ReportTableName20;
                idsMemberAnnualStatement.Tables.Add(ldtbMASEstMonthlyDBBenefit.Copy());

                // Estimated Monthly Benefit Judges
                DataTable ldtbMASEstMonthlyJudgesBenefit = ldtbMASPersonPlan.AsEnumerable().Where(row => row.Field<int>("PLAN_ID") == busConstant.PlanIdJudges).AsDataTable();
                ldtbMASEstMonthlyJudgesBenefit.TableName = busConstant.ReportTableName21;
                idsMemberAnnualStatement.Tables.Add(ldtbMASEstMonthlyJudgesBenefit.Copy());

                // Estimated Monthly Benefit HP
                DataTable ldtbMASEstMonthlyHPBenefit = ldtbMASPersonPlan.AsEnumerable().Where(row => row.Field<int>("PLAN_ID") == busConstant.PlanIdHP).AsDataTable();
                ldtbMASEstMonthlyHPBenefit.TableName = busConstant.ReportTableName22;
                idsMemberAnnualStatement.Tables.Add(ldtbMASEstMonthlyHPBenefit.Copy());

                // QDRO Amount
                DataTable ldtbMASQDROAmt = ldtbMASPersonPlan.AsEnumerable().Where(row => (!Convert.IsDBNull(row["QDRO_AMOUNT"]))).AsDataTable();
                ldtbMASQDROAmt.TableName = busConstant.ReportTableName24;
                idsMemberAnnualStatement.Tables.Add(ldtbMASQDROAmt.Copy());

                // EE RHIC Amount
                DataTable ldtbMASEERHICAmt = ldtbMASPersonPlan.AsEnumerable().Where(row => (!Convert.IsDBNull(row["EE_RHIC_CONTRIBUTION_AMOUNT"])) &&
                                                                                           (row.Field<decimal>("EE_RHIC_CONTRIBUTION_AMOUNT") > 0M)).AsDataTable();
                ldtbMASEERHICAmt.TableName = busConstant.ReportTableName27;
                idsMemberAnnualStatement.Tables.Add(ldtbMASEERHICAmt.Copy());

                // Summary statement - Retirement Benefits
                DataTable ldtbMASSummaryBenefits = new DataTable();
                ldtbMASSummaryBenefits = ldtbMASDBPersonCalculation.Clone();
                DateTime ldteStatementEffectiveDate = new DateTime();
                if (ldtbBatchRequest.Rows.Count > 0)
                {
                    if (!Convert.IsDBNull(ldtbBatchRequest.Rows[0]["STATEMENT_EFFECTIVE_DATE"]))
                        ldteStatementEffectiveDate = Convert.ToDateTime(ldtbBatchRequest.Rows[0]["STATEMENT_EFFECTIVE_DATE"]);
                }
                foreach (DataRow ldtrRow in ldtbMASPersonPlan.Rows)
                {
                    if (ldtbMASDBPersonCalculation.AsEnumerable().Where(o => o.Field<int>("PLAN_ID") == Convert.ToInt32(ldtrRow["PLAN_ID"]) &&
                                                                                            o.Field<string>("IS_RULE_OR_AGE_INDICATOR") == busConstant.Flag_Yes &&
                                                                                            o.Field<DateTime>("RETIREMENT_DATE") != DateTime.MinValue).Any())
                    {
                        DataTable ldtbMASSummaryStmtRetrBenefits = ldtbMASDBPersonCalculation.AsEnumerable().Where(o => o.Field<int>("PLAN_ID") == Convert.ToInt32(ldtrRow["PLAN_ID"]) &&
                                                                                                o.Field<string>("IS_RULE_OR_AGE_INDICATOR") == busConstant.Flag_Yes)
                                                                                                .OrderBy(o => o.Field<DateTime>("RETIREMENT_DATE")).AsDataTable();

                        if (ldtbMASSummaryStmtRetrBenefits.Rows.Count > 0)
                        {
                            // if (Convert.ToDateTime(ldtbMASSummaryStmtRetrBenefits.Rows[0]["RETIREMENT_DATE"]) < ldteStatementEffectiveDate) PROD PIR 7330
                            // PIR 8824 - to exclude past age from Member's Retirement Benefits grid in Summary report
                            int rowIndex = 0;
                            foreach (DataRow ldtrRow1 in ldtbMASSummaryStmtRetrBenefits.Rows)
                            {
                                string lstrAge = ldtrRow1["AGE_DESCRIPTION"].ToString();
                                string[] lastrAge = lstrAge.Split(' ');
                                if (lstrAge.Contains("Age"))
                                {
                                    ldecMasCalcAge = Convert.ToDecimal(lastrAge[1]);
                                }
                                else
                                {
                                    ldecMasCalcAge = Convert.ToDecimal(ldtrRow["AGE_DESCRIPTION"]);
                                }
                                if (ldecMasCalcAge >= ldecAge)
                                {
                                    ldtbMASSummaryBenefits.ImportRow(ldtbMASSummaryStmtRetrBenefits.Rows[rowIndex]);
                                }
                                rowIndex++;                
                            }
                        }
                        else
                        {
                            DataTable ldtbTemp = ldtbMASDBPersonCalculation.AsEnumerable().Where(o => o.Field<string>("IS_DEFAULT_RETIREMENT") == busConstant.Flag_Yes &&
                                                                                                        o.Field<int>("PLAN_ID") == Convert.ToInt32(ldtrRow["PLAN_ID"])).AsDataTable();
                            if (ldtbTemp.Rows.Count > 0)
                                ldtbMASSummaryBenefits.ImportRow(ldtbTemp.Rows[0]);
                        }
                    }
                }
                ldtbMASSummaryBenefits.TableName = busConstant.ReportTableName28;
                idsMemberAnnualStatement.Tables.Add(ldtbMASSummaryBenefits.Copy());
            }
            return idsMemberAnnualStatement;
        }

        
        // PIR 8824
        private decimal CalculateAgeBasedOnStatementEffectiveDate(DateTime DOB, DateTime statementEffectiveDate)
        {
            decimal ldecMonthAndYear = 0.00M;
            int lintMonths = 0, lintMemberAgeYear = 0, lintMemberAgeMonths = 0;

            busPersonBase.CalculateAge(DOB, statementEffectiveDate, ref lintMonths, ref ldecMonthAndYear, 4, ref lintMemberAgeYear, ref lintMemberAgeMonths);

            return ldecMonthAndYear;
        }
    }
}
