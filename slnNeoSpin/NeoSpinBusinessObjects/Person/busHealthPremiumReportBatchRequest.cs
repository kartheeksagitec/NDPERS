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
using System.Linq;

#endregion

namespace NeoSpin.BusinessObjects
{
	/// <summary>
	/// Class NeoSpin.BusinessObjects.busHealthPremiumReportBatchRequest:
	/// Inherited from busHealthPremiumReportBatchRequestGen, the class is used to customize the business object busHealthPremiumReportBatchRequestGen.
	/// </summary>
	[Serializable]
	public class busHealthPremiumReportBatchRequest : busHealthPremiumReportBatchRequestGen
	{
        public busBase ibusbase { get; set; }
        public busDBCacheData ibusDBCacheData { get; set; }

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

        public Collection<busOrgPlan> iclbProviderOrgPlan { get; set; }        
        public void LoadActiveProviders()
        {
            DataTable ldtbActiveProviders = busNeoSpinBase.Select("cdoIbsHeader.LoadAllActiveProviders", new object[1] { history_date_no_null });
            iclbProviderOrgPlan = new busBase().GetCollection<busOrgPlan>(ldtbActiveProviders, "icdoOrgPlan");
        }

        public DataTable idtbOrgPlanProviders { get; set; }
        public void LoadAllOrgPlanProviders()
        {
            idtbOrgPlanProviders = busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadAllOrgPlanProviders", new object[0] { });
        }

        public DataTable idtbOrgPlan { get; set; }
        public void LoadOrgPlan()
        {
            idtbOrgPlan = busNeoSpinBase.Select("cdoHealthPremiumReportBatchRequest.LoadOrgPlanForHealth", new object[0] { });
        }

        public DataTable idtbActiveHistory { get; set; }

        DataTable ldtbFinalReportTable = new DataTable();

        public DataSet CreateHealthPremiumReport()
        {
            DataSet ldsReport = new DataSet("Health Premium Report");
            ibusbase = new busBase();
            idtbActiveHistory = Select("cdoHealthPremiumReportBatchRequest.LoadAllActiveMembersHistory", new object[1] { history_date_no_null });
            ldtbFinalReportTable = CreateReportTable();
            LoadDBCacheData();
            LoadActiveProviders();
            LoadAllOrgPlanProviders();
            LoadOrgPlan();
            LoadActiveHealthMembers();
            LoadRGroupMembers();
            if (IsIBS)
                LoadIBSMembers();
            ldsReport.Tables.Add(ldtbFinalReportTable);
            return ldsReport;
        }

        private void LoadActiveHealthMembers()
        {            
            //uat pir 1344
            DataTable ldtbResult = Select("cdoHealthPremiumReportBatchRequest.LoadActiveEnrollments", new object[7]{ 
                                                history_date_no_null, icdoHealthPremiumReportBatchRequest.health_insurance_type_value ?? string.Empty,
                                                start_date_no_null, end_date_no_null, icdoHealthPremiumReportBatchRequest.coverage_code ?? string.Empty,
                                                icdoHealthPremiumReportBatchRequest.perslink_id, icdoHealthPremiumReportBatchRequest.org_id});
            foreach (DataRow ldtrRow in ldtbResult.Rows)
            {
                CalculatePremiumAmount(ldtrRow, false);
            }
        }

