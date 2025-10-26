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
using NeoSpin.DataObjects;
using CrystalDecisions.CrystalReports.Engine;


namespace NeoSpinBatch
{
    public class busRetireeAnnualStatementBatch : busNeoSpinBatch
    {
        public DataSet idsRetireeAnnualStatement { get; set; }

        public string istrReportName { get; set; }

        public string istrStepName { get; set; }

        public DataTable ldtbAllMASPerson { get; set; }

        public DataTable ldtbAllMASPersonPlan { get; set; }

        public DataTable ldtbAllMASPersonPlanDC { get; set; }

        public DataTable ldtbAllMASBenDependent { get; set; }

        public DataTable ldtbAllMASLifeOptions { get; set; }

        public DataTable ldtbAllMembers { get; set; }

        public DataTable ldtbAllMemberAccounts { get; set; }

        public DataTable ldtbAllPlans { get; set; }

        public busDBCacheData ibusDBCacheData { get; set; }

        public DataTable ldtbPapit { get; set; }

        public DataTable ldtbPapitItems { get; set; }

        public DataTable ldtbBenefitOption { get; set; }

        public DataTable ldtbPayeeAccount { get; set; }

        public Collection<busOrgPlan> iclbProviderOrgPlan { get; set; }

        public DataTable idtbPALifeOptionHistory { get; set; }

        public DataTable idtbGHDVHistory { get; set; }

        public DataTable idtbLifeHistory { get; set; }


        busBase lbusBase = new busBase();

