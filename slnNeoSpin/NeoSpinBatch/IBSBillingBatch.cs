#region Using directives
using System;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.DataObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using System.Globalization;
using System.Collections;
using Sagitec.CorBuilder;
using System.Linq;
using Sagitec.ExceptionPub;
using Sagitec.Bpm;
#endregion
namespace NeoSpinBatch
{
    class IBSBillingBatch : busNeoSpinBatch
    {
        DataTable ldtbIBSMbrs = new DataTable();
        public IBSBillingBatch()
        {

        }

        private busIbsHeader _ibusIbsHeader;
        public busIbsHeader ibusIbsHeader
        {
            get
            {
                return _ibusIbsHeader;
            }
            set
            {
                _ibusIbsHeader = value;
            }
        }

        public busIbsHeader ibusLastPostedRegularIBSHeader { get; set; }
        public void LoadLastPostedRegularIBSHeader()
        {
            ibusLastPostedRegularIBSHeader = new busIbsHeader { icdoIbsHeader = new cdoIbsHeader() };
            DataTable ldtbList = busNeoSpinBase.SelectWithOperator<cdoIbsHeader>(
              new string[3] { "report_type_value", "report_status_value", "ibs_header_id" },
               new string[3] { "=", "=", "<>" },
              new object[3] { busConstant.IBSHeaderReportTypeRegular, busConstant.IBSHeaderStatusPosted, ibusIbsHeader.icdoIbsHeader.ibs_header_id }, "billing_month_and_year desc");
            if (ldtbList.Rows.Count > 0)
            {
                ibusLastPostedRegularIBSHeader.icdoIbsHeader.LoadData(ldtbList.Rows[0]);
            }
        }

        public DataTable idtbPersonIBSRemittanceBalanceForward { get; set; }
        public void LoadPersonIBSRemittnaceBalanceForward()
        {
            if (ibusLastPostedRegularIBSHeader == null)
                LoadLastPostedRegularIBSHeader();
            idtbPersonIBSRemittanceBalanceForward = new DataTable();
            if (ibusLastPostedRegularIBSHeader.icdoIbsHeader.run_date != DateTime.MinValue)
                idtbPersonIBSRemittanceBalanceForward = busNeoSpinBase.Select("cdoIbsHeader.LoadPersonIBSRemittanceBalanceForward", new object[1] { ibusLastPostedRegularIBSHeader.icdoIbsHeader.run_date });
        }

        public DataTable idtbPersonAdjustmentAmount { get; set; }
        public void LoadPersonAdjustmentAmount()
        {
            idtbPersonAdjustmentAmount = busNeoSpinBase.Select("cdoIbsHeader.LoadPersonAdjustmentAmount", new object[0] { });
        }

        private void LoadIBSMembers()
        {
            ldtbIBSMbrs = busNeoSpinBase.Select("cdoIbsHeader.LoadIBSMembersForRegularBillingBatch",
                                                new object[2]
                                                    {
                                                        ibusIbsHeader.icdoIbsHeader.ibs_header_id,
                                                        ibusIbsHeader.icdoIbsHeader.billing_month_and_year
                                                    });
        }