        private void CalculatePremiumAmount(DataRow adtrRow, bool ablnIsRGroup)
        {
            decimal ldecEmprSharePremium = 0.00M, ldecEmprShareFee = 0.00M, ldecEmprShareRHICAmt = 0.00M, ldecEmprShareOtherRHICAmt = 0.00M, ldecEmprShareJSRHICAmt = 0.00M;
            decimal ldecEmprShareBuydown = 0.00M , ldecEmprShareMedicarePartDAmt = 0.00M;
            var lobjGhdv = new busPersonAccountGhdv
            {
                icdoPersonAccountGhdv = new cdoPersonAccountGhdv(),
                icdoPersonAccount = new cdoPersonAccount(),
                ibusPerson = new busPerson { icdoPerson = new cdoPerson() },
                ibusPlan = new busPlan { icdoPlan = new cdoPlan() },
                ibusPaymentElection = new busPersonAccountPaymentElection { icdoPersonAccountPaymentElection = new cdoPersonAccountPaymentElection() },
                ibusOrgPlan = new busOrgPlan { icdoOrgPlan = new cdoOrgPlan() },
                ibusPersonEmploymentDetail = new busPersonEmploymentDetail
                {
                    icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail(),
                    ibusPersonEmployment = new busPersonEmployment
                    {
                        icdoPersonEmployment = new cdoPersonEmployment(),
                        ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() }
                    },
                }
            };
            lobjGhdv.icdoPersonAccountGhdv.LoadData(adtrRow);
            lobjGhdv.icdoPersonAccount.LoadData(adtrRow);
            lobjGhdv.ibusPerson.icdoPerson.LoadData(adtrRow);
            lobjGhdv.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.LoadData(adtrRow);
            lobjGhdv.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.LoadData(adtrRow);
            lobjGhdv.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.LoadData(adtrRow);
            lobjGhdv.ibusPlan.icdoPlan.LoadData(adtrRow);
            lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.LoadData(adtrRow);
            if (idtbOrgPlan.IsNotNull() && idtbOrgPlan.Rows.Count > 0)
            {
                 var lenuList = from row in idtbOrgPlan.AsEnumerable()
                           where
                               row.Field<int>("org_id") ==lobjGhdv.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id &&
                               busGlobalFunctions.CheckDateOverlapping(history_date_no_null,
                                                                       row.Field<DateTime>("participation_start_date"),
                                                                       row.Field<DateTime?>("participation_end_date"))
                           select row;
                DataTable ldtbResult = lenuList.AsDataTable();
                if (ldtbResult.Rows.Count > 0)
                {
                    lobjGhdv.ibusOrgPlan.icdoOrgPlan.LoadData(ldtbResult.Rows[0]);
                }
            }

            //Loading the History Object                
            if ((idtbActiveHistory != null) && (idtbActiveHistory.Rows.Count > 0))
            {
                DataRow[] larrRow = idtbActiveHistory.FilterTable(busConstant.DataType.Numeric, "person_account_ghdv_id", lobjGhdv.icdoPersonAccountGhdv.person_account_ghdv_id);
                lobjGhdv.iclbPersonAccountGHDVHistory = ibusbase.GetCollection<busPersonAccountGhdvHistory>(larrRow, "icdoPersonAccountGhdvHistory");
            }
            busPersonAccountGhdvHistory lbusHistory = lobjGhdv.LoadHistoryByDate(history_date_no_null);
            lobjGhdv = lbusHistory.LoadGHDVObject(lobjGhdv);
            if (lobjGhdv.ibusPerson == null) lobjGhdv.LoadPerson();
            if (lobjGhdv.ibusPlan == null) lobjGhdv.LoadPlan();

            if ((idtbOrgPlanProviders != null) && (lobjGhdv.ibusOrgPlan != null)
                && (lobjGhdv.ibusOrgPlan.icdoOrgPlan.org_plan_id > 0) && (iclbProviderOrgPlan != null))
            {
                DataRow[] larrRow = idtbOrgPlanProviders.FilterTable(busConstant.DataType.Numeric, "org_plan_id", lobjGhdv.ibusOrgPlan.icdoOrgPlan.org_plan_id);
                if (larrRow != null && larrRow.Length > 0)
                {
                    lobjGhdv.ibusOrgPlan.iclbOrgPlanProvider = GetCollection<busOrgPlanProvider>(larrRow, "icdoOrgPlanProvider");
                    lobjGhdv.ibusProviderOrgPlan = LoadProviderOrgPlanByProvider(lobjGhdv.ibusOrgPlan.iclbOrgPlanProvider, busConstant.PlanIdGroupHealth);
                }
            }