        public void LoadActiveProviders()
        {
            DataTable ldtbActiveProviders = busNeoSpinBase.Select("cdoIbsHeader.LoadAllActiveProviders",
                new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
            iclbProviderOrgPlan = new busBase().GetCollection<busOrgPlan>(ldtbActiveProviders, "icdoOrgPlan");
        }

        public void LoadLifeOptionData()
        {
            idtbPALifeOptionHistory =
                busNeoSpinBase.Select("cdoIbsHeader.LoadLifeOption",
                new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
        }

        public void LoadGHDVHistory()
        {
            idtbGHDVHistory =
                busNeoSpinBase.Select("cdoIbsHeader.LoadGHDVHistory", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
        }

        public void LoadLifeHistory()
        {
            idtbLifeHistory =
                busNeoSpinBase.Select("cdoIbsHeader.LoadLifeHistory", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
        }


        public busOrgPlan LoadProviderOrgPlanByProviderOrgId(int aintProviderOrgId, int aintPlanId)
        {
            busOrgPlan lbusOrgPlanToReturn = new busOrgPlan { icdoOrgPlan = new cdoOrgPlan() };
            foreach (var lbusOrgPlan in iclbProviderOrgPlan)
            {
                if ((lbusOrgPlan.icdoOrgPlan.org_id == aintProviderOrgId) &&
                   (lbusOrgPlan.icdoOrgPlan.plan_id == aintPlanId))
                {
                    if (busGlobalFunctions.CheckDateOverlapping(iobjSystemManagement.icdoSystemManagement.batch_date,
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
        public busMASBatchRequest ibusCurrentRequest { get; set; }

        private void InitializeDataTables()
        {
            ldtbAllMASPerson = new DataTable();
            ldtbAllMASPersonPlan = new DataTable();
            ldtbAllMASBenDependent = new DataTable();
            ldtbAllMASLifeOptions = new DataTable();
        }

        public void CreateRetireeAnnualStatements()
        {
            istrProcessName = iobjBatchSchedule.step_name;
            istrStepName = iobjBatchSchedule.step_name;
            idlgUpdateProcessLog(istrProcessName + " Started", "INFO", istrProcessName);

            //Loading all Data (optimization)
            LoadDBCacheData();
            LoadActiveProviders();
            LoadLifeOptionData();
            LoadGHDVHistory();
            LoadLifeHistory();

            DataTable ldtbResult = busBase.Select<cdoMasBatchRequest>
                                        (new string[2] { "ACTION_STATUS_VALUE", "GROUP_TYPE_VALUE" },
                                         new object[2] { busConstant.StatusPending, busConstant.GroupTypeRetired }, null, null);

            // Cache all Plans
            ldtbAllPlans = busBase.Select<cdoPlan>(new string[] { }, new object[] { }, null, null);
            foreach (DataRow ldtr in ldtbResult.Rows)
            {
                ibusCurrentRequest = new busMASBatchRequest { icdoMasBatchRequest = new cdoMasBatchRequest() };
                ibusCurrentRequest.icdoMasBatchRequest.LoadData(ldtr);
                idlgUpdateProcessLog("Processing " + ibusCurrentRequest.icdoMasBatchRequest.batch_request_type_description + " Batch Request", "INFO", istrStepName);
                bool lblnTransaction = false;
                try
                {
                    if (!lblnTransaction)
                    {
                        utlPassInfo.iobjPassInfo.BeginTransaction();
                        lblnTransaction = true;
                    }
                    //Do not insert retiree data if already inserted for batch request ID
                    DataTable ldtbMASSelForRequest = busBase.Select<cdoMasSelection>(new string[1] { enmMasSelection.mas_batch_request_id.ToString() }, new object[1] { ibusCurrentRequest.icdoMasBatchRequest.mas_batch_request_id }, null, null);
                    if (ldtbMASSelForRequest.IsNotNull() && ldtbMASSelForRequest.Rows.Count == 0)
                    {

                        idlgUpdateProcessLog("STEP 1: Inserting Retiree's Data into the Tables", "INFO", istrProcessName);
                        //Insert all retiree information for the batch request into sgt_mas_selection table
                        int i1 = DBFunction.DBNonQuery("cdoMASBatchRequest.Insert_into_MAS_Selection_Retirees", new object[2] { 
                                    ibusCurrentRequest.icdoMasBatchRequest.mas_batch_request_id,iobjBatchSchedule.batch_schedule_id},
                                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                        //Insert all retiree insurance information for the batch request into sgt_mas_person table
                        int i = DBFunction.DBNonQuery("cdoMASBatchRequest.Insert_into_MAS_Person_Insurance_Retirees", new object[2] { 
                                    ibusCurrentRequest.icdoMasBatchRequest.mas_batch_request_id,iobjBatchSchedule.batch_schedule_id},
                                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                        //Insert all retiree information for the batch request into sgt_mas_payee_account and sgt_mas_payee_account_papit table
                        int i2 = DBFunction.DBNonQuery("cdoMASBatchRequest.Insert_into_MAS_Payee_Account", new object[2] { 
                                    ibusCurrentRequest.icdoMasBatchRequest.mas_batch_request_id,iobjBatchSchedule.batch_schedule_id},
                                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                        //Insert all beneficiary and dependent of the retiree related information for the batch request into sgt_mas_payee_account
                        //and sgt_mas_payee_account_papit table
                        int l = DBFunction.DBNonQuery("cdoMASBatchRequest.Insert_into_MAS_Ben_Dep", new object[2] { 
                                    ibusCurrentRequest.icdoMasBatchRequest.mas_batch_request_id,iobjBatchSchedule.batch_schedule_id},
                                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                    }
                    if (lblnTransaction)
                    {
                        utlPassInfo.iobjPassInfo.Commit();
                        lblnTransaction = false;
                    }
                    /////// Get all the Data from the temporary tables to the local DataTable to create report.
                    idlgUpdateProcessLog("STEP 2: Fetch All Retiree's Data to Create Report", "INFO", istrProcessName);
					FetchAllRetireeData();
                    /// Create the Summary & Online report for the current batch request Persons with Benefit Calculations.
                    idlgUpdateProcessLog("STEP 3: Creating Summary & Online Report for all Retirees", "INFO", istrProcessName);
                    CreateMASReport();

                    ///// Update the Batch Request to Processed.
                    idlgUpdateProcessLog("STEP 4: Updating Batch Request status to processed", "INFO", istrProcessName);
                    ibusCurrentRequest.icdoMasBatchRequest.action_status_value = busConstant.BatchRequestActionStatusProcessed;
                    ibusCurrentRequest.icdoMasBatchRequest.Update();
                }
                catch (Exception Ex)
                {
                    ExceptionManager.Publish(Ex);
                    if (lblnTransaction)
                    {
                        utlPassInfo.iobjPassInfo.Rollback();
                        lblnTransaction = false;
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
        }

        /// Load All Retirees Info into the DataTable to create Report
        private void FetchAllRetireeData()
        {
            InitializeDataTables();

            /// Sort by Person Last Name by joining with Person Table
            ldtbAllMASPerson = busBase.Select("cdoMasPerson.LoadMasPerson",
                                            new object[2] { ibusCurrentRequest.icdoMasBatchRequest.statement_effective_date, ibusCurrentRequest.icdoMasBatchRequest.mas_batch_request_id });

            ldtbPapit = busBase.Select("cdoMasPayeeAccountPapit.LoadMASPapit",
                                            new object[1] { ibusCurrentRequest.icdoMasBatchRequest.mas_batch_request_id });

            ldtbPapitItems = busBase.Select("cdoMasPayeeAccountPapit.LoadMASPapitItems",
                                            new object[1] { ibusCurrentRequest.icdoMasBatchRequest.mas_batch_request_id });

            ldtbBenefitOption = busBase.Select("cdoMasPayeeAccount.LoadBenefitOption",
                                           new object[1] { ibusCurrentRequest.icdoMasBatchRequest.mas_batch_request_id });

            //PIR 20544 Retiree Annual Statement
            //ldtbAllMASBenDependent = busBase.Select("cdoMasPersonPlanBeneficiaryDependent.LoadMASBenDependent",
            //                                new object[1] { ibusCurrentRequest.icdoMasBatchRequest.mas_batch_request_id }).AsEnumerable().OrderByDescending( o => o.Field<string>("BENEFICIARY_TYPE_DESCRIPTION")).AsDataTable();
            ldtbAllMASBenDependent = busBase.Select("cdoMasPersonPlanBeneficiaryDependent.LoadMASBenDependent",
                                            new object[1] { ibusCurrentRequest.icdoMasBatchRequest.mas_batch_request_id });

            ldtbAllMASLifeOptions = busBase.Select("cdoMasLifeOption.LoasMASLifeOption",
                                            new object[1] { ibusCurrentRequest.icdoMasBatchRequest.mas_batch_request_id });

            ldtbPayeeAccount = busBase.Select("cdoMasPayeeAccount.LoadMasPayeeAccount",
                                            new object[3] { ibusCurrentRequest.icdoMasBatchRequest.mas_batch_request_id, ibusCurrentRequest.icdoMasBatchRequest.statement_effective_date, 0 });
        }

        public void CreateStatementFile(int ainMasSelectionId, string astrStatementType, string astrStatementName)
        {
            cdoMasStatementFile lobjStatementFile = new cdoMasStatementFile();
            lobjStatementFile.mas_selection_id = ainMasSelectionId;
            lobjStatementFile.statement_type_id = 3056;
            lobjStatementFile.statement_type_value = astrStatementType;
            lobjStatementFile.statement_name = astrStatementName;
            lobjStatementFile.Insert();
        }

        /// Filter into the DataTable and create the 
        /// for every Member.
        private void CreateMASReport()
        {
            DataTable ldtbAllMASSelection = busBase.Select<cdoMasSelection>(new string[2] { enmMasSelection.mas_batch_request_id.ToString(), enmMasSelection.is_report_created_flag.ToString() },
                                                    new object[2] { ibusCurrentRequest.icdoMasBatchRequest.mas_batch_request_id, busConstant.Flag_No }, null, enmMasSelection.person_id.ToString());
            // Load Person, Plan & Person Account info for all the MAS Selected Members.
            busRetireeAnnualStatement lobjRetireeAnnualStatement = new busRetireeAnnualStatement();

            lobjRetireeAnnualStatement.ldtbAllMASPerson = ldtbAllMASPerson;

            lobjRetireeAnnualStatement.ldtbPapit = ldtbPapit;

            lobjRetireeAnnualStatement.ldtbPapitItems = ldtbPapitItems;

            lobjRetireeAnnualStatement.ldtbBenefitOption = ldtbBenefitOption;

            lobjRetireeAnnualStatement.ldtbAllMASBenDependent = ldtbAllMASBenDependent;

            lobjRetireeAnnualStatement.ldtbAllMASLifeOptions = ldtbAllMASLifeOptions;

            lobjRetireeAnnualStatement.ldtbPayeeAccount = ldtbPayeeAccount;

            lobjRetireeAnnualStatement.ibusDBCacheData = new busDBCacheData();

            lobjRetireeAnnualStatement.idtEffectiveDate = ibusCurrentRequest.icdoMasBatchRequest.statement_effective_date;

            lobjRetireeAnnualStatement.ibusDBCacheData.idtbCachedCoverageRef = ibusDBCacheData.idtbCachedCoverageRef;

            lobjRetireeAnnualStatement.ibusDBCacheData.idtbCachedHealthRate = ibusDBCacheData.idtbCachedHealthRate;

            lobjRetireeAnnualStatement.ibusDBCacheData.idtbCachedVisionRate = ibusDBCacheData.idtbCachedVisionRate;

            lobjRetireeAnnualStatement.ibusDBCacheData.idtbCachedRateStructureRef = ibusDBCacheData.idtbCachedRateStructureRef;

            lobjRetireeAnnualStatement.ibusDBCacheData.idtbCachedRateRef = ibusDBCacheData.idtbCachedRateRef;

            lobjRetireeAnnualStatement.ibusDBCacheData.idtbCachedHMORate = ibusDBCacheData.idtbCachedHMORate;

            lobjRetireeAnnualStatement.ibusDBCacheData.idtbCachedLtcRate = ibusDBCacheData.idtbCachedLtcRate;

            lobjRetireeAnnualStatement.ibusDBCacheData.idtbCachedLifeRate = ibusDBCacheData.idtbCachedLifeRate;

            lobjRetireeAnnualStatement.ibusDBCacheData.idtbCachedDentalRate = ibusDBCacheData.idtbCachedDentalRate;

            lobjRetireeAnnualStatement.idtbGHDVHistory = idtbGHDVHistory;

            lobjRetireeAnnualStatement.idtbLifeHistory = idtbLifeHistory;

            lobjRetireeAnnualStatement.idtbPALifeOptionHistory = idtbPALifeOptionHistory;

            lobjRetireeAnnualStatement.iclbProviderOrgPlan = iclbProviderOrgPlan;

            foreach (DataRow ldtrMember in ldtbAllMASSelection.Rows)
            {

                busMASSelection lobjMASSelection = new busMASSelection { icdoMasSelection = new cdoMasSelection() };
                try
                {
                    lobjMASSelection.icdoMasSelection.LoadData(ldtrMember);

                    idsRetireeAnnualStatement = lobjRetireeAnnualStatement.CreateMASReport(lobjMASSelection.icdoMasSelection.person_id,
                        ibusCurrentRequest.icdoMasBatchRequest.statement_effective_date);
                    //   idlgUpdateProcessLog("Creating Summary Annual Statement" + " for PERSLink ID:" + lobjMASSelection.icdoMasSelection.person_id, "INFO", istrStepName);
                    if (idsRetireeAnnualStatement != null && idsRetireeAnnualStatement.Tables.Count > 0 && idsRetireeAnnualStatement.Tables["ReportTable01"].Rows.Count > 0)
                    {
                        string lstrStatementName = null;
                        string lstrAnnualStatement = null;
						 // Annual Statements - PIR 17506
                        if (ibusCurrentRequest.icdoMasBatchRequest.mailing_generate_flag == busConstant.Flag_Yes)
                        {
                            if (ibusCurrentRequest.icdoMasBatchRequest.batch_request_type_value == busConstant.BatchRequestTypeAnnual)
                            {
                                string lstrSummaryStmt = CreateReportWithPrefix("rptSummaryRetireeStatement.rpt", idsRetireeAnnualStatement,
                                    lobjMASSelection.icdoMasSelection.person_id.ToString() + "_", busConstant.ReportMASPath);
                                string[] lstrTemp = lstrSummaryStmt.Split('\\');
                                if (lstrTemp.Count() > 0)
                                {
                                    lstrStatementName = lstrTemp[lstrTemp.Count() - 1];
                                }
                                //  idlgUpdateProcessLog("Creating Online Annual Statement" + " for PERSLink ID:" + lobjMASSelection.icdoMasSelection.person_id, "INFO", istrStepName);                       
                            }
                            else
                            {
                                string lstrOnlineStmt = CreateReportWithPrefix("rptOnlineRetireeStatement.rpt", idsRetireeAnnualStatement,
                                    lobjMASSelection.icdoMasSelection.person_id.ToString() + "_", busConstant.ReportMASPath);

                                string[] lstrTemp2 = lstrOnlineStmt.Split('\\');
                                if (lstrTemp2.Count() > 0)
                                {
                                    lstrAnnualStatement = lstrTemp2[lstrTemp2.Count() - 1];
                                }
                            }
                        }
                        CreateStatementFile(lobjMASSelection.icdoMasSelection.mas_selection_id, busConstant.SummaryStatementFile, lstrStatementName);
                        CreateStatementFile(lobjMASSelection.icdoMasSelection.mas_selection_id, busConstant.OnlineStatementFile, lstrAnnualStatement);
                        lobjMASSelection.icdoMasSelection.is_report_created_flag = busConstant.Flag_Yes;
                        lobjMASSelection.icdoMasSelection.Update();
                    }
                }
                catch (Exception Ex)
                {
                    ExceptionManager.Publish(Ex);
                    idlgUpdateProcessLog("Error in Creating Online Annual Statement for PERSLink ID:"
                                                    + lobjMASSelection.icdoMasSelection.person_id, "INFO", istrStepName);
                    throw Ex;
                }
            }
            if (ldtbAllMASSelection.Rows.Count == 0)
            {
                idlgUpdateProcessLog("There are no records selected to generate the annual statement for the request ID:"
                    + ibusCurrentRequest.icdoMasBatchRequest.mas_batch_request_id.ToString(), "INFO", istrStepName);
            }
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

        private busPlan GetPlan(int aintPlanID)
        {
            busPlan lobjPlan = new busPlan { icdoPlan = new cdoPlan() };
            DataTable ldtbPlan = ldtbAllPlans.AsEnumerable().Where(o => o.Field<int>("plan_id") == aintPlanID).AsDataTable();
            if (ldtbAllPlans.Rows.Count > 0)
                lobjPlan.icdoPlan.LoadData(ldtbPlan.Rows[0]);
            return lobjPlan;
        }
    }
}