        public void ProcessIBSDetailRecords()
        {
            istrProcessName = "IBS Billing";

            idlgUpdateProcessLog("IBS Billing", "INFO", istrProcessName);
            if (ibusIbsHeader == null)
            {
                ibusIbsHeader = new busIbsHeader { icdoIbsHeader = new cdoIbsHeader() };
                ibusIbsHeader.icdoIbsHeader.billing_month_and_year = iobjSystemManagement.icdoSystemManagement.batch_date.AddMonths(1);
                ibusIbsHeader.icdoIbsHeader.billing_month_and_year =
                    new DateTime(ibusIbsHeader.icdoIbsHeader.billing_month_and_year.Year,
                                 ibusIbsHeader.icdoIbsHeader.billing_month_and_year.Month, 1);
            }

            if (!ibusIbsHeader.LoadCurrentRegularIBSHeader())
            {
                iobjPassInfo.BeginTransaction();
                try
                {
                    idlgUpdateProcessLog("Creating IBS Header", "INFO", istrProcessName);
                    ibusIbsHeader.CreateIBSRegularHeader();
                    idlgUpdateProcessLog("IBS Header Created", "INFO", istrProcessName);
                    idlgUpdateProcessLog("Creating JS RHIC Bill", "INFO", istrProcessName);
                    ibusIbsHeader.CreateJSRHICBill();
                    idlgUpdateProcessLog("JS RHIC Bill created", "INFO", istrProcessName);
                    iobjPassInfo.Commit();
                }
                catch (Exception e)
                {
                    iobjPassInfo.Rollback();
                    idlgUpdateProcessLog("Exception Occured :" + e.Message, "ERR", istrProcessName);
                    ExceptionManager.Publish(e);
                    return;
                }
            }

            //Set the Batch Date Property
            ibusIbsHeader.idtBatchDate = ibusIbsHeader.icdoIbsHeader.billing_month_and_year;
            //set the gl posting date
            ibusIbsHeader.idtGLPostingDate = iobjSystemManagement.icdoSystemManagement.batch_date;

            //Loading DB Cache (optimization)
            idlgUpdateProcessLog("Loading DB Cache Data", "INFO", istrProcessName);
            ibusIbsHeader.LoadDBCacheData();

            idlgUpdateProcessLog("Loading All Active Providers", "INFO", iobjBatchSchedule.step_name);
            //Loading Complete Activte Provider Org Plan List (Optimization Purpose)
            ibusIbsHeader.LoadActiveProviders();

            idlgUpdateProcessLog("Loading Life Option Records for All Insurance Members", "INFO", iobjBatchSchedule.step_name);
            //Loading the Life Option Data by Org (Optimization)
            ibusIbsHeader.LoadLifeOptionData();

            idlgUpdateProcessLog("Loading GHDV History Records for All Insurance Members", "INFO", iobjBatchSchedule.step_name);
            //Loading the GHDV History (Optimization)
            ibusIbsHeader.LoadGHDVHistory();

            idlgUpdateProcessLog("Loading LIFE History Records for All Insurance Members", "INFO", iobjBatchSchedule.step_name);
            //Loading the Life History (Optimization)
            ibusIbsHeader.LoadLifeHistory();

            idlgUpdateProcessLog("Loading Medicare Part D History Records for All Insurance Members", "INFO", iobjBatchSchedule.step_name);
            //Loading the Medicare Part D History (Optimization)
            ibusIbsHeader.LoadMedicarePartDHistory();

            //Exlcuded the R Group Members (IBS ORG ID IS NOT NULL for all plans except LIFE.. LIFE we will do the logic here)
            idlgUpdateProcessLog("Loading IBS Members", "INFO", iobjBatchSchedule.step_name);
            LoadIBSMembers();

            //prod pir 933
            //loading all person account dependent information
            idlgUpdateProcessLog("Loading Person Account Depenedent Information", "INFO", iobjBatchSchedule.step_name);
            LoadPersonAccountDepenedents();

            DataTable ldtCodeValue = iobjPassInfo.isrvDBCache.GetCodeValues(1922);
                    
            int lintCurrIndex = 0;
            bool lblnInTransaction = false;
            bool lblnSuccess = false;
            bool lblnErrorFound = false;
            //uat pir 1344
            decimal ldecEmprSharePremium = 0.00M, ldecEmprShareFee = 0.00M, ldecEmprShareRHICAmt = 0.00M, ldecEmprShareOtherRHICAmt = 0.00M, ldecEmprShareJSRHICAmt = 0.00M;
            try
            {
                idlgUpdateProcessLog("Processing IBS Detail Records", "INFO", istrProcessName);
                busBase lbusBase = new busBase();
                foreach (DataRow ldrRow in ldtbIBSMbrs.Rows)
                {
                    if (!lblnInTransaction)
                    {
                        iobjPassInfo.BeginTransaction();
                        lblnInTransaction = true;
                    }
                    lintCurrIndex++;
                    lblnErrorFound = false;

                    var lbusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                    lbusPersonAccount.icdoPersonAccount.LoadData(ldrRow);

                    lbusPersonAccount.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                    lbusPersonAccount.ibusPerson.icdoPerson.LoadData(ldrRow);

                    lbusPersonAccount.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
                    lbusPersonAccount.ibusPlan.icdoPlan.LoadData(ldrRow);

                    lbusPersonAccount.ibusPaymentElection = new busPersonAccountPaymentElection
                                                                {
                                                                    icdoPersonAccountPaymentElection =
                                                                        new cdoPersonAccountPaymentElection()
                                                                };
                    lbusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.LoadData(ldrRow);

                    //uat pir 1461 --//start
                    string lstrPaymentMethod = string.Empty;
                    if (lbusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date > ibusIbsHeader.icdoIbsHeader.billing_month_and_year)
                    {
                        lstrPaymentMethod = busConstant.IBSModeOfPaymentPersonalCheck;
                    }
                    else if (lbusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value == busConstant.IBSModeOfPaymentACH)
                    {
                        lbusPersonAccount.LoadPersonAccountAchDetail();
                        if (lbusPersonAccount.iclbPersonAccountAchDetail.Count == 0)
                        {
                            lstrPaymentMethod = busConstant.IBSModeOfPaymentPersonalCheck;
                        }
                        busPersonAccountAchDetail lobjACHDetail = lbusPersonAccount.iclbPersonAccountAchDetail.Where(o => busGlobalFunctions.CheckDateOverlapping(ibusIbsHeader.icdoIbsHeader.billing_month_and_year,
                            o.icdoPersonAccountAchDetail.ach_start_date, o.icdoPersonAccountAchDetail.ach_end_date == DateTime.MinValue ? DateTime.MaxValue : o.icdoPersonAccountAchDetail.ach_end_date))
                            .FirstOrDefault();
                        if (lobjACHDetail != null && lobjACHDetail.icdoPersonAccountAchDetail.pre_note_flag == busConstant.Flag_No)
                        {
                            lstrPaymentMethod = lbusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value;
                        }
                        else
                            lstrPaymentMethod = busConstant.IBSModeOfPaymentPersonalCheck;
                    }
                    else
                    {
                        lstrPaymentMethod = lbusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value;
                    }

                    idlgUpdateProcessLog("Processing IBS Detail for the PERSLinkID " +
                                                Convert.ToString(lbusPersonAccount.icdoPersonAccount.person_id), "INFO", istrProcessName);

                    busIbsDetail lobjIbsDetail = null;
                    //PIR 26419
                    string lstrCurrentPAPlanParticipationStatus = lbusPersonAccount.icdoPersonAccount.plan_participation_status_value;

                    if ((lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth) ||
                        (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental) ||
                       (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision))
                    {
                        string lstrCoverageCode = string.Empty;
                        decimal ldecGroupHealthFeeAmt = 0.00M;
                        decimal ldecRHICAmt = 0.00M;
                        /* UAT PIR 476, Including other and JS RHIC Amount */
                        decimal ldecOthrRHICAmount = 0.00M;
                        decimal ldecJSRHICAmount = 0.00M;
                        /* UAT PIR 476 ends here */
                        decimal ldecPremiumAmt = 0.00M;
                        decimal ldecTotalPremiumAmt = 0.00M;
                        decimal ldecBuydownAmount = 0.00M;
                        decimal ldecProviderPremiumAmt = 0.00M;
                        decimal ldecMemberPremiumAmt = 0.00M;
                        decimal ldecMedicarePartDAmount = 0.00M;
                        //uat pir 1429 :- add ghdv_history_id in ibs_Detail
                        int lintGHDVHistoryID = 0;
                        string lstrGroupNumber = string.Empty;
                        //prod pir 6076
                        string lstrCoverageCodeValue = string.Empty, lstrRateStructureCode = string.Empty;

                        var lobjGhdv = new busPersonAccountGhdv { icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };
                        lobjGhdv.icdoPersonAccountGhdv.LoadData(ldrRow);
                        lobjGhdv.icdoPersonAccount = lbusPersonAccount.icdoPersonAccount;
                        lobjGhdv.ibusPerson = lbusPersonAccount.ibusPerson;
                        lobjGhdv.ibusPlan = lbusPersonAccount.ibusPlan;
                        lobjGhdv.ibusPaymentElection = lbusPersonAccount.ibusPaymentElection;

                        lobjGhdv.idtbCachedCoverageRef = ibusIbsHeader.ibusDBCacheData.idtbCachedCoverageRef;
                        lobjGhdv.idtbCachedDentalRate = ibusIbsHeader.ibusDBCacheData.idtbCachedDentalRate;
                        lobjGhdv.idtbCachedHealthRate = ibusIbsHeader.ibusDBCacheData.idtbCachedHealthRate;
                        lobjGhdv.idtbCachedHmoRate = ibusIbsHeader.ibusDBCacheData.idtbCachedHMORate;
                        lobjGhdv.idtbCachedRateRef = ibusIbsHeader.ibusDBCacheData.idtbCachedRateRef;
                        lobjGhdv.idtbCachedRateStructureRef = ibusIbsHeader.ibusDBCacheData.idtbCachedRateStructureRef;
                        lobjGhdv.idtbCachedVisionRate = ibusIbsHeader.ibusDBCacheData.idtbCachedVisionRate;

                        //Loading the History Object                
                        if ((ibusIbsHeader.idtbGHDVHistory != null) && (ibusIbsHeader.idtbGHDVHistory.Rows.Count > 0))
                        {
                            DataRow[] larrRow = ibusIbsHeader.idtbGHDVHistory.FilterTable(busConstant.DataType.Numeric,
                                                                            "person_account_ghdv_id",
                                                                            lobjGhdv.icdoPersonAccountGhdv.person_account_ghdv_id);

                            lobjGhdv.iclbPersonAccountGHDVHistory =
                                lbusBase.GetCollection<busPersonAccountGhdvHistory>(larrRow, "icdoPersonAccountGhdvHistory");
                        }

                        //Look the Provider Org ID first , If null, get the default provider
                        //Mail From Satya Dated On 1/29/2009

                        //Get the GHDV History Object By Billing Month Year
                        busPersonAccountGhdvHistory lobjPAGhdvHistory = lobjGhdv.LoadHistoryByDate(ibusIbsHeader.icdoIbsHeader.billing_month_and_year);
                        if (lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id == 0)
                        {
                            idlgUpdateProcessLog(
                                "Error : No History Record Found for Person Account = " +
                                lbusPersonAccount.icdoPersonAccount.person_account_id, "INFO", istrProcessName);
                            lblnErrorFound = true;
                        }

                        if (!lblnErrorFound)
                        {
                            //For Health and Medicare we load this after initialize objects method call
                            if (!IsHealthOrMedicare(lbusPersonAccount.icdoPersonAccount.plan_id))
                            {
                                LoadGHDVProviderOrgPlan(lobjGhdv);
                            }

                            if (IsHealthOrMedicare(lbusPersonAccount.icdoPersonAccount.plan_id))
                            {
                                lobjGhdv = lobjPAGhdvHistory.LoadGHDVObject(lobjGhdv);
                                if (lobjGhdv.icdoPersonAccount.from_person_account_id == 0 && lbusPersonAccount.icdoPersonAccount.from_person_account_id > 0)
                                    lobjGhdv.icdoPersonAccount.from_person_account_id = lbusPersonAccount.icdoPersonAccount.from_person_account_id;
                                //uat pir 1429 :- to post ghdv_history_id
                                lintGHDVHistoryID = lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id;
                                //lstrGroupNumber = lobjGhdv.GetGroupNumber(); //pir 7973
                                                                
                                if (lobjGhdv.ibusPerson == null)
                                    lobjGhdv.LoadPerson();

                                if (lobjGhdv.ibusPlan == null)
                                    lobjGhdv.LoadPlan();
                                //Initialize the Org Object to Avoid the NULL error
                                lobjGhdv.InitializeObjects();
                                lobjGhdv.idtPlanEffectiveDate = ibusIbsHeader.icdoIbsHeader.billing_month_and_year;

                                //For Dependent COBRA, we need to load Member Employment
                                if (lobjGhdv.icdoPersonAccount.from_person_account_id > 0)
                                {
                                    //Load Member GHDV Object
                                    lobjGhdv.ibusMemberGHDVForDependent = new busPersonAccountGhdv();
                                    lobjGhdv.ibusMemberGHDVForDependent.FindGHDVByPersonAccountID(lobjGhdv.icdoPersonAccount.from_person_account_id);
                                    lobjGhdv.ibusMemberGHDVForDependent.FindPersonAccount(lobjGhdv.icdoPersonAccount.from_person_account_id);
                                    lobjGhdv.iblnIsDependentCobra = true;
                                }
                                //Loading Provider Org Plan
                                LoadGHDVProviderOrgPlan(lobjGhdv);

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
                                    lobjGhdv.LoadRateStructure(ibusIbsHeader.icdoIbsHeader.billing_month_and_year);
                                }
                                lstrGroupNumber = lobjGhdv.GetGroupNumber(); //pir 7973
                                //Get the Coverage Ref ID
                                lobjGhdv.LoadCoverageRefID();

                                //prod pir 6076
                                lstrCoverageCodeValue = lobjGhdv.icdoPersonAccountGhdv.coverage_code;
                                lstrRateStructureCode = !string.IsNullOrEmpty(lobjGhdv.icdoPersonAccountGhdv.overridden_structure_code) ?
                                    lobjGhdv.icdoPersonAccountGhdv.overridden_structure_code : lobjGhdv.icdoPersonAccountGhdv.rate_structure_code;

                                //Get the Premium Amount
                                lobjGhdv.GetMonthlyPremiumAmountByRefID(ibusIbsHeader.icdoIbsHeader.billing_month_and_year);

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
                                        ibusIbsHeader.GetGroupHealthCoverageCodeDescription(lobjGhdv.icdoPersonAccountGhdv.Coverage_Ref_ID);
                                    //uat pir 1344
                                    //--Start--//
                                    ldecEmprSharePremium = ldecEmprShareFee = ldecEmprShareRHICAmt = ldecEmprShareOtherRHICAmt = ldecEmprShareJSRHICAmt = 0.0m;
                                    if (!string.IsNullOrEmpty(lobjGhdv.icdoPersonAccountGhdv.cobra_type_value) &&
                                        lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id > 0 &&
                                        lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share > 0 &&
                                        lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share < 100)
                                    {
                                        ldecEmprSharePremium = Math.Round(lobjGhdv.icdoPersonAccountGhdv.PremiumExcludingFeeAmount *
                                                                   lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                                        ldecEmprShareFee = Math.Round(lobjGhdv.icdoPersonAccountGhdv.FeeAmount *
                                            lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                                        ldecEmprShareRHICAmt = Math.Round(lobjGhdv.icdoPersonAccountGhdv.total_rhic_amount *
                                            lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                                        ldecEmprShareOtherRHICAmt = Math.Round(lobjGhdv.icdoPersonAccountGhdv.other_rhic_amount *
                                            lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                                        ldecEmprShareJSRHICAmt = Math.Round(lobjGhdv.icdoPersonAccountGhdv.js_rhic_amount *
                                            lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                                    }
                                    ldecPremiumAmt = lobjGhdv.icdoPersonAccountGhdv.PremiumExcludingFeeAmount - ldecEmprSharePremium;
                                    ldecGroupHealthFeeAmt = lobjGhdv.icdoPersonAccountGhdv.FeeAmount - ldecEmprShareFee;

                                    ldecRHICAmt = lobjGhdv.icdoPersonAccountGhdv.total_rhic_amount - ldecEmprShareRHICAmt;
                                    ldecBuydownAmount = lobjGhdv.icdoPersonAccountGhdv.BuydownAmount;
                                    ldecMedicarePartDAmount = lobjGhdv.icdoPersonAccountGhdv.MedicarePartDAmount;
                                    /* UAT PIR 476, Including other and JS RHIC Amount */
                                    ldecOthrRHICAmount = lobjGhdv.icdoPersonAccountGhdv.other_rhic_amount - ldecEmprShareOtherRHICAmt;
                                    ldecJSRHICAmount = lobjGhdv.icdoPersonAccountGhdv.js_rhic_amount - ldecEmprShareJSRHICAmt;
                                    /* UAT PIR 476 ends here */
                                    //--End--//
                                    ldecMemberPremiumAmt = ldecPremiumAmt + ldecGroupHealthFeeAmt - ldecRHICAmt - ldecBuydownAmount + ldecMedicarePartDAmount;//PIR 14271
                                    ldecTotalPremiumAmt = ldecPremiumAmt + ldecGroupHealthFeeAmt - ldecBuydownAmount + ldecMedicarePartDAmount;//PIR 14271
                                    ldecProviderPremiumAmt = ldecPremiumAmt;
                                }
                            }
                            else if (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental)
                            {
                                //uat pir 2056
                                lintGHDVHistoryID = lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id;
                                ldecPremiumAmt =
                                    busRateHelper.GetDentalPremiumAmount(
                                        lobjGhdv.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                                        lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.dental_insurance_type_value,
                                        lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value,
                                        ibusIbsHeader.icdoIbsHeader.billing_month_and_year,
                                        ibusIbsHeader.ibusDBCacheData.idtbCachedDentalRate, iobjPassInfo);

                                ldecMemberPremiumAmt = ldecPremiumAmt;
                                ldecTotalPremiumAmt = ldecPremiumAmt;
                                ldecProviderPremiumAmt = ldecPremiumAmt;
                                lstrCoverageCode = lobjGhdv.icdoPersonAccountGhdv.level_of_coverage_description;

                                //6077 : removal of person account ghdv history id
                                //lstrGroupNumber = lobjGhdv.GetBranch();
                                //lstrGroupNumber += lobjGhdv.GetBenefitOptionCode();
								
								/*IBS Adjustment creation logic changed (Affected area enroll to enroll scenario) - logged under Backlog PIR 8022 and 8111 - Internal finding 
								*Look for provider_org_id from person account
								*/
                                lobjGhdv = lobjPAGhdvHistory.LoadGHDVObject(lobjGhdv);
                                if (busGlobalFunctions.GetOrgCodeFromOrgId(lbusPersonAccount.icdoPersonAccount.provider_org_id) ==
                                     busGlobalFunctions.GetData1ByCodeValue(1213, busConstant.DELTAProviderCodeValue, iobjPassInfo))
                                {
                                    lstrGroupNumber = lobjGhdv.GetGroupNumber(); // PIR 10448 - new provider
                                }
                                else
                                {
                                    //6077 : removal of person account ghdv history id
                                    lstrGroupNumber = lobjGhdv.GetBranch();
                                    lstrGroupNumber += lobjGhdv.GetBenefitOptionCode();
                                }
                            }
                            else if (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision)
                            {
                                //uat pir 2056
                                lintGHDVHistoryID = lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id;

                                //PROD PIR 5308
                                //to get the group number for Superior Vision provider
                                lobjGhdv = lobjPAGhdvHistory.LoadGHDVObject(lobjGhdv);
                                if (busGlobalFunctions.GetOrgCodeFromOrgId(lbusPersonAccount.icdoPersonAccount.provider_org_id) ==
                                    busGlobalFunctions.GetData1ByCodeValue(1213, busConstant.SuperiorVisionProviderCodeValue, iobjPassInfo))
                                {
                                    lstrGroupNumber = lobjGhdv.GetGroupNumber();
                                }
                                //6077 : removal of person account ghdv history id
                                else
                                {
                                    lstrGroupNumber = lobjGhdv.GetHIPAAReferenceID();
                                }

                                //prod pir 6076 
                                lstrCoverageCodeValue = lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.coverage_code;
                                lstrRateStructureCode = !string.IsNullOrEmpty(lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.overridden_structure_code) ?
                                    lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.overridden_structure_code : lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.rate_structure_code;

                                ldecPremiumAmt =
                                    busRateHelper.GetVisionPremiumAmount(
                                        lobjGhdv.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                                        lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.vision_insurance_type_value,
                                        lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value,
                                        ibusIbsHeader.icdoIbsHeader.billing_month_and_year,
                                        ibusIbsHeader.ibusDBCacheData.idtbCachedVisionRate, iobjPassInfo);

                                ldecMemberPremiumAmt = ldecPremiumAmt;
                                ldecTotalPremiumAmt = ldecPremiumAmt;
                                ldecProviderPremiumAmt = ldecPremiumAmt;
                                lstrCoverageCode = lobjGhdv.icdoPersonAccountGhdv.level_of_coverage_description;
                            }

                            if (!lblnErrorFound)
                            {
                                /* UAT PIR 476, Including other and JS RHIC Amount */
                                lobjIbsDetail =
                                    ibusIbsHeader.CreateIBSDetailForGHDV(lbusPersonAccount.icdoPersonAccount.person_account_id,
                                                                         lbusPersonAccount.icdoPersonAccount.person_id,
                                                                         lbusPersonAccount.icdoPersonAccount.plan_id,
                                                                         ibusIbsHeader.icdoIbsHeader.billing_month_and_year,
                                                                         lstrPaymentMethod,
                                                                         lstrCoverageCode, ldecGroupHealthFeeAmt, ldecBuydownAmount,ldecMedicarePartDAmount,
                                                                         ldecMemberPremiumAmt, ldecRHICAmt, ldecOthrRHICAmount, ldecJSRHICAmount,
                                                                         ldecProviderPremiumAmt, ldecTotalPremiumAmt, lobjGhdv.ibusProviderOrgPlan.icdoOrgPlan.org_id,
                                                                         aintGHDVHistoryID: lintGHDVHistoryID, astrGroupNumber: lstrGroupNumber,
                                                                         astrCoverageCodeValue: lstrCoverageCodeValue, astrRateStructureCode: lstrRateStructureCode);//uat pir 1429
                                /* UAT PIR 476 ends here */
                            }
                        }
                    }

                    if (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife)
                    {
                        var lobjLife = new busPersonAccountLife { icdoPersonAccountLife = new cdoPersonAccountLife() };
                        lobjLife.icdoPersonAccountLife.LoadData(ldrRow);
                        lobjLife.icdoPersonAccount = lbusPersonAccount.icdoPersonAccount;
                        lobjLife.ibusPerson = lbusPersonAccount.ibusPerson;
                        lobjLife.ibusPlan = lbusPersonAccount.ibusPlan;

                        if ((ibusIbsHeader.idtbPALifeOptionHistory != null) && (ibusIbsHeader.idtbPALifeOptionHistory.Rows.Count > 0))
                        {
                            DataRow[] larrRow = ibusIbsHeader.idtbPALifeOptionHistory.FilterTable(busConstant.DataType.Numeric, "person_account_id",
                                                                             lbusPersonAccount.icdoPersonAccount.person_account_id);

                            //Loading the Life Option Data
                            lobjLife.LoadLifeOptionDataFromHistory(larrRow);
                        }

                        lobjLife.ibusPaymentElection = lbusPersonAccount.ibusPaymentElection;
                        lobjLife.ibusPaymentElection.icdoPersonAccount = lbusPersonAccount.icdoPersonAccount;

                        lobjLife.idtbCachedLifeRate = ibusIbsHeader.ibusDBCacheData.idtbCachedLifeRate;

                        //Get the Provider Org ID from History
                        busPersonAccountLifeHistory lobjPALifeHistory = new busPersonAccountLifeHistory();
                        lobjPALifeHistory.icdoPersonAccountLifeHistory = new cdoPersonAccountLifeHistory();
                        foreach (busPersonAccountLifeOption lobjPALifeOption in lobjLife.iclbLifeOption)
                        {
                            if (lobjPALifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Basic)
                            {
                                lobjPALifeHistory = lobjLife.LoadHistoryByDate(lobjPALifeOption, ibusIbsHeader.icdoIbsHeader.billing_month_and_year);
                                break;
                            }
                        }

                        if (ibusIbsHeader.iclbProviderOrgPlan != null)
                        {
                            busOrgPlan lbusProviderOrgPlan = ibusIbsHeader.iclbProviderOrgPlan.FirstOrDefault(i => i.icdoOrgPlan.plan_id == lobjLife.icdoPersonAccount.plan_id);
                            if (lbusProviderOrgPlan != null)
                            {
                                lobjLife.ibusProviderOrgPlan = lbusProviderOrgPlan;
                            }
                            else
                            {
                                lobjLife.LoadActiveProviderOrgPlan(ibusIbsHeader.icdoIbsHeader.billing_month_and_year);
                            }
                        }
                        else
                        {
                            lobjLife.LoadActiveProviderOrgPlan(ibusIbsHeader.icdoIbsHeader.billing_month_and_year);
                        }

                        //Loading the History Object
                        if ((ibusIbsHeader.idtbLifeHistory != null) && (ibusIbsHeader.idtbLifeHistory.Rows.Count > 0))
                        {
                            DataRow[] larrRow = ibusIbsHeader.idtbLifeHistory.FilterTable(busConstant.DataType.Numeric, "person_account_id",
                                                                            lbusPersonAccount.icdoPersonAccount.person_account_id);

                            lobjLife.iclbPersonAccountLifeHistory =
                                lbusBase.GetCollection<busPersonAccountLifeHistory>(larrRow, "icdoPersonAccountLifeHistory");
                        }

                        lobjLife.LoadMemberAge(ibusIbsHeader.icdoIbsHeader.billing_month_and_year);
                        lobjLife.GetMonthlyPremiumAmount(ibusIbsHeader.icdoIbsHeader.billing_month_and_year);

                        decimal ldecMemberPremiumAmt = 0.00M;
                        decimal ldecTotalPremiumAmt = 0.00M;
                        decimal ldecProviderPremiumAmt = 0.00M;
                        decimal ldecLifeBasicPremiumAmt = 0.00M;
                        decimal ldecLifeSuppPremiumAmt = 0.00M;
                        decimal ldecLifeSpouseSuppPremiumAmt = 0.00M;
                        decimal ldecLifeDepSuppPremiumAmt = 0.00M;


                        ldecLifeBasicPremiumAmt = lobjLife.idecLifeBasicPremiumAmt;
                        ldecLifeSuppPremiumAmt = lobjLife.idecLifeSupplementalPremiumAmount;
                        ldecLifeSpouseSuppPremiumAmt = lobjLife.idecSpouseSupplementalPremiumAmt;
                        ldecLifeDepSuppPremiumAmt = lobjLife.idecDependentSupplementalPremiumAmt;

                        ldecMemberPremiumAmt = lobjLife.idecTotalMonthlyPremium;
                        ldecTotalPremiumAmt = lobjLife.idecTotalMonthlyPremium;
                        ldecProviderPremiumAmt = lobjLife.idecTotalMonthlyPremium;

                        //Exclude the Amount which are paid by ORG PIR : 1793
                        if (lobjLife.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id > 0)
                        {
                            ldecMemberPremiumAmt -= ldecLifeBasicPremiumAmt;
                            ldecTotalPremiumAmt -= ldecLifeBasicPremiumAmt;
                            ldecProviderPremiumAmt -= ldecLifeBasicPremiumAmt;

                            ldecLifeBasicPremiumAmt = 0;
                        }

                        if (lobjLife.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_supplemental_org_id > 0)
                        {
                            ldecMemberPremiumAmt -= ldecLifeSuppPremiumAmt;
                            ldecTotalPremiumAmt -= ldecLifeSuppPremiumAmt;
                            ldecProviderPremiumAmt -= ldecLifeSuppPremiumAmt;

                            ldecLifeSuppPremiumAmt = 0;
                        }


                        if (ldecMemberPremiumAmt != 0)
                        {
                            lobjIbsDetail =
                                ibusIbsHeader.CreateIBSDetailForLife(lbusPersonAccount.icdoPersonAccount.person_account_id,
                                                                     lbusPersonAccount.icdoPersonAccount.person_id,
                                                                     lbusPersonAccount.icdoPersonAccount.plan_id,
                                                                     ibusIbsHeader.icdoIbsHeader.billing_month_and_year,
                                                                     lstrPaymentMethod,
                                                                     ldecMemberPremiumAmt, ldecProviderPremiumAmt,
                                                                     ldecTotalPremiumAmt, ldecLifeBasicPremiumAmt, ldecLifeSuppPremiumAmt,
                                                                     ldecLifeSpouseSuppPremiumAmt, ldecLifeDepSuppPremiumAmt,
                                                                     lobjLife.idecADAndDBasicRate, lobjLife.idecADAndDSupplementalRate,
                                                                     lobjLife.idecBasicCoverageAmount, lobjLife.idecSuppCoverageAmount,
                                                                     lobjLife.idecSpouseSuppCoverageAmount, lobjLife.idecDepSuppCoverageAmount, lobjLife.ibusProviderOrgPlan.icdoOrgPlan.org_id);
                        }
                    }

                    if (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdLTC)
                    {
                        busPersonAccountLtc lobjLtc = new busPersonAccountLtc();
                        lobjLtc.FindPersonAccount(lbusPersonAccount.icdoPersonAccount.person_account_id);
                        lobjLtc.LoadLtcOptionUpdateMemberFromHistory(ibusIbsHeader.icdoIbsHeader.billing_month_and_year);
                        lobjLtc.LoadLtcOptionUpdateSpouseFromHistory(ibusIbsHeader.icdoIbsHeader.billing_month_and_year);
                        lobjLtc.idtbCachedLtcRate = ibusIbsHeader.ibusDBCacheData.idtbCachedLtcRate;

                        busPersonAccountLtcOptionHistory lobjPALtcHistory = new busPersonAccountLtcOptionHistory();
                        lobjPALtcHistory.icdoPersonAccountLtcOptionHistory = new cdoPersonAccountLtcOptionHistory();
                        //Load the Provider Org ID by History
                        if (lobjLtc.iclbLtcOptionMember.Count > 0)
                        {
                            lobjPALtcHistory = lobjLtc.LoadHistoryByDate(lobjLtc.iclbLtcOptionMember[0], ibusIbsHeader.icdoIbsHeader.billing_month_and_year);
                        }

                        lobjLtc.LoadActiveProviderOrgPlan(ibusIbsHeader.icdoIbsHeader.billing_month_and_year);

                        lobjLtc.GetMonthlyPremiumAmount(ibusIbsHeader.icdoIbsHeader.billing_month_and_year);
                        decimal ldecMemberPremiumAmt = 0.00M;
                        decimal ldecMember3YrsPremium = 0.00M;
                        decimal ldecMember5YrsPremium = 0.00M;
                        decimal ldecSpouse3YrsPremium = 0.00M;
                        decimal ldecSpouse5YrsPremium = 0.00M;
                        ldecMemberPremiumAmt = lobjLtc.idecTotalMonthlyPremium;
                        foreach (var lbusLtcOption in lobjLtc.iclbLtcOptionMember)
                        {
                            if (lbusLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value == busConstant.LTCLevelOfCoverage3YRS)
                            {
                                ldecMember3YrsPremium = lbusLtcOption.idecMonthlyPremium;
                            }
                            else if (lbusLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value == busConstant.LTCLevelOfCoverage5YRS)
                            {
                                ldecMember5YrsPremium = lbusLtcOption.idecMonthlyPremium;
                            }
                        }

                        foreach (var lbusLtcOption in lobjLtc.iclbLtcOptionSpouse)
                        {
                            if (lbusLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value == busConstant.LTCLevelOfCoverage3YRS)
                            {
                                ldecSpouse3YrsPremium = lbusLtcOption.idecMonthlyPremium;
                            }
                            else if (lbusLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value == busConstant.LTCLevelOfCoverage5YRS)
                            {
                                ldecSpouse5YrsPremium = lbusLtcOption.idecMonthlyPremium;
                            }
                        }

                        lobjIbsDetail =
                            ibusIbsHeader.CreateIBSDetailForLTC(lbusPersonAccount.icdoPersonAccount.person_account_id,
                                                                 lbusPersonAccount.icdoPersonAccount.person_id,
                                                                 lbusPersonAccount.icdoPersonAccount.plan_id,
                                                                 ibusIbsHeader.icdoIbsHeader.billing_month_and_year,
                                                                 lstrPaymentMethod, ldecMemberPremiumAmt,
                                                                 ldecMember3YrsPremium, ldecMember5YrsPremium, ldecSpouse3YrsPremium, ldecSpouse5YrsPremium,
                                                                 lobjLtc.ibusProviderOrgPlan.icdoOrgPlan.org_id);
                    }

                    if (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD)
                    {
                        var lobjMedicarePartD = new busPersonAccountMedicarePartDHistory { icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory() };
                        lobjMedicarePartD.icdoPersonAccountMedicarePartDHistory.LoadData(ldrRow);
                        lobjMedicarePartD.icdoPersonAccount = lbusPersonAccount.icdoPersonAccount;
                        lobjMedicarePartD.ibusPerson = lbusPersonAccount.ibusPerson;
                        lobjMedicarePartD.ibusPlan = lbusPersonAccount.ibusPlan;
                        lobjMedicarePartD.ibusPaymentElection = lbusPersonAccount.ibusPaymentElection;

                        decimal ldecTotalPremiumAmt = 0.00M;
                        decimal ldecMemberPremiumAmt = 0.00M;
                        Decimal ldecLowIncomeCreditAmount = 0;

                        lobjMedicarePartD.idtbCachedHealthRate = ibusIbsHeader.ibusDBCacheData.idtbCachedMedicarePartDRate;
                        lobjMedicarePartD.iblnIsFromIBSBilling = true;

                        //Get the Medicare Part D History Object By Billing Month Year
                        busPersonAccountMedicarePartDHistory lobjPAMedicarePartDHistory = lobjMedicarePartD.LoadHistoryByDate(ibusIbsHeader.icdoIbsHeader.billing_month_and_year);
                        if (lobjPAMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.person_account_medicare_part_d_history_id == 0)
                        {
                            idlgUpdateProcessLog(
                                "Error : No History Record Found for Person Account = " +
                                lbusPersonAccount.icdoPersonAccount.person_account_id, "INFO", istrProcessName);
                            lblnErrorFound = true;
                        }

                        if (ibusIbsHeader.iclbProviderOrgPlan != null)
                        {
                            busOrgPlan lbusProviderOrgPlan = ibusIbsHeader.iclbProviderOrgPlan.FirstOrDefault(i => i.icdoOrgPlan.plan_id == lobjMedicarePartD.icdoPersonAccount.plan_id);
                            if (lbusProviderOrgPlan != null)
                            {
                                lobjMedicarePartD.ibusProviderOrgPlan = lbusProviderOrgPlan;
                            }
                            else
                            {
                                lobjMedicarePartD.LoadActiveProviderOrgPlan(ibusIbsHeader.icdoIbsHeader.billing_month_and_year);
                            }
                        }
                        else
                        {
                            lobjMedicarePartD.LoadActiveProviderOrgPlan(ibusIbsHeader.icdoIbsHeader.billing_month_and_year);
                        }

                        lobjMedicarePartD.GetTotalPremiumAmountForMedicare();
                        lobjMedicarePartD.GetPremiumAmountFromRef();

                        ldecMemberPremiumAmt = lobjMedicarePartD.TotalMonthlyPremiumAmount;
                        ldecTotalPremiumAmt = lobjMedicarePartD.TotalMonthlyPremiumAmount;

                        //lobj.FindPersonAccount(lobj.icdoPersonAccountMedicarePartDHistory.person_account_id);
                        //lobjMedicarePartD.LoadPlanEffectiveDate(); //PIR 18629

                        //Low Income Credit Amount should be populated from Ref table. 
                        
                        DataTable adtbCachedLowIncomeCreditRef = busGlobalFunctions.LoadLowIncomeCreditRefCacheData(iobjPassInfo);
                        var lenumList = adtbCachedLowIncomeCreditRef.AsEnumerable().Where(i => i.Field<Decimal>("low_income_credit") == lobjMedicarePartD.icdoPersonAccountMedicarePartDHistory.idecLISAmount).OrderByDescending(i => i.Field<DateTime>("effective_date"));
                        foreach (DataRow dr in lenumList)
                        {
                            if (Convert.ToDateTime(dr["effective_date"]).Date <= lobjMedicarePartD.idtPlanEffectiveDate.Date)
                            {
                                ldecLowIncomeCreditAmount = Convert.ToDecimal(dr["amount"]);
                                break;
                            }
                        }

                        if (!lblnErrorFound)
                        {
                            lobjIbsDetail =
                                    ibusIbsHeader.CreateIBSDetailForMedicarePartD(lbusPersonAccount.icdoPersonAccount.person_account_id,
                                    lobjMedicarePartD.icdoPersonAccountMedicarePartDHistory.member_person_id,
                                    lbusPersonAccount.icdoPersonAccount.plan_id,
                                    ibusIbsHeader.icdoIbsHeader.billing_month_and_year,
                                    lstrPaymentMethod,
                                    ldecMemberPremiumAmt,
                                    ldecTotalPremiumAmt,
                                    lobjMedicarePartD.ibusProviderOrgPlan.icdoOrgPlan.org_id, ldecLowIncomeCreditAmount, 
                                    lobjMedicarePartD.icdoPersonAccountMedicarePartDHistory.late_enrollment_penalty,
                                    lobjMedicarePartD.icdoPersonAccountMedicarePartDHistory.MonthlyPremiumAmountFromRef);//PIR 15786

                            if (lobjIbsDetail != null)
                            {
                                lobjMedicarePartD.GetTotalPremiumAmountForMedicareForPapit();
                                lobjIbsDetail.ldecTotalPremiumAmountForPAPIT = lobjMedicarePartD.TotalMonthlyPremiumAmountPAPIT;
                            }
                        }

                    }


                    if (lobjIbsDetail != null)
                    {
                        lobjIbsDetail.icdoIbsDetail.Insert();
                        lobjIbsDetail.ibusPersonAccount = lbusPersonAccount;
                        lobjIbsDetail.ibusPerson = lbusPersonAccount.ibusPerson;
                        lobjIbsDetail.ibusIbsHeader = ibusIbsHeader;
                        lobjIbsDetail.icdoIbsDetail.istrPersonAccountParticipationStatus = lstrCurrentPAPlanParticipationStatus;
                        //fixed pir : 2035
                        lobjIbsDetail.CreateOrUpdatePAPITItems();

                        //prod pir 933 : linking person account dependents for billing
                        if (lobjIbsDetail.icdoIbsDetail.plan_id == busConstant.PlanIdGroupHealth &&
                            ldtCodeValue.AsEnumerable().Where(o => o.Field<string>("data1") == lobjIbsDetail.icdoIbsDetail.coverage_code_value).Any())
                        {
                            DataRow[] ldarrPADep = idtPersonAccountDependent.FilterTable(busConstant.DataType.Numeric, "person_account_id", lobjIbsDetail.icdoIbsDetail.person_account_id);
                            lobjIbsDetail.InsertPersonAccountDependentBillingLink(ldarrPADep);
                        }
                    }

                    if (((lintCurrIndex % 100) == 0) || (lintCurrIndex == ldtbIBSMbrs.Rows.Count))
                    {
                        if (lblnInTransaction)
                        {
                            if (lintCurrIndex == ldtbIBSMbrs.Rows.Count)
                                idlgUpdateProcessLog("Processing IBS Detail for All Members Completed", "INFO", istrProcessName);

                            iobjPassInfo.Commit();
                            lblnInTransaction = false;
                            lblnSuccess = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (lblnInTransaction)
                {
                    iobjPassInfo.Rollback();
                    lblnInTransaction = false;
                    lblnSuccess = false;
                }
                ExceptionManager.Publish(e);
                idlgUpdateProcessLog("Error Occured with Message = " + e.Message, "ERR", istrProcessName);
                return;
            }

            if (lblnSuccess)
            {
                try
                {
                    iobjPassInfo.BeginTransaction();

                    //Reload the IBS Detail Collection
                    ibusIbsHeader.LoadIbsDetails();

                    ProcessIBSPersonSummary();

                    ibusIbsHeader.icdoIbsHeader.ienuObjectState = ObjectState.Update;
                    ibusIbsHeader.UpdateSummaryData(busConstant.IBSHeaderStatusPosted);

                    bool lblnAllocated = ibusIbsHeader.AllocateJSRHICRemittance();
                    //uat pir 2061 : added the gl creation along with other amount common GL creation code as per satya
                    /*if (lblnAllocated)
                    {
                        idlgUpdateProcessLog("Bill Amount Allocated to JS RHIC Bill", "INFO", istrProcessName);

                        idlgUpdateProcessLog("Generating GL for JS RHIC", "INFO", istrProcessName);
                        ibusIbsHeader.GenerateGLForJSRHICAllocation(ibusIbsHeader.ibusJsRhicBill.icdoJsRhicBill.org_id, busConstant.ItemTypeJobSeriveHealthCredit,
                                                     ibusIbsHeader.ibusJsRhicBill.icdoJsRhicBill.allocated_amount);
                        idlgUpdateProcessLog("GL Generated for JS RHIC", "INFO", istrProcessName);

                    }
                    else
                    {
                        idlgUpdateProcessLog("No Amount to Allocate to JS RHIC Bill", "INFO", istrProcessName);
                    }
                    */
                    idlgUpdateProcessLog("Posting IBS Details into Contributions", "INFO", istrProcessName);
                    ibusIbsHeader.PostIBSContributionDetails(busConstant.TransactionTypeRegularIBS);
                    idlgUpdateProcessLog("Posting Contributions Process Completed", "INFO", istrProcessName);

                    idlgUpdateProcessLog("Generating GL", "INFO", istrProcessName);
                    ibusIbsHeader.GenerateGL();
                    idlgUpdateProcessLog("GL Generated", "INFO", istrProcessName);

                    idlgUpdateProcessLog("Posting IBS Details into Provider Data", "INFO", istrProcessName);
                    ibusIbsHeader.PostProviderData();
                    idlgUpdateProcessLog("Posting Provider Data Process Completed", "INFO", istrProcessName);

                    iobjPassInfo.Commit();
                }
                catch (Exception e)
                {
                    iobjPassInfo.Rollback();
                    idlgUpdateProcessLog("Error Occured with Message = " + e.Message, "ERR", istrProcessName);
                    ExceptionManager.Publish(e);
                }
            }
			
			//PIR 15417
            //if (ibusIbsHeader.icolIbsDetail == null)
            ibusIbsHeader.LoadIbsDetailsForCorrespondance();

            //Generate Job Service RHIC Report
            GenerateJobServiceRHICReport();

            //Generate the Letter
            GenerateSFN16789();

            //UCS 40 Rate change for November month
            int lintLifeAgeChangeRunningMonth = Convert.ToInt32(busGlobalFunctions.GetData1ByCodeValue(52,
                                                                busConstant.lstrLifeAgeChangeLetterMonthData1, iobjPassInfo));
            if (ibusIbsHeader.icdoIbsHeader.billing_month_and_year.Month == lintLifeAgeChangeRunningMonth)
            {
                //ibusIbsHeader = new busIbsHeader(); //??? Remove this After testing
                //ibusIbsHeader.idtBatchDate = iobjSystemManagement.icdoSystemManagement.batch_date; //??? Remove this After testing
                if (ibusIbsHeader.iclbProviderOrgPlan == null)
                    ibusIbsHeader.LoadActiveProviders();

                //Get all the Life Providers
                IEnumerable<int> lenuLifeProvider =
                    ibusIbsHeader.iclbProviderOrgPlan.Where(i => i.icdoOrgPlan.plan_id == busConstant.PlanIdGroupLife).
                        Select(i => i.icdoOrgPlan.org_id).Distinct();

                if (lenuLifeProvider != null)
                {
                    foreach (var lintProviderOrgID in lenuLifeProvider)
                    {
                        idlgUpdateProcessLog("Generating Life Age Change Letter For the Provider :" + lintProviderOrgID.ToString(), "INFO", istrProcessName);
                        GenerateLifeAgeChangeLetter(lintProviderOrgID);
                        idlgUpdateProcessLog("Generating Life Age Change Letter Ended For the Provider :" + lintProviderOrgID.ToString(), "INFO", istrProcessName);
                    }
                }

            }
            InitiateWorkflowForDebitACHRequest();
            idlgUpdateProcessLog("IBS Billing Process Completed", "INFO", istrProcessName);
        }

        private void LoadGHDVProviderOrgPlan(busPersonAccountGhdv lobjGhdv)
        {
            //Loading the Provider Org Plan for All GHDV Plans here.
            if (ibusIbsHeader.iclbProviderOrgPlan != null)
            {
                busOrgPlan lbusProviderOrgPlan = ibusIbsHeader.iclbProviderOrgPlan.FirstOrDefault(i => i.icdoOrgPlan.plan_id == lobjGhdv.icdoPersonAccount.plan_id);
                if (lbusProviderOrgPlan != null)
                {
                    lobjGhdv.ibusProviderOrgPlan = lbusProviderOrgPlan;
                }
                else
                {
                    lobjGhdv.LoadActiveProviderOrgPlan(ibusIbsHeader.icdoIbsHeader.billing_month_and_year);
                }
            }
            else
            {
                lobjGhdv.LoadActiveProviderOrgPlan(ibusIbsHeader.icdoIbsHeader.billing_month_and_year);
            }
        }

        //Update the IBS Person Summary Data
        private void ProcessIBSPersonSummary()
        {
            if (ibusIbsHeader.iclbIBSMembers == null)
                ibusIbsHeader.LoadDistinctMembersFromIBSDetail();

            if (ibusLastPostedRegularIBSHeader == null)
                LoadLastPostedRegularIBSHeader();

            if (idtbPersonIBSRemittanceBalanceForward == null)
                LoadPersonIBSRemittnaceBalanceForward();

            if (idtbPersonAdjustmentAmount == null)
                LoadPersonAdjustmentAmount();

            if (ibusLastPostedRegularIBSHeader.iclbIbsPersonSummary == null)
                ibusLastPostedRegularIBSHeader.LoadIbsPersonSummary();

            foreach (busPerson lobjPerson in ibusIbsHeader.iclbIBSMembers)
            {
                busIbsPersonSummary lbusIbsLastPostedIbsPersonSummary = ibusLastPostedRegularIBSHeader.iclbIbsPersonSummary
                                                                        .Where(i => i.icdoIbsPersonSummary.person_id == lobjPerson.icdoPerson.person_id).FirstOrDefault();

                cdoIbsPersonSummary lcdoIbsPersonSummary = new cdoIbsPersonSummary();
                lcdoIbsPersonSummary.person_id = lobjPerson.icdoPerson.person_id;
                lcdoIbsPersonSummary.ibs_header_id = ibusIbsHeader.icdoIbsHeader.ibs_header_id;

                //Remittance Forward
                if (idtbPersonIBSRemittanceBalanceForward.Rows.Count > 0)
                {
                    DataRow[] larrRemRow = idtbPersonIBSRemittanceBalanceForward.FilterTable(busConstant.DataType.Numeric,
                                                                               "person_id",
                                                                               lobjPerson.icdoPerson.person_id);
                    if (larrRemRow != null && larrRemRow.Count() > 0)
                    {
                        if (!Convert.IsDBNull(larrRemRow[0]["remittance_balance_forward"]))
                            lcdoIbsPersonSummary.remittance_balance_forward = Convert.ToDecimal(larrRemRow[0]["remittance_balance_forward"]);
                    }
                }
                //Adjustment Amount
                if (idtbPersonAdjustmentAmount.Rows.Count > 0)
                {
                    DataRow[] larrAdjRow = idtbPersonAdjustmentAmount.FilterTable(busConstant.DataType.Numeric,
                                                                               "person_id",
                                                                               lobjPerson.icdoPerson.person_id);
                    if (larrAdjRow != null && larrAdjRow.Count() > 0)
                    {
                        if (!Convert.IsDBNull(larrAdjRow[0]["adjustment_amount"]))
                            lcdoIbsPersonSummary.adjustment_amount = Convert.ToDecimal(larrAdjRow[0]["adjustment_amount"]);
                    }
                }
                lcdoIbsPersonSummary.member_premium_amount = ibusIbsHeader.icolIbsDetail.Where(i => i.icdoIbsDetail.person_id == lobjPerson.icdoPerson.person_id)
                                                                                        .Sum(i => i.icdoIbsDetail.member_premium_amount);

                decimal ldecLastMonthAmount = 0.00M;
                if (lbusIbsLastPostedIbsPersonSummary != null)
                {
                    ldecLastMonthAmount = lbusIbsLastPostedIbsPersonSummary.icdoIbsPersonSummary.balance_forward +
                                          lbusIbsLastPostedIbsPersonSummary.icdoIbsPersonSummary.member_premium_amount +
                                          lbusIbsLastPostedIbsPersonSummary.icdoIbsPersonSummary.adjustment_amount;
                }
                //if last month no entry is present, need to enter remittance balance forward as negative, 
                //since next month we wont be using remittance balance forward to arrive at balance forward
                lcdoIbsPersonSummary.balance_forward = ldecLastMonthAmount - lcdoIbsPersonSummary.remittance_balance_forward;
                lcdoIbsPersonSummary.Insert();
            }
            //block to carry forward ibs summary from last month for those who doesnot have ibs billing this month
            //--Start--//
            Collection<busIbsPersonSummary> lclbSummaryNotInThisMonth = new Collection<busIbsPersonSummary>();
            foreach (busIbsPersonSummary lobjSummary in ibusLastPostedRegularIBSHeader.iclbIbsPersonSummary)
            {
                if (ibusIbsHeader.iclbIBSMembers.Where(o => o.icdoPerson.person_id == lobjSummary.icdoIbsPersonSummary.person_id).Any())
                    continue;

                lclbSummaryNotInThisMonth.Add(lobjSummary);
            }
            foreach (busIbsPersonSummary lobjSummary in lclbSummaryNotInThisMonth)
            {
                cdoIbsPersonSummary lcdoIbsPersonSummary = new cdoIbsPersonSummary();
                lcdoIbsPersonSummary.person_id = lobjSummary.icdoIbsPersonSummary.person_id;
                lcdoIbsPersonSummary.ibs_header_id = ibusIbsHeader.icdoIbsHeader.ibs_header_id;

                //Remittance Forward
                if (idtbPersonIBSRemittanceBalanceForward.Rows.Count > 0)
                {
                    DataRow[] larrRemRow = idtbPersonIBSRemittanceBalanceForward.FilterTable(busConstant.DataType.Numeric,
                                                                               "person_id",
                                                                               lobjSummary.icdoIbsPersonSummary.person_id);
                    if (larrRemRow != null && larrRemRow.Count() > 0)
                    {
                        if (!Convert.IsDBNull(larrRemRow[0]["remittance_balance_forward"]))
                            lcdoIbsPersonSummary.remittance_balance_forward = Convert.ToDecimal(larrRemRow[0]["remittance_balance_forward"]);
                    }
                }
                //Adjustment Amount
                if (idtbPersonAdjustmentAmount.Rows.Count > 0)
                {
                    DataRow[] larrAdjRow = idtbPersonAdjustmentAmount.FilterTable(busConstant.DataType.Numeric,
                                                                               "person_id",
                                                                               lobjSummary.icdoIbsPersonSummary.person_id);
                    if (larrAdjRow != null && larrAdjRow.Count() > 0)
                    {
                        if (!Convert.IsDBNull(larrAdjRow[0]["adjustment_amount"]))
                            lcdoIbsPersonSummary.adjustment_amount = Convert.ToDecimal(larrAdjRow[0]["adjustment_amount"]);
                    }
                }
                lcdoIbsPersonSummary.balance_forward = lobjSummary.icdoIbsPersonSummary.balance_forward +
                                                        lobjSummary.icdoIbsPersonSummary.member_premium_amount +
                                                        lobjSummary.icdoIbsPersonSummary.adjustment_amount -
                                                        lcdoIbsPersonSummary.remittance_balance_forward;

                lcdoIbsPersonSummary.Insert();
            }
            //PROD PIR 5415
            //--Start--//
            Collection<busIbsPersonSummary> lclbSummaryNotAvailable = new Collection<busIbsPersonSummary>();
            foreach (DataRow dr in idtbPersonIBSRemittanceBalanceForward.Rows)
            {
                if (dr["person_id"] != DBNull.Value)
                {
                    if (ibusIbsHeader.iclbIBSMembers.Where(o => o.icdoPerson.person_id == Convert.ToInt32(dr["person_id"])).Any() ||
                        lclbSummaryNotInThisMonth.Where(o => o.icdoIbsPersonSummary.person_id == Convert.ToInt32(dr["person_id"])).Any())
                    {
                        continue;
                    }
                    busIbsPersonSummary lobjSummary = new busIbsPersonSummary { icdoIbsPersonSummary = new cdoIbsPersonSummary() };
                    lobjSummary.icdoIbsPersonSummary.person_id = Convert.ToInt32(dr["person_id"]);
                    lclbSummaryNotAvailable.Add(lobjSummary);
                }
            }
            foreach (DataRow dr in idtbPersonAdjustmentAmount.Rows)
            {
                if (dr["person_id"] != DBNull.Value)
                {
                    if (ibusIbsHeader.iclbIBSMembers.Where(o => o.icdoPerson.person_id == Convert.ToInt32(dr["person_id"])).Any() ||
                        lclbSummaryNotInThisMonth.Where(o => o.icdoIbsPersonSummary.person_id == Convert.ToInt32(dr["person_id"])).Any())
                    {
                        continue;
                    }
                    busIbsPersonSummary lobjSummary = new busIbsPersonSummary { icdoIbsPersonSummary = new cdoIbsPersonSummary() };
                    lobjSummary.icdoIbsPersonSummary.person_id = Convert.ToInt32(dr["person_id"]);
                    lclbSummaryNotAvailable.Add(lobjSummary);
                }
            }
            foreach (busIbsPersonSummary lobjSummary in lclbSummaryNotAvailable)
            {
                cdoIbsPersonSummary lcdoIbsPersonSummary = new cdoIbsPersonSummary();
                lcdoIbsPersonSummary.person_id = lobjSummary.icdoIbsPersonSummary.person_id;
                lcdoIbsPersonSummary.ibs_header_id = ibusIbsHeader.icdoIbsHeader.ibs_header_id;

                //Remittance Forward
                if (idtbPersonIBSRemittanceBalanceForward.Rows.Count > 0)
                {
                    DataRow[] larrRemRow = idtbPersonIBSRemittanceBalanceForward.FilterTable(busConstant.DataType.Numeric,
                                                                               "person_id",
                                                                               lobjSummary.icdoIbsPersonSummary.person_id);
                    if (larrRemRow != null && larrRemRow.Count() > 0)
                    {
                        if (!Convert.IsDBNull(larrRemRow[0]["remittance_balance_forward"]))
                        {
                            lcdoIbsPersonSummary.remittance_balance_forward = Convert.ToDecimal(larrRemRow[0]["remittance_balance_forward"]);
                            //as per mail with Satya on Feb 23, 2011 - Sub :- PERSLink ID 9205 
                            lcdoIbsPersonSummary.balance_forward = -Convert.ToDecimal(larrRemRow[0]["remittance_balance_forward"]);
                        }
                    }
                }
                //Adjustment Amount
                if (idtbPersonAdjustmentAmount.Rows.Count > 0)
                {
                    DataRow[] larrAdjRow = idtbPersonAdjustmentAmount.FilterTable(busConstant.DataType.Numeric,
                                                                               "person_id",
                                                                               lobjSummary.icdoIbsPersonSummary.person_id);
                    if (larrAdjRow != null && larrAdjRow.Count() > 0)
                    {
                        if (!Convert.IsDBNull(larrAdjRow[0]["adjustment_amount"]))
                            lcdoIbsPersonSummary.adjustment_amount = Convert.ToDecimal(larrAdjRow[0]["adjustment_amount"]);
                    }
                }
                lcdoIbsPersonSummary.Insert();
            }
            //--End--//
            //--End--//
        }

        private void GenerateLifeAgeChangeLetter(int aintProviderOrgID)
        {
            //??? aintProviderOrgID = 593; //Testing Purpose
            busRateChangeLetterRequest lbusRateChangeLetterRequest = new busRateChangeLetterRequest
            {
                icdoRateChangeLetterRequest = new cdoRateChangeLetterRequest()
            };

            DateTime ldtEffectiveDate = new DateTime(DateTime.Now.Year + 1, 1, 1);
            lbusRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date = ldtEffectiveDate;
            lbusRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId = busConstant.PlanIdGroupLife;
            lbusRateChangeLetterRequest.icdoRateChangeLetterRequest.provider_org_id = aintProviderOrgID;
            lbusRateChangeLetterRequest.icdoRateChangeLetterRequest.letter_type_value = busConstant.LetterTypeValueLIFE;
            lbusRateChangeLetterRequest.LoadPlan();

            //Loading All the Providers
            if (ibusIbsHeader.iclbProviderOrgPlan == null)
                ibusIbsHeader.LoadActiveProviders();
            lbusRateChangeLetterRequest.iclbProviderOrgPlan = ibusIbsHeader.iclbProviderOrgPlan;

            //load cached data as per plan
            lbusRateChangeLetterRequest.LoadDBCacheData();

            //Load New and current effective date (Do not call load current and New Effective Date as if our Life Rate will changing based on Min Age / Max Age)
            lbusRateChangeLetterRequest.adtNewEffectiveDate = ldtEffectiveDate;
            lbusRateChangeLetterRequest.adtCurrentEffectiveDate = ldtEffectiveDate.AddDays(-1);

            //Load Provider Org Plan for New Effective Date
            lbusRateChangeLetterRequest.LoadProviderOrgPlan();

            //Loading All Employer Org Plans 
            idlgUpdateProcessLog("Loading All Employer Org Plans", "INFO", istrProcessName);
            lbusRateChangeLetterRequest.LoadAllEmployerOrgPlans();

            idlgUpdateProcessLog("Loading All Active Members Life", "INFO", istrProcessName);
            lbusRateChangeLetterRequest.idtbLifeAgeEmployerLetterLifeMembers = busBase.Select("cdoRateChangeLetterRequest.LoadActiveLifeMembers",
                                                                            new object[2] { lbusRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date, 1 });

            idlgUpdateProcessLog("Loading All Active Members Life Option", "INFO", istrProcessName);
            lbusRateChangeLetterRequest.idtbLifeAgeEmployerLetterLifeOptionHistory = busBase.Select("cdoRateChangeLetterRequest.LoadActiveLifeOption",
                                                                            new object[1] { lbusRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date });

            idlgUpdateProcessLog("Loading All Active Members Life History", "INFO", istrProcessName);
            lbusRateChangeLetterRequest.idtbLifeAgeEmployerLetterLifeHistory = busBase.Select("cdoRateChangeLetterRequest.LoadActiveLifeMembersHistory",
                                                         new object[1] { lbusRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date });

            idlgUpdateProcessLog("Loading All Members History", "INFO", istrProcessName);
            lbusRateChangeLetterRequest.idtbOrgToBillLIFEHistory = busBase.Select("cdoRateChangeLetterRequest.LoadOrgToBillLifeHistory",
                                                                            new object[1] { lbusRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date });

            lbusRateChangeLetterRequest.idtbOrgToBillLIFEOption = busBase.Select("cdoRateChangeLetterRequest.LoadOrgToBillLifeOptions",
                                                         new object[1] { lbusRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date });

            idlgUpdateProcessLog("Loading All TFFR Pension Check Members History Records", "INFO", istrProcessName);
            lbusRateChangeLetterRequest.idtbTFFRPensionCheckLIFEHistory = busBase.Select("cdoRateChangeLetterRequest.LoadTFFRPensionCheckLifeHistory",
                                                                            new object[1] { lbusRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date });

            lbusRateChangeLetterRequest.idtbTFFRPensionCheckLIFEOption = busBase.Select("cdoRateChangeLetterRequest.LoadTFFRPensionCheckLifeOptions",
                                                         new object[1] { lbusRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date });

            idlgUpdateProcessLog("Loading All IBS Members History Records", "INFO", istrProcessName);
            lbusRateChangeLetterRequest.idtbIBSMembersLIFEHistory = busBase.Select("cdoRateChangeLetterRequest.LoadIBSMembersLifeHistory",
                                                                            new object[1] { lbusRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date });
            lbusRateChangeLetterRequest.idtbIBSMembersLIFEOption = busBase.Select("cdoRateChangeLetterRequest.LoadIBSMembersLifeOptions",
                                                                            new object[1] { lbusRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date });

            //Generating Employer Rate Change Letter
            foreach (busOrgPlan lobjOrgPlan in lbusRateChangeLetterRequest.iclbOrgPlan)
            {
                //Initilaize the Collection to Avoid NULL Exception.
                lobjOrgPlan.iclbLifeAgeEmployerPremium = new Collection<busInsurancePremium>();
                lobjOrgPlan.iclbTFFRPensionCheckPremium = new Collection<busInsurancePremium>();
                lobjOrgPlan.iclbOrgToBillPremium = new Collection<busInsurancePremium>();

                idlgUpdateProcessLog(
                    "Generating Employer Life Age Change Letter for ORG Code : " +
                    lobjOrgPlan.ibusOrganization.icdoOrganization.org_code, "INFO",
                    istrProcessName);

                lbusRateChangeLetterRequest.GenerateEmployerLifeAgeChangeLetter(lobjOrgPlan);

                //Generating ORG to Bill Letters
                idlgUpdateProcessLog("Generating Employer Life Age Change Letter for Org To Bill Members of Org Code : " +
                    lobjOrgPlan.ibusOrganization.icdoOrganization.org_code, "INFO", istrProcessName);

                lbusRateChangeLetterRequest.GenerateEmployerLetterForOrgToBill(lobjOrgPlan, true, aintExcludeOnlyBasicMembers: 1);

                //TFFR Pension Check Members Logic Starts Here
                if (lobjOrgPlan.ibusOrganization.icdoOrganization.org_code == busConstant.RetirementAndInvestmentOrgCodeId)
                {
                    idlgUpdateProcessLog("Generating Employer Letter for TFFR Pension Check Members", "INFO", istrProcessName);
                    lbusRateChangeLetterRequest.GenerateEmployerLetterForTFFRPensionCheck(lobjOrgPlan, true, aintExcludeOnlyBasicMembers: 1);

                }

                lobjOrgPlan.ibusRateChangeLetterRequest = lbusRateChangeLetterRequest;

                if (lobjOrgPlan.iclbLifeAgeEmployerPremium.Count > 0 ||
                   lobjOrgPlan.iclbTFFRPensionCheckPremium.Count > 0 ||
                   lobjOrgPlan.iclbOrgToBillPremium.Count > 0)
                {
                    //Generating Employer Letter Correspondence
                    idlgUpdateProcessLog(
                       "Creating Employer Letter Correspondence for Org Code : " +
                       lobjOrgPlan.ibusOrganization.icdoOrganization.org_code, "INFO",
                       istrProcessName);
                    CreateEmployerLetterCorrespondence(lobjOrgPlan);
                }
                //if (lobjOrgPlan.iclbLifeAgeEmployerPremium.Count > 2) break; //???For Testing One Record Only
            }

            //IBS Member Letter Logic Starts
            idlgUpdateProcessLog("Generating IBS Member Rate Change Letter Started", "INFO", iobjBatchSchedule.step_name);
            idlgUpdateProcessLog("Load All IBS Members", "INFO", iobjBatchSchedule.step_name);

            DataTable ldtbIBSMembers = lbusRateChangeLetterRequest.LoadIBSMembers();
            foreach (DataRow ldrRow in ldtbIBSMembers.Rows)
            {
                busInsurancePremium lobjInsurancePremium = lbusRateChangeLetterRequest.ProcessInsurancePremiumForIBSMember(ldrRow);
                lobjInsurancePremium.ibusRateChangeLetterRequest = lbusRateChangeLetterRequest;

                // PROD PIR 5238
                lobjInsurancePremium.CalculateMemberAge();

                bool lblnGenerateLetter = true;
                bool lblnMiniumumOneChangesFound = false;
                //Systest PIR 2020 : Dont Generate Letter if either of the Amount is ZERO
                //For Life if any option premium matches, dont print it. if all of them matches, dont generate letters.
                if (lobjInsurancePremium.iclbCoverageLevelLifePremium.Count > 0)
                {
                    //PIR 23437 – Life Age update letters (PAY-4308) For Life if any option premium missmatch, then only generate the letter and showing all the life options.
                    lblnMiniumumOneChangesFound = lobjInsurancePremium.iclbCoverageLevelLifePremium.Any(i => i.idecCurrentPremium > 0 && i.idecNewPremium > 0
                                                                                                        && i.idecNewPremium > i.idecCurrentPremium);
                    
                    //PIR 23437 – Life Age update letters (PAY - 4308) if any option premium missmatch then showing all the rows even though premium is zero; hence commented
                    //for (int i = lobjInsurancePremium.iclbCoverageLevelLifePremium.Count - 1; i >= 0; i--)
                    //{
                    //   if ((lobjInsurancePremium.iclbCoverageLevelLifePremium[i].idecCurrentPremium == 0) ||
                    //        (lobjInsurancePremium.iclbCoverageLevelLifePremium[i].idecNewPremium == 0))
                    //    {
                    //        lobjInsurancePremium.iclbCoverageLevelLifePremium.RemoveAt(i);
                    //    }
                    //}

                    // 1) PROD PIR : 4776 //Satya says this change need not to be there for Members..
                    // 2) PROD PIR : 8384 //If the Premium didn't changed no need to generate letters. So uncommented.
                    // 3) PROD PIR : 8431 //Evewn if premium did not change we have to show all levels of coverages. So commented Date :--> 2012-11-28
                    //for (int i = lobjInsurancePremium.iclbCoverageLevelLifePremium.Count - 1; i >= 0; i--)
                    //{
                    //    if (lobjInsurancePremium.iclbCoverageLevelLifePremium[i].idecNewPremium == lobjInsurancePremium.iclbCoverageLevelLifePremium[i].idecCurrentPremium)
                    //    {
                    //        lobjInsurancePremium.iclbCoverageLevelLifePremium.RemoveAt(i);
                    //    }
                    //}

                    // 1) UAT PIR 1190 : If both amounts are same, dont generate the letter too. so added
                    // 2) PROD PIR 5238 : Even if no difference, generate the letter. so commented
                    //for (int i = lobjInsurancePremium.iclbCoverageLevelLifePremium.Count - 1; i >= 0; i--)
                    //{
                    //    if (lobjInsurancePremium.iclbCoverageLevelLifePremium[i].idecNewPremium != lobjInsurancePremium.iclbCoverageLevelLifePremium[i].idecCurrentPremium)
                    //    {
                    //        lblnMiniumumOneChangesFound = true;
                    //        break;
                    //    }
                    //}

                    // PROD PIR 5238
                    foreach (busInsurancePremium lobjPremium in lobjInsurancePremium.iclbCoverageLevelLifePremium)
                        lobjPremium.iintMemberAge = lobjInsurancePremium.iintMemberAge;
                }
                // PROD PIR 5238
                // if ((lobjInsurancePremium.iclbCoverageLevelLifePremium.Count == 0) || (!lblnMiniumumOneChangesFound))
                // PROD PIR 8384 : Maik Mail dated Tue 11/22/2011 8:19 PM . "I confirmed with Sharmain.  Do not generate if there is only a change in Basic coverage."
                //PIR 23437 – Life Age update letters (PAY-4308) For Life if any option premium missmatch, generate the letter.
                //if (lobjInsurancePremium.iclbCoverageLevelLifePremium.Count <= 1)
                if (!lblnMiniumumOneChangesFound)                
                    lblnGenerateLetter = false;

                if (lblnGenerateLetter)
                {
                    idlgUpdateProcessLog("Creating IBS Member Correspondence for " + lobjInsurancePremium.istrIBSMemberFullName, "INFO", iobjBatchSchedule.step_name);
                    CreateIBSLetterCorrespondence(lobjInsurancePremium);
                }
                //break; //???For Testing One Record Only
            }
        }

        private void CreateEmployerLetterCorrespondence(busOrgPlan aobjOrgPlan)
        {
            //ArrayList larrlist = new ArrayList();
            //larrlist.Add(aobjOrgPlan);
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            CreateCorrespondence("PAY-4307", aobjOrgPlan, lhstDummyTable);
        }

        private void CreateIBSLetterCorrespondence(busInsurancePremium lobjInsurancePremium)
        {
            //ArrayList larrlist = new ArrayList();
            //larrlist.Add(lobjInsurancePremium);
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            CreateCorrespondence("PAY-4308", lobjInsurancePremium, lhstDummyTable);
        }

        private void SetDeductionIdentifier(busPerson aobjPerson, busIbsDetail aobjIbsDetail)
        {
            if (aobjIbsDetail.icdoIbsDetail.mode_of_payment_value == busConstant.IBSModeOfPaymentACH)
            {
                aobjPerson.DeductionMethodIdentifier = "3";
            }
            else if (aobjIbsDetail.icdoIbsDetail.mode_of_payment_value == busConstant.IBSModeOfPaymentPensionCheck)
            {
                aobjPerson.DeductionMethodIdentifier = "1";
            }
            else if (aobjIbsDetail.icdoIbsDetail.mode_of_payment_value == busConstant.IBSModeOfPaymentPersonalCheck)
            {
                //lblnPaymentModePersonalCheck = true;
                aobjPerson.DeductionMethodIdentifier = "2";
            }
        }

        private bool IsHealthOrMedicare(int aintPlanID)
        {
            bool lblnResult = false;
            if (aintPlanID == busConstant.PlanIdGroupHealth) //Correspondance issue as discussed on call with Maik
                lblnResult = true;
            return lblnResult;
        }

        public void GenerateSFN16789()
        {
            istrProcessName = "Billing Statement";
            idlgUpdateProcessLog("Generating Individual Insurance Billing Statement", "INFO", istrProcessName);

            //Loading the Distince IBS Members
			// Loading again for SFN-16789 correspondance - To load members name correctly and not the spouse name
            //if (ibusIbsHeader.iclbIBSMembers == null)
                ibusIbsHeader.LoadDistinctMembersFromIBSDetail();
            if (ibusIbsHeader.iclbIbsPersonSummary == null)
                ibusIbsHeader.LoadIbsPersonSummary();

            foreach (busPerson lobjPerson in ibusIbsHeader.iclbIBSMembers)
            {
                bool lblnPaymentModePersonalCheck = false;

                lobjPerson.BillDate = ibusIbsHeader.icdoIbsHeader.billing_month_and_year.ToString("MMMM yyyy");
                lobjPerson.DueDate = ibusIbsHeader.icdoIbsHeader.billing_month_and_year.ToString(busConstant.DateFormatLongDate);

                foreach (busIbsDetail lobjIbsDetail in ibusIbsHeader.icolIbsDetail)
                {
                    bool lblnAmountToBeAddedInLetter = false;
                    if (lobjIbsDetail.icdoIbsDetail.person_id == lobjPerson.icdoPerson.person_id)
                    {
                        if (lobjIbsDetail.ibusIbsPersonSummary == null)
                            lobjIbsDetail.LoadIbsPersonSummary();

                        if (lobjIbsDetail.ibusPersonAccount == null)
                            lobjIbsDetail.LoadPersonAccount();

                        if (lobjIbsDetail.ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
                        {
                            //Set the Deduction Method Identifier
                            SetDeductionIdentifier(lobjPerson, lobjIbsDetail);

                            //Set the Personal Check to True if the mode of payment is Personal Check.
                            // For Regular IBS, Only for Person Check, we need to Generate Letter
                            if (lobjIbsDetail.icdoIbsDetail.mode_of_payment_value == busConstant.IBSModeOfPaymentPersonalCheck)
                            {
                                lblnPaymentModePersonalCheck = true;
                                lblnAmountToBeAddedInLetter = true;
                            }
                            else if (lobjIbsDetail.icdoIbsDetail.mode_of_payment_value == busConstant.IBSModeOfPaymentACH)
                            {
                                if (lobjIbsDetail.ibusPersonAccount.iclbPersonAccountAchDetail == null)
                                    lobjIbsDetail.ibusPersonAccount.LoadPersonAccountAchDetail();
                                foreach (busPersonAccountAchDetail lobjAchDetail in lobjIbsDetail.ibusPersonAccount.iclbPersonAccountAchDetail)
                                {
                                    if (lobjIbsDetail.ibusPersonAccount.ibusIBSBatchSchedule == null)
                                        lobjIbsDetail.ibusPersonAccount.LoadIBSBatchSchedule();
                                    if ((lobjAchDetail.icdoPersonAccountAchDetail.ach_start_date <= ibusIbsHeader.icdoIbsHeader.billing_month_and_year) // uat pir : 1461 iobjSystemManagement.icdoSystemManagement.batch_date)
                                         && (lobjAchDetail.icdoPersonAccountAchDetail.ach_end_date == DateTime.MinValue))
                                    {
                                        if (lobjAchDetail.icdoPersonAccountAchDetail.pre_note_flag == busConstant.Flag_Yes)
                                        {
                                            lblnPaymentModePersonalCheck = true;
                                            lblnAmountToBeAddedInLetter = true;
                                        }
                                    }
                                }
                            }
                            if (lblnAmountToBeAddedInLetter)
                            {
                                if (IsHealthOrMedicare(lobjIbsDetail.ibusPersonAccount.icdoPersonAccount.plan_id))
                                {
                                    lobjPerson.istrGroupHealthCoverageDescription = lobjIbsDetail.icdoIbsDetail.coverage_code;

                                    lobjPerson.idecGropHealthPremiumAmt = lobjIbsDetail.icdoIbsDetail.total_premium_amount;
                                    lobjPerson.idecHealthCredit = lobjIbsDetail.icdoIbsDetail.rhic_amount;
                                    lobjPerson.idecTotalHealthPremiumAmt = lobjIbsDetail.icdoIbsDetail.member_premium_amount;
                                }
                                else if (lobjIbsDetail.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental)
                                {
                                    lobjPerson.idecGroupDentalPremiumAmt = lobjIbsDetail.icdoIbsDetail.total_premium_amount;
                                    lobjPerson.istrGroupDentalDescription = lobjIbsDetail.icdoIbsDetail.coverage_code;
                                }
                                else if (lobjIbsDetail.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision)
                                {
                                    lobjPerson.idecGroupVisionPremiumAmt = lobjIbsDetail.icdoIbsDetail.total_premium_amount;
                                    lobjPerson.istrGroupVisionDescription = lobjIbsDetail.icdoIbsDetail.coverage_code;
                                }
                                else if (lobjIbsDetail.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife)
                                {
                                    lobjPerson.idecLifeBasicPremiumAmt = lobjIbsDetail.icdoIbsDetail.life_basic_premium_amount;
                                    lobjPerson.idecLifeSupplementalPremiumAmount = lobjIbsDetail.icdoIbsDetail.life_supp_premium_amount;
                                    lobjPerson.idecSpouseSupplementalPremiumAmt = lobjIbsDetail.icdoIbsDetail.life_spouse_supp_premium_amount;
                                    lobjPerson.idecDependentSupplementalPremiumAmt = lobjIbsDetail.icdoIbsDetail.life_dep_supp_premium_amount;
                                    lobjPerson.idecTotalLifePremiumAmount = lobjIbsDetail.icdoIbsDetail.total_premium_amount;
                                }
                                else if (lobjIbsDetail.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdLTC)
                                {
                                    lobjPerson.idecLTCPremium = lobjIbsDetail.icdoIbsDetail.total_premium_amount;
                                }
								//PIR 15417
                                else if (lobjIbsDetail.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD)
                                {
                                    //PIR 15347 - Medicare Part D bookmarks
                                    lobjPerson.idecTotalMedicarePremium = busGlobalFunctions.GetTotalMedicarePremiumAmountFromIBS(lobjPerson.icdoPerson.person_id, ibusIbsHeader.icdoIbsHeader.billing_month_and_year);
                                }
                            }
                        }
                    }
                }

                busIbsPersonSummary lobjIBSPersonSummary = ibusIbsHeader.iclbIbsPersonSummary
                                                                        .Where(o => o.icdoIbsPersonSummary.person_id == lobjPerson.icdoPerson.person_id)
                                                                        .FirstOrDefault();
                if (lobjIBSPersonSummary != null)
                {
                    lobjPerson.idecAdjustmentAmount = lobjIBSPersonSummary.icdoIbsPersonSummary.adjustment_amount;
                    lobjPerson.idecBalanceForward = lobjIBSPersonSummary.icdoIbsPersonSummary.balance_forward;
                }
                if (lobjPerson.idecBalanceForward > 0.00M)
                {
                    lobjPerson.PastDueIdentifier = "1";
                }
                if (lobjPerson.idecHealthCredit > 0.00M)
                {
                    lobjPerson.HealthCreditIdentifier = "1";
                }
                if (lobjPerson.idecAdjustmentAmount > 0.00M)
                {
                    lobjPerson.istrAdjustmentIdentifier = "1";
                }
               
                lobjPerson.idecTotalDue = lobjPerson.idecTotalLifePremiumAmount + lobjPerson.idecTotalHealthPremiumAmt +
                                          lobjPerson.idecGroupVisionPremiumAmt + lobjPerson.idecGroupDentalPremiumAmt +
                                          lobjPerson.idecBalanceForward + lobjPerson.idecAdjustmentAmount + lobjPerson.idecLTCPremium
                                          + lobjPerson.idecTotalMedicarePremium; //Correspondance issues as discussed with Maik on call
                if (lblnPaymentModePersonalCheck)
                {
                    lobjPerson.IBSReportType = ibusIbsHeader.icdoIbsHeader.report_type_value;
                    CreateIBSStatement(lobjPerson);
                }
            }
        }

        private void CreateIBSStatement(busPerson lobjPerson)
        {
            try
            {
                //ArrayList larrlist = new ArrayList();
                //larrlist.Add(lobjPerson);
                idlgUpdateProcessLog("Creating Billing Statement for IBS Member PERSLinkID " +
                    Convert.ToString(lobjPerson.icdoPerson.person_id), "INFO", istrProcessName);
                Hashtable lhstDummyTable = new Hashtable();
                lhstDummyTable.Add("sfwCallingForm", "Batch");
                CreateCorrespondence("SFN-16789", lobjPerson, lhstDummyTable);
            }
            catch (Exception _exc)
            {
                idlgUpdateProcessLog("ERROR:" + _exc.Message, "INFO", istrProcessName);
            }
        }

        private void CreateCorrespondenceForLifeRateChange(busOrgPlan aobjOrgPlan, busPerson aobjPerson, string astrCorTamplateName)
        {
            //ArrayList larrlist = new ArrayList();
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            if (aobjOrgPlan.IsNotNull())
                CreateCorrespondence(astrCorTamplateName, aobjOrgPlan, lhstDummyTable);
            else
                CreateCorrespondence(astrCorTamplateName, aobjPerson, lhstDummyTable);
        }

        //Generate Job service rhic report
        public void GenerateJobServiceRHICReport()
        {
            idlgUpdateProcessLog("Generate Job Service RHIC Report", "INFO", istrProcessName);
            ibusIbsHeader.idtJSRHICReportTable = new DataTable();
            ibusIbsHeader.idtJSRHICReportTable = ibusIbsHeader.CreateNewDataTableForJSRHICReport();
            foreach (busIbsDetail lobjIbsDetail in ibusIbsHeader.icolIbsDetail)
            {
                if (lobjIbsDetail.icdoIbsDetail.js_rhic_amount > 0.0m)
                {
                    if (lobjIbsDetail.ibusPerson.IsNull())
                        lobjIbsDetail.LoadPerson();
                    ibusIbsHeader.AddToNewDataRow(lobjIbsDetail);
                }
            }
            if (ibusIbsHeader.idtJSRHICReportTable.Rows.Count > 0)
            {
                //prod pir 5762 : order by last name
                ibusIbsHeader.idtJSRHICReportTable = ibusIbsHeader.idtJSRHICReportTable.AsEnumerable()
                                                        .OrderBy(o => o.Field<string>("LastName"))
                                                        .ThenBy(o => o.Field<string>("FirstName")).AsDataTable();
                //create report for JobService Rhic
                CreateReport("rptJobServiceRHICReport.rpt", ibusIbsHeader.idtJSRHICReportTable);

                idlgUpdateProcessLog("Job Service RHIC Report generated succesfully", "INFO", istrProcessName);
            }
            else
            {
                idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
            }
        }


        public void InitiateWorkflowForDebitACHRequest()
        {
            //if (busWorkflowHelper.IsWorkflowRequestNotProcessedForProcessID(busConstant.Map_ACH_Pull_For_IBS_Insurance) && 
            //    busWorkflowHelper.IsActiveInstanceNotAvailableForFirstActivityofProcess(busConstant.ACH_Pull_For_IBS_Insurance_FirstActivityID))
            //{
                Dictionary<string, object> ldctParams = new Dictionary<string, object>();
                ldctParams["org_code"] = busConstant.DebitACHRequestWorkflowOrgCode;

                int aintOrgId = busGlobalFunctions.GetOrgIdFromOrgCode(busConstant.DebitACHRequestWorkflowOrgCode.ToString());

                busWorkflowHelper.InitiateBpmRequest(busConstant.Map_ACH_Pull_For_IBS_Insurance, 0, aintOrgId, 0, iobjPassInfo, BpmRequestSource.Batch, ldctParams);

            //}
        }

        /// <summary>
        /// property to contain person account dependent details
        /// </summary>
        public DataTable idtPersonAccountDependent { get; set; }
        /// <summary>
        /// method to load person account dependent
        /// </summary>
        private void LoadPersonAccountDepenedents()
        {
            idtPersonAccountDependent = new DataTable();
            idtPersonAccountDependent = busBase.Select("cdoPersonAccountDependent.LoadDependentForBilling",
                                        new object[1] { ibusIbsHeader.icdoIbsHeader.billing_month_and_year });
        }
    }
}