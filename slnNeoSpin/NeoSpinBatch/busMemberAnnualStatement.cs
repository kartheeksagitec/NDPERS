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
using System.Threading.Tasks;
using Sagitec.Common;

namespace NeoSpinBatch
{
    public class busMemberAnnualStatement : busNeoSpinBatch
    {
        public string istrReportName { get; set; }

        public string istrStepName { get; set; }

        public DataTable ldtbAllPlans { get; set; }

        public busMASBatchRequest ibusCurrentRequest { get; set; }

        public bool iblnTransaction { get; set; }

        // PIR 9481,9482 - Member's Retirement Section visibility
        public bool iblnIsMemberVested { get; set; }

        public void CreateMemberAnnualStatements()
        {
            istrProcessName = "Member Annual Statement Batch";
            istrStepName = "Member Annual Statement Batch";

            // Cache all Plans only once for all Requests
            ldtbAllPlans = iobjPassInfo.isrvDBCache.GetCacheData("sgt_plan", null);

            DataTable ldtbResult = busBase.Select<cdoMasBatchRequest>(
                                        new string[2] { "ACTION_STATUS_VALUE", "GROUP_TYPE_VALUE" },
                                        new object[2] { busConstant.StatusPending, busConstant.GroupTypeNonRetired }, null, null);
            foreach (DataRow ldtr in ldtbResult.Rows)
            {
                ibusCurrentRequest = new busMASBatchRequest { icdoMasBatchRequest = new cdoMasBatchRequest() };
                ibusCurrentRequest.icdoMasBatchRequest.LoadData(ldtr);
                ibusCurrentRequest.ldtbAllMASSelection = new DataTable();
                idlgUpdateProcessLog("Processing " + ibusCurrentRequest.icdoMasBatchRequest.batch_request_type_description + " Batch Request : Batch Request ID - "
                        + Convert.ToString(ibusCurrentRequest.icdoMasBatchRequest.mas_batch_request_id), "INFO", istrStepName);
                iblnTransaction = false;
                try
                {
                    /// This bulk update query inserts data into all the temporary tables as per the current batch request.
                    idlgUpdateProcessLog("STEP 1: Inserting Member's Data into Temporary Tables", "INFO", istrStepName);
                    #region Insert MAS selection for the Request
                    if (ibusCurrentRequest.icdoMasBatchRequest.bulk_insert_mas_data_flag != busConstant.Flag_Yes)
                    {
                        if (!iblnTransaction)
                        {
                            utlPassInfo.iobjPassInfo.BeginTransaction();
                            iblnTransaction = true;
                        }
                        DBFunction.DBNonQuery("cdoMASBatchRequest.BulkInsertMASReportData", new object[7] { 
                                    ibusCurrentRequest.icdoMasBatchRequest.mas_batch_request_id,
                                    iobjSystemManagement.icdoSystemManagement.batch_date,
                                    ibusCurrentRequest.icdoMasBatchRequest.batch_request_type_value,
                                    ibusCurrentRequest.icdoMasBatchRequest.person_id,
                                    ibusCurrentRequest.icdoMasBatchRequest.statement_effective_date,
                                    ibusCurrentRequest.icdoMasBatchRequest.created_date,
                                    iobjBatchSchedule.batch_schedule_id
                                    }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);


                        DBFunction.DBNonQuery("cdoMASBatchRequest.Insert_MAS_Person_Plan", new object[5] {
                                    ibusCurrentRequest.icdoMasBatchRequest.statement_effective_date,
                                    iobjSystemManagement.icdoSystemManagement.batch_date,
                                    ibusCurrentRequest.icdoMasBatchRequest.mas_batch_request_id,
                                    ibusCurrentRequest.icdoMasBatchRequest.batch_request_type_value,
                                    iobjBatchSchedule.batch_schedule_id
                                    }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                        DBFunction.DBNonQuery("cdoMASBatchRequest.Update_Vested_Employer_Percentage", new object[2] { 
                                    ibusCurrentRequest.icdoMasBatchRequest.statement_effective_date,
                                    ibusCurrentRequest.icdoMasBatchRequest.mas_batch_request_id
                                    }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                        ibusCurrentRequest.icdoMasBatchRequest.bulk_insert_mas_data_flag = busConstant.Flag_Yes;
                        ibusCurrentRequest.icdoMasBatchRequest.Update();

                        if (iblnTransaction)
                        {
                            utlPassInfo.iobjPassInfo.Commit();
                            iblnTransaction = false;
                        }
                    }

                    #endregion

                    /// Update the Data-Pulled flag in Batch Request once calculation is created.   
                    idlgUpdateProcessLog("STEP 2: Calculate Member's Benefit & Insert into Temporary Table", "INFO", istrStepName);
                    ibusCurrentRequest.LoadMASSelection(busConstant.Flag_No, busConstant.Flag_No);
                    if (ibusCurrentRequest.ldtbAllMASSelection.Rows.Count > 0)
                        CalculateMembersBenefit();

                    /// Get all the Data from the temporary tables to the local DataTable to create report.
                    idlgUpdateProcessLog("STEP 3: Fetch All Member's Data to Create Report", "INFO", istrStepName);
                    if (ibusCurrentRequest.icdoMasBatchRequest.mailing_generate_flag == busConstant.Flag_Yes)
                        ibusCurrentRequest.FetchAllReportData(ibusCurrentRequest.icdoMasBatchRequest.mas_batch_request_id, 0);

                    /// Create the Summary & Online report for the current batch request Persons with Benefit Calculations.
                    idlgUpdateProcessLog("STEP 4: Creating Summary & Online Report for all Members", "INFO", istrStepName);
                    ibusCurrentRequest.LoadMASSelection(busConstant.Flag_Yes, busConstant.Flag_No);
                    if (ibusCurrentRequest.ldtbAllMASSelection.Rows.Count > 0)
                    {
                        int lintTotalRecordCreated = ibusCurrentRequest.CreateMASReport();
                        idlgUpdateProcessLog("Total no. of Reports created : " + Convert.ToString(lintTotalRecordCreated), "INFO", istrStepName);
                    }

                    /// Update the Batch Request to Processed.
                    idlgUpdateProcessLog("STEP 5: Updating Batch Request status to processed", "INFO", istrStepName);
                    if (ibusCurrentRequest.ldtbAllMASSelection.Rows.Count == 0)
                        idlgUpdateProcessLog("No Member Selected for this Request", "INFO", istrStepName);
                    ibusCurrentRequest.icdoMasBatchRequest.action_status_value = busConstant.BatchRequestActionStatusProcessed;
                    ibusCurrentRequest.icdoMasBatchRequest.Update();
                }
                catch (Exception Ex)
                {
                    ExceptionManager.Publish(Ex);
                    if (iblnTransaction)
                    {
                        utlPassInfo.iobjPassInfo.Rollback();
                        iblnTransaction = false;
                    }
                    else
                    {
                        /// Change the Batch Request status to Failed
                        ibusCurrentRequest.icdoMasBatchRequest.action_status_value = busConstant.BatchRequestActionStatusFailed;
                        ibusCurrentRequest.icdoMasBatchRequest.Update();
                    }
                    idlgUpdateProcessLog("Message:" + Ex.Message, "ERR", istrProcessName);
                }
            }
            if (ldtbResult.Rows.Count == 0)
                idlgUpdateProcessLog("No Pending Batch Requests", "ERR", istrProcessName);
        }

        /// Filter the DataTable by FilterString, Returns the DataTable
        private static DataTable FilterTable<T>(DataTable adtSource, busConstant.DataType adtpDataType, string astrfilterFieldName, T atfilterFieldValue)
        {
            DataTable ldtbResuls = new DataTable();
            string lstrFilterString = string.Empty;
            if (adtpDataType == busConstant.DataType.String)
                lstrFilterString = astrfilterFieldName + "= '" + atfilterFieldValue + "'";
            else
                lstrFilterString = astrfilterFieldName + "=" + atfilterFieldValue;
            adtSource.DefaultView.RowFilter = lstrFilterString;
            ldtbResuls = adtSource.DefaultView.ToTable();
            return ldtbResuls;
        }

        private busPlan GetPlan(int aintPlanID)
        {
            busPlan lobjPlan = new busPlan { icdoPlan = new cdoPlan() };
            DataRow[] ldtrPlan = busGlobalFunctions.FilterTable(ldtbAllPlans, busConstant.DataType.Numeric, "plan_id", aintPlanID);
            if (ldtrPlan.Count() > 0)
                lobjPlan.icdoPlan.LoadData(ldtrPlan[0]);
            return lobjPlan;
        }

        private busPerson GetPerson(int aintPersonID)
        {
            busPerson lobjPerson = new busPerson { icdoPerson = new cdoPerson() };
            DataRow[] ldtrMember = busGlobalFunctions.FilterTable(ldtbAllMembers, busConstant.DataType.Numeric, "person_id", aintPersonID);
            if (ldtrMember.Count() > 0)
                lobjPerson.icdoPerson.LoadData(ldtrMember[0]);
            return lobjPerson;
        }

        private busPerson GetAnnuitant(int aintPersonID)
        {
            busPerson lobjAnnuitant = new busPerson { icdoPerson = new cdoPerson() };
            DataRow[] ldtrMember = busGlobalFunctions.FilterTable(ldtbAllAnnuitants, busConstant.DataType.Numeric, "MEMBER_ID", aintPersonID);
            if (ldtrMember.Count() > 0)
                lobjAnnuitant.icdoPerson.LoadData(ldtrMember[0]);
            return lobjAnnuitant;
        }

        private busPersonAccount GetPersonAccount(int aintPersonID, int aintPlanID)
        {
            busPersonAccount lobjPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            DataTable ldtbPersonAccount = ldtbAllMemberAccounts.AsEnumerable().Where(o =>
                            o.Field<int>("person_id") == aintPersonID &&
                            o.Field<int>("plan_id") == aintPlanID).AsDataTable();
            if (ldtbPersonAccount.Rows.Count > 0)
                lobjPersonAccount.icdoPersonAccount.LoadData(ldtbPersonAccount.Rows[0]);
            return lobjPersonAccount;
        }

        public DataTable ldtbAllMASPerson { get; set; }
        public DataTable ldtbAllMASPersonPlan { get; set; }
        public DataTable ldtbAllMembers { get; set; }
        public DataTable ldtbAllMemberAccounts { get; set; }
        public DataTable ldtbAllEmployments { get; set; }
        public DataTable ldtbAllAnnuitants { get; set; }
        public DataTable ldtbBenProvisionBenType { get; set; }
        public DataTable ldtbAllPersonEmployments { get; set; }
        public DataTable ldtbAllPersonEmploymentDetails { get; set; }
        public DataTable ldtbAllBeneficiary { get; set; }
        public DataTable ldtbBenOptionFactor { get; set; }

        public DataTable ldtbAgeEstimate { get; set; }
        public DateTime ldtLastIntertestPostDate { get; set; }
        public DataTable ldtbDeathBenOptionCodeValue { get; set; }
        public DataTable ldtbBenOptionCodeValue { get; set; }

        public DataTable ldtbBenefitProvisionExclusion { get; set; }

        public busDBCacheData lobjDBCacheData { get; set; }
        private void CalculateMembersBenefit()
        {
            ldtbAllMASPerson = busBase.Select("cdoMASBatchRequest.Select_MAS_Person", new object[1] { ibusCurrentRequest.icdoMasBatchRequest.mas_batch_request_id });
            ldtbAllMASPersonPlan = busBase.Select("cdoMASBatchRequest.Select_MAS_Person_Plan", new object[1] { ibusCurrentRequest.icdoMasBatchRequest.mas_batch_request_id });
            //// Load Person, Plan & Person Account info for all the MAS Selected Members.
            ldtbAllMembers = busBase.Select("cdoMASBatchRequest.LoadAllMembers", new object[1] { ibusCurrentRequest.icdoMasBatchRequest.mas_batch_request_id });
            ldtbAllMemberAccounts = busBase.Select("cdoMASBatchRequest.LoadAllMemberAccounts", new object[1] { ibusCurrentRequest.icdoMasBatchRequest.mas_batch_request_id });
            ldtbAllAnnuitants = busBase.Select("cdoMASBatchRequest.LoadAllJointAnnuitants", new object[1] { ibusCurrentRequest.icdoMasBatchRequest.mas_batch_request_id });
            ldtbAllEmployments = busBase.Select("cdoMASBatchRequest.LoadAllEmploymentByPersonAccount", new object[1] { ibusCurrentRequest.icdoMasBatchRequest.mas_batch_request_id });
            ldtbAllPersonEmployments = busBase.Select("cdoMASBatchRequest.LoadAllPersonEmployment", new object[1] { ibusCurrentRequest.icdoMasBatchRequest.mas_batch_request_id });
            ldtbAllPersonEmploymentDetails = busBase.Select("cdoMASBatchRequest.LoadAllPersonEmploymentDetail", new object[1] { ibusCurrentRequest.icdoMasBatchRequest.mas_batch_request_id });
            ldtbAllBeneficiary = busBase.Select("cdoMASBatchRequest.LoadAllBeneficiaries", new object[1] { ibusCurrentRequest.icdoMasBatchRequest.mas_batch_request_id });
            lobjDBCacheData = new busDBCacheData();
            lobjDBCacheData.idtbCachedBenefitProvisionEligibility = busGlobalFunctions.LoadBenefitProvisionEligibilityCacheData(iobjPassInfo);
            ldtbBenOptionFactor = busBase.Select<cdoBenefitOptionFactor>(
                           new string[0] { },
                           new object[0] { }, null, null);
            ldtbBenProvisionBenType = busBase.Select("cdoBenefitProvisionBenefitType.GetAllBenefitProvision", new object[0] { });
            ldtbAgeEstimate = busBase.Select<cdoCodeValue>(new string[1] { "CODE_ID" }, new object[1] { 1311 }, null, null);
            ldtLastIntertestPostDate = busInterestCalculationHelper.GetInterestBatchLastRunDate();
            ldtbDeathBenOptionCodeValue = iobjPassInfo.isrvDBCache.GetCodeValues(2406);
            ldtbBenOptionCodeValue = iobjPassInfo.isrvDBCache.GetCodeValues(1903);

            ldtbBenefitProvisionExclusion = busBase.Select<cdoBenefitProvisionExclusion>(
                              new string[0] { },
                              new object[0] { }, null, null);
            utlPassInfo lutlOrigPassInfo = utlPassInfo.iobjPassInfo;
            Parallel.ForEach(ibusCurrentRequest.ldtbAllMASSelection.AsEnumerable(), datarow => { CalculateEachMemberBenefit(datarow); });
            utlPassInfo.iobjPassInfo = lutlOrigPassInfo;
        }
        public static void InitializeUtlPassInfo()
        {
            utlPassInfo.iobjPassInfo = new utlPassInfo();
            utlPassInfo.iobjPassInfo.idictParams = new Dictionary<string, object>();
            utlPassInfo.iobjPassInfo.idictParams[utlConstants.istrConstUserID] = busConstant.PERSLinkBatchUser101;
            utlPassInfo.iobjPassInfo.iconFramework = DBFunction.GetDBConnection();
        }
        public static void FreePassInfo(utlPassInfo aobjPassInfo)
        {
            if ((aobjPassInfo.iconFramework != null) &&
                (aobjPassInfo.iconFramework.State == ConnectionState.Open))
            {
                aobjPassInfo.iconFramework.Close();
                aobjPassInfo.iconFramework.Dispose();
            }
            aobjPassInfo.isrvDBCache = null;
            aobjPassInfo.isrvMetaDataCache = null;
            aobjPassInfo = null;
        }

        public static void FreePassInfo()
        {
            FreePassInfo(utlPassInfo.iobjPassInfo);
        }
        private void CalculateEachMemberBenefit(DataRow adtrMemberSelection)
        {
            try
            {
                InitializeUtlPassInfo();
                if (!utlPassInfo.iobjPassInfo.iblnInTransaction)
                    utlPassInfo.iobjPassInfo.BeginTransaction();
                busMASSelection lobjMASSelection = new busMASSelection { icdoMasSelection = new cdoMasSelection() };
                lobjMASSelection.icdoMasSelection.LoadData(adtrMemberSelection);

                DataRow[] ldtbPerson = busGlobalFunctions.FilterTable(ldtbAllMASPerson,
                                            busConstant.DataType.Numeric, "MAS_SELECTION_ID", lobjMASSelection.icdoMasSelection.mas_selection_id);
                if (ldtbPerson?.Count() > 0)
                {
                    busMASPerson lobjMASPerson = new busMASPerson { icdoMasPerson = new cdoMasPerson() };
                    lobjMASPerson.icdoMasPerson.LoadData(ldtbPerson[0]);

                    DataRow[] ldtbDBPersonPlan = busGlobalFunctions.FilterTable(ldtbAllMASPersonPlan,
                                            busConstant.DataType.Numeric, "MAS_PERSON_ID", lobjMASPerson.icdoMasPerson.mas_person_id);
                    if (ldtbDBPersonPlan?.Count() > 0)
                    {
                        foreach (DataRow ldtrPersonAccount in ldtbDBPersonPlan)
                        {
                            busMASPersonPlan lobjMASPersonPlan = new busMASPersonPlan { icdoMasPersonPlan = new cdoMasPersonPlan() };
                            lobjMASPersonPlan.icdoMasPersonPlan.LoadData(ldtrPersonAccount);

                        /// Load objects from the cached DataTable instead of hitting the DB.
                        busPerson lobjPerson = new busPerson { icdoPerson = new cdoPerson() };
                        lobjPerson = GetPerson(lobjMASSelection.icdoMasSelection.person_id);

                        // Load Member's Employments
                        lobjPerson.icolPersonEmployment = new Collection<busPersonEmployment>();
                        DataRow[] ldtrEmploymentsByPerson = busGlobalFunctions.FilterTable(ldtbAllPersonEmployments, busConstant.DataType.Numeric, "person_id",
                            lobjPerson.icdoPerson.person_id);
                        foreach (DataRow ldrRow in ldtrEmploymentsByPerson)
                        {
                            busPersonEmployment lbusPerEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
                            lbusPerEmployment.icdoPersonEmployment.LoadData(ldrRow);

                            DataRow[] ldtrEmploymentDetailByEmployment = busGlobalFunctions.FilterTable(ldtbAllPersonEmploymentDetails, busConstant.DataType.Numeric, "person_employment_id",
                                                                                                            lbusPerEmployment.icdoPersonEmployment.person_employment_id);
                            lbusPerEmployment.icolPersonEmploymentDetail = new Collection<busPersonEmploymentDetail>();
                            foreach (DataRow ldrSubRow in ldtrEmploymentDetailByEmployment)
                            {
                                busPersonEmploymentDetail lbusPerEmpDetail = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
                                lbusPerEmpDetail.icdoPersonEmploymentDetail.LoadData(ldrSubRow);
                                lbusPerEmployment.icolPersonEmploymentDetail.Add(lbusPerEmpDetail);
                            }
                            lobjPerson.icolPersonEmployment.Add(lbusPerEmployment);
                        }

                        // Load Member's Beneficiaries
                        lobjPerson.iclbPersonBeneficiary = new Collection<busPersonBeneficiary>();
                        DataRow[] ldtrBeneficiaryByPerson = busGlobalFunctions.FilterTable(ldtbAllBeneficiary, busConstant.DataType.Numeric, "person_id",
                            lobjPerson.icdoPerson.person_id);
                        foreach (DataRow ldrRow in ldtrBeneficiaryByPerson)
                        {
                            busPersonBeneficiary lbusPersonBeneficiary = new busPersonBeneficiary { icdoPersonBeneficiary = new cdoPersonBeneficiary() };
                            lbusPersonBeneficiary.icdoPersonBeneficiary.LoadData(ldrRow);

                            lbusPersonBeneficiary.ibusPersonAccountBeneficiary = new busPersonAccountBeneficiary { icdoPersonAccountBeneficiary = new cdoPersonAccountBeneficiary() };
                            lbusPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.LoadData(ldrRow);
                            lobjPerson.iclbPersonBeneficiary.Add(lbusPersonBeneficiary);
                        }

                        // Load All Member Accounts
                        lobjPerson.icolPersonAccount = new Collection<busPersonAccount>();
                        DataRow[] ldtrPersonAccountsByPerson = busGlobalFunctions.FilterTable(ldtbAllMemberAccounts, busConstant.DataType.Numeric, "person_id",
                           lobjPerson.icdoPerson.person_id);
                        foreach (DataRow ldrRow in ldtrPersonAccountsByPerson)
                        {
                            busPersonAccount lbusPersonAccount = new busPersonAccount
                            {
                                icdoPersonAccount = new cdoPersonAccount(),
                                ibusPlan = new busPlan { icdoPlan = new cdoPlan() }
                            };
                            lbusPersonAccount.icdoPersonAccount.LoadData(ldrRow);
                            lbusPersonAccount.ibusPlan.icdoPlan.LoadData(ldrRow);
                            lobjPerson.icolPersonAccount.Add(lbusPersonAccount);
                        }

                        busPlan lobjCurrentPlan = new busPlan { icdoPlan = new cdoPlan() };
                        lobjCurrentPlan = GetPlan(lobjMASPersonPlan.icdoMasPersonPlan.plan_id);
                        busPersonAccount lobjCurrentAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                        lobjCurrentAccount = GetPersonAccount(lobjPerson.icdoPerson.person_id, lobjCurrentPlan.icdoPlan.plan_id);
                        lobjCurrentAccount.ibusPerson = lobjPerson;
                        lobjCurrentAccount.ibusPlan = lobjCurrentPlan;

                        //Load the Employment Detail and Employment
                        lobjCurrentAccount.iclbEmploymentDetail = new Collection<busPersonEmploymentDetail>();
                        DataRow[] ldtrEmploymentsByPA = busGlobalFunctions.FilterTable(ldtbAllEmployments, busConstant.DataType.Numeric, "person_account_id",
                            lobjCurrentAccount.icdoPersonAccount.person_account_id);
                        foreach (DataRow ldrRow in ldtrEmploymentsByPA)
                        {
                            busPersonEmploymentDetail lbusPAEmpDetail = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
                            lbusPAEmpDetail.icdoPersonEmploymentDetail.LoadData(ldrRow);

                            lbusPAEmpDetail.ibusPersonEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
                            lbusPAEmpDetail.ibusPersonEmployment.icdoPersonEmployment.LoadData(ldrRow);
                            lobjCurrentAccount.iclbEmploymentDetail.Add(lbusPAEmpDetail);
                        }

                        busPerson lobjAnnuitant = new busPerson { icdoPerson = new cdoPerson() };
                        lobjAnnuitant = GetAnnuitant(lobjMASSelection.icdoMasSelection.person_id);
                        /// Get Eligible Age Provision 
                        Collection<cdoBenefitProvisionEligibility> lclbBenefitProvisionNormalEligibility = new Collection<cdoBenefitProvisionEligibility>();
                        lclbBenefitProvisionNormalEligibility = busPersonBase.LoadEligibilityForPlan(lobjCurrentPlan.icdoPlan.plan_id,
                                                        lobjCurrentPlan.icdoPlan.benefit_provision_id,
                                                        lobjCurrentPlan.icdoPlan.benefit_type_value,
                                                        busConstant.BenefitProvisionEligibilityNormal, iobjPassInfo, lobjCurrentAccount?.icdoPersonAccount?.start_date, lobjDBCacheData);
                        
                        busRetirementBenefitCalculation lobjRetirementCalculation = new busRetirementBenefitCalculation
                        {
                            icdoBenefitCalculation = new cdoBenefitCalculation(),
                            ibusMember = new busPerson { icdoPerson = new cdoPerson() },
                            ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount(), ibusPlan = new busPlan { icdoPlan = new cdoPlan() } },
                            ibusPlan = new busPlan { icdoPlan = new cdoPlan() },
                            ibusJointAnnuitant = new busPerson { icdoPerson = new cdoPerson() }
                        };
                        //prod pir 7330 : setting the filtering date for contribution to be statement effective date
                        lobjCurrentAccount.idtMASStatementEffectiveDate = ibusCurrentRequest.icdoMasBatchRequest.statement_effective_date;
                        lobjRetirementCalculation.ibusMember = lobjPerson;
                        lobjRetirementCalculation.ibusPersonAccount = lobjCurrentAccount;
                        lobjRetirementCalculation.ibusPlan = lobjCurrentPlan;
                        lobjRetirementCalculation.ibusPersonAccount.ibusPlan = lobjCurrentPlan;
                        lobjRetirementCalculation.ibusJointAnnuitant = lobjAnnuitant;
                        lobjRetirementCalculation.idtbBenOptionFactor = ldtbBenOptionFactor;
                        lobjRetirementCalculation.idtLastIntertestPostDate = ldtLastIntertestPostDate;
                        lobjRetirementCalculation.idtbBenefitProvisionExclusion = ldtbBenefitProvisionExclusion;
                        lobjRetirementCalculation.iblnUseDataTableForBenOptionFactor = true;
                        lobjRetirementCalculation.icdoBenefitCalculation.plan_id = lobjRetirementCalculation.ibusPlan.icdoPlan.plan_id;
                        lobjRetirementCalculation.icdoBenefitCalculation.person_id = lobjRetirementCalculation.ibusMember.icdoPerson.person_id;
                        lobjRetirementCalculation.icdoBenefitCalculation.rhic_option_value = busConstant.RHICOptionStandard;
                        lobjRetirementCalculation.icdoBenefitCalculation.created_date = ibusCurrentRequest.icdoMasBatchRequest.statement_effective_date;
                        lobjRetirementCalculation.icdoBenefitCalculation.termination_date = ibusCurrentRequest.icdoMasBatchRequest.statement_effective_date;
                        lobjRetirementCalculation.icdoBenefitCalculation.retirement_date = ibusCurrentRequest.icdoMasBatchRequest.statement_effective_date.GetFirstDayofNextMonth();
                        lobjRetirementCalculation.iblnConsoldatedVSCLoaded = false;
                        lobjRetirementCalculation.iblnConsolidatedPSCLoaded = false;
                        lobjRetirementCalculation.iblnBenefitMultiplierLoaded = false;
                        lobjRetirementCalculation.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypeRetirement;
                        if (lobjRetirementCalculation.iclbBenefitCalculationPersonAccount.IsNull())
                            lobjRetirementCalculation.LoadPersonPlanAccounts();
                        DateTime ldteActualEmployeeTerminationDate = new DateTime();
                        lobjRetirementCalculation.GetOrgIdAsLatestEmploymentOrgId(
                                            lobjCurrentAccount,
                                            lobjRetirementCalculation.icdoBenefitCalculation.benefit_account_type_value,
                                            ref ldteActualEmployeeTerminationDate);                        
                        lobjRetirementCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeEstimate;
                        if (ldteActualEmployeeTerminationDate != DateTime.MinValue)
                        {
                            lobjRetirementCalculation.icdoBenefitCalculation.termination_date = ldteActualEmployeeTerminationDate;
                            if (!(lobjRetirementCalculation.IsMemberDual()))
                                lobjRetirementCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeFinal;
                        }

                        // Get the Eligible Esimates for the current plan.
                        DataTable ldtbEstimatePerPlan = ldtbAgeEstimate.AsEnumerable().Where(o =>
                                            o.Field<string>("DATA1") == lobjRetirementCalculation.icdoBenefitCalculation.plan_id.ToString()).AsDataTable();

                        lobjRetirementCalculation.CalculateAgeOfMember();//PIR 11734
                        lobjRetirementCalculation.LoadBenefitCalculationPayeeForNewMode();
                        lobjRetirementCalculation.LoadBenefitProvisionBenefitType(ldtbBenProvisionBenType);
                        //prod pir 7330 :load total vsc as of statement effective date
                        lobjRetirementCalculation.ibusPersonAccount.LoadTotalVSCForMAS(ibusCurrentRequest.icdoMasBatchRequest.statement_effective_date);
                        if (lobjRetirementCalculation.icdoBenefitCalculation.plan_id == busConstant.PlanIdDC ||
                                 lobjRetirementCalculation.icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2020 || //PIR 20232
                                 lobjRetirementCalculation.icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2025) //PIR 25920
                        {
                            // PIR 9481,9482 - Member's Retirement Section visibility
                            iblnIsMemberVested = true;

                            // DC RHIC amount is only needed in report.
                            if (!lobjRetirementCalculation.iblnConsolidatedPSCLoaded)
                                lobjRetirementCalculation.CalculateConsolidatedPSC();
                            lobjRetirementCalculation.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypeRetirement;
                            decimal ldecRHICAmount = Math.Round(lobjRetirementCalculation.icdoBenefitCalculation.consolidated_psc_in_years, 4, MidpointRounding.AwayFromZero) *
                                                    lobjRetirementCalculation.ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.rhic_service_factor;
                            lobjRetirementCalculation.icdoBenefitCalculation.unreduced_rhic_amount = Math.Round(ldecRHICAmount, 2, MidpointRounding.AwayFromZero);
                            InsertMASPersonCalculation(
                                             lobjMASPerson.icdoMasPerson.mas_person_id,
                                             lobjRetirementCalculation.ibusPlan.icdoPlan.plan_id,
                                             lobjRetirementCalculation.ibusPlan.icdoPlan.plan_name,
                                             busConstant.ApplicationBenefitTypeRetirement,
                                             "Age " + Convert.ToString(lobjRetirementCalculation.idecMemberAgeBasedOnRetirementDate),
                                             lobjRetirementCalculation.idecMASBenefitAmount,
                                             lobjRetirementCalculation.idecReducedRHICAmount,
                                             busConstant.Flag_No, lobjRetirementCalculation.icdoBenefitCalculation.member_account_balance,
                                             lobjRetirementCalculation.icdoBenefitCalculation.qdro_amount,
                                             lobjRetirementCalculation.icdoBenefitCalculation.normal_retirement_date,
                                             lobjRetirementCalculation.icdoBenefitCalculation.calculation_final_average_salary, busConstant.Flag_No, 0M, 
                                             lobjRetirementCalculation.icdoBenefitCalculation.retirement_date,
                                             lobjRetirementCalculation.icdoBenefitCalculation.termination_date,
                                             lobjRetirementCalculation.icdoBenefitCalculation.calculation_type_value, 0M,
                                             lobjRetirementCalculation.icdoBenefitCalculation.credited_psc,
                                             lobjRetirementCalculation.icdoBenefitCalculation.credited_vsc);

                            // UAT PIR ID 1978
                            foreach (DataRow ldtbRows in ldtbEstimatePerPlan.Rows)
                            {
                                cdoCodeValue lcdoCV = new cdoCodeValue();
                                lcdoCV.LoadData(ldtbRows);
                                if (lcdoCV.data3 == busConstant.Flag_No)
                                {
                                    DateTime ldteMemberBday = lobjRetirementCalculation.ibusMember.icdoPerson.date_of_birth.AddYears(Convert.ToInt32(lcdoCV.data2));
                                    if (ldteMemberBday >= ibusCurrentRequest.icdoMasBatchRequest.statement_effective_date)
                                    {
                                        lobjRetirementCalculation.icdoBenefitCalculation.retirement_date = ldteMemberBday.GetLastDayofMonth();
                                        lobjRetirementCalculation.icdoBenefitCalculation.termination_date = ldteMemberBday.GetFirstDayofNextMonth();
                                        lobjRetirementCalculation.idecMemberAgeBasedOnRetirementDate = Convert.ToDecimal(lcdoCV.data2); //PROD PIR ID 7632
                                        lobjRetirementCalculation.CalculateConsolidatedPSC();
                                        decimal ldecRHICAmt = Math.Round(lobjRetirementCalculation.icdoBenefitCalculation.consolidated_psc_in_years, 4, MidpointRounding.AwayFromZero) *
                                                                lobjRetirementCalculation.ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.rhic_service_factor;
                                        lobjRetirementCalculation.icdoBenefitCalculation.unreduced_rhic_amount = Math.Round(ldecRHICAmt, 2, MidpointRounding.AwayFromZero);
                                        InsertMASPersonCalculation(
                                                                lobjMASPerson.icdoMasPerson.mas_person_id,
                                                                lobjRetirementCalculation.ibusPlan.icdoPlan.plan_id,
                                                                lobjRetirementCalculation.ibusPlan.icdoPlan.plan_name,
                                                                busConstant.ApplicationBenefitTypeRetirement,
                                                                "Age " + Convert.ToString(lobjRetirementCalculation.idecMemberAgeBasedOnRetirementDate),
                                                                lobjRetirementCalculation.idecMASBenefitAmount,
                                                                lobjRetirementCalculation.idecReducedRHICAmount,
                                                                busConstant.Flag_No, lobjRetirementCalculation.icdoBenefitCalculation.member_account_balance,
                                                                lobjRetirementCalculation.icdoBenefitCalculation.qdro_amount,
                                                                lobjRetirementCalculation.icdoBenefitCalculation.normal_retirement_date,
                                                                lobjRetirementCalculation.icdoBenefitCalculation.calculation_final_average_salary, busConstant.Flag_No, 0M,
                                                                lobjRetirementCalculation.icdoBenefitCalculation.retirement_date,
                                                                lobjRetirementCalculation.icdoBenefitCalculation.termination_date,
                                                                lobjRetirementCalculation.icdoBenefitCalculation.calculation_type_value, 0M,
                                                                lobjRetirementCalculation.icdoBenefitCalculation.credited_psc,
                                                                lobjRetirementCalculation.icdoBenefitCalculation.credited_vsc);
                                    }
                                }
                                //pir 7692
                                // PIR 9674
                                else
                                {
                                    lobjRetirementCalculation.icdoBenefitCalculation.termination_date = ibusCurrentRequest.icdoMasBatchRequest.statement_effective_date;
                                    lobjRetirementCalculation.icdoBenefitCalculation.retirement_date = ibusCurrentRequest.icdoMasBatchRequest.statement_effective_date.GetFirstDayofNextMonth();
                                    lobjRetirementCalculation.iblnConsoldatedVSCLoaded = false;
                                    lobjRetirementCalculation.iblnConsolidatedPSCLoaded = false;
                                    lobjRetirementCalculation.iblnBenefitMultiplierLoaded = false;
                                    lobjRetirementCalculation.SetRuleIndicator(); // PROD PIR ID 7379
                                    DateTime ldteNRDByAge = new DateTime();
                                    DateTime ldteNRDByRule = new DateTime();
                                    DateTime ldteTempNRD = busPersonBase.GetNormalRetirementDateBasedOnNormalEligibility(
                                            lobjRetirementCalculation.ibusPlan.icdoPlan.plan_id,
                                            lobjRetirementCalculation.ibusPlan.icdoPlan.plan_code,
                                            lobjRetirementCalculation.ibusPlan.icdoPlan.benefit_provision_id,
                                            lobjRetirementCalculation.ibusPlan.icdoPlan.benefit_type_value,
                                            lobjRetirementCalculation.ibusMember.icdoPerson.date_of_birth,
                                            lobjRetirementCalculation.ibusPersonAccount.icdoPersonAccount.Total_VSC, 2, iobjPassInfo,
                                            lobjRetirementCalculation.icdoBenefitCalculation.termination_date, lobjCurrentAccount.icdoPersonAccount.person_account_id,
                                            (lobjRetirementCalculation.icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate), 0,
                                            ref ldteNRDByAge, ref ldteNRDByRule, true, lobjRetirementCalculation.icdoBenefitCalculation.retirement_date, //PIR 14646
                                            lobjDBCacheData, lobjRetirementCalculation.ibusPersonAccount);
                                    if ((ldteNRDByRule < ldteNRDByAge) &&
                                        (ldteTempNRD > ibusCurrentRequest.icdoMasBatchRequest.statement_effective_date.GetFirstDayofNextMonth()) &&
                                        lobjRetirementCalculation.icdoBenefitCalculation.rule_indicator_value.IsNullOrEmpty()) // PROD PIR ID 7379
                                    {
                                        if (ldteNRDByRule != DateTime.MinValue)
                                            lobjRetirementCalculation.icdoBenefitCalculation.retirement_date = ldteNRDByRule;
                                        lobjRetirementCalculation.iblnConsoldatedVSCLoaded = false;
                                        lobjRetirementCalculation.iblnConsolidatedPSCLoaded = false;
                                        lobjRetirementCalculation.iblnBenefitMultiplierLoaded = false;
                                        if (ldteActualEmployeeTerminationDate != DateTime.MinValue)
                                            lobjRetirementCalculation.icdoBenefitCalculation.termination_date = ldteActualEmployeeTerminationDate;
                                        else if (lobjRetirementCalculation.icdoBenefitCalculation.retirement_date != DateTime.MinValue)
                                            lobjRetirementCalculation.icdoBenefitCalculation.termination_date = lobjRetirementCalculation.icdoBenefitCalculation.retirement_date.AddDays(-1);
                                        lobjRetirementCalculation.CalculateFAS();
                                        lobjRetirementCalculation.LoadBenefitProvisionBenefitType(ldtbBenProvisionBenType);
                                        lobjRetirementCalculation.CalculateConsolidatedPSC();
                                        lobjRetirementCalculation.idecMemberAgeBasedOnRetirementDate = Convert.ToDecimal(lcdoCV.data2) - (Convert.ToInt32(lobjRetirementCalculation.icdoBenefitCalculation.credited_psc/12));
                                        
                                        decimal ldecRHICAmt = Math.Round(lobjRetirementCalculation.icdoBenefitCalculation.consolidated_psc_in_years, 4, MidpointRounding.AwayFromZero) *
                                                                lobjRetirementCalculation.ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.rhic_service_factor;
                                        lobjRetirementCalculation.icdoBenefitCalculation.unreduced_rhic_amount = Math.Round(ldecRHICAmt, 2, MidpointRounding.AwayFromZero);
                                        InsertMASPersonCalculation(
                                                                lobjMASPerson.icdoMasPerson.mas_person_id,
                                                                lobjRetirementCalculation.ibusPlan.icdoPlan.plan_id,
                                                                lobjRetirementCalculation.ibusPlan.icdoPlan.plan_name,
                                                                busConstant.ApplicationBenefitTypeRetirement,
                                                                "Age " + Convert.ToString(lobjRetirementCalculation.idecMemberAgeBasedOnRetirementDate) + " (" +
                                                                "Rule of " + Convert.ToString(lcdoCV.data2) + ")",
                                                                lobjRetirementCalculation.idecMASBenefitAmount,
                                                                lobjRetirementCalculation.idecReducedRHICAmount,
                                                                busConstant.Flag_No, lobjRetirementCalculation.icdoBenefitCalculation.member_account_balance,
                                                                lobjRetirementCalculation.icdoBenefitCalculation.qdro_amount,
                                                                lobjRetirementCalculation.icdoBenefitCalculation.normal_retirement_date,
                                                                lobjRetirementCalculation.icdoBenefitCalculation.calculation_final_average_salary, busConstant.Flag_No, 0M,
                                                                lobjRetirementCalculation.icdoBenefitCalculation.retirement_date,
                                                                lobjRetirementCalculation.icdoBenefitCalculation.termination_date,
                                                                lobjRetirementCalculation.icdoBenefitCalculation.calculation_type_value, 0M,
                                                                lobjRetirementCalculation.icdoBenefitCalculation.credited_psc,
                                                                lobjRetirementCalculation.icdoBenefitCalculation.credited_vsc);
                                    }
                                }
                            }
                        }
                        else
                        {
                            /// Calculates and Inserts for the Retirement
                            lobjRetirementCalculation.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypeRetirement;
                            if (!lobjRetirementCalculation.iblnConsoldatedVSCLoaded)
                                lobjRetirementCalculation.CalculateConsolidatedVSC();
                            bool ablnDefaultRetirementCalculation = false;
                            DateTime ldteTempNRDDate = new DateTime();
                            if (lobjRetirementCalculation.CheckPersonEligible(lobjDBCacheData))
                            {
                                ldteTempNRDDate = lobjRetirementCalculation.icdoBenefitCalculation.termination_date;
                                ablnDefaultRetirementCalculation = true;
                                SetFlagForHPIndexing(lobjRetirementCalculation);
                                lobjRetirementCalculation.CalculateFAS();
                                lobjRetirementCalculation.CalculateMASRetirementBenefit(lobjDBCacheData);
                                InsertMASPersonCalculation(
                                                lobjMASPerson.icdoMasPerson.mas_person_id,
                                                lobjRetirementCalculation.ibusPlan.icdoPlan.plan_id,
                                                lobjRetirementCalculation.ibusPlan.icdoPlan.plan_name,
                                                busConstant.ApplicationBenefitTypeRetirement,
                                                "Age " + Convert.ToString(lobjRetirementCalculation.idecMemberAgeBasedOnRetirementDate),
                                                // UAT PIR: 1700 DNRO is not applicable for MAS, so only Unreduced benefit amount is only paid to Member
                                                // UAT PIR: 2321 Reduce QDRO amount if exists.
                                                ((!string.IsNullOrEmpty(lobjRetirementCalculation.icdoBenefitCalculation.benefit_account_sub_type_value)) && lobjRetirementCalculation.icdoBenefitCalculation.benefit_account_sub_type_value.Equals(busConstant.ApplicationBenefitSubTypeDNRO)) ?
                                                (lobjRetirementCalculation.icdoBenefitCalculation.unreduced_benefit_amount - lobjRetirementCalculation.icdoBenefitCalculation.qdro_amount) :
                                                lobjRetirementCalculation.idecMASBenefitAmount,
                                                lobjRetirementCalculation.idecReducedRHICAmount,
                                                busConstant.Flag_No, lobjRetirementCalculation.icdoBenefitCalculation.member_account_balance,
                                                lobjRetirementCalculation.icdoBenefitCalculation.qdro_amount,
                                                lobjRetirementCalculation.icdoBenefitCalculation.normal_retirement_date,
                                                lobjRetirementCalculation.icdoBenefitCalculation.calculation_final_average_salary, busConstant.Flag_Yes, 0M,
                                                lobjRetirementCalculation.icdoBenefitCalculation.retirement_date,
                                                lobjRetirementCalculation.icdoBenefitCalculation.termination_date,
                                                lobjRetirementCalculation.icdoBenefitCalculation.calculation_type_value, 0M,
                                                lobjRetirementCalculation.icdoBenefitCalculation.credited_psc,
                                                lobjRetirementCalculation.icdoBenefitCalculation.credited_vsc,
                                                busConstant.Flag_Yes);
                            }

                            // PIR 9652
                            if (lobjRetirementCalculation.iblnIsMemberVested)
                            {
                                iblnIsMemberVested = true;
                            }

                            foreach (DataRow ldtbRows in ldtbEstimatePerPlan.Rows)
                            {
                                cdoCodeValue lcdoCV = new cdoCodeValue();
                                lcdoCV.LoadData(ldtbRows);
                                if (lcdoCV.data3 == busConstant.Flag_No)
                                {
                                    // Age
                                    DateTime ldteMemberBday = lobjRetirementCalculation.ibusMember.icdoPerson.date_of_birth.AddYears(Convert.ToInt32(lcdoCV.data2));
                                    if (ldteMemberBday.GetFirstDayofNextMonth() > ibusCurrentRequest.icdoMasBatchRequest.statement_effective_date.GetFirstDayofNextMonth())
                                    {
                                        lobjRetirementCalculation.icdoBenefitCalculation.retirement_date = ldteMemberBday.GetFirstDayofNextMonth();
                                        lobjRetirementCalculation.iblnConsoldatedVSCLoaded = false;
                                        lobjRetirementCalculation.iblnConsolidatedPSCLoaded = false;
                                        lobjRetirementCalculation.iblnBenefitMultiplierLoaded = false;
                                        if (ldteActualEmployeeTerminationDate != DateTime.MinValue)
                                            lobjRetirementCalculation.icdoBenefitCalculation.termination_date = ldteActualEmployeeTerminationDate;
                                        else
                                            lobjRetirementCalculation.icdoBenefitCalculation.termination_date = ldteMemberBday.GetLastDayofMonth();
                                        SetFlagForHPIndexing(lobjRetirementCalculation);
                                        lobjRetirementCalculation.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypeRetirement;
                                        lobjRetirementCalculation.LoadBenefitProvisionBenefitType(ldtbBenProvisionBenType);
                                        lobjRetirementCalculation.CalculateFAS();
                                        lobjRetirementCalculation.CalculateMASRetirementBenefit(lobjDBCacheData);

                                        string lstrRuleorAgeIndicator = busConstant.Flag_No;
                                        if (((lobjCurrentAccount.icdoPersonAccount.plan_id == busConstant.PlanIdLEWithoutPS) ||
                                            (lobjCurrentAccount.icdoPersonAccount.plan_id == busConstant.PlanIdLE) ||
                                            (lobjCurrentAccount.icdoPersonAccount.plan_id == busConstant.PlanIdNG) ||
                                            (lobjCurrentAccount.icdoPersonAccount.plan_id == busConstant.PlanIdHP) ||
                                            (lobjCurrentAccount.icdoPersonAccount.plan_id == busConstant.PlanIdBCILawEnf) || // pir7943
                                            (lobjCurrentAccount.icdoPersonAccount.plan_id == busConstant.PlanIdStatePublicSafety)) && //PIR 25729
                                            (Convert.ToInt32(lcdoCV.data2) == 55))
                                            lstrRuleorAgeIndicator = busConstant.Flag_Yes;
                                        else if (((lobjCurrentAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMain) ||
                                            (lobjCurrentAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMain2020) || // PIR 20232
                                                (lobjCurrentAccount.icdoPersonAccount.plan_id == busConstant.PlanIdJudges)) &&
                                                (Convert.ToInt32(lcdoCV.data2) == 65))
                                            lstrRuleorAgeIndicator = busConstant.Flag_Yes;
											//PIR-17312
                                        if (lobjRetirementCalculation.CheckPersonEligible(lobjDBCacheData))
                                        {
                                            InsertMASPersonCalculation(
                                                    lobjMASPerson.icdoMasPerson.mas_person_id,
                                                    lobjRetirementCalculation.ibusPlan.icdoPlan.plan_id,
                                                    lobjRetirementCalculation.ibusPlan.icdoPlan.plan_name,
                                                    busConstant.ApplicationBenefitTypeRetirement,
                                                    "Age " + Convert.ToString(lcdoCV.data2),
                                                // UAT PIR: 1700 DNRO is not applicable for MAS, so only Unreduced benefit amount is only paid to Member
                                                // UAT PIR: 2321 Reduce QDRO amount if exists.
                                                    ((!string.IsNullOrEmpty(lobjRetirementCalculation.icdoBenefitCalculation.benefit_account_sub_type_value)) && lobjRetirementCalculation.icdoBenefitCalculation.benefit_account_sub_type_value.Equals(busConstant.ApplicationBenefitSubTypeDNRO)) ?
                                                    (lobjRetirementCalculation.icdoBenefitCalculation.unreduced_benefit_amount - lobjRetirementCalculation.icdoBenefitCalculation.qdro_amount) :
                                                    lobjRetirementCalculation.idecMASBenefitAmount,
                                                    lobjRetirementCalculation.idecReducedRHICAmount,
                                                    busConstant.Flag_No, lobjRetirementCalculation.icdoBenefitCalculation.minimum_guarentee_amount,
                                                    lobjRetirementCalculation.icdoBenefitCalculation.qdro_amount,
                                                    lobjRetirementCalculation.icdoBenefitCalculation.normal_retirement_date,
                                                    lobjRetirementCalculation.icdoBenefitCalculation.calculation_final_average_salary,
                                                    ablnDefaultRetirementCalculation ? busConstant.Flag_No : busConstant.Flag_Yes, 0M,
                                                    lobjRetirementCalculation.icdoBenefitCalculation.retirement_date,
                                                    lobjRetirementCalculation.icdoBenefitCalculation.termination_date,
                                                    lobjRetirementCalculation.icdoBenefitCalculation.calculation_type_value, 0M,
                                                    lobjRetirementCalculation.icdoBenefitCalculation.credited_psc,
                                                    lobjRetirementCalculation.icdoBenefitCalculation.credited_vsc,
                                                    lstrRuleorAgeIndicator);
                                        }
                                        if (!ablnDefaultRetirementCalculation)
                                        {
                                            ablnDefaultRetirementCalculation = true;
                                            ldteTempNRDDate = lobjRetirementCalculation.icdoBenefitCalculation.termination_date;
                                        }
                                    }
                                }
                                else
                                {
                                    // Rule                                    
                                    //Systest PIR: 1227. The Service Credit sent as the last parameter is the selected service purchase in the 
                                    //Estimate screen. For all others it should be zero.
                                    //Dont Modify the Code.
                                    lobjRetirementCalculation.icdoBenefitCalculation.termination_date = ibusCurrentRequest.icdoMasBatchRequest.statement_effective_date;
                                    lobjRetirementCalculation.icdoBenefitCalculation.retirement_date = ibusCurrentRequest.icdoMasBatchRequest.statement_effective_date.GetFirstDayofNextMonth();
                                    lobjRetirementCalculation.iblnConsoldatedVSCLoaded = false;
                                    lobjRetirementCalculation.iblnConsolidatedPSCLoaded = false;
                                    lobjRetirementCalculation.iblnBenefitMultiplierLoaded = false;
                                    lobjRetirementCalculation.SetRuleIndicator(); // PROD PIR ID 7379
                                    DateTime ldteNRDByAge = new DateTime();
                                    DateTime ldteNRDByRule = new DateTime();
                                    DateTime ldteTempNRD = busPersonBase.GetNormalRetirementDateBasedOnNormalEligibility(
                                            lobjRetirementCalculation.ibusPlan.icdoPlan.plan_id,
                                            lobjRetirementCalculation.ibusPlan.icdoPlan.plan_code,
                                            lobjRetirementCalculation.ibusPlan.icdoPlan.benefit_provision_id,
                                            lobjRetirementCalculation.ibusPlan.icdoPlan.benefit_type_value,
                                            lobjRetirementCalculation.ibusMember.icdoPerson.date_of_birth,
                                            lobjRetirementCalculation.ibusPersonAccount.icdoPersonAccount.Total_VSC, 2, iobjPassInfo,
                                            ldteActualEmployeeTerminationDate, lobjCurrentAccount.icdoPersonAccount.person_account_id,
                                            (lobjRetirementCalculation.icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate), 0,
                                            ref ldteNRDByAge, ref ldteNRDByRule, false, lobjRetirementCalculation.icdoBenefitCalculation.retirement_date, //PIR 14646
                                            lobjDBCacheData, lobjRetirementCalculation.ibusPersonAccount);
                                    if ((ldteNRDByRule < ldteNRDByAge) &&
                                        (ldteTempNRD > ibusCurrentRequest.icdoMasBatchRequest.statement_effective_date.GetFirstDayofNextMonth()) &&
                                        lobjRetirementCalculation.icdoBenefitCalculation.rule_indicator_value.IsNullOrEmpty()) // PROD PIR ID 7379
                                    {
                                        if (ldteNRDByRule != DateTime.MinValue)
                                            lobjRetirementCalculation.icdoBenefitCalculation.retirement_date = ldteNRDByRule;
                                        lobjRetirementCalculation.iblnConsoldatedVSCLoaded = false;
                                        lobjRetirementCalculation.iblnConsolidatedPSCLoaded = false;
                                        lobjRetirementCalculation.iblnBenefitMultiplierLoaded = false;
                                        if (ldteActualEmployeeTerminationDate != DateTime.MinValue)
                                            lobjRetirementCalculation.icdoBenefitCalculation.termination_date = ldteActualEmployeeTerminationDate;
                                        else if (lobjRetirementCalculation.icdoBenefitCalculation.retirement_date != DateTime.MinValue)
                                            lobjRetirementCalculation.icdoBenefitCalculation.termination_date = lobjRetirementCalculation.icdoBenefitCalculation.retirement_date.AddDays(-1);
                                        SetFlagForHPIndexing(lobjRetirementCalculation);
                                        lobjRetirementCalculation.CalculateFAS();
                                        lobjRetirementCalculation.LoadBenefitProvisionBenefitType(ldtbBenProvisionBenType);
                                        lobjRetirementCalculation.CalculateMASRetirementBenefit(lobjDBCacheData);
                                        if (lobjRetirementCalculation.CheckPersonEligible(lobjDBCacheData))
                                        {
                                            if ((lobjRetirementCalculation.icdoBenefitCalculation.plan_id == busConstant.PlanIdMain)  //PIR 20232 
                                                 &&
                                                ((lobjRetirementCalculation.ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.benefit_tier_value.IsNullOrEmpty() ||
                                                lobjRetirementCalculation.ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.benefit_tier_value == busConstant.MainBenefit1997Tier) &&
                                                 ((lobjRetirementCalculation.icdoBenefitCalculation.plan_id == busConstant.PlanIdMain && lcdoCV.code_value == busConstant.Main85))
                                                 ||
                                                ((lobjRetirementCalculation.ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.benefit_tier_value == busConstant.MainBenefit2016Tier) &&
                                                lobjRetirementCalculation.idecMemberAgeBasedOnRetirementDate >= 60 && 
                                                (((lobjRetirementCalculation.icdoBenefitCalculation.plan_id == busConstant.PlanIdMain) && !(lcdoCV.code_value == busConstant.Main85))))))
                                            {
                                                InsertMASPersonCalculation(
                                                    lobjMASPerson.icdoMasPerson.mas_person_id,
                                                    lobjRetirementCalculation.ibusPlan.icdoPlan.plan_id,
                                                    lobjRetirementCalculation.ibusPlan.icdoPlan.plan_name,
                                                    busConstant.ApplicationBenefitTypeRetirement,
                                                    "Age " + Convert.ToString(lobjRetirementCalculation.idecMemberAgeBasedOnRetirementDate) + " (" +
                                                    "Rule of " + Convert.ToString(lcdoCV.data2) + ")",
                                                    // UAT PIR: 1700 DNRO is not applicable for MAS, so only Unreduced benefit amount is only paid to Member
                                                    // UAT PIR: 2321 Reduce QDRO amount if exists.
                                                    (lobjRetirementCalculation.icdoBenefitCalculation.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeDNRO) ?
                                                    (lobjRetirementCalculation.icdoBenefitCalculation.unreduced_benefit_amount - lobjRetirementCalculation.icdoBenefitCalculation.qdro_amount) :
                                                    lobjRetirementCalculation.idecMASBenefitAmount,
                                                    lobjRetirementCalculation.idecReducedRHICAmount,
                                                    busConstant.Flag_No, lobjRetirementCalculation.icdoBenefitCalculation.member_account_balance,
                                                    lobjRetirementCalculation.icdoBenefitCalculation.qdro_amount,
                                                    lobjRetirementCalculation.icdoBenefitCalculation.normal_retirement_date,
                                                    lobjRetirementCalculation.icdoBenefitCalculation.calculation_final_average_salary,
                                                    ablnDefaultRetirementCalculation ? busConstant.Flag_No : busConstant.Flag_Yes, 0M,
                                                    lobjRetirementCalculation.icdoBenefitCalculation.retirement_date,
                                                    lobjRetirementCalculation.icdoBenefitCalculation.termination_date,
                                                    lobjRetirementCalculation.icdoBenefitCalculation.calculation_type_value, 0M,
                                                    lobjRetirementCalculation.icdoBenefitCalculation.credited_psc,
                                                    lobjRetirementCalculation.icdoBenefitCalculation.credited_vsc,
                                                    busConstant.Flag_Yes);
                                            }
                                            if (lobjRetirementCalculation.icdoBenefitCalculation.plan_id != busConstant.PlanIdMain) //PIR 20232
                                            {
                                                InsertMASPersonCalculation(
                                                    lobjMASPerson.icdoMasPerson.mas_person_id,
                                                    lobjRetirementCalculation.ibusPlan.icdoPlan.plan_id,
                                                    lobjRetirementCalculation.ibusPlan.icdoPlan.plan_name,
                                                    busConstant.ApplicationBenefitTypeRetirement,
                                                    "Age " + Convert.ToString(lobjRetirementCalculation.idecMemberAgeBasedOnRetirementDate) + " (" +
                                                    "Rule of " + Convert.ToString(lcdoCV.data2) + ")",
                                                    // UAT PIR: 1700 DNRO is not applicable for MAS, so only Unreduced benefit amount is only paid to Member
                                                    // UAT PIR: 2321 Reduce QDRO amount if exists.
                                                    (lobjRetirementCalculation.icdoBenefitCalculation.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeDNRO) ?
                                                    (lobjRetirementCalculation.icdoBenefitCalculation.unreduced_benefit_amount - lobjRetirementCalculation.icdoBenefitCalculation.qdro_amount) :
                                                    lobjRetirementCalculation.idecMASBenefitAmount,
                                                    lobjRetirementCalculation.idecReducedRHICAmount,
                                                    busConstant.Flag_No, lobjRetirementCalculation.icdoBenefitCalculation.member_account_balance,
                                                    lobjRetirementCalculation.icdoBenefitCalculation.qdro_amount,
                                                    lobjRetirementCalculation.icdoBenefitCalculation.normal_retirement_date,
                                                    lobjRetirementCalculation.icdoBenefitCalculation.calculation_final_average_salary,
                                                    ablnDefaultRetirementCalculation ? busConstant.Flag_No : busConstant.Flag_Yes, 0M,
                                                    lobjRetirementCalculation.icdoBenefitCalculation.retirement_date,
                                                    lobjRetirementCalculation.icdoBenefitCalculation.termination_date,
                                                    lobjRetirementCalculation.icdoBenefitCalculation.calculation_type_value, 0M,
                                                    lobjRetirementCalculation.icdoBenefitCalculation.credited_psc,
                                                    lobjRetirementCalculation.icdoBenefitCalculation.credited_vsc,
                                                    busConstant.Flag_Yes);
                                            }
                                            if (!ablnDefaultRetirementCalculation)
                                            {
                                                ablnDefaultRetirementCalculation = true;
                                                ldteTempNRDDate = lobjRetirementCalculation.icdoBenefitCalculation.termination_date;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        /// Calculates and Inserts for the Disability
                        lobjRetirementCalculation.icdoBenefitCalculation.termination_date = ibusCurrentRequest.icdoMasBatchRequest.statement_effective_date;
                        lobjRetirementCalculation.icdoBenefitCalculation.retirement_date = ibusCurrentRequest.icdoMasBatchRequest.statement_effective_date.GetFirstDayofNextMonth();
                        lobjRetirementCalculation.iblnConsoldatedVSCLoaded = false;
                        lobjRetirementCalculation.iblnConsolidatedPSCLoaded = false;
                        lobjRetirementCalculation.iblnBenefitMultiplierLoaded = false;
                        lobjRetirementCalculation.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypeDisability;
                        lobjRetirementCalculation.icdoBenefitCalculation.benefit_account_sub_type_value = null;
                        lobjRetirementCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeFinal; // No Projections
                        lobjRetirementCalculation.LoadBenefitProvisionBenefitType(ldtbBenProvisionBenType);
                        SetFlagForHPIndexing(lobjRetirementCalculation);
                        lobjRetirementCalculation.CalculateFAS();
                        lobjRetirementCalculation.CalculateMASRetirementBenefit(lobjDBCacheData);
                        InsertMASPersonCalculation(
                                        lobjMASPerson.icdoMasPerson.mas_person_id,
                                        lobjRetirementCalculation.ibusPlan.icdoPlan.plan_id,
                                        lobjRetirementCalculation.ibusPlan.icdoPlan.plan_name,
                                        busConstant.ApplicationBenefitTypeDisability,
                                        "Age " + Convert.ToString(lobjRetirementCalculation.idecMemberAgeBasedOnRetirementDate),
                                        lobjRetirementCalculation.idecMASBenefitAmount,
                                        lobjRetirementCalculation.idecReducedRHICAmount,
                                        busConstant.Flag_No, lobjRetirementCalculation.icdoBenefitCalculation.member_account_balance,
                                        lobjRetirementCalculation.icdoBenefitCalculation.qdro_amount,
                                        lobjRetirementCalculation.icdoBenefitCalculation.normal_retirement_date,
                                        lobjRetirementCalculation.icdoBenefitCalculation.calculation_final_average_salary, busConstant.Flag_No,
                                        lobjRetirementCalculation.idecDisabilityBenefitPercentage,
                                        lobjRetirementCalculation.icdoBenefitCalculation.retirement_date,
                                        lobjRetirementCalculation.icdoBenefitCalculation.termination_date,
                                        lobjRetirementCalculation.icdoBenefitCalculation.calculation_type_value, 0M,
                                        lobjRetirementCalculation.icdoBenefitCalculation.credited_psc,
                                        lobjRetirementCalculation.icdoBenefitCalculation.credited_vsc);

                            /// Calculates and Inserts for the Pre-Retirement Death
                            busPreRetirementDeathBenefitCalculation lobjPreRetirementDeath = new busPreRetirementDeathBenefitCalculation
                            {
                                icdoBenefitCalculation = new cdoBenefitCalculation(),
                                ibusMember = new busPerson { icdoPerson = new cdoPerson() },
                                ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount(), ibusPlan = new busPlan { icdoPlan = new cdoPlan() } },
                                ibusPlan = new busPlan { icdoPlan = new cdoPlan() }
                            };
                            lobjPreRetirementDeath.ibusMember = lobjPerson;
                            lobjPreRetirementDeath.ibusPlan = lobjCurrentPlan;
                            lobjPreRetirementDeath.ibusPersonAccount = lobjCurrentAccount;
                            lobjPreRetirementDeath.ibusPersonAccount.ibusPerson = lobjPerson;
                            lobjPreRetirementDeath.ibusPersonAccount.ibusPlan = lobjCurrentPlan;
                            lobjPreRetirementDeath.idtbBenOptionFactor = ldtbBenOptionFactor;
                            lobjPreRetirementDeath.idtbBenefitProvisionExclusion = ldtbBenefitProvisionExclusion;
                            lobjPreRetirementDeath.iblnUseDataTableForBenOptionFactor = true;
                            lobjPreRetirementDeath.idtbBenOptionCodeValue = ldtbBenOptionCodeValue;
                            lobjPreRetirementDeath.idtbDeathBenOptionCodeValue = ldtbDeathBenOptionCodeValue;
                            lobjPreRetirementDeath.idtLastIntertestPostDate = ldtLastIntertestPostDate;
                            lobjPreRetirementDeath.idteLastContributedDate = lobjRetirementCalculation.idteLastContributedDate;
                            lobjPreRetirementDeath.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypePreRetirementDeath;
                            lobjPreRetirementDeath.icdoBenefitCalculation.person_id = lobjPreRetirementDeath.ibusMember.icdoPerson.person_id;
                            lobjPreRetirementDeath.icdoBenefitCalculation.plan_id = lobjPreRetirementDeath.ibusPlan.icdoPlan.plan_id;
                            lobjPreRetirementDeath.icdoBenefitCalculation.created_date = ibusCurrentRequest.icdoMasBatchRequest.statement_effective_date;
                            lobjPreRetirementDeath.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeFinal; // No Projections     
                                                                                                                                     //prod pir 7329 : for HP need to take the actual termination date for Suspended person accounts to do the indexing properly
                                                                                                                                     //--start--//
                            if (lobjPreRetirementDeath.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdHP)
                            {
                                busPersonAccountRetirement lobjPARetr = new busPersonAccountRetirement();
                                lobjPARetr.FindPersonAccountRetirement(lobjPreRetirementDeath.ibusPersonAccount.icdoPersonAccount.person_account_id);
                                lobjPARetr.LoadPersonAccountRetirementHistory(false);
                                busPersonAccountRetirementHistory lobjPARetrHistory = lobjPARetr.icolPersonAccountRetirementHistory
                                    .Where(o => busGlobalFunctions.CheckDateOverlapping(ibusCurrentRequest.icdoMasBatchRequest.statement_effective_date,
                                                                                        o.icdoPersonAccountRetirementHistory.start_date,
                                                                                        o.icdoPersonAccountRetirementHistory.end_date)).FirstOrDefault();
                                if (lobjPARetrHistory != null &&
                                    lobjPARetrHistory.icdoPersonAccountRetirementHistory.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended)
                                {
                                    lobjPreRetirementDeath.icdoBenefitCalculation.termination_date = lobjPARetrHistory.icdoPersonAccountRetirementHistory.start_date.AddDays(-1);
                                }
                                else
                                    lobjPreRetirementDeath.icdoBenefitCalculation.termination_date = ibusCurrentRequest.icdoMasBatchRequest.statement_effective_date;
                            }
                            else
                            {
                                lobjPreRetirementDeath.icdoBenefitCalculation.termination_date = ibusCurrentRequest.icdoMasBatchRequest.statement_effective_date;
                            }
                            //--end--//
                            lobjPreRetirementDeath.icdoBenefitCalculation.date_of_death = ibusCurrentRequest.icdoMasBatchRequest.statement_effective_date;
                            lobjPreRetirementDeath.icdoBenefitCalculation.retirement_date = ibusCurrentRequest.icdoMasBatchRequest.statement_effective_date.GetFirstDayofNextMonth();
                            lobjRetirementCalculation.iblnConsoldatedVSCLoaded = false;
                            lobjRetirementCalculation.iblnConsolidatedPSCLoaded = false;
                            lobjPreRetirementDeath.SetNormalRetirementDate(lobjDBCacheData);
                            lobjPreRetirementDeath.GetNormalEligibilityDetails(lobjDBCacheData);
                            lobjPreRetirementDeath.LoadBenefitCalculationPayeeForNewMode(true);
                            //prod pir 1920
                            //if no open beneficiary, take from contact with relationship spouse
                            if (lobjPreRetirementDeath.iclbBenefitCalculationPayee == null || lobjPreRetirementDeath.iclbBenefitCalculationPayee.Count == 0)
                                lobjPreRetirementDeath.LoadMASBenefitCalculationPayeeForNewModeFromContact();
                            lobjPreRetirementDeath.LoadBenefitProvisionBenefitType(ldtbBenProvisionBenType);
                            lobjPreRetirementDeath.CalculatePreRetirementDeathBenefit();
                            // UAT PIR ID 1771. Do not show RHIC benefit amount 
                            if (lobjPreRetirementDeath.icdoBenefitCalculation.final_monthly_benefit <= 0M)
                                lobjPreRetirementDeath.icdoBenefitCalculation.unreduced_rhic_amount = 0M;
                            DataTable ldtbResult = ldtbAllMASPersonPlan.AsEnumerable().Where(row =>
                                                            row.Field<int>("PLAN_ID") == lobjRetirementCalculation.icdoBenefitCalculation.plan_id &&
                                                            row.Field<int>("MAS_PERSON_ID") == lobjMASPerson.icdoMasPerson.mas_person_id).AsDataTable(); // PROD PIR ID 7329
                            decimal ldecMABasonEffectiveDate = 0M;
                            if (ldtbResult.Rows.Count > 0)
                                ldecMABasonEffectiveDate = Convert.ToDecimal(ldtbResult.Rows[0]["MEMBER_ACCOUNT_BALANCE_LTD"]);
                            InsertMASPersonCalculation(
                                            lobjMASPerson.icdoMasPerson.mas_person_id,
                                            lobjPreRetirementDeath.ibusPlan.icdoPlan.plan_id,
                                            lobjPreRetirementDeath.ibusPlan.icdoPlan.plan_name,
                                            busConstant.ApplicationBenefitTypePreRetirementDeath,
                                            Convert.ToString(lobjPreRetirementDeath.idecNormalEligibilityAttainedAge),
                                            lobjPreRetirementDeath.icdoBenefitCalculation.final_monthly_benefit,
                                            lobjPreRetirementDeath.icdoBenefitCalculation.unreduced_rhic_amount,
                                            busConstant.Flag_No, lobjPreRetirementDeath.icdoBenefitCalculation.minimum_guarentee_amount,
                                            lobjPreRetirementDeath.icdoBenefitCalculation.qdro_amount,
                                            lobjPreRetirementDeath.icdoBenefitCalculation.normal_retirement_date,
                                            lobjPreRetirementDeath.icdoBenefitCalculation.calculation_final_average_salary, busConstant.Flag_No, 0M,
                                            lobjRetirementCalculation.icdoBenefitCalculation.retirement_date,
                                            lobjRetirementCalculation.icdoBenefitCalculation.termination_date,
                                            lobjRetirementCalculation.icdoBenefitCalculation.calculation_type_value,
                                            ldecMABasonEffectiveDate,
                                            lobjRetirementCalculation.icdoBenefitCalculation.credited_psc,
                                            lobjRetirementCalculation.icdoBenefitCalculation.credited_vsc);
                            lobjRetirementCalculation = null;
                            lobjPreRetirementDeath = null;
                        }
                    }
                    // PIR 9481,9482 - Member's Retirement Section visibility
                    if (iblnIsMemberVested)
                    {
                        DBFunction.DBNonQuery("cdoMASBatchRequest.Update_Vested_Flag", new object[1] { 
                                    lobjMASPerson.icdoMasPerson.mas_person_id
                                    }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                        iblnIsMemberVested = false;
                    }
                }

                // Update Data pulled flag.
                lobjMASSelection.icdoMasSelection.is_data_pulled_flag = busConstant.Flag_Yes;
                lobjMASSelection.icdoMasSelection.Update();
                if (utlPassInfo.iobjPassInfo.iblnInTransaction)
                    utlPassInfo.iobjPassInfo.Commit();
            }
            catch (Exception ex)
            {
                if (utlPassInfo.iobjPassInfo.iblnInTransaction)
                    utlPassInfo.iobjPassInfo.Rollback();
                ExceptionManager.Publish(ex);
                throw;
            }
            finally
            {
                FreePassInfo(utlPassInfo.iobjPassInfo);
            }
        }

        private void SetFlagForHPIndexing(busBenefitCalculation aobjBenefitCalculation)
        {
            if (aobjBenefitCalculation?.icdoBenefitCalculation?.termination_date != DateTime.MinValue &&
                                aobjBenefitCalculation?.icdoBenefitCalculation?.termination_date < busConstant.idteHPIndexEligibleDate)
            {
                aobjBenefitCalculation.iblnIsActualTerminationDate = true;
            }
        }

        private int InsertMASPersonCalculation(int aintMasPersonID, int aintPlanID, string astrPlanName, string astrBenefitType, string astrAge,
                            decimal adecMonthlyBenefit, decimal adecRHICBenefit, string astrIsNRDDate, decimal adecMemberAccountBalance,
                            decimal adecQDRODeductions, DateTime adteNRD, decimal adecFAS, string astrIsDefaultRetirementFlag, decimal adecDisabilityBenefitPercentage,
                            DateTime adtRetirementDate, DateTime adtTerminationDate, string astrCalculationType,
                            decimal adecMABasonEffectiveDate, decimal adecPSC, decimal adecVSC, string astrIsRuleOrAge = null)
        {
            cdoMasPersonCalculation lcdoPersonPlan = new cdoMasPersonCalculation
            {
                mas_person_id = aintMasPersonID,
                plan_id = aintPlanID,
                plan_name = astrPlanName,
                benefit_account_type_value = astrBenefitType,
                age_description = astrAge,
                monthly_benefit = adecMonthlyBenefit,
                rhic_benefit = adecRHICBenefit,
                is_nrd_date = astrIsNRDDate,
                member_account_balance = adecMemberAccountBalance,
                qdro_deductions = adecQDRODeductions,
                normal_retirement_date = adteNRD,
                final_average_salary = adecFAS,
                is_default_retirement = astrIsDefaultRetirementFlag,
                disability_benefit_percentage = adecDisabilityBenefitPercentage,
                retirement_date = adtRetirementDate,
                termination_date = adtTerminationDate,
                calculation_type = astrCalculationType,
                pension_service_credit = adecPSC,
                vested_service_credit = adecVSC,
                mab_as_on_effective_date=adecMABasonEffectiveDate,
                is_rule_or_age_indicator = astrIsRuleOrAge
            };
            return lcdoPersonPlan.Insert();
        }
    }
}
