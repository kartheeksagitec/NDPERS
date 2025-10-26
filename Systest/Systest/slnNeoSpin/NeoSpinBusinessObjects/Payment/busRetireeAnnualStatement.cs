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


namespace NeoSpin.BusinessObjects
{
    public class busRetireeAnnualStatement : busExtendBase
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
        public DateTime idtEffectiveDate { get; set; }

        public decimal idecTotalPaidPremiumAmt { get; set; }

        public void LoadActiveProviders()
        {
            DataTable ldtbActiveProviders = busNeoSpinBase.Select("cdoIbsHeader.LoadAllActiveProviders",
                new object[1] { DateTime.Today });
            iclbProviderOrgPlan = new busBase().GetCollection<busOrgPlan>(ldtbActiveProviders, "icdoOrgPlan");
        }

        public void LoadLifeOptionData()
        {
            idtbPALifeOptionHistory =
                busNeoSpinBase.Select("cdoIbsHeader.LoadLifeOption",
                new object[1] { DateTime.Today });
        }

        public void LoadGHDVHistory()
        {
            idtbGHDVHistory =
                busNeoSpinBase.Select("cdoIbsHeader.LoadGHDVHistory", new object[1] { DateTime.Today });
        }

        public void LoadLifeHistory()
        {
            idtbLifeHistory =
                busNeoSpinBase.Select("cdoIbsHeader.LoadLifeHistory", new object[1] { DateTime.Today });
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
        public busOrgPlan LoadProviderOrgPlanByProviderOrgId(int aintProviderOrgId, int aintPlanId)
        {
            busOrgPlan lbusOrgPlanToReturn = new busOrgPlan { icdoOrgPlan = new cdoOrgPlan() };
            foreach (var lbusOrgPlan in iclbProviderOrgPlan)
            {
                if ((lbusOrgPlan.icdoOrgPlan.org_id == aintProviderOrgId) &&
                   (lbusOrgPlan.icdoOrgPlan.plan_id == aintPlanId))
                {
                    if (busGlobalFunctions.CheckDateOverlapping(DateTime.Today,
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
        public void CreateRetireeAnnualStatements()
        {
            //Loading DB Cache (optimization)
            //idlgUpdateProcessLog("Loading DB Cache Data", "INFO", istrProcessName);
            LoadDBCacheData();

            //idlgUpdateProcessLog("Loading All Active Providers", "INFO", iobjBatchSchedule.step_name);
            //Loading Complete Activte Provider Org Plan List (Optimization Purpose)
            LoadActiveProviders();

            //idlgUpdateProcessLog("Loading Life Option Records for All Insurance Members", "INFO", iobjBatchSchedule.step_name);
            //Loading the Life Option Data by Org (Optimization)
            LoadLifeOptionData();

            //idlgUpdateProcessLog("Loading GHDV History Records for All Insurance Members", "INFO", iobjBatchSchedule.step_name);
            //Loading the GHDV History (Optimization)
            LoadGHDVHistory();

            //idlgUpdateProcessLog("Loading LIFE History Records for All Insurance Members", "INFO", iobjBatchSchedule.step_name);
            //Loading the Life History (Optimization)
            LoadLifeHistory();
        }
        /// Load All Retirees Info into the DataTable to create Report
        /// Method used to display report from person overview screen
        public void FetchAllRetireeDataBySelection(int aintMASSelectionID)
        {

            InitializeDataTables();
           
            //Load the statement effective Date
            DataTable ldtStatementEffectiveDate = busBase.Select("cdoMasPayeeAccount.LoadStatementEffectiveDate", new object[1] { aintMASSelectionID });
            if (ldtStatementEffectiveDate.Rows.Count > 0)
            {
                DataRow ldr = ldtStatementEffectiveDate.Rows[0];
                idtEffectiveDate = Convert.ToDateTime(ldr["STATEMENT_EFFECTIVE_DATE"].ToString());
            }
            else
                idtEffectiveDate = DateTime.Today; //Added Just in case there is a problem. Logically unreachable code.

            /// Load all Persons by Selection ID and statement effective date
            ldtbAllMASPerson = busBase.Select("cdoMasPerson.LoadMasPersonBySelection",
                                            new object[2] { aintMASSelectionID, idtEffectiveDate });

            ////Commented and made the formula change in report 
            //PIR 9790
            //ldtbAllMASPersonPlan = busBase.Select("cdoMasPerson.LoadMasPersonPlan",
            //                                new object[1] { aintMASSelectionID });

            //Load all Papit By selection
            ldtbPapit = busBase.Select("cdoMasPayeeAccountPapit.LoadMASPapitBySelection",
                                            new object[1] { aintMASSelectionID });

            //Load all Papit items by selection
            ldtbPapitItems = busBase.Select("cdoMasPayeeAccountPapit.LoadMASPapitItemsBySelection",
                                            new object[1] { aintMASSelectionID });

            //Load all benefit options by Selection
            ldtbBenefitOption = busBase.Select("cdoMasPayeeAccount.LoadBenefitOptionBySelection",
                                           new object[1] { aintMASSelectionID });

            //Load all Beneficiary Dependents by Selection and order by Beneficiary Type
            //PIR 20544 Retiree Annual Statement
            //ldtbAllMASBenDependent = busBase.Select("cdoMasPersonPlanBeneficiaryDependent.LoadMASBenDependentBySelection",
            //                                new object[1] { aintMASSelectionID }).AsEnumerable().OrderByDescending( o => o.Field<string>("BENEFICIARY_TYPE_DESCRIPTION")).AsDataTable();
            ldtbAllMASBenDependent = busBase.Select("cdoMasPersonPlanBeneficiaryDependent.LoadMASBenDependentBySelection",
                                            new object[1] { aintMASSelectionID });

            //Load all MAS Life Options By Selection
            ldtbAllMASLifeOptions = busBase.Select("cdoMasLifeOption.LoasMASLifeOptionBySelection",
                                            new object[1] { aintMASSelectionID });

            //Load all Payee Accounts (NOTE: ORIGINAL QUERY where Batch Request ID is passed as 0)
            ldtbPayeeAccount = busBase.Select("cdoMasPayeeAccount.LoadMasPayeeAccount",
                                            new object[3] { 0, idtEffectiveDate, aintMASSelectionID });
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
        //PIR-8051
        public static DataTable RemoveCOLAforJobService(DataTable source)
        {
            DataTable ldtbResuls = new DataTable();
            string lstrFilterString = string.Empty;
            source.DefaultView.RowFilter = "PLAN_ID <> 6 OR ITEM_TYPE_CODE NOT IN ('ITEM39','ITEM44','ITEM50','ITEM56','ITEM402','ITEM412','ITEM422','ITEM432')";
            ldtbResuls = source.DefaultView.ToTable();
            return ldtbResuls;
        }

      
        private bool IsHealthOrMedicare(int aintPlanID)
        {
            bool lblnResult = false;
            if ((aintPlanID == busConstant.PlanIdGroupHealth) || (aintPlanID == busConstant.PlanIdMedicarePartD))
                lblnResult = true;
            return lblnResult;
        }
        /// Filter into the DataTable and create the 
        /// for every Member.
        public DataSet CreateMASReport(int aintPersonID, DateTime adtEffectiveDate)
        {

            DataSet idsRetireeAnnualStatement = new DataSet();
            //Filter Person by Person ID
            DataTable ldtbMASPerson = FilterTable(ldtbAllMASPerson, busConstant.DataType.Numeric, "PERSON_ID",
                                                   aintPersonID);
            if (ldtbMASPerson.Rows.Count > 0)
            {
                //pir 7992 -- start
                //Added here for summary statement
                foreach (DataRow ldtr in ldtbMASPerson.Rows)
                {
                    //DO NOT DISPLAY RHIC if both member RHIC amount and Spouse RHIC amount is zero.
                    bool lblnRHICExists = ldtbMASPerson.AsEnumerable().Where(o =>
                         o.Field<decimal>("MEMBERRHICAMOUNT") > Convert.ToDecimal(0)
                                    || o.Field<decimal>("SPOUSERHICAMOUNT") > Convert.ToDecimal(0)
                                   ).Any();
                    if (lblnRHICExists)
                    {
                        ldtr["DISPLAY_RHIC"] = "Y";
                    }
                    else
                    {
                        ldtr["DISPLAY_RHIC"] = "N";
                    }
                }
                //end
                ldtbMASPerson.TableName = busConstant.ReportTableName;
                idsRetireeAnnualStatement.Tables.Add(ldtbMASPerson);

                ////Commented and made the formula change in report 
                //PIR 9790
                //DataTable ldtbMASPersonPlan = FilterTable(ldtbAllMASPersonPlan, busConstant.DataType.Numeric, "PERSON_ID",
                //                                      aintPersonID);
                //ldtbMASPersonPlan.TableName = busConstant.ReportTableName34;
                //idsRetireeAnnualStatement.Tables.Add(ldtbMASPersonPlan);

                //Filter Payee Accounts for the person ID
                DataTable ldtbMASPayeeAccount = FilterTable(ldtbPayeeAccount, busConstant.DataType.Numeric, "PERSON_ID",
                                          aintPersonID);

                if (ldtbMASPayeeAccount.Rows.Count > 1)
                {
                    ldtbMASPayeeAccount = (from c in ldtbMASPayeeAccount.AsEnumerable()
                                           orderby c.Field<int>("ACCOUNT_ORDER")
                                           select c).AsDataTable();
                }
                //if (ldtbMASPayeeAccount.Rows.Count > 0)
                //{
                    
                    //foreach (DataRow ldtr in ldtbMASPayeeAccount.Rows)
                    //{
                    //    //PIR 8090 - TO Display the statement “Current beneficiary not on file” 
                    //    //If there no Life plan beneficiary on person maintenance or beneficiary are ended and plan is enrolled.
                    //    //For Pension plan, when there is no beneficiary or beneficiary is ended and payee account
                    //    ldtr["IS_CURRENT_LIFE_BENEFICIARY_EXISTS"] = ldtbAllMASBenDependent.AsEnumerable().Where(o => o.Field<string>("IS_BENEFICIARY_FLAG") == "Y"
                    //                 && o.Field<int>("PLAN_ID") == busConstant.PlanIdGroupLife
                    //                 && o.Field<int>("PERSON_ID") == aintPersonID).Any() ? "Y" : "N";
                    //    bool isRetirementBeneficiaryExists = ldtbAllMASBenDependent.AsEnumerable().Where(o => o.Field<string>("IS_BENEFICIARY_FLAG") == "Y"
                    //                 && o.Field<int>("PERSON_ID") == aintPersonID
                    //                 && ldtbMASPayeeAccount.Rows[0]["PLAN_ID"] != DBNull.Value &&
                    //                o.Field<int>("PLAN_ID") == Convert.ToInt32(ldtbMASPayeeAccount.Rows[0]["PLAN_ID"])).Any() ;
                    //    busPerson lobjPerson = new busPerson { icdoPerson = new cdoPerson() };
                    //    DataTable ldtbPerson = Select<cdoPerson>(new string[1] { "person_id" }, new object[1] { aintPersonID}, null, null);
                    //    lobjPerson.icdoPerson.LoadData(ldtbPerson.Rows[0]);
                    //    //To set the beneficiary_required_flag_for_display -- NOTE: icdoPerson.BeneficiaryRequiredFlag != beneficiary_required_flag_for_display
                    //    lobjPerson.SetBeneficiaryRequiredFlag();

                    //    if (!isRetirementBeneficiaryExists && lobjPerson.beneficiary_required_flag_for_display == "N")
                    //        ldtr["IS_CURRENT_RETIREMENT_BENEFICIARY_EXISTS"] = "Y";
                    //    else
                    //        ldtr["IS_CURRENT_RETIREMENT_BENEFICIARY_EXISTS"] = "N";
                    //    //PIR 8651
                    //    //reevaluate IS_BENEFICIARY_EXIST FLAG to display Retirement Beneficiaries --- 
                    //    if (ldtr["IS_BENEFICIARY_EXIST"].ToString() == "Y" && lobjPerson.beneficiary_required_flag_for_display == "Y")
                    //    {
                    //        ldtr["IS_BENEFICIARY_EXIST"] = "Y";
                    //    }
                    //    else
                    //        ldtr["IS_BENEFICIARY_EXIST"] = "N";
                    //    //PIR 8649
                    //    // If the benefit option = Term Certain, 
                    //    //the RHIC option equals Standard RHIC and the Term Certain End Date has expired, display Spouse RHIC Amount as $0.00
                    //    if((ldtr["BENEFIT_OPTION_VALUE"].ToString() == busConstant.BenefitOption10YearCertain
                    //        || ldtr["BENEFIT_OPTION_VALUE"].ToString() == busConstant.BenefitOption15YearCertain
                    //        || ldtr["BENEFIT_OPTION_VALUE"].ToString() == busConstant.BenefitOption20YearCertain
                    //        )
                    //        &&
                    //        (ldtr["RHIC_BENEFIT_OPTION_VALUE"].ToString() == busConstant.RHICOptionStandard)
                    //        &&
                    //        (ldtr["TERM_CERTAIN_END_DATE"] != DBNull.Value)
                    //        &&
                    //        (Convert.ToDateTime( ldtr["TERM_CERTAIN_END_DATE"].ToString()) < adtEffectiveDate)
                    //        )
                    //    	{
                    //        	ldtr["SPOUSERHICAMOUNT"] = 0;
                    //    	}
                    //    //PIR 7992
                    //    //IF THE MEMBER RHIC AMOUNT AND SPOUSE RHIC AMOUNT = 0 - THEN DO NOT DISPLAY RHIC
                    //    //Added for RHIC display in Online Statement
                    //    bool lblnRHICExists = ldtbMASPayeeAccount.AsEnumerable().Where(o =>
                    //    o.Field<decimal>("MEMBERRHICAMOUNT") > Convert.ToDecimal(0)
                    //               || o.Field<decimal>("SPOUSERHICAMOUNT") > Convert.ToDecimal(0)
                    //              ).Any();
                    //    if (lblnRHICExists)
                    //    {
                    //        ldtr["DISPLAY_RHIC"] = "Y";
                    //    }
                    //    else
                    //    {
                    //        ldtr["DISPLAY_RHIC"] = "N";
                    //    }
                    //} 
                //}
              
                ldtbMASPayeeAccount.TableName = busConstant.ReportTableName05;
                idsRetireeAnnualStatement.Tables.Add(ldtbMASPayeeAccount);

              
                int lintPayeeAccountOwnerID = 0;
                int lintBenefitPlanID = 0;

                decimal ldecLifeBasicPremiumAmt = 0.00M;
                decimal ldecLifeSuppPremiumAmt = 0.00M;
                decimal ldecLifeSpouseSuppPremiumAmt = 0.00M;
                decimal ldecLifeDepSuppPremiumAmt = 0.00M;
                int lintPayeePERSLinkId = 0;
                if (ldtbMASPayeeAccount.Rows.Count > 0)
                {
                    if (ldtbMASPayeeAccount.Rows[0]["MEMBER_PERSON_ID"] != DBNull.Value
                        && Convert.ToInt32(ldtbMASPayeeAccount.Rows[0]["MEMBER_PERSON_ID"]) > 0)
                    {
                        lintPayeeAccountOwnerID = Convert.ToInt32(ldtbMASPayeeAccount.Rows[0]["MEMBER_PERSON_ID"]);
                    }
                    else
                    {
                        lintPayeeAccountOwnerID = aintPersonID;
                    }
                    if (ldtbMASPayeeAccount.Rows[0]["PAYEE_PERSLINK_ID"] != DBNull.Value
                        && Convert.ToInt32(ldtbMASPayeeAccount.Rows[0]["PAYEE_PERSLINK_ID"]) > 0)
                    {
                        lintPayeePERSLinkId = Convert.ToInt32(ldtbMASPayeeAccount.Rows[0]["PAYEE_PERSLINK_ID"]);
                    }
                    else
                    {
                        lintPayeePERSLinkId = aintPersonID;
                    }
                    if (ldtbMASPayeeAccount.Rows[0]["PLAN_ID"] != DBNull.Value
                           && Convert.ToInt32(ldtbMASPayeeAccount.Rows[0]["PLAN_ID"]) > 0)
                    {
                        lintBenefitPlanID = Convert.ToInt32(ldtbMASPayeeAccount.Rows[0]["PLAN_ID"]);
                    }
                    if (ldtbMASPayeeAccount.Rows[0]["benefit_account_type_value"] != DBNull.Value &&
                        ldtbMASPayeeAccount.Rows[0]["benefit_account_type_value"].ToString() == busConstant.ApplicationBenefitTypeDisability)
                    {
                        busPersonAccountRetirement lbusPersonAccountRetirement = new busPersonAccountRetirement
                        {
                            icdoPersonAccount = new cdoPersonAccount(),
                            icdoPersonAccountRetirement = new cdoPersonAccountRetirement()
                        };
                        lbusPersonAccountRetirement.FindPersonAccountRetirement(busPersonAccountHelper.GetPersonAccountID(lintBenefitPlanID, lintPayeeAccountOwnerID));
                        lbusPersonAccountRetirement.LoadLTDSummaryAsonDate(adtEffectiveDate);
                        //pir 8101
                        string lstrStartMonth, lstrEndMonth;
                        if (adtEffectiveDate.Month >= 7)
                        {
                            lstrStartMonth = Convert.ToString(adtEffectiveDate.Year) + "07";
                            lstrEndMonth = Convert.ToString(adtEffectiveDate.Year + 1) + "06";
                        }
                        else
                        {
                            lstrStartMonth = Convert.ToString(adtEffectiveDate.Year - 1) + "07";
                            lstrEndMonth = Convert.ToString(adtEffectiveDate.Year) + "06";
                        }
                      
                        lbusPersonAccountRetirement.LoadFYTDSummary(lstrStartMonth, lstrEndMonth);
                        ldtbMASPayeeAccount.Rows[0]["ACCOUNT_BALANCE"] = lbusPersonAccountRetirement.Member_Account_Balance_ltd;
                        ldtbMASPayeeAccount.Rows[0]["Previous_Statement_Balance"] =
                            lbusPersonAccountRetirement.Member_Account_Balance_ltd - lbusPersonAccountRetirement.Member_Account_Balance;
                        ldtbMASPayeeAccount.Rows[0]["Interest_Amount"] = lbusPersonAccountRetirement.interest_amount;

                        ldtbMASPerson.Rows[0]["ACCOUNT_BALANCE"] = lbusPersonAccountRetirement.Member_Account_Balance_ltd;
                    }

                    string lstrCoverageCode = string.Empty;

                    decimal ldecRHICAmt = 0.0m, ldecGHDVPremiumAmt = 0.00m, ldecLifePremiumAmt = 0.0m, ldecLTCPremiumAmt = 0.0m, ldecMedicarePremiumAmt = 0.0m;
                    if ((ldtbMASPayeeAccount.Rows[0]["IS_HEALTH_ENROLLED"] != DBNull.Value
                        && ldtbMASPayeeAccount.Rows[0]["IS_HEALTH_ENROLLED"].ToString() == busConstant.Flag_Yes)
                        && aintPersonID > 0)
                    {
                        //GetGHDVPremium(aintPersonID, ref ldecRHICAmt, ref ldecHealthPremiumAmt, ref ldecVisionPremiumAmt, ref ldecDentalPremiumAmt, "H", ref lstrCoverageCode);
                        GetTotalPremiumAmount(aintPersonID, idtEffectiveDate, ref ldecGHDVPremiumAmt, "H");
                        ldtbMASPayeeAccount.Rows[0]["HealthPremiumAmount"] = ldecGHDVPremiumAmt;
                        ldtbMASPayeeAccount.Rows[0]["HealthRHICAmount"] = ldecRHICAmt;
                        ldtbMASPerson.Rows[0]["HEALTH_PREMIUM_AMT"] = busGlobalFunctions.RoundToTwoDecimalPoints(ldecGHDVPremiumAmt); //PIR 10818
                    }

                    //PIR 16868
                    if (ldtbMASPayeeAccount.Rows[0]["IS_MEDICARE_PART_D_ENROLLED"] != DBNull.Value &&
                        ldtbMASPayeeAccount.Rows[0]["IS_MEDICARE_PART_D_ENROLLED"].ToString() == busConstant.Flag_Yes &&
                        aintPersonID > 0)
                    {
                        GetTotalPremiumAmountMedicare(aintPersonID, idtEffectiveDate, ref ldecMedicarePremiumAmt);
                        ldtbMASPayeeAccount.Rows[0]["MedicarePremiumAmount"] = ldecMedicarePremiumAmt;
                        ldtbMASPerson.Rows[0]["MEDICARE_PREMIUM_AMT"] = busGlobalFunctions.RoundToTwoDecimalPoints(ldecMedicarePremiumAmt);
                    }


                    if ((ldtbMASPayeeAccount.Rows[0]["IS_VISION_ENROLLED"] != DBNull.Value
                        && ldtbMASPayeeAccount.Rows[0]["IS_VISION_ENROLLED"].ToString() == busConstant.Flag_Yes) && aintPersonID > 0)
                    {
                        //GetGHDVPremium(aintPersonID, ref ldecRHICAmt, ref ldecHealthPremiumAmt, ref ldecVisionPremiumAmt, ref ldecDentalPremiumAmt, "V", ref lstrCoverageCode);
                        GetTotalPremiumAmount(aintPersonID, idtEffectiveDate, ref ldecGHDVPremiumAmt, "V");
                        ldtbMASPayeeAccount.Rows[0]["VisionPremiumAmount"] = ldecGHDVPremiumAmt;
                        ldtbMASPerson.Rows[0]["VISION_PREMIUM_AMT"] = busGlobalFunctions.RoundToTwoDecimalPoints(ldecGHDVPremiumAmt); //PIR 10818
                    }
                    if (ldtbMASPayeeAccount.Rows[0]["IS_DENTAL_ENROLLED"] != DBNull.Value
                        && ldtbMASPayeeAccount.Rows[0]["IS_DENTAL_ENROLLED"].ToString() == busConstant.Flag_Yes && lintPayeePERSLinkId > 0)
                    {
                        //GetGHDVPremium(aintPersonID, ref ldecRHICAmt, ref ldecHealthPremiumAmt, ref ldecVisionPremiumAmt, ref ldecDentalPremiumAmt, "D", ref lstrCoverageCode);
                        GetTotalPremiumAmount(aintPersonID, idtEffectiveDate, ref ldecGHDVPremiumAmt, "D");
                        ldtbMASPayeeAccount.Rows[0]["DetntalPremiumAmount"] = ldecGHDVPremiumAmt;
                        ldtbMASPerson.Rows[0]["DENTAL_PREMIUM_AMT"] = busGlobalFunctions.RoundToTwoDecimalPoints(ldecGHDVPremiumAmt); //PIR 10818
                    }
                    if (ldtbMASPayeeAccount.Rows[0]["IS_LIFE_ENROLLED"] != null
                          && ldtbMASPayeeAccount.Rows[0]["IS_LIFE_ENROLLED"].ToString() == busConstant.Flag_Yes
                          && lintPayeePERSLinkId > 0)
                    {
                        var lobjLife = new busPersonAccountLife { icdoPersonAccountLife = new cdoPersonAccountLife() };
                        lobjLife.FindPersonAccount(busPersonAccountHelper.GetPersonAccountID(busConstant.PlanIdGroupLife, aintPersonID));
                        lobjLife.FindPersonAccountLife(busPersonAccountHelper.GetPersonAccountID(busConstant.PlanIdGroupLife, aintPersonID));
                        lobjLife.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                        lobjLife.ibusPerson.FindPerson(lobjLife.icdoPersonAccount.person_id);
                        lobjLife.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
                        lobjLife.ibusPlan.FindPlan(lobjLife.icdoPersonAccount.plan_id);

                        if (lobjLife.icdoPersonAccountLife.premium_waiver_flag == busConstant.Flag_Yes)
                        {
                            ldtbMASPayeeAccount.Rows[0]["IsLifePremiumwaiver"] = busConstant.Flag_Yes;
                            ldtbMASPayeeAccount.Rows[0]["WaivedAmount"] = lobjLife.icdoPersonAccountLife.waived_amount;
                        }
                        if ((idtbPALifeOptionHistory != null) && (idtbPALifeOptionHistory.Rows.Count > 0))
                        {
                            DataRow[] larrRow = idtbPALifeOptionHistory.FilterTable(busConstant.DataType.Numeric, "person_account_id",
                                                                             lobjLife.icdoPersonAccount.person_account_id);

                            //Loading the Life Option Data
                            lobjLife.LoadLifeOptionDataFromHistory(larrRow);
                        }

                        //Get the Provider Org ID from History
                        busPersonAccountLifeHistory lobjPALifeHistory = new busPersonAccountLifeHistory();
                        lobjPALifeHistory.icdoPersonAccountLifeHistory = new cdoPersonAccountLifeHistory();
                        foreach (busPersonAccountLifeOption lobjPALifeOption in lobjLife.iclbLifeOption)
                        {
                            if (lobjPALifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Basic)
                            {
                                lobjPALifeHistory = lobjLife.LoadHistoryByDate(lobjPALifeOption, adtEffectiveDate);
                                break;
                            }
                        }

                        lobjLife.LoadActiveProviderOrgPlan(adtEffectiveDate);

                        //Loading the History Object
                        if ((idtbLifeHistory != null) && (idtbLifeHistory.Rows.Count > 0))
                        {
                            DataRow[] larrRow = idtbLifeHistory.FilterTable(busConstant.DataType.Numeric, "person_account_id",
                                                                            lobjLife.icdoPersonAccount.person_account_id);
                            lobjLife.iclbPersonAccountLifeHistory =
                                lbusBase.GetCollection<busPersonAccountLifeHistory>(larrRow, "icdoPersonAccountLifeHistory");
                        }

                        lobjLife.LoadMemberAge(adtEffectiveDate);
                        lobjLife.GetMonthlyPremiumAmount(adtEffectiveDate);


                        ldecLifeBasicPremiumAmt = lobjLife.idecLifeBasicPremiumAmt;
                        ldecLifeSuppPremiumAmt = lobjLife.idecLifeSupplementalPremiumAmount;
                        ldecLifeSpouseSuppPremiumAmt = lobjLife.idecSpouseSupplementalPremiumAmt;
                        ldecLifeDepSuppPremiumAmt = lobjLife.idecDependentSupplementalPremiumAmt;

                        GetTotalPremiumAmountLife(aintPersonID, idtEffectiveDate, ref ldecLifePremiumAmt);
                        //ldtbMASPerson.Rows[0]["Life_PREMIUM_AMT"] = busGlobalFunctions.RoundToTwoDecimalPoints(ldecLifeBasicPremiumAmt + ldecLifeSuppPremiumAmt + ldecLifeSpouseSuppPremiumAmt + ldecLifeDepSuppPremiumAmt); //PIR 10818
                        ldtbMASPerson.Rows[0]["Life_PREMIUM_AMT"] = busGlobalFunctions.RoundToTwoDecimalPoints(ldecLifePremiumAmt);
                    }

                    if (ldtbMASPayeeAccount.Rows[0]["IS_LTC_ENROLLED"] != DBNull.Value
                       && ldtbMASPayeeAccount.Rows[0]["IS_LTC_ENROLLED"].ToString() == busConstant.Flag_Yes
                       && lintPayeePERSLinkId > 0)
                    {
                        //idsRetireeAnnualStatement.Tables.Add(GetLTCPremium(aintPersonID,
                        //    adtEffectiveDate));
                        //ldtbMASPerson.Rows[0]["LTC_PREMIUM_AMT"] = busGlobalFunctions.RoundToTwoDecimalPoints(Convert.ToDecimal(idsRetireeAnnualStatement.Tables["ReportTable15"].Rows[0]["LTCPremium"])); //PIR 10818
                        
                        GetTotalPremiumAmountLTC(aintPersonID, idtEffectiveDate, ref ldecLTCPremiumAmt);
                        ldtbMASPerson.Rows[0]["LTC_PREMIUM_AMT"] = busGlobalFunctions.RoundToTwoDecimalPoints(ldecLTCPremiumAmt);
                    }
                }
                DataTable ldtbMASBenefitOption = new DataTable();
                if (ldtbBenefitOption.Rows.Count > 0)
                {
                    ldtbMASBenefitOption = FilterTable(ldtbBenefitOption, busConstant.DataType.Numeric, "PERSON_ID",
                                               aintPersonID);
                    ldtbMASBenefitOption.TableName = busConstant.ReportTableName02;
                    idsRetireeAnnualStatement.Tables.Add(ldtbMASBenefitOption);
                }
                if (ldtbPapit.Rows.Count > 0)
                {
                    DataTable ldtbMASPapitperson = FilterTable(ldtbPapit, busConstant.DataType.Numeric, "PERSON_ID",
                                               aintPersonID);
					//PIR 8590
                    ldtbMASPapitperson = ldtbMASPapitperson.AsEnumerable().Where(o => o.Field<string>("PLAN_NAME") != "Defined Contribution Retirement").AsDataTable();
                    ldtbMASPapitperson.TableName = busConstant.ReportTableName03;
                    idsRetireeAnnualStatement.Tables.Add(ldtbMASPapitperson);
                }
                //PIR-20544 Retiree Annual Statement – Need to a Beneficiary section to the report. 
                DataTable ldtbMASBeneficiaryHealthDep = ldtbAllMASBenDependent.AsEnumerable().Where(o => o.Field<string>("IS_BENEFICIARY_FLAG") == "Y"
                  && o.Field<int>("PERSON_ID") == aintPersonID).AsDataTable();
                if (ldtbMASBeneficiaryHealthDep.Rows.Count == 0)
                    ldtbMASBeneficiaryHealthDep = ldtbAllMASBenDependent.Clone();
                ldtbMASBeneficiaryHealthDep.TableName = busConstant.ReportTableName11;
                idsRetireeAnnualStatement.Tables.Add(ldtbMASBeneficiaryHealthDep.Copy());
                //if (ldtbPapitItems.Rows.Count > 0)
                //{
                //    DataTable ldtbMASPapitItems = FilterTable(ldtbPapitItems, busConstant.DataType.Numeric, "PERSON_ID",
                //                               aintPersonID);
                //    //PIR-8051
                //   // ldtbMASPapitItems = RemoveCOLAforJobService(ldtbMASPapitItems);
                //    ldtbMASPapitItems.TableName = busConstant.ReportTableName06;
                //    idsRetireeAnnualStatement.Tables.Add(ldtbMASPapitItems);
                //}
                //Load Retirement beneficiary
                //DataTable ldtbMASBeneficiary = ldtbAllMASBenDependent.AsEnumerable().Where(o => o.Field<string>("IS_BENEFICIARY_FLAG") == "Y"
                //                     && o.Field<int>("PERSON_ID") == aintPersonID
                //                     && ldtbMASPayeeAccount.Rows[0]["PLAN_ID"] != DBNull.Value &&
                //                     o.Field<int>("PLAN_ID") == Convert.ToInt32(ldtbMASPayeeAccount.Rows[0]["PLAN_ID"])).AsDataTable();
                ////PIR 8090
                //if (ldtbMASBeneficiary.Rows.Count > 0)
                //    foreach (DataRow ldtr in ldtbMASBeneficiary.Rows)
                //    {
                //        if (ldtr["BENEFICIARY_FULL_NAME"] != null)
                //        {
                //            ldtr["BENEFICIARY_FULL_NAME"] = ldtr["BENEFICIARY_FULL_NAME"].ToString().ToUpper();
                //        }
                //    }

                //if (ldtbMASBeneficiary.Rows.Count == 0)
                //    ldtbMASBeneficiary = ldtbAllMASBenDependent.Clone();
                //ldtbMASBeneficiary.TableName = busConstant.ReportTableName07;
                //idsRetireeAnnualStatement.Tables.Add(ldtbMASBeneficiary.Copy());

                //DataTable ldtbMASLifeOptions = FilterTable(ldtbAllMASLifeOptions, busConstant.DataType.Numeric, "PERSON_ID",
                //                            aintPersonID);
                //ldtbMASLifeOptions.TableName = busConstant.ReportTableName08;
                //idsRetireeAnnualStatement.Tables.Add(ldtbMASLifeOptions);
                //if (ldtbMASPayeeAccount.Rows.Count > 0)
                //{
                //    foreach (DataRow ldtr in ldtbMASLifeOptions.Rows)
                //    {
                //        if (ldtr["LEVEL_OF_COVERAGE_VALUE"] != null && ldtr["LEVEL_OF_COVERAGE_VALUE"].ToString() == busConstant.LevelofCoverage_Basic)
                //        {
                //            ldtr["PREMIUM_AMOUNT"] = ldecLifeBasicPremiumAmt;
                //        }
                //        else if (ldtr["LEVEL_OF_COVERAGE_VALUE"] != null && ldtr["LEVEL_OF_COVERAGE_VALUE"].ToString() == busConstant.LevelofCoverage_Supplemental)
                //        {
                //            ldtr["PREMIUM_AMOUNT"] = ldecLifeSuppPremiumAmt;
                //            ldtbMASPayeeAccount.Rows[0]["IsLifeCoverageValid"] = busConstant.Flag_Yes;
                //        }
                //        else if (ldtr["LEVEL_OF_COVERAGE_VALUE"] != null && ldtr["LEVEL_OF_COVERAGE_VALUE"].ToString() == busConstant.LevelofCoverage_SpouseSupplemental)
                //        {
                //            ldtbMASPayeeAccount.Rows[0]["IsLifeCoverageValid"] = busConstant.Flag_Yes;
                //            ldtr["PREMIUM_AMOUNT"] = ldecLifeSpouseSuppPremiumAmt;
                //        }
                //        else if (ldtr["LEVEL_OF_COVERAGE_VALUE"] != null && ldtr["LEVEL_OF_COVERAGE_VALUE"].ToString() == busConstant.LevelofCoverage_DependentSupplemental)
                //        {
                //            ldtr["PREMIUM_AMOUNT"] = ldecLifeDepSuppPremiumAmt;
                //            ldtbMASPayeeAccount.Rows[0]["IsLifeCoverageValid"] = busConstant.Flag_Yes;
                //        }
                //    }
                //}

                //DataTable ldtbMASBeneficiaryPartD = ldtbAllMASBenDependent.AsEnumerable().Where(o =>
                //    o.Field<string>("IS_BENEFICIARY_FLAG") == "N"
                //     && o.Field<int>("PLAN_ID") == busConstant.PlanIdMedicarePartD
                //     && o.Field<int>("PERSON_ID") == aintPersonID).AsDataTable();
                //if (ldtbMASBeneficiaryPartD.Rows.Count > 0)
                //    ldtbMASPayeeAccount.Rows[0]["MedicareDep"] = busConstant.Flag_Yes;
                //if (ldtbMASBeneficiaryPartD.Rows.Count == 0)
                //    ldtbMASBeneficiaryPartD = ldtbAllMASBenDependent.Clone();
                //ldtbMASBeneficiaryPartD.TableName = busConstant.ReportTableName09;
                //idsRetireeAnnualStatement.Tables.Add(ldtbMASBeneficiaryPartD.Copy());

                //DataTable ldtbMASBeneficiaryHealthDep = ldtbAllMASBenDependent.AsEnumerable().Where(o => o.Field<string>("IS_BENEFICIARY_FLAG") == "N"
                //     && o.Field<int>("PLAN_ID") == busConstant.PlanIdGroupHealth
                //     && o.Field<int>("PERSON_ID") == aintPersonID).AsDataTable();

                //if (ldtbMASBeneficiaryHealthDep.Rows.Count == 0)
                //    ldtbMASBeneficiaryHealthDep = ldtbAllMASBenDependent.Clone();
                //if (ldtbMASBeneficiaryHealthDep.Rows.Count > 0)
                //    ldtbMASPayeeAccount.Rows[0]["Healthdep"] = busConstant.Flag_Yes;
                //ldtbMASBeneficiaryHealthDep.TableName = busConstant.ReportTableName11;
                //idsRetireeAnnualStatement.Tables.Add(ldtbMASBeneficiaryHealthDep.Copy());

                //DataTable ldtbMASBeneficiaryLifeBen = ldtbAllMASBenDependent.AsEnumerable().Where(o => o.Field<string>("IS_BENEFICIARY_FLAG") == "Y"
                //                     && o.Field<int>("PLAN_ID") == busConstant.PlanIdGroupLife
                //                     && o.Field<int>("PERSON_ID") == aintPersonID).AsDataTable();
                ////PIR 8090
                //if (ldtbMASBeneficiaryLifeBen.Rows.Count > 0)
                //    foreach (DataRow ldtr in ldtbMASBeneficiaryLifeBen.Rows)
                //    {
                //        if (ldtr["BENEFICIARY_FULL_NAME"] != null )
                //        {
                //            ldtr["BENEFICIARY_FULL_NAME"] = ldtr["BENEFICIARY_FULL_NAME"].ToString().ToUpper();
                //        }
                //    }

                //if (ldtbMASBeneficiaryLifeBen.Rows.Count == 0)
                //    ldtbMASBeneficiaryLifeBen = ldtbAllMASBenDependent.Clone();
                //ldtbMASBeneficiaryLifeBen.TableName = busConstant.ReportTableName12;
                //idsRetireeAnnualStatement.Tables.Add(ldtbMASBeneficiaryLifeBen.Copy());

                //DataTable ldtbMASBeneficiaryDentalDep = ldtbAllMASBenDependent.AsEnumerable().Where(o => o.Field<string>("IS_BENEFICIARY_FLAG") == "N"
                //                     && o.Field<int>("PLAN_ID") == busConstant.PlanIdDental
                //                     && o.Field<int>("PERSON_ID") == aintPersonID
                //                    ).AsDataTable();
                //if (ldtbMASBeneficiaryDentalDep.Rows.Count == 0)
                //    ldtbMASBeneficiaryDentalDep = ldtbAllMASBenDependent.Clone();
                //if (ldtbMASBeneficiaryDentalDep.Rows.Count > 0)
                //    ldtbMASPayeeAccount.Rows[0]["DentalDep"] = busConstant.Flag_Yes;

                //ldtbMASBeneficiaryDentalDep.TableName = busConstant.ReportTableName13;
                //idsRetireeAnnualStatement.Tables.Add(ldtbMASBeneficiaryDentalDep.Copy());

                //DataTable ldtbMASBeneficiaryVisionDep = ldtbAllMASBenDependent.AsEnumerable().Where(o => o.Field<string>("IS_BENEFICIARY_FLAG") == "N"
                //                                     && o.Field<int>("PLAN_ID") == busConstant.PlanIdVision
                //                                     && o.Field<int>("PERSON_ID") == aintPersonID
                //                                    ).AsDataTable();

                //if (ldtbMASBeneficiaryVisionDep.Rows.Count > 0)
                //    ldtbMASPayeeAccount.Rows[0]["VisionDep"] = busConstant.Flag_Yes;

                //if (ldtbMASBeneficiaryVisionDep.Rows.Count == 0)
                //    ldtbMASBeneficiaryVisionDep = ldtbAllMASBenDependent.Clone();
                //ldtbMASBeneficiaryVisionDep.TableName = busConstant.ReportTableName14;
                //idsRetireeAnnualStatement.Tables.Add(ldtbMASBeneficiaryVisionDep.Copy());

                ////PIR 8090 - To load current Payment Details Cross Tab in Online statement
                //DataTable ldtbMASGrossPaidLifeToDate = new DataTable();   
                // ldtbMASGrossPaidLifeToDate =   busBase.Select("cdoMasPayeeAccount.LoadGrossAmountByPlan",
                //                          new object[2] { aintPersonID, adtEffectiveDate });
                //ldtbMASGrossPaidLifeToDate.TableName = busConstant.ReportTableName16;
                //ldtbMASGrossPaidLifeToDate.DataSet.Tables.RemoveAt(0);
                //idsRetireeAnnualStatement.Tables.Add(ldtbMASGrossPaidLifeToDate);

                //PIR 7992
                //DataTable ldtbMASRHICInfo = ldtbMASPayeeAccount.AsEnumerable().Where(o =>
                //    o.Field<decimal>("MEMBERRHICAMOUNT") > Convert.ToDecimal(0)
                //                     || o.Field<decimal>("SPOUSERHICAMOUNT") > Convert.ToDecimal(0)
                //                    ).AsDataTable();
                //if (ldtbMASRHICInfo.Rows.Count == 0)
                //    ldtbMASRHICInfo = ldtbMASPayeeAccount.Clone();
                //ldtbMASRHICInfo.TableName = busConstant.ReportTableName17;
                //idsRetireeAnnualStatement.Tables.Add(ldtbMASRHICInfo);


            }
            return idsRetireeAnnualStatement;
        }

        private static void LoadLifeOptionPremium(DataTable ldtbMASPayeeAccount, decimal ldecLifeBasicPremiumAmt, decimal ldecLifeSuppPremiumAmt, decimal ldecLifeSpouseSuppPremiumAmt, decimal ldecLifeDepSuppPremiumAmt, DataTable ldtbMASLifeOptions)
        {
            if (ldtbMASPayeeAccount.Rows.Count > 0)
            {
                foreach (DataRow ldtr in ldtbMASLifeOptions.Rows)
                {
                    if (ldtr["LEVEL_OF_COVERAGE_VALUE"] != DBNull.Value && ldtr["LEVEL_OF_COVERAGE_VALUE"].ToString() == busConstant.LevelofCoverage_Basic)
                    {
                        ldtr["PREMIUM_AMOUNT"] = ldecLifeBasicPremiumAmt;
                        ldtbMASPayeeAccount.Rows[0]["IsLifeCoverageValid"] = busConstant.Flag_Yes;
                    }
                    else if (ldtr["LEVEL_OF_COVERAGE_VALUE"] != DBNull.Value && ldtr["LEVEL_OF_COVERAGE_VALUE"].ToString() == busConstant.LevelofCoverage_Supplemental)
                    {
                        ldtr["PREMIUM_AMOUNT"] = ldecLifeSuppPremiumAmt;
                        ldtbMASPayeeAccount.Rows[0]["IsLifeCoverageValid"] = busConstant.Flag_Yes;
                    }
                    else if (ldtr["LEVEL_OF_COVERAGE_VALUE"] != DBNull.Value && ldtr["LEVEL_OF_COVERAGE_VALUE"].ToString() == busConstant.LevelofCoverage_SpouseSupplemental)
                    {
                        ldtbMASPayeeAccount.Rows[0]["IsLifeCoverageValid"] = busConstant.Flag_Yes;
                        ldtr["PREMIUM_AMOUNT"] = ldecLifeSpouseSuppPremiumAmt;
                    }
                    else if (ldtr["LEVEL_OF_COVERAGE_VALUE"] != DBNull.Value && ldtr["LEVEL_OF_COVERAGE_VALUE"].ToString() == busConstant.LevelofCoverage_DependentSupplemental)
                    {
                        ldtr["PREMIUM_AMOUNT"] = ldecLifeDepSuppPremiumAmt;
                    }
                }
            }
        }

        public void GetGHDVPremium(int lintPayeeAccountOwnerID, ref decimal ldecRHICAmt,
            ref decimal ldecHealthPremiumAmt, ref decimal ldecVisonPremium, ref decimal ldecDentalPremium, string lstrGHDVIndicator, ref string astrCoverageCode)
        {
            decimal ldecGroupHealthFeeAmt = 0.00M;
            ldecRHICAmt = 0.00M;
            decimal ldecPremiumAmt = 0.00M;
            ldecHealthPremiumAmt = 0.00M;
            decimal ldecProviderPremiumAmt = 0.00M;
            decimal ldecMemberPremiumAmt = 0.00M;
            decimal ldecBuydownAmt = 0.00M;

            var lobjGhdv = new busPersonAccountGhdv { icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };
            if (lstrGHDVIndicator == "H")
            {
                int lintPersonAccountID = busPersonAccountHelper.GetPersonAccountID(busConstant.PlanIdGroupHealth, lintPayeeAccountOwnerID);
                lobjGhdv.FindPersonAccount(lintPersonAccountID);
                lobjGhdv.FindGHDVByPersonAccountID(lintPersonAccountID);
            }
            else if (lstrGHDVIndicator == "D")
            {
                int lintPersonAccountId = busPersonAccountHelper.GetPersonAccountID(busConstant.PlanIdDental, lintPayeeAccountOwnerID);
                lobjGhdv.FindPersonAccount(lintPersonAccountId);
                lobjGhdv.FindGHDVByPersonAccountID(lintPersonAccountId);
            }
            else if (lstrGHDVIndicator == "V")
            {
                int lintPersonAccountId = busPersonAccountHelper.GetPersonAccountID(busConstant.PlanIdVision, lintPayeeAccountOwnerID);
                lobjGhdv.FindPersonAccount(lintPersonAccountId);
                lobjGhdv.FindGHDVByPersonAccountID(lintPersonAccountId);
            }
            lobjGhdv.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lobjGhdv.ibusPerson.FindPerson(lobjGhdv.icdoPersonAccount.person_id);
            lobjGhdv.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
            lobjGhdv.ibusPlan.FindPlan(lobjGhdv.icdoPersonAccount.plan_id);

            lobjGhdv.idtbCachedCoverageRef = ibusDBCacheData.idtbCachedCoverageRef;
            lobjGhdv.idtbCachedDentalRate = ibusDBCacheData.idtbCachedDentalRate;
            lobjGhdv.idtbCachedHealthRate = ibusDBCacheData.idtbCachedHealthRate;
            lobjGhdv.idtbCachedHmoRate = ibusDBCacheData.idtbCachedHMORate;
            lobjGhdv.idtbCachedRateRef = ibusDBCacheData.idtbCachedRateRef;
            lobjGhdv.idtbCachedRateStructureRef = ibusDBCacheData.idtbCachedRateStructureRef;
            lobjGhdv.idtbCachedVisionRate = ibusDBCacheData.idtbCachedVisionRate;

            //Loading the History Object                
            if ((idtbGHDVHistory != null) && (idtbGHDVHistory.Rows.Count > 0))
            {
                DataRow[] larrRow = idtbGHDVHistory.FilterTable(busConstant.DataType.Numeric,
                                                                "person_account_ghdv_id",
                                                                lobjGhdv.icdoPersonAccountGhdv.person_account_ghdv_id);

                lobjGhdv.iclbPersonAccountGHDVHistory =
                    lbusBase.GetCollection<busPersonAccountGhdvHistory>(larrRow, "icdoPersonAccountGhdvHistory");
            }

            //Look the Provider Org ID first , If null, get the default provider
            //Mail From Satya Dated On 1/29/2009

            //Get the GHDV History Object By Billing Month Year
            busPersonAccountGhdvHistory lobjPAGhdvHistory = lobjGhdv.LoadHistoryByDate(idtEffectiveDate);
            if (iclbProviderOrgPlan != null)
            {
                busOrgPlan lbusProviderOrgPlan = iclbProviderOrgPlan.FirstOrDefault(i => i.icdoOrgPlan.plan_id == lobjGhdv.icdoPersonAccount.plan_id);
                if (lbusProviderOrgPlan != null)
                {
                    lobjGhdv.ibusProviderOrgPlan = lbusProviderOrgPlan;
                }
                else
                {
                    lobjGhdv.LoadActiveProviderOrgPlan(idtEffectiveDate);
                }
            }
            else
            {
                lobjGhdv.LoadActiveProviderOrgPlan(idtEffectiveDate);
            }

            if (IsHealthOrMedicare(lobjGhdv.icdoPersonAccount.plan_id))
            {
                lobjGhdv = lobjPAGhdvHistory.LoadGHDVObject(lobjGhdv);
                if (lobjGhdv.ibusPerson == null)
                    lobjGhdv.LoadPerson();

                if (lobjGhdv.ibusPlan == null)
                    lobjGhdv.LoadPlan();
                //Initialize the Org Object to Avoid the NULL error
                lobjGhdv.InitializeObjects();
                lobjGhdv.idtPlanEffectiveDate = idtEffectiveDate;

                if (lobjGhdv.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                {
                    lobjGhdv.LoadRateStructureForUserStructureCode();
                }
                else
                {
                    lobjGhdv.LoadHealthParticipationDate();
                    //To Get the Rate Structure Code (Derived Field)
                    lobjGhdv.LoadRateStructure(idtEffectiveDate);
                }

                //Get the Coverage Ref ID
                lobjGhdv.LoadCoverageRefID();

                //Get the Premium Amount
                lobjGhdv.GetMonthlyPremiumAmountByRefID(idtEffectiveDate);


                astrCoverageCode =
                    GetGroupHealthCoverageCodeDescription(lobjGhdv.icdoPersonAccountGhdv.Coverage_Ref_ID);

                ldecPremiumAmt = lobjGhdv.icdoPersonAccountGhdv.PremiumExcludingFeeAmount;
                ldecGroupHealthFeeAmt = lobjGhdv.icdoPersonAccountGhdv.FeeAmount;

                ldecRHICAmt = lobjGhdv.icdoPersonAccountGhdv.total_rhic_amount;
                ldecBuydownAmt = lobjGhdv.icdoPersonAccountGhdv.BuydownAmount;
                ldecMemberPremiumAmt = ldecPremiumAmt + ldecGroupHealthFeeAmt - ldecRHICAmt - ldecBuydownAmt;
                ldecHealthPremiumAmt = lobjGhdv.icdoPersonAccountGhdv.MonthlyPremiumAmount ;//ldecPremiumAmt + ldecGroupHealthFeeAmt;
                ldecProviderPremiumAmt = ldecPremiumAmt;
            }
            else if (lobjGhdv.icdoPersonAccount.plan_id == busConstant.PlanIdDental)
            {
                ldecPremiumAmt =
                    busRateHelper.GetDentalPremiumAmount(
                        lobjGhdv.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                        lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.dental_insurance_type_value,
                        lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value,
                        idtEffectiveDate,
                        ibusDBCacheData.idtbCachedDentalRate, iobjPassInfo);

                ldecDentalPremium = ldecPremiumAmt;
                astrCoverageCode = lobjGhdv.icdoPersonAccountGhdv.level_of_coverage_description;
            }
            else if (lobjGhdv.icdoPersonAccount.plan_id == busConstant.PlanIdVision)
            {
                ldecPremiumAmt =
                    busRateHelper.GetVisionPremiumAmount(
                        lobjGhdv.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                        lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.vision_insurance_type_value,
                        lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value,
                        idtEffectiveDate,
                        ibusDBCacheData.idtbCachedVisionRate, iobjPassInfo);

                ldecVisonPremium = ldecPremiumAmt;
                astrCoverageCode = lobjGhdv.icdoPersonAccountGhdv.level_of_coverage_description;
            }
        }
        public DataTable GetLTCPremium(int aintPersonid, DateTime adtEffectiveDate)
        {
            busPersonAccountLtc lobjLtc = new busPersonAccountLtc();
            lobjLtc.FindPersonAccount(busPersonAccountHelper.GetPersonAccountID(busConstant.PlanIdLTC, aintPersonid));
            lobjLtc.LoadLtcOptionUpdateMemberFromHistory(adtEffectiveDate);
            lobjLtc.LoadLtcOptionUpdateSpouseFromHistory(adtEffectiveDate);
            lobjLtc.idtbCachedLtcRate = ibusDBCacheData.idtbCachedLtcRate;

            busPersonAccountLtcOptionHistory lobjPALtcHistory = new busPersonAccountLtcOptionHistory();
            lobjPALtcHistory.icdoPersonAccountLtcOptionHistory = new cdoPersonAccountLtcOptionHistory();
            //Load the Provider Org ID by History
            if (lobjLtc.iclbLtcOptionMember.Count > 0)
            {
                lobjPALtcHistory = lobjLtc.LoadHistoryByDate(lobjLtc.iclbLtcOptionMember[0], DateTime.Today);
            }

            lobjLtc.LoadActiveProviderOrgPlan(idtEffectiveDate);

            lobjLtc.GetMonthlyPremiumAmount(idtEffectiveDate);
            decimal ldecMemberPremiumAmt = 0.00M;
            decimal ldecMember3YrsPremium = 0.00M;
            decimal ldecMember5YrsPremium = 0.00M;
            decimal ldecSpouse3YrsPremium = 0.00M;
            decimal ldecSpouse5YrsPremium = 0.00M;
            ldecMemberPremiumAmt = lobjLtc.idecTotalMonthlyPremium;
            DataTable ldtbLTC = new DataTable();
            DataColumn ldc1 = new DataColumn("Coverage", Type.GetType("System.String"));
            DataColumn ldc2 = new DataColumn("LTCPremium", Type.GetType("System.Decimal"));
            ldtbLTC.Columns.Add(ldc1);
            ldtbLTC.Columns.Add(ldc2);
            ldtbLTC.TableName = busConstant.ReportTableName15;
            foreach (var lbusLtcOption in lobjLtc.iclbLtcOptionMember)
            {
                if (lbusLtcOption.icdoPersonAccountLtcOption.effective_start_date != DateTime.MinValue &&
                    busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate, lbusLtcOption.icdoPersonAccountLtcOption.effective_start_date, lbusLtcOption.icdoPersonAccountLtcOption.effective_end_date))
                {
                    DataRow dr = ldtbLTC.NewRow();
                    if (lbusLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value == busConstant.LTCLevelOfCoverage3YRS)
                    {
                        ldecMember3YrsPremium = lbusLtcOption.idecMonthlyPremium;
                        dr["Coverage"] = lbusLtcOption.icdoPersonAccountLtcOption.level_of_coverage_description;
                        dr["LTCPremium"] = ldecMember3YrsPremium;
                    }
                    else if (lbusLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value == busConstant.LTCLevelOfCoverage5YRS)
                    {
                        ldecMember5YrsPremium = lbusLtcOption.idecMonthlyPremium;
                        dr["Coverage"] = lbusLtcOption.icdoPersonAccountLtcOption.level_of_coverage_description;
                        dr["LTCPremium"] = ldecMember5YrsPremium;
                    }
                    ldtbLTC.Rows.Add(dr);
                }
            }

            foreach (var lbusLtcOption in lobjLtc.iclbLtcOptionSpouse)
            {
                if (lbusLtcOption.icdoPersonAccountLtcOption.effective_start_date != DateTime.MinValue &&
                    busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate, lbusLtcOption.icdoPersonAccountLtcOption.effective_start_date, lbusLtcOption.icdoPersonAccountLtcOption.effective_end_date))
                {
                    DataRow dr = ldtbLTC.NewRow();
                    if (lbusLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value == busConstant.LTCLevelOfCoverage3YRS)
                    {
                        ldecSpouse3YrsPremium = lbusLtcOption.idecMonthlyPremium;
                        dr["Coverage"] = lbusLtcOption.icdoPersonAccountLtcOption.level_of_coverage_description;
                        dr["LTCPremium"] = ldecSpouse3YrsPremium;
                    }
                    else if (lbusLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value == busConstant.LTCLevelOfCoverage5YRS)
                    {
                        ldecSpouse5YrsPremium = lbusLtcOption.idecMonthlyPremium;
                        dr["Coverage"] = lbusLtcOption.icdoPersonAccountLtcOption.level_of_coverage_description;
                        dr["LTCPremium"] = ldecSpouse5YrsPremium;
                    }
                    ldtbLTC.Rows.Add(dr);
                }
            }
            return ldtbLTC;
        }
        private busPlan GetPlan(int aintPlanID)
        {
            busPlan lobjPlan = new busPlan { icdoPlan = new cdoPlan() };
            DataTable ldtbPlan = ldtbAllPlans.AsEnumerable().Where(o => o.Field<int>("plan_id") == aintPlanID).AsDataTable();
            if (ldtbAllPlans.Rows.Count > 0)
                lobjPlan.icdoPlan.LoadData(ldtbPlan.Rows[0]);
            return lobjPlan;
        }

        public void GetTotalPremiumAmount(int lintPayeeAccountOwnerID, DateTime adtEffectiveDate, ref decimal ldecGHDVPremiumAmt, 
            string lstrGHDVIndicator)
        {
            ldecGHDVPremiumAmt = 0.00M;
            

            var lobjGhdv = new busPersonAccountGhdv { icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };
            if (lstrGHDVIndicator == "H")
            {
                int lintPersonAccountID = busPersonAccountHelper.GetPersonAccountID(busConstant.PlanIdGroupHealth, lintPayeeAccountOwnerID);
                lobjGhdv.FindPersonAccount(lintPersonAccountID);

            }
            else if (lstrGHDVIndicator == "D")
            {
                int lintPersonAccountId = busPersonAccountHelper.GetPersonAccountID(busConstant.PlanIdDental, lintPayeeAccountOwnerID);
                lobjGhdv.FindPersonAccount(lintPersonAccountId);
                lobjGhdv.FindGHDVByPersonAccountID(lintPersonAccountId);
            }
            else if (lstrGHDVIndicator == "V")
            {
                int lintPersonAccountId = busPersonAccountHelper.GetPersonAccountID(busConstant.PlanIdVision, lintPayeeAccountOwnerID);
                lobjGhdv.FindPersonAccount(lintPersonAccountId);
                lobjGhdv.FindGHDVByPersonAccountID(lintPersonAccountId);
            }
            //Premium calculation for GHDV
            if (IsHealthOrMedicare(lobjGhdv.icdoPersonAccount.plan_id) || lobjGhdv.icdoPersonAccount.plan_id == busConstant.PlanIdVision
            || lobjGhdv.icdoPersonAccount.plan_id == busConstant.PlanIdDental)
            {
                DataTable ldtPaidInsuranceAmt = busBase.Select("cdoMasPerson.Load_Insurance_YTD_For_Retiree",
                                        new object[2] { lobjGhdv.icdoPersonAccount.person_account_id, adtEffectiveDate.Year });

                if (ldtPaidInsuranceAmt.Rows.Count > 0)
                {
                    DataRow ldr = ldtPaidInsuranceAmt.Rows[0];
                    idecTotalPaidPremiumAmt = Convert.ToDecimal(ldr["PAID_PREMIUM_AMOUNT"]);
                }
                ldecGHDVPremiumAmt = idecTotalPaidPremiumAmt;
            }
        }

        public void GetTotalPremiumAmountMedicare(int lintPayeeAccountOwnerID, DateTime adtEffectiveDate, ref decimal ldecMedicarePremiumAmt)
        {
            ldecMedicarePremiumAmt = 0.0M;

            DataTable ldtPaidInsuranceAmt = busBase.Select("cdoMasPerson.Load_Insurance_YTD_For_Retiree_Medicare",
                                            new object[2] { adtEffectiveDate.Year, lintPayeeAccountOwnerID });
            if (ldtPaidInsuranceAmt.Rows.Count > 0)
            {
                DataRow ldr = ldtPaidInsuranceAmt.Rows[0];
                idecTotalPaidPremiumAmt = Convert.ToDecimal(ldr["PAID_PREMIUM_AMOUNT"]);
            }
            ldecMedicarePremiumAmt = idecTotalPaidPremiumAmt;

        }

        public void GetTotalPremiumAmountLife(int lintPayeeAccountOwnerID, DateTime adtEffectiveDate, ref decimal ldecLifePremiumAmt)
        {
            ldecLifePremiumAmt = 0.00M;
            var lobjLife = new busPersonAccountLife { icdoPersonAccountLife = new cdoPersonAccountLife() };

            int lintPersonAccountID = busPersonAccountHelper.GetPersonAccountID(busConstant.PlanIdGroupLife, lintPayeeAccountOwnerID);
            lobjLife.FindPersonAccount(lintPersonAccountID);

            if (lobjLife.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife)
            {
                DataTable ldtPaidInsuranceAmt = busBase.Select("cdoMasPerson.Load_Insurance_YTD_For_Retiree",
                                        new object[2] { lobjLife.icdoPersonAccount.person_account_id, adtEffectiveDate.Year });

                if (ldtPaidInsuranceAmt.Rows.Count > 0)
                {
                    DataRow ldr = ldtPaidInsuranceAmt.Rows[0];
                    idecTotalPaidPremiumAmt = Convert.ToDecimal(ldr["PAID_PREMIUM_AMOUNT"]);
                }
                ldecLifePremiumAmt = idecTotalPaidPremiumAmt;
            }
        }

        public void GetTotalPremiumAmountLTC(int lintPayeeAccountOwnerID, DateTime adtEffectiveDate, ref decimal ldecLTCPremiumAmt)
        {
            ldecLTCPremiumAmt = 0.00m;
            busPersonAccountLtc lobjLtc = new busPersonAccountLtc();
            lobjLtc.FindPersonAccount(busPersonAccountHelper.GetPersonAccountID(busConstant.PlanIdLTC, lintPayeeAccountOwnerID));

            DataTable ldtPaidInsuranceAmt = busBase.Select("cdoMasPerson.Load_Insurance_YTD_For_Retiree",
                                        new object[2] { adtEffectiveDate.Year, lobjLtc.icdoPersonAccount.person_account_id });

            if (ldtPaidInsuranceAmt.Rows.Count > 0)
            {
                DataRow ldr = ldtPaidInsuranceAmt.Rows[0];
                idecTotalPaidPremiumAmt = Convert.ToDecimal(ldr["PAID_PREMIUM_AMOUNT"]);
            }
            ldecLTCPremiumAmt = idecTotalPaidPremiumAmt;
        }
    }
}