            /* UAT PIR 476, Including other and JS RHIC Amount */
            decimal ldecFeeAmt = 0.00M; decimal ldecRHICAmt = 0.00M;
            decimal ldecBuydownAmt = 0.00M;
            decimal ldecMedicarePartDAmt = 0.00M;
            /* UAT PIR 476, Including other and JS RHIC Amount */
            decimal ldecOthrRHICAmt = 0.00M; decimal ldecJSRHICAmt = 0.00M;
            /* UAT PIR 476 ends here */
            //uat pir 1429 : to post ghdv_history_id
            int lintGHDVHistoryID = 0;
            string lstrGroupNumber = string.Empty;
            //prod pir 6076
            string lstrCoverageCodeValue = string.Empty, lstrRateStructureCode = string.Empty;
            //pir 7705
            decimal ldecHSAAmt = 0.00M;
            decimal ldecHSAVendorAmt = 0.0M;
            busPersonAccountLife lobjLife = new busPersonAccountLife();
            busPersonAccountLtc lobjLTC = new busPersonAccountLtc();
            busPersonAccountEAP lobjEAP = new busPersonAccountEAP();
            busPersonAccountMedicarePartDHistory lobjMedicare = new busPersonAccountMedicarePartDHistory();  //PIR 15434
            decimal ldecPremiumAmount = busRateHelper.GetInsurancePremiumAmount(lobjGhdv.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization,
                                                        history_date_no_null, lobjGhdv.icdoPersonAccountGhdv.person_account_id, busConstant.PlanIdGroupHealth,
                                                        ref ldecFeeAmt, ref ldecBuydownAmt, ref ldecMedicarePartDAmt, ref ldecRHICAmt, ref ldecOthrRHICAmt, ref ldecJSRHICAmt, ref ldecHSAAmt, ref ldecHSAVendorAmt,
                                                        lobjLife, lobjGhdv,
                                                        lobjLTC, lobjEAP, lobjMedicare, iobjPassInfo, ibusDBCacheData, ref lintGHDVHistoryID, ref lstrGroupNumber,
                                                        ref lstrCoverageCodeValue, ref lstrRateStructureCode); //prod pir 6076
            //uat pir 1344
            //--Start--//
            ldecEmprSharePremium = ldecEmprShareFee = ldecEmprShareRHICAmt = ldecEmprShareOtherRHICAmt = ldecEmprShareJSRHICAmt = 0.0m;
            if (ablnIsRGroup &&
                !string.IsNullOrEmpty(lobjGhdv.icdoPersonAccountGhdv.cobra_type_value) &&
                lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id > 0 &&
                lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share > 0 &&
                lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share < 100)
            {
                ldecEmprSharePremium = Math.Round(ldecPremiumAmount *
                                           lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                ldecEmprShareFee = Math.Round(ldecFeeAmt *
                    lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                ldecEmprShareBuydown = Math.Round(ldecBuydownAmt *
                    lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                ldecEmprShareRHICAmt = Math.Round(ldecRHICAmt *
                    lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                ldecEmprShareOtherRHICAmt = Math.Round(ldecOthrRHICAmt *
                    lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                ldecEmprShareJSRHICAmt = Math.Round(ldecJSRHICAmt *
                    lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                ldecEmprShareMedicarePartDAmt = Math.Round(ldecMedicarePartDAmt *
                    lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);//PIR 14271

                ldecPremiumAmount = ldecEmprSharePremium;
                ldecFeeAmt = ldecEmprShareFee;
                ldecBuydownAmt = ldecEmprShareBuydown;
                ldecRHICAmt = ldecEmprShareRHICAmt;
                ldecOthrRHICAmt = ldecEmprShareOtherRHICAmt;
                ldecJSRHICAmt = ldecEmprShareJSRHICAmt;
                ldecMedicarePartDAmt = ldecEmprShareMedicarePartDAmt;//PIR 14271
            }

            DateTime ldteStartDate; string lstrEndDate = string.Empty;
            if (lobjGhdv.icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeRetiree)
            {
                lobjGhdv.LoadHealthParticipationDate();
                ldteStartDate = lobjGhdv.idtHealthParticipationDate;
            }
            else
                ldteStartDate = lobjGhdv.icdoPersonAccount.start_date;
            lobjGhdv.icdoPersonAccountGhdv.rate_structure_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(411, lobjGhdv.icdoPersonAccountGhdv.rate_structure_value);
            if (lobjGhdv.icdoPersonAccount.end_date != DateTime.MinValue)
                lstrEndDate = lobjGhdv.icdoPersonAccount.end_date.ToString(busConstant.DateFormatMMddyyyy);

            if (icdoHealthPremiumReportBatchRequest.rate_structure_code.IsNullOrEmpty() ||
                    (icdoHealthPremiumReportBatchRequest.rate_structure_code.IsNotNullOrEmpty() &&
                     icdoHealthPremiumReportBatchRequest.rate_structure_code == lobjGhdv.icdoPersonAccountGhdv.rate_structure_code))
            {
                AddRow(lobjGhdv.icdoPersonAccount.person_id, lobjGhdv.icdoPersonAccountGhdv.health_insurance_type_description,
                    lobjGhdv.icdoPersonAccountGhdv.coverage_code, lobjGhdv.icdoPersonAccountGhdv.rate_structure_description,
                    ldteStartDate, lstrEndDate, lobjGhdv.icdoPersonAccountGhdv.rate_structure_code,
                    ldecPremiumAmount, ldecRHICAmt, lobjGhdv.icdoPersonAccountGhdv.FeeAmount);
            }
        }

        private void LoadRGroupMembers()
        {
            DataTable ldtbResult = Select("cdoHealthPremiumReportBatchRequest.LoadRGroupMembers", new object[7]{  
                                                icdoHealthPremiumReportBatchRequest.health_insurance_type_value ?? string.Empty,
                                                icdoHealthPremiumReportBatchRequest.coverage_code ?? string.Empty,
                                                start_date_no_null, end_date_no_null, history_date_no_null,
                                                icdoHealthPremiumReportBatchRequest.perslink_id, icdoHealthPremiumReportBatchRequest.org_id});
            foreach (DataRow ldtrRow in ldtbResult.Rows)
            {
                CalculatePremiumAmount(ldtrRow, true);
            }
        }

        private void LoadIBSMembers()
        {
            //uat pir 1344
            decimal ldecEmprSharePremium = 0.00M, ldecEmprShareFee = 0.00M, ldecEmprShareRHICAmt = 0.00M, ldecEmprShareOtherRHICAmt = 0.00M, ldecEmprShareJSRHICAmt = 0.00M;
            decimal ldecEmpShareBuydown = 0.00M;
            decimal ldecEmpShareMedicarePartD = 0.00M;
            DataTable ldtbIBSHistory = Select("cdoHealthPremiumReportBatchRequest.LoadAllIBSHistory", new object[1] { history_date_no_null });
            DataTable ldtbResult = Select("cdoHealthPremiumReportBatchRequest.LoadIBSMembers", new object[6]{  
                                                icdoHealthPremiumReportBatchRequest.health_insurance_type_value ?? string.Empty,
                                                icdoHealthPremiumReportBatchRequest.coverage_code ?? string.Empty,
                                                start_date_no_null, end_date_no_null, history_date_no_null,
                                                icdoHealthPremiumReportBatchRequest.perslink_id});
            foreach (DataRow ldtrRow in ldtbResult.Rows)
            {
                string lstrCoverageCode = string.Empty;
                decimal ldecGroupHealthFeeAmt = 0.00M;
                decimal ldecBuydownAmt = 0.00M;
                decimal ldecRHICAmt = 0.00M;
                /* UAT PIR 476, Including other and JS RHIC Amount */
                decimal ldecOthrRHICAmount = 0.00M;
                decimal ldecJSRHICAmount = 0.00M;
                /* UAT PIR 476 ends here */
                decimal ldecPremiumAmt = 0.00M;
                decimal ldecTotalPremiumAmt = 0.00M;
                decimal ldecProviderPremiumAmt = 0.00M;
                decimal ldecMemberPremiumAmt = 0.00M;
                decimal ldecMedicarePartD = 0.00M;

                var lobjGhdv = new busPersonAccountGhdv
                {
                    icdoPersonAccountGhdv = new cdoPersonAccountGhdv(),
                    icdoPersonAccount = new cdoPersonAccount(),
                    ibusPerson = new busPerson { icdoPerson = new cdoPerson() },
                    ibusPlan = new busPlan { icdoPlan = new cdoPlan() },
                    ibusPaymentElection = new busPersonAccountPaymentElection { icdoPersonAccountPaymentElection = new cdoPersonAccountPaymentElection() }
                };
                lobjGhdv.icdoPersonAccountGhdv.LoadData(ldtrRow);
                lobjGhdv.icdoPersonAccount.LoadData(ldtrRow);
                lobjGhdv.ibusPerson.icdoPerson.LoadData(ldtrRow);
                lobjGhdv.ibusPlan.icdoPlan.LoadData(ldtrRow);
                lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.LoadData(ldtrRow);

                lobjGhdv.idtbCachedCoverageRef = ibusDBCacheData.idtbCachedCoverageRef;
                lobjGhdv.idtbCachedRateRef = ibusDBCacheData.idtbCachedRateRef;
                lobjGhdv.idtbCachedRateStructureRef = ibusDBCacheData.idtbCachedRateStructureRef;

                //Loading the History Object                
                if ((ldtbIBSHistory != null) && (ldtbIBSHistory.Rows.Count > 0))
                {
                    DataRow[] larrRow = ldtbIBSHistory.FilterTable(busConstant.DataType.Numeric,  "person_account_ghdv_id",
                                                                    lobjGhdv.icdoPersonAccountGhdv.person_account_ghdv_id);

                    lobjGhdv.iclbPersonAccountGHDVHistory =
                        ibusbase.GetCollection<busPersonAccountGhdvHistory>(larrRow, "icdoPersonAccountGhdvHistory");
                }
                busPersonAccountGhdvHistory lobjPAGhdvHistory = lobjGhdv.LoadHistoryByDate(history_date_no_null);
                lobjGhdv = lobjPAGhdvHistory.LoadGHDVObject(lobjGhdv);
                if (lobjGhdv.ibusPerson == null) lobjGhdv.LoadPerson();
                if (lobjGhdv.ibusPlan == null) lobjGhdv.LoadPlan();

                //Initialize the Org Object to Avoid the NULL error
                lobjGhdv.InitializeObjects();
                lobjGhdv.idtPlanEffectiveDate = history_date_no_null;

                //For Dependent COBRA, we need to load Member Employment
                if (lobjGhdv.icdoPersonAccount.from_person_account_id > 0)
                {
                    //Load Member GHDV Object
                    lobjGhdv.ibusMemberGHDVForDependent = new busPersonAccountGhdv();
                    lobjGhdv.ibusMemberGHDVForDependent.FindGHDVByPersonAccountID(lobjGhdv.icdoPersonAccount.from_person_account_id);
                    lobjGhdv.ibusMemberGHDVForDependent.FindPersonAccount(lobjGhdv.icdoPersonAccount.from_person_account_id);
                    lobjGhdv.iblnIsDependentCobra = true;
                }

                //Loading the Provider Org Plan for All GHDV Plans here.
                if (iclbProviderOrgPlan != null)
                {
                    busOrgPlan lbusProviderOrgPlan = iclbProviderOrgPlan.FirstOrDefault(i => i.icdoOrgPlan.plan_id == lobjGhdv.icdoPersonAccount.plan_id);
                    if (lbusProviderOrgPlan != null)
                    {
                        lobjGhdv.ibusProviderOrgPlan = lbusProviderOrgPlan;
                    }
                    else
                    {
                        lobjGhdv.LoadActiveProviderOrgPlan(history_date_no_null);
                    }
                }
                else
                {
                    lobjGhdv.LoadActiveProviderOrgPlan(history_date_no_null);
                }

                //we need org plan object for determining health participation date for IBS COBRA Members
                if (lobjGhdv.icdoPersonAccountGhdv.cobra_type_value.IsNotNullOrEmpty())
                {
                    //For Dependent COBRA, we need to load Member Employment
                    if (lobjGhdv.icdoPersonAccount.from_person_account_id > 0)
                    {
                        lobjGhdv.LoadEmploymentDetailByDate(lobjGhdv.idtPlanEffectiveDate, lobjGhdv.ibusMemberGHDVForDependent, true, true);
                    }
                    else
                    {
                        lobjGhdv.LoadEmploymentDetailByDate(lobjGhdv.idtPlanEffectiveDate, true);
                    }
                    if (lobjGhdv.icdoPersonAccount.person_employment_dtl_id > 0)
                    {
                        lobjGhdv.LoadPersonEmploymentDetail();
                        lobjGhdv.ibusPersonEmploymentDetail.LoadPersonEmployment();
                        lobjGhdv.LoadOrgPlan(lobjGhdv.idtPlanEffectiveDate);
                    }
                }

                if (lobjGhdv.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                {
                    lobjGhdv.LoadRateStructureForUserStructureCode();
                }
                else
                {
                    lobjGhdv.LoadHealthParticipationDate();
                    //To Get the Rate Structure Code (Derived Field)
                    lobjGhdv.LoadRateStructure(history_date_no_null);
                }

                //Get the Coverage Ref ID
                lobjGhdv.LoadCoverageRefID();

                //Get the Premium Amount
                lobjGhdv.GetMonthlyPremiumAmountByRefID(history_date_no_null);

                if (lobjGhdv.icdoPersonAccountGhdv.Coverage_Ref_ID > 0)
                {
                    DataTable ldtbCoverageCode = busNeoSpinBase.Select("cdoIbsHeader.GetCoverageCodeDescription", new object[1] { lobjGhdv.icdoPersonAccountGhdv.Coverage_Ref_ID });
                    if (ldtbCoverageCode.Rows.Count > 0)
                        lstrCoverageCode = ldtbCoverageCode.Rows[0]["CLIENT_DESCRIPTION"].ToString();
                    //uat pir 1344
                    //--Start--//
                    ldecEmprSharePremium = ldecEmprShareFee = ldecEmprShareRHICAmt = ldecEmprShareOtherRHICAmt = ldecEmprShareJSRHICAmt = 0.0m;
                    ldecEmpShareBuydown = ldecEmpShareMedicarePartD = 0.0m;
                    if (!string.IsNullOrEmpty(lobjGhdv.icdoPersonAccountGhdv.cobra_type_value) &&
                        lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id > 0 &&
                        lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share > 0 &&
                        lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share < 100)
                    {
                        ldecEmprSharePremium = Math.Round(lobjGhdv.icdoPersonAccountGhdv.PremiumExcludingFeeAmount *
                                                   lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                        ldecEmprShareFee = Math.Round(lobjGhdv.icdoPersonAccountGhdv.FeeAmount *
                            lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                        ldecEmpShareBuydown = Math.Round(lobjGhdv.icdoPersonAccountGhdv.BuydownAmount *
                            lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                        ldecEmpShareMedicarePartD = Math.Round(lobjGhdv.icdoPersonAccountGhdv.MedicarePartDAmount *
                            lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);//PIR 14271
                        ldecEmprShareRHICAmt = Math.Round(lobjGhdv.icdoPersonAccountGhdv.total_rhic_amount *
                            lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                        ldecEmprShareOtherRHICAmt = Math.Round(lobjGhdv.icdoPersonAccountGhdv.other_rhic_amount *
                            lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                        ldecEmprShareJSRHICAmt = Math.Round(lobjGhdv.icdoPersonAccountGhdv.js_rhic_amount *
                            lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                    }
                    ldecPremiumAmt = lobjGhdv.icdoPersonAccountGhdv.PremiumExcludingFeeAmount - ldecEmprSharePremium;
                    ldecGroupHealthFeeAmt = lobjGhdv.icdoPersonAccountGhdv.FeeAmount - ldecEmprShareFee;
                    ldecBuydownAmt = lobjGhdv.icdoPersonAccountGhdv.BuydownAmount - ldecEmpShareBuydown;
                    ldecMedicarePartD = lobjGhdv.icdoPersonAccountGhdv.MedicarePartDAmount - ldecEmpShareMedicarePartD;//PIR 14271

                    ldecRHICAmt = lobjGhdv.icdoPersonAccountGhdv.total_rhic_amount - ldecEmprShareRHICAmt;
                    /* UAT PIR 476, Including other and JS RHIC Amount */
                    ldecOthrRHICAmount = lobjGhdv.icdoPersonAccountGhdv.other_rhic_amount - ldecEmprShareOtherRHICAmt;
                    ldecJSRHICAmount = lobjGhdv.icdoPersonAccountGhdv.js_rhic_amount - ldecEmprShareJSRHICAmt;
                    /* UAT PIR 476 ends here */
                    //--End--//
                    ldecMemberPremiumAmt = ldecPremiumAmt + ldecGroupHealthFeeAmt - ldecRHICAmt - ldecBuydownAmt + ldecMedicarePartD;//PIR 14271
                    ldecTotalPremiumAmt = ldecPremiumAmt + ldecGroupHealthFeeAmt - ldecBuydownAmt + ldecMedicarePartD;//PIR 14271
                    ldecProviderPremiumAmt = ldecPremiumAmt;
                }

                DateTime ldteStartDate; String lstrEndDate = string.Empty;
                if (lobjGhdv.icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeRetiree)
                {
                    lobjGhdv.LoadHealthParticipationDate();
                    ldteStartDate = lobjGhdv.idtHealthParticipationDate;
                }
                else
                    ldteStartDate = lobjGhdv.icdoPersonAccount.start_date;
                lobjGhdv.icdoPersonAccountGhdv.rate_structure_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(411, lobjGhdv.icdoPersonAccountGhdv.rate_structure_value);
                if (lobjGhdv.icdoPersonAccount.end_date != DateTime.MinValue)
                    lstrEndDate = lobjGhdv.icdoPersonAccount.end_date.ToString(busConstant.DateFormatMMddyyyy);

                if (icdoHealthPremiumReportBatchRequest.rate_structure_code.IsNullOrEmpty() ||
                    (icdoHealthPremiumReportBatchRequest.rate_structure_code.IsNotNullOrEmpty() &&
                     icdoHealthPremiumReportBatchRequest.rate_structure_code == lobjGhdv.icdoPersonAccountGhdv.rate_structure_code))
                {
                    AddRow(lobjGhdv.icdoPersonAccount.person_id, lobjGhdv.icdoPersonAccountGhdv.health_insurance_type_description,
                        lobjGhdv.icdoPersonAccountGhdv.coverage_code, lobjGhdv.icdoPersonAccountGhdv.rate_structure_description,
                        ldteStartDate, lstrEndDate, lobjGhdv.icdoPersonAccountGhdv.rate_structure_code, ldecTotalPremiumAmt,
                        lobjGhdv.icdoPersonAccountGhdv.total_rhic_amount, lobjGhdv.icdoPersonAccountGhdv.FeeAmount);
                }
            }
        }

        private void AddRow(int aintPersonId, string astrHealthInsuranceType, string astrCoverageCode, string astrRateStructureValue, DateTime adteStartDate,
                            String astrEndDate, string astrRateStructureCode, decimal adecTotalPremiumAmount, decimal adecRHICAmount, decimal adecAdminFee)
        {
            DataRow ldtr = ldtbFinalReportTable.NewRow();

            ldtr["PERSLINK_ID"] = aintPersonId;
            ldtr["HEALTH_INSURANCE_TYPE"] = astrHealthInsuranceType;
            ldtr["COVERAGE_CODE"] = astrCoverageCode;
            ldtr["RATE_STRUCTURE_VALUE"] = astrRateStructureValue;
            ldtr["RATE_STRUCTURE_CODE"] = astrRateStructureCode;
            ldtr["START_DATE"] = adteStartDate;
            ldtr["END_DATE"] = astrEndDate;
            ldtr["TOTAL_PREMIUM_AMOUNT"] = adecTotalPremiumAmount;
            ldtr["RHIC_AMOUNT"] = adecRHICAmount;
            ldtr["ADMIN_FEE"] = adecAdminFee;

            ldtbFinalReportTable.Rows.Add(ldtr);
        }

        private DataTable CreateReportTable()
        {
            DataTable ldtbHealthPremiumReport = new DataTable();
            ldtbHealthPremiumReport.TableName = busConstant.ReportTableName;
            ldtbHealthPremiumReport.Columns.Add("PERSLINK_ID", Type.GetType("System.Int32"));
            ldtbHealthPremiumReport.Columns.Add("HEALTH_INSURANCE_TYPE", Type.GetType("System.String"));
            ldtbHealthPremiumReport.Columns.Add("COVERAGE_CODE", Type.GetType("System.String"));
            ldtbHealthPremiumReport.Columns.Add("RATE_STRUCTURE_CODE", Type.GetType("System.String"));
            ldtbHealthPremiumReport.Columns.Add("RATE_STRUCTURE_VALUE", Type.GetType("System.String"));
            ldtbHealthPremiumReport.Columns.Add("START_DATE", Type.GetType("System.DateTime"));
            ldtbHealthPremiumReport.Columns.Add("END_DATE", Type.GetType("System.String"));
            ldtbHealthPremiumReport.Columns.Add("TOTAL_PREMIUM_AMOUNT", Type.GetType("System.Decimal"));
            ldtbHealthPremiumReport.Columns.Add("RHIC_AMOUNT", Type.GetType("System.Decimal"));
            ldtbHealthPremiumReport.Columns.Add("ADMIN_FEE", Type.GetType("System.Decimal"));
            return ldtbHealthPremiumReport;
        }


        /// <summary>
        /// Returns Next Bill date if null
        /// </summary>
        public DateTime history_date_no_null
        {
            get
            {
                if (icdoHealthPremiumReportBatchRequest.history_date == DateTime.MinValue)
                    return DateTime.Today.GetFirstDayofNextMonth();
                return icdoHealthPremiumReportBatchRequest.history_date;
            }
        }

        /// <summary>
        /// Returns SQL min value if null
        /// </summary>
        public DateTime start_date_no_null
        {
            get
            {
                if (icdoHealthPremiumReportBatchRequest.plan_start_date == DateTime.MinValue)
                    return new DateTime(1900, 01, 01);
                return icdoHealthPremiumReportBatchRequest.plan_start_date;
            }
        }

        /// <summary>
        /// Returns SQL min value if null
        /// </summary>
        public DateTime end_date_no_null
        {
            get
            {
                if (icdoHealthPremiumReportBatchRequest.plan_end_date == DateTime.MinValue)
                    return new DateTime(2900, 01, 01);
                return icdoHealthPremiumReportBatchRequest.plan_end_date;
            }
        }

        public bool IsIBS
        {
            get
            {
                if (icdoHealthPremiumReportBatchRequest.org_id == 0)
                    return true;
                return false;
            }
        }

        private busOrgPlan LoadProviderOrgPlanByProvider(Collection<busOrgPlanProvider> aclbOrgPlanProvider, int aintPlanId)
        {
            busOrgPlan lbusOrgPlanToReturn = new busOrgPlan { icdoOrgPlan = new cdoOrgPlan() };
            foreach (busOrgPlanProvider lbusOPProvider in aclbOrgPlanProvider)
            {
                foreach (var lbusOrgPlan in iclbProviderOrgPlan)
                {
                    if ((lbusOrgPlan.icdoOrgPlan.org_id == lbusOPProvider.icdoOrgPlanProvider.provider_org_id) &&
                       (lbusOrgPlan.icdoOrgPlan.plan_id == aintPlanId))
                    {
                        if (busGlobalFunctions.CheckDateOverlapping(history_date_no_null, lbusOrgPlan.icdoOrgPlan.participation_start_date,
                            lbusOrgPlan.icdoOrgPlan.participation_end_date))
                        {
                            lbusOrgPlanToReturn = lbusOrgPlan;
                            break;
                        }
                    }
                }
                if (lbusOrgPlanToReturn.icdoOrgPlan.org_plan_id > 0) break;
            }
            return lbusOrgPlanToReturn;
        }

        public string istrOrgCode { get; set; }

        public override void BeforePersistChanges()
        {
            icdoHealthPremiumReportBatchRequest.org_id = busGlobalFunctions.GetOrgIdFromOrgCode(istrOrgCode);
            if (icdoHealthPremiumReportBatchRequest.batch_request_id == 0)
                icdoHealthPremiumReportBatchRequest.status_value = busConstant.Vendor_Payment_Status_NotProcessed;
            base.BeforePersistChanges();
        }

        public override void AfterPersistChanges()
        {
            istrOrgCode = busGlobalFunctions.GetOrgCodeFromOrgId(icdoHealthPremiumReportBatchRequest.org_id);
            base.AfterPersistChanges();
        }

        
	}
}
