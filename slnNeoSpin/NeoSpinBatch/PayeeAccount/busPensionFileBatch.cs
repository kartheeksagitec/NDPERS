#region Using directives
using System;
using System.Data;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using System.Linq;
using Sagitec.ExceptionPub;
using System.Collections.Generic;
using System.Diagnostics;
#endregion

namespace NeoSpinBatch
{
    public class busPensionFileBatch : busNeoSpinBatch
    {
        public busActuaryFileHeader ibusActuaryFileHeader { get; set; }
        //prop to ad-hoc file head
        public Collection<busActuaryFileHeader> iclbActuaryFileHeader { get; set; }
        //Load Actuarial File header When adhoc batch runs
        busBase lobjBase = new busBase();
        public void LoadActuarialFileHeader(string astrPensionFileType, string astrFileTypeValue)
        {
            if (ibusActuaryFileHeader == null) ibusActuaryFileHeader = new busActuaryFileHeader { icdoActuaryFileHeader = new cdoActuaryFileHeader() };

            DataTable ldtbList = busBase.Select<cdoActuaryFileHeader>(new string[3] { "FILE_TYPE_VALUE", "STATUS_VALUE", "PENSION_FILE_TYPE_VALUE" },
                new object[3] { astrFileTypeValue, busConstant.AdhocActuaryFileStatusPending, astrPensionFileType }, null, null);
            iclbActuaryFileHeader = lobjBase.GetCollection<busActuaryFileHeader>(ldtbList, "icdoActuaryFileHeader");
        }
        //Create Actuarial file header when annual batch runs
        public int CreateActuarialFileHeader(string astrFileType)
        {
            if (ibusActuaryFileHeader == null) ibusActuaryFileHeader = new busActuaryFileHeader { icdoActuaryFileHeader = new cdoActuaryFileHeader() };
            ibusActuaryFileHeader.icdoActuaryFileHeader.file_type_value = astrFileType == busConstant.AdhocActuaryFileTypePensionFile ?
                busConstant.AdhocActuaryFileTypePensionFile : busConstant.AdhocActuaryFileTypeRHICFile;
            ibusActuaryFileHeader.icdoActuaryFileHeader.status_value = busConstant.AdhocActuaryFileStatusPending;
            DateTime ldtEffectiveDate = new DateTime(DateTime.Today.Year, 6, 30);
            if (DateTime.Today < ldtEffectiveDate)
                ldtEffectiveDate = new DateTime(DateTime.Today.Year - 1, 6, 30);
            ibusActuaryFileHeader.icdoActuaryFileHeader.effective_date = ldtEffectiveDate;

            DataTable ldtbList = busBase.Select<cdoActuaryFileHeader>(new string[2] { "PENSION_FILE_TYPE_VALUE", "file_type_value" },
                new object[2] { busConstant.PensionFileTypeAnnual, astrFileType }, null, null);
            Collection<busActuaryFileHeader> lclbActuaryFileHeader = lobjBase.GetCollection<busActuaryFileHeader>(ldtbList, "icdoActuaryFileHeader");

            if (lclbActuaryFileHeader.Where(o => o.icdoActuaryFileHeader.effective_date == ldtEffectiveDate).Count() > 0)
                return 0;

            ibusActuaryFileHeader.icdoActuaryFileHeader.pension_file_type_value = busConstant.PensionFileTypeAnnual;
            ibusActuaryFileHeader.icdoActuaryFileHeader.Insert();
            return ibusActuaryFileHeader.icdoActuaryFileHeader.actuary_file_header_id;
        }
        //Prop to Decrement file records
        DataTable idtDecrementResultTable = new DataTable();
        //Prop to load pension file records
        DataTable idtPensionFileTable = new DataTable();

        public void ProcessPensionFiles(string astrPensionFileType)
        {
            istrProcessName = iobjBatchSchedule.step_name;
            idlgUpdateProcessLog(istrProcessName + " Started", "INFO", istrProcessName);

            //if it is annual pension file batch,then create a new a header and process
            if (astrPensionFileType == busConstant.PensionFileTypeAnnual)
            {
                if (CreateActuarialFileHeader(busConstant.AdhocActuaryFileTypePensionFile) > 0)
                    GenerateFiles(astrPensionFileType);
                else
                    idlgUpdateProcessLog("Annual Pension File Batch already executed for the year", "INFO", istrProcessName);
                if (CreateActuarialFileHeader(busConstant.AdhocActuaryFileTypeRHICFile) > 0)
                    GenerateRHICFile(astrPensionFileType);
                else
                    idlgUpdateProcessLog("Annual RHIC File Batch already executed for the year", "INFO", istrProcessName);
            }
            else
            {

                //Processing Pension File Data
                LoadActuarialFileHeader(astrPensionFileType, busConstant.AdhocActuaryFileTypePensionFile);
                foreach (busActuaryFileHeader lobjActuaryFileHeader in iclbActuaryFileHeader)
                {
                    ibusActuaryFileHeader = lobjActuaryFileHeader;
                    GenerateFiles(astrPensionFileType);
                }

                //Processing RHIC File Data
                LoadActuarialFileHeader(astrPensionFileType, busConstant.AdhocActuaryFileTypeRHICFile);
                foreach (busActuaryFileHeader lobjActuaryFileHeader in iclbActuaryFileHeader)
                {
                    ibusActuaryFileHeader = lobjActuaryFileHeader;
                    GenerateRHICFile(astrPensionFileType);
                }
            }
            idlgUpdateProcessLog(istrProcessName + " Batch Ended", "INFO", istrProcessName);
        }

        public void GenerateRHICFile(string astrPensionFileType)
        {
            idlgUpdateProcessLog("Loading RHIC File Data", "INFO", istrProcessName);
            ProcessRHICFileData();
            if (iclbRHICFile.Count > 0)
            {
                idlgUpdateProcessLog("Generating Actuary RHIC File Process Started", "INFO", istrProcessName);
                try
                {
                    busProcessOutboundFile lobjProcessPensionFile = new busProcessOutboundFile();
                    lobjProcessPensionFile.iarrParameters = new object[1];
                    lobjProcessPensionFile.iarrParameters[0] = iclbRHICFile;
                    lobjProcessPensionFile.CreateOutboundFile(74);
                    idlgUpdateProcessLog("Actuary RHIC File Successfully Generated with Count of " + iclbRHICFile.Count + " Records", "INFO", istrProcessName);
                }
                catch (Exception e)
                {
                    ExceptionManager.Publish(e);
                    idlgUpdateProcessLog("Actuary RHIC File Generation failed. Message :" + e.Message, "INFO", istrProcessName);
                }
            }
            CreatePayeeRecapreport();
            ibusActuaryFileHeader.icdoActuaryFileHeader.status_value = busConstant.AdhocActuaryFileStatusProcessed;
            ibusActuaryFileHeader.icdoActuaryFileHeader.Update();
        }
        //create data for rhic file
        public Collection<busPensionFile> iclbRHICFile { get; set; }
        public void ProcessRHICFileData()
        {
            iclbRHICFile = new Collection<busPensionFile>();
            busBase lobjBase = new busBase();
            DataTable ldtbRhicFile = busBase.Select("cdoActuaryFileRhicDetail.fleRhicFileData",
                                                new object[3] { ibusActuaryFileHeader.icdoActuaryFileHeader.effective_date, 
                                                                ibusActuaryFileHeader.icdoActuaryFileHeader.actuary_file_header_id ,
                                                                ibusActuaryFileHeader.icdoActuaryFileHeader.plan_id});
            foreach (DataRow ldtr in ldtbRhicFile.Rows)
            {
                busPensionFile lobjPensionFile = new busPensionFile { icdoActuaryFileRhicDetail = new cdoActuaryFileRhicDetail() };
                lobjPensionFile.ibusMember = new busPerson { icdoPerson = new cdoPerson() };
                lobjPensionFile.icdoActuaryFileRhicDetail.LoadData(ldtr);
                lobjPensionFile.ibusMember.icdoPerson.LoadData(ldtr);
                if (!string.IsNullOrEmpty(lobjPensionFile.icdoActuaryFileRhicDetail.account_relation_value) && lobjPensionFile.icdoActuaryFileRhicDetail.account_relation_value != busConstant.AccountRelationshipMember)
                {
                    lobjPensionFile.icdoActuaryFileRhicDetail.ben_acc_owner_perslinkid = ldtr["OWNER_PERSLINK_ID"] == DBNull.Value ? 0 : Convert.ToInt32(ldtr["OWNER_PERSLINK_ID"]);
                }
                //PIR 13366 - populate org_code
                int lintOrgId = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPersonEmployment.GetOrgIdForPensionFile", new object[1] { lobjPensionFile.icdoActuaryFileRhicDetail.person_account_id },
                                                                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                lobjPensionFile.icdoActuaryFileRhicDetail.org_code = busGlobalFunctions.GetOrgCodeFromOrgId(lintOrgId);

                iclbRHICFile.Add(lobjPensionFile);
            }
            CreateReport("rptActuaryRHICFileReport.rpt", ldtbRhicFile);
        }
        public void GenerateFiles(string astrPensionFileType)
        {
            idtDecrementResultTable = CreateNewDecrementFileDataTable();
            idtPensionFileTable = CreateNewPensionFileDataTable();

            if (ibusActuaryFileHeader != null && ibusActuaryFileHeader.icdoActuaryFileHeader.actuary_file_header_id > 0)
            {
                busDBCacheData lobjDBCacheData = new busDBCacheData();
                lobjDBCacheData.idtbCachedBenefitProvisionEligibility = busGlobalFunctions.LoadBenefitProvisionEligibilityCacheData(iobjPassInfo);

                ProcessPensionFileData(lobjDBCacheData);

                ProcessDecrementFileData(lobjDBCacheData);

                if (idtPensionFileTable.Rows.Count > 0)
                {
                    idlgUpdateProcessLog("Generating Actuary Pension File Process Started", "INFO", istrProcessName);
                    try
                    {
                        busProcessOutboundFile lobjProcessPensionFile = new busProcessOutboundFile();
                        lobjProcessPensionFile.iarrParameters = new object[2];
                        lobjProcessPensionFile.iarrParameters[0] = idtPensionFileTable;
                        if(ibusActuaryFileHeader.ibusPlan == null)
                            ibusActuaryFileHeader.LoadPlan();
                        lobjProcessPensionFile.iarrParameters[1] = ibusActuaryFileHeader.ibusPlan.icdoPlan.plan_name;
                        lobjProcessPensionFile.CreateOutboundFile(70);
                        idlgUpdateProcessLog("Actuary Pension File Successfully Generated with Count of " + idtPensionFileTable.Rows.Count + " Records", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Actuary Pension File Generation failed. Message :" + e.Message, "INFO", istrProcessName);
                    }
                }
                else
                {
                    idlgUpdateProcessLog("No Pension File Generated since there is no record for the plan", "INFO", istrProcessName);
                }
                if (idtDecrementResultTable.Rows.Count > 0)
                {
                    idlgUpdateProcessLog("Generating Decrement File Process Started", "INFO", istrProcessName);
                    try
                    {
                        busProcessOutboundFile lobjProcessPensionFile = new busProcessOutboundFile();
                        lobjProcessPensionFile.iarrParameters = new object[2];
                        lobjProcessPensionFile.iarrParameters[0] = idtDecrementResultTable;
                        if (ibusActuaryFileHeader.ibusPlan == null)
                            ibusActuaryFileHeader.LoadPlan();
                        lobjProcessPensionFile.iarrParameters[1] = ibusActuaryFileHeader.ibusPlan.icdoPlan.plan_name;
                        lobjProcessPensionFile.CreateOutboundFile(71);
                        idlgUpdateProcessLog("Decrement File Successfully Generated with Count of" + idtDecrementResultTable.Rows.Count + " Records", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Generating Decrement File Failed. Message :" + e.Message, "INFO", istrProcessName);
                    }
                }
                else
                {
                    idlgUpdateProcessLog("No Decrement File Generated since there is no status Change", "INFO", istrProcessName);
                }
                ibusActuaryFileHeader.icdoActuaryFileHeader.status_value = busConstant.AdhocActuaryFileStatusProcessed;
                ibusActuaryFileHeader.icdoActuaryFileHeader.Update();
                //Generating Reports
                CreateBatchReports();
            }
        }
        public DataTable idtbPensionFileAll { get; set; }
        public DataTable idtbBenProvisionBenType { get; set; }

        /// <summary>
        /// Method to select all records which are in enrolled and suspended status and
        /// </summary>
        /// <param name="adtPensionFile"></param>
        public void ProcessPensionFileData(busDBCacheData aobjDBCacheData)
        {
            idlgUpdateProcessLog("Loading Actuary Pension File Data", "INFO", istrProcessName);
            //Load all person account and payee account related records
            idtbPensionFileAll = busBase.Select("cdoActuaryFilePensionDetail.flePensionFileOut",
                                                new object[3]{ibusActuaryFileHeader.icdoActuaryFileHeader.actuary_file_header_id,
                                                                ibusActuaryFileHeader.icdoActuaryFileHeader.effective_date,
                                                                ibusActuaryFileHeader.icdoActuaryFileHeader.plan_id
                                                });

            var ldtFilteredPensionFile = idtbPensionFileAll.AsEnumerable().Where(o =>
                (o.Field<string>("plan_participation_status_value") != null &&
                (o.Field<string>("plan_participation_status_value") == busConstant.PlanParticipationStatusRetimentSuspended ||
                 o.Field<string>("plan_participation_status_value") == busConstant.PlanParticipationStatusRetirementEnrolled)) ||
                (o.Field<string>("payee_status_value") != null && !(
                 o.Field<string>("payee_status_value") == busConstant.PayeeAccountStatusPostRetirementDeathPaymentCompleted ||
                 o.Field<string>("payee_status_value") == busConstant.PayeeAccountStatusRefundProcessed ||
                 o.Field<string>("payee_status_value") == busConstant.PayeeAccountStatusRetirementPaymentCompleted ||
                 o.Field<string>("payee_status_value") == busConstant.PayeeAccountStatusPreRetirementDeathPaymentCompleted ||
                 o.Field<string>("payee_status_value") == busConstant.PayeeAccountStatusDisabilityPaymentCompleted ||
                 o.Field<string>("payee_status_value") == busConstant.PayeeAccountStatusDisabilityCancelled ||
                 o.Field<string>("payee_status_value") == busConstant.PayeeAccountStatusPostRetirementDeathCancelled ||
                 o.Field<string>("payee_status_value") == busConstant.PayeeAccountStatusPreRetirementDeathCancelled ||
                 o.Field<string>("payee_status_value") == busConstant.PayeeAccountStatusRefundCancelled ||
                 o.Field<string>("payee_status_value") == busConstant.PayeeAccountStatusRetirmentCancelled)));            

            idtbBenProvisionBenType = busBase.Select("cdoBenefitProvisionBenefitType.GetAllBenefitProvision", new object[0] { });

            DataTable ldtbBenOptionFactor = busBase.Select<cdoBenefitOptionFactor>(
                          new string[0] { },
                          new object[0] { }, null, null);
            DataTable ldtbBenefitProvisionEligibility = busBase.Select<cdoBenefitProvisionEligibility>(
                          new string[0] { },
                          new object[0] { }, null, "effective_date desc");
            DataTable ldtbEligibilityForNormal = ldtbBenefitProvisionEligibility.AsEnumerable()
                                                .Where(o => o.Field<string>("benefit_account_type_value") == busConstant.ApplicationBenefitTypeRetirement
                                                            && o.Field<string>("ELIGIBILITY_TYPE_VALUE") == busConstant.BenefitProvisionEligibilityNormal)
                                                .AsDataTable();

            idlgUpdateProcessLog("Creating Pension File Details", "INFO", istrProcessName);
            DataTable ldtbFilteredPensionFile = ldtFilteredPensionFile.AsDataTable();
            foreach (DataRow ldtrPensionFileDate in ldtFilteredPensionFile)
            {
                
                busPensionFile lobjPensionFile = new busPensionFile { icdoActuaryFilePensionDetail = new cdoActuaryFilePensionDetail() };
                lobjPensionFile.icdoActuaryFilePensionDetail.LoadData(ldtrPensionFileDate);
                lobjPensionFile.icdoActuaryFilePensionDetail.actuary_file_header_id = ibusActuaryFileHeader.icdoActuaryFileHeader.actuary_file_header_id;
                try
                {       //PIR 16078
                    if (//(lobjPensionFile.icdoActuaryFilePensionDetail.totctr == lobjPensionFile.icdoActuaryFilePensionDetail.ehctr ||
                        //lobjPensionFile.icdoActuaryFilePensionDetail.totctr == lobjPensionFile.icdoActuaryFilePensionDetail.edctr) &&
                        (lobjPensionFile.icdoActuaryFilePensionDetail.payee_acct_exists == busConstant.Flag_No))
                    {
                        //lobjPensionFile.icdoActuaryFilePensionDetail.employment_status_value = "TERM";
                        lobjPensionFile.icdoActuaryFilePensionDetail.accrued_benefit_calculation =
                                lobjPensionFile.GetAccruedBenefitAmount(ibusActuaryFileHeader.icdoActuaryFileHeader.effective_date,
                                ldtrPensionFileDate, aobjDBCacheData, idtbBenProvisionBenType, ldtbBenOptionFactor, ldtbEligibilityForNormal);
                    }
                    //Reseting the audit log fields
                    lobjPensionFile.icdoActuaryFilePensionDetail.created_by = null;
                    lobjPensionFile.icdoActuaryFilePensionDetail.created_date = DateTime.MinValue;
                    lobjPensionFile.icdoActuaryFilePensionDetail.modified_by = null;
                    lobjPensionFile.icdoActuaryFilePensionDetail.modified_date = DateTime.MinValue;
                    lobjPensionFile.icdoActuaryFilePensionDetail.update_seq = 0;
                    lobjPensionFile.icdoActuaryFilePensionDetail.pension_service_credit = Math.Round(lobjPensionFile.icdoActuaryFilePensionDetail.pension_service_credit / 12,
                                                                                            6, MidpointRounding.AwayFromZero);
                    DataRow[] larrPersonRows = ldtbFilteredPensionFile.FilterTable(busConstant.DataType.Numeric, "person_id", lobjPensionFile.icdoActuaryFilePensionDetail.person_id);
                    if (lobjPensionFile.icdoActuaryFilePensionDetail.plan_id != busConstant.PlanIdJobService)
                    {
                        if (larrPersonRows != null && larrPersonRows.Count() > 0)
                        {
                            lobjPensionFile.icdoActuaryFilePensionDetail.total_vested_service_credit = 
                                                                larrPersonRows.AsEnumerable()
                                                                    .Where(o => o.Field<int>("plan_id") != busConstant.PlanIdJobService)
                                                                    .Sum(o => o.Field<decimal>("total_vested_service_credit")) +
                                                                    lobjPensionFile.icdoActuaryFilePensionDetail.tffr_tiaa_service;
                            lobjPensionFile.icdoActuaryFilePensionDetail.total_vested_service_credit = Math.Round(lobjPensionFile.icdoActuaryFilePensionDetail.total_vested_service_credit / 12,
                                                                                                        6, MidpointRounding.AwayFromZero);
                        }                        
                    }
                    lobjPensionFile.icdoActuaryFilePensionDetail.pension_service_credit_ba = Math.Round(lobjPensionFile.icdoActuaryFilePensionDetail.pension_service_credit_ba / 12,
                                                                                            6, MidpointRounding.AwayFromZero);
                    lobjPensionFile.icdoActuaryFilePensionDetail.total_vested_service_credit_ba = Math.Round(lobjPensionFile.icdoActuaryFilePensionDetail.total_vested_service_credit_ba / 12,
                                                                                                        6, MidpointRounding.AwayFromZero);
                    lobjPensionFile.icdoActuaryFilePensionDetail.purchased_service_credit_sum = Math.Round(lobjPensionFile.icdoActuaryFilePensionDetail.purchased_service_credit_sum / 12,
                                                                                                        6, MidpointRounding.AwayFromZero);
                    lobjPensionFile.icdoActuaryFilePensionDetail.benefit_sub_type_value = ldtrPensionFileDate["BENEFIT_ACCOUNT_SUB_TYPE_VALUE"].ToString();

                    //PIR 13366 - populate org_code
                    if (lobjPensionFile.icdoActuaryFilePensionDetail.employment_status_value == busConstant.EmploymentStatusTerminated
                        || (busGlobalFunctions.GetData2ByCodeValue(2203, lobjPensionFile.icdoActuaryFilePensionDetail.payee_status_value,iobjPassInfo) == busConstant.PayeeAccountStatusReceiving &&
                        (lobjPensionFile.icdoActuaryFilePensionDetail.account_relation_value == busConstant.AccountRelationshipJointAnnuitant
                        || lobjPensionFile.icdoActuaryFilePensionDetail.account_relation_value == busConstant.AccountRelationshipAlternatePayee
                        || lobjPensionFile.icdoActuaryFilePensionDetail.account_relation_value == busConstant.AccountRelationshipBeneficiary)))
                    {
                        lobjPensionFile.icdoActuaryFilePensionDetail.org_id = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPersonEmployment.GetOrgIdForPensionFile",
                                                                                                new object[1] { lobjPensionFile.icdoActuaryFilePensionDetail.person_account_id },
                                                                                                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                        lobjPensionFile.icdoActuaryFilePensionDetail.org_code = busGlobalFunctions.GetOrgCodeFromOrgId(lobjPensionFile.icdoActuaryFilePensionDetail.org_id);
                    }

                    lobjPensionFile.icdoActuaryFilePensionDetail.Insert();
                    if (!string.IsNullOrEmpty(ldtrPensionFileDate["GRADUATED_BENEFIT_OPTION_VALUE"].ToString()))
                    {
                        lobjPensionFile.icdoActuaryFilePensionDetail.graduated_benefit_option_percentage = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1913, ldtrPensionFileDate["GRADUATED_BENEFIT_OPTION_VALUE"].ToString());
                    }
                    if (!string.IsNullOrEmpty(lobjPensionFile.icdoActuaryFilePensionDetail.account_relation_value))
                    {
                        if (lobjPensionFile.icdoActuaryFilePensionDetail.account_relation_value != busConstant.AccountRelationshipMember)
                        {
                            lobjPensionFile.icdoActuaryFilePensionDetail.ben_acc_owner_perslinkID = ldtrPensionFileDate["PERSON_ID1"] == DBNull.Value ? 0 : Convert.ToInt32(ldtrPensionFileDate["PERSON_ID1"]);
                        }
                    }
                    AddToNewDataRowPensionFile(lobjPensionFile);
                }
                catch (Exception e)
                {
                    idlgUpdateProcessLog("Creating Pension File Details Failed for person id "+lobjPensionFile.icdoActuaryFilePensionDetail.person_id.ToString()+ 
                        ", Message : "+e.Message, "INFO", istrProcessName);
                    throw e;
                }
            }
        }
        public void AddToNewDataRowPensionFile(busPensionFile aobjPensionFile)
        {
            aobjPensionFile.icdoActuaryFilePensionDetail.employment_type_description = aobjPensionFile.icdoActuaryFilePensionDetail.employment_type_value != null ?
                            iobjPassInfo.isrvDBCache.GetCodeDescriptionString(313, aobjPensionFile.icdoActuaryFilePensionDetail.employment_type_value) : string.Empty;
            aobjPensionFile.icdoActuaryFilePensionDetail.hourly_description = aobjPensionFile.icdoActuaryFilePensionDetail.hourly_value != null ?
                             iobjPassInfo.isrvDBCache.GetCodeDescriptionString(311, aobjPensionFile.icdoActuaryFilePensionDetail.hourly_value) : string.Empty;
            aobjPensionFile.icdoActuaryFilePensionDetail.seasonal_description = aobjPensionFile.icdoActuaryFilePensionDetail.seasonal_value != null ?
                            iobjPassInfo.isrvDBCache.GetCodeDescriptionString(312, aobjPensionFile.icdoActuaryFilePensionDetail.seasonal_value) : string.Empty;
            if (aobjPensionFile.icdoActuaryFilePensionDetail.MemberGender == busConstant.GenderTypeMale)
            {
                aobjPensionFile.icdoActuaryFilePensionDetail.MemberGenderCode = "M";
            }
            else if (aobjPensionFile.icdoActuaryFilePensionDetail.MemberGender == busConstant.GenderTypeFemale)
            {
                aobjPensionFile.icdoActuaryFilePensionDetail.MemberGenderCode = "F";
            }
            if (aobjPensionFile.icdoActuaryFilePensionDetail.SpouseGender == busConstant.GenderTypeMale)
            {
                aobjPensionFile.icdoActuaryFilePensionDetail.SpouseGenderCode = "M";
            }
            else if (aobjPensionFile.icdoActuaryFilePensionDetail.SpouseGender == busConstant.GenderTypeFemale)
            {
                aobjPensionFile.icdoActuaryFilePensionDetail.SpouseGenderCode = "F";
            }
            if (!string.IsNullOrEmpty(aobjPensionFile.icdoActuaryFilePensionDetail.plan_participation_status_value))
            {
                DataTable ldtbDataTable = iobjPassInfo.isrvDBCache.GetCodeDescription(337,
                  aobjPensionFile.icdoActuaryFilePensionDetail.plan_participation_status_value);
                aobjPensionFile.icdoActuaryFilePensionDetail.plan_participation_value_formatted = ldtbDataTable.Rows.Count > 0 ? ldtbDataTable.Rows[0]["data2"].ToString() : string.Empty;
            }
            if (!string.IsNullOrEmpty(aobjPensionFile.icdoActuaryFilePensionDetail.employment_status_value))
            {
                if (aobjPensionFile.icdoActuaryFilePensionDetail.totctr == aobjPensionFile.icdoActuaryFilePensionDetail.ehctr ||
                    aobjPensionFile.icdoActuaryFilePensionDetail.totctr == aobjPensionFile.icdoActuaryFilePensionDetail.edctr)
                {
                    aobjPensionFile.icdoActuaryFilePensionDetail.employment_status_value_formatted = "TERM";
                }
                else
                {
                    aobjPensionFile.icdoActuaryFilePensionDetail.employment_status_value_formatted = aobjPensionFile.icdoActuaryFilePensionDetail.employment_status_value;
                }
            }
            DataRow dr = idtPensionFileTable.NewRow();
            dr["payee_account_id"] = aobjPensionFile.icdoActuaryFilePensionDetail.payee_account_id;
            dr["person_account_id"] = aobjPensionFile.icdoActuaryFilePensionDetail.person_account_id;
            dr["org_id"] = aobjPensionFile.icdoActuaryFilePensionDetail.org_id;
            dr["person_id"] = aobjPensionFile.icdoActuaryFilePensionDetail.person_id;
            dr["plan_id"] = aobjPensionFile.icdoActuaryFilePensionDetail.plan_id;
            dr["ssn"] = aobjPensionFile.icdoActuaryFilePensionDetail.ssn;
            dr["account_relation_value"] = aobjPensionFile.icdoActuaryFilePensionDetail.account_relation_value;
            dr["last_name"] = aobjPensionFile.icdoActuaryFilePensionDetail.last_name;
            dr["first_name"] = aobjPensionFile.icdoActuaryFilePensionDetail.first_name;
            dr["plan_name"] = aobjPensionFile.icdoActuaryFilePensionDetail.plan_name;
            dr["plan_participation_status_value"] = aobjPensionFile.icdoActuaryFilePensionDetail.plan_participation_status_value;
            dr["plan_participation_value_formatted"] = aobjPensionFile.icdoActuaryFilePensionDetail.plan_participation_value_formatted;
            dr["application_status_value"] = aobjPensionFile.icdoActuaryFilePensionDetail.application_status_value;
            dr["employment_status_value"] = aobjPensionFile.icdoActuaryFilePensionDetail.employment_status_value;
            dr["employment_status_value_formatted"] = aobjPensionFile.icdoActuaryFilePensionDetail.employment_status_value_formatted;
            dr["employment_type_value"] = aobjPensionFile.icdoActuaryFilePensionDetail.employment_type_value;
            dr["employment_type_description"] = aobjPensionFile.icdoActuaryFilePensionDetail.employment_type_description;
            dr["payee_status_value"] = aobjPensionFile.icdoActuaryFilePensionDetail.payee_status_value;
            dr["payee_status_data2"] = aobjPensionFile.icdoActuaryFilePensionDetail.payee_status_data2;
            dr["Hourly"] = aobjPensionFile.icdoActuaryFilePensionDetail.hourly_value;
            dr["Seasonal"] = aobjPensionFile.icdoActuaryFilePensionDetail.seasonal_value;
            dr["hourly_value"] = aobjPensionFile.icdoActuaryFilePensionDetail.hourly_value;
            dr["seasonal_value"] = aobjPensionFile.icdoActuaryFilePensionDetail.seasonal_value;
            dr["hourly_description"] = aobjPensionFile.icdoActuaryFilePensionDetail.hourly_description;
            dr["seasonal_description"] = aobjPensionFile.icdoActuaryFilePensionDetail.seasonal_description;
            dr["benefit_option_value"] = aobjPensionFile.icdoActuaryFilePensionDetail.benefit_option_value;
            dr["Planid"] = aobjPensionFile.icdoActuaryFilePensionDetail.plan_name;
            dr["marital_status_value"] = aobjPensionFile.icdoActuaryFilePensionDetail.marital_status_value;
            if (aobjPensionFile.icdoActuaryFilePensionDetail.date_of_birth != DateTime.MinValue)
                dr["date_of_birth"] = aobjPensionFile.icdoActuaryFilePensionDetail.date_of_birth;
            if (aobjPensionFile.icdoActuaryFilePensionDetail.plan_participation_start_date != DateTime.MinValue)
                dr["plan_participation_start_date"] = aobjPensionFile.icdoActuaryFilePensionDetail.plan_participation_start_date;
            dr["pension_service_credit"] = aobjPensionFile.icdoActuaryFilePensionDetail.pension_service_credit;            
            dr["total_vested_service_credit"] = aobjPensionFile.icdoActuaryFilePensionDetail.total_vested_service_credit;
            if (aobjPensionFile.icdoActuaryFilePensionDetail.spouse_dob != DateTime.MinValue)
                dr["spouse_dob"] = aobjPensionFile.icdoActuaryFilePensionDetail.spouse_dob;
            if (aobjPensionFile.icdoActuaryFilePensionDetail.joint_annuitant_dob != DateTime.MinValue)
                dr["joint_annuitant_dob"] = aobjPensionFile.icdoActuaryFilePensionDetail.joint_annuitant_dob;
            if (aobjPensionFile.icdoActuaryFilePensionDetail.retirement_date != DateTime.MinValue)
                dr["retirement_date"] = aobjPensionFile.icdoActuaryFilePensionDetail.retirement_date;
            dr["MemberGenderCode"] = aobjPensionFile.icdoActuaryFilePensionDetail.MemberGenderCode;
            dr["SpouseGenderCode"] = aobjPensionFile.icdoActuaryFilePensionDetail.SpouseGenderCode;
            dr["org_code"] = aobjPensionFile.icdoActuaryFilePensionDetail.org_code;
            dr["total_salary"] = aobjPensionFile.icdoActuaryFilePensionDetail.total_salary;
            dr["account_balance"] = aobjPensionFile.icdoActuaryFilePensionDetail.account_balance;
            dr["total_vested_service_credit_ba"] = aobjPensionFile.icdoActuaryFilePensionDetail.total_vested_service_credit_ba;
            dr["pension_service_credit_ba"] = aobjPensionFile.icdoActuaryFilePensionDetail.pension_service_credit_ba;
            dr["accrued_benefit_calculation"] = aobjPensionFile.icdoActuaryFilePensionDetail.accrued_benefit_calculation;
            dr["gross_current_monthly_benefit_amount"] = aobjPensionFile.icdoActuaryFilePensionDetail.gross_current_monthly_benefit_amount;
            dr["purchased_service_credit_sum"] = aobjPensionFile.icdoActuaryFilePensionDetail.purchased_service_credit_sum;
            dr["ssli_or_uniform_income_commencement_age"] = aobjPensionFile.icdoActuaryFilePensionDetail.ssli_or_uniform_income_commencement_age;
            dr["estimated_ssli_benefit_amount"] = aobjPensionFile.icdoActuaryFilePensionDetail.estimated_ssli_benefit_amount;
            if (aobjPensionFile.icdoActuaryFilePensionDetail.ssli_change_date != DateTime.MinValue)
                dr["ssli_change_date"] = aobjPensionFile.icdoActuaryFilePensionDetail.ssli_change_date;
            dr["vested_er_amount"] = aobjPensionFile.icdoActuaryFilePensionDetail.vested_er_amount;
            dr["travellers_base_benefit_amount"] = aobjPensionFile.icdoActuaryFilePensionDetail.travellers_base_benefit_amount;
            dr["travellers_cumulative_cola_amount"] = aobjPensionFile.icdoActuaryFilePensionDetail.travellers_cumulative_cola_amount;
            dr["minimum_guarantee_amount"] = aobjPensionFile.icdoActuaryFilePensionDetail.minimum_guarantee_amount;
            dr["amount_paid_ltd"] = aobjPensionFile.icdoActuaryFilePensionDetail.amount_paid_ltd;
            dr["BENEFIT_ACCOUNT_TYPE_VALUE"] = aobjPensionFile.icdoActuaryFilePensionDetail.benefit_account_type_value;
            dr["BENEFIT_SUB_TYPE_VALUE"] = aobjPensionFile.icdoActuaryFilePensionDetail.benefit_sub_type_value;
            dr["OPTION_FACTOR"] = aobjPensionFile.icdoActuaryFilePensionDetail.option_factor;
            dr["ben_acc_owner_perslinkID"] = aobjPensionFile.icdoActuaryFilePensionDetail.ben_acc_owner_perslinkID;
            dr["GRADUATED_BENEFIT_OPTION_PERCENTAGE"] = aobjPensionFile.icdoActuaryFilePensionDetail.graduated_benefit_option_percentage;
            idtPensionFileTable.Rows.Add(dr);
        }

        private void ProcessDecrementFileData(busDBCacheData aobjDBCacheData)
        {
            try
            {
                idlgUpdateProcessLog("Loading Decrement File Data", "INFO", istrProcessName);
                //Load Previous Year Annual pension file records from sgt_actuarial_file_pension_detail
                DataTable ldtbPrevYearPensionFile = busBase.Select("cdoActuaryFilePensionDetail.LoadPreviousPensionFileDetails",
                                new object[1] { ibusActuaryFileHeader.icdoActuaryFileHeader.plan_id });
                DataTable ldtbPensionFileMembersTIAATFFRService = busBase.Select("cdoActuaryFilePensionDetail.LoadPensionFileMembersTIAATFFRService",
                             new object[1] { ibusActuaryFileHeader.icdoActuaryFileHeader.actuary_file_header_id });
                foreach (DataRow ldtrPensionFile in ldtbPrevYearPensionFile.Rows)
                {
                    busPensionFile lobjDecrmentFile = new busPensionFile { icdoActuaryFilePensionDetail = new cdoActuaryFilePensionDetail() };

                    busPensionFile lobjPrevPensionFile = new busPensionFile { icdoActuaryFilePensionDetail = new cdoActuaryFilePensionDetail() }; ;
                    lobjPrevPensionFile.icdoActuaryFilePensionDetail.LoadData(ldtrPensionFile);

                    lobjPrevPensionFile.ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                    lobjPrevPensionFile.ibusMember = new busPerson { icdoPerson = new cdoPerson() };
                    lobjPrevPensionFile.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
                    lobjPrevPensionFile.ibusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                    lobjPrevPensionFile.ibusPayeeAccount.ibusPayeeAccountActiveStatus = new busPayeeAccountStatus { icdoPayeeAccountStatus = new cdoPayeeAccountStatus() };

                    lobjPrevPensionFile.ibusPersonAccount.icdoPersonAccount.LoadData(ldtrPensionFile);
                    lobjPrevPensionFile.ibusMember.icdoPerson.LoadData(ldtrPensionFile);
                    lobjPrevPensionFile.ibusPlan.icdoPlan.LoadData(ldtrPensionFile);
                    lobjPrevPensionFile.ibusPayeeAccount.icdoPayeeAccount.LoadData(ldtrPensionFile);
                    lobjPrevPensionFile.ibusPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.LoadData(ldtrPensionFile);

                    DataRow ldrCurrentYearRow = null;

                    busPensionFile lobjCurrentPensionFile = new busPensionFile() { icdoActuaryFilePensionDetail = new cdoActuaryFilePensionDetail() };
                    //Get the record from previous year for a given person id and payee account id
                    DataRow[] ldrFilterdPersonRows = idtbPensionFileAll.FilterTable(busConstant.DataType.Numeric, "person_id", 
                                                        lobjPrevPensionFile.icdoActuaryFilePensionDetail.person_id);
                    if (ldrFilterdPersonRows != null && ldrFilterdPersonRows.Count() > 0)
                    {
                        if (lobjPrevPensionFile.icdoActuaryFilePensionDetail.payee_account_id > 0)
                        {
                            ldrCurrentYearRow = ldrFilterdPersonRows.AsEnumerable().Where(o =>
                                !string.IsNullOrEmpty(o.Field<string>("payee_status_value")) &&
                               o.Field<int>("payee_account_id") == lobjPrevPensionFile.icdoActuaryFilePensionDetail.payee_account_id).FirstOrDefault();
                        }
                        else//Get the record from previous year for a given person id and person account id
                        {
                            ldrCurrentYearRow = ldrFilterdPersonRows.AsEnumerable().Where(o =>
                              o.Field<int>("person_account_id") == lobjPrevPensionFile.icdoActuaryFilePensionDetail.person_account_id &&
                              string.IsNullOrEmpty(o.Field<string>("payee_status_value"))).FirstOrDefault();
                        }
                    }
                    if (ldrCurrentYearRow != null)
                    {
                        lobjCurrentPensionFile.icdoActuaryFilePensionDetail.LoadData(ldrCurrentYearRow);

                        lobjCurrentPensionFile.ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                        lobjCurrentPensionFile.ibusMember = new busPerson { icdoPerson = new cdoPerson() };
                        lobjCurrentPensionFile.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
                        lobjCurrentPensionFile.ibusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                        lobjCurrentPensionFile.ibusPayeeAccount.ibusPayeeAccountActiveStatus = new busPayeeAccountStatus { icdoPayeeAccountStatus = new cdoPayeeAccountStatus() };

                        lobjCurrentPensionFile.ibusPersonAccount.icdoPersonAccount.LoadData(ldrCurrentYearRow);
                        lobjCurrentPensionFile.ibusMember.icdoPerson.LoadData(ldrCurrentYearRow);
                        lobjCurrentPensionFile.ibusPlan.icdoPlan.LoadData(ldrCurrentYearRow);
                        lobjCurrentPensionFile.ibusPayeeAccount.icdoPayeeAccount.LoadData(ldrCurrentYearRow);
                        lobjCurrentPensionFile.ibusPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.LoadData(ldrCurrentYearRow);

                        //if record exist in previous year,Take it and compare with current year record,if there is any in plan particiaption status or payee account status or employment status
                        //Send the record in decremnt file
                        if (lobjCurrentPensionFile != null && lobjCurrentPensionFile.icdoActuaryFilePensionDetail.person_account_id > 0)
                        {
                            if (lobjCurrentPensionFile.icdoActuaryFilePensionDetail.plan_participation_status_value != lobjPrevPensionFile.icdoActuaryFilePensionDetail.plan_participation_status_value
                                || lobjCurrentPensionFile.icdoActuaryFilePensionDetail.employment_status_value != lobjPrevPensionFile.icdoActuaryFilePensionDetail.employment_status_value
                                || lobjCurrentPensionFile.icdoActuaryFilePensionDetail.payee_status_value != lobjPrevPensionFile.icdoActuaryFilePensionDetail.payee_status_value)
                            {
                                SetDecrementFileData(lobjDecrmentFile, lobjPrevPensionFile, lobjCurrentPensionFile, ldtbPensionFileMembersTIAATFFRService, aobjDBCacheData);

                                if (!string.IsNullOrEmpty(lobjPrevPensionFile.icdoActuaryFilePensionDetail.plan_participation_status_value))
                                {
                                    DataTable ldtbDataTable = iobjPassInfo.isrvDBCache.GetCodeDescription(337,
                                      lobjPrevPensionFile.icdoActuaryFilePensionDetail.plan_participation_status_value);
                                    lobjDecrmentFile.icdoActuaryFilePensionDetail.previous_plan_participation_value_formatted = ldtbDataTable.Rows.Count > 0 ? ldtbDataTable.Rows[0]["data2"].ToString() : string.Empty;
                                }

                                if (!string.IsNullOrEmpty(lobjCurrentPensionFile.icdoActuaryFilePensionDetail.plan_participation_status_value))
                                {
                                    DataTable ldtbDataTable = iobjPassInfo.isrvDBCache.GetCodeDescription(337,
                                      lobjCurrentPensionFile.icdoActuaryFilePensionDetail.plan_participation_status_value);
                                    lobjDecrmentFile.icdoActuaryFilePensionDetail.current_plan_participation_value_formatted = ldtbDataTable.Rows.Count > 0 ? ldtbDataTable.Rows[0]["data2"].ToString() : string.Empty;
                                }
                                if (!string.IsNullOrEmpty(lobjDecrmentFile.icdoActuaryFilePensionDetail.decrement_reason))
                                {
                                    AddToNewDataRowDecrmentFile(lobjDecrmentFile);
                                }
                            }
                        }
                        else
                        {
                            SetDecrementFileData(lobjDecrmentFile, lobjPrevPensionFile, null, null, aobjDBCacheData);
                            AddToNewDataRowDecrmentFile(lobjDecrmentFile);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                idlgUpdateProcessLog("Loading Decrement File Data failed", "INFO", istrProcessName);
                ExceptionManager.Publish(e);
            }
        }

        //Set current year values and previous year values
        private void SetDecrementFileData(busPensionFile aobjDecrmentFile, busPensionFile aobjPrevPensionFile, busPensionFile aobjCurrentPensionFile, 
            DataTable adtbPensionFileMembersTIAATFFRService, busDBCacheData aobjDBCacheData)
        {
            if (aobjCurrentPensionFile == null && aobjPrevPensionFile != null)
            {
                aobjDecrmentFile.icdoActuaryFilePensionDetail.person_id = aobjPrevPensionFile.icdoActuaryFilePensionDetail.person_id;
                aobjDecrmentFile.icdoActuaryFilePensionDetail.ssn = aobjPrevPensionFile.icdoActuaryFilePensionDetail.ssn;
                aobjDecrmentFile.icdoActuaryFilePensionDetail.first_name = aobjPrevPensionFile.icdoActuaryFilePensionDetail.first_name;
                aobjDecrmentFile.icdoActuaryFilePensionDetail.last_name = aobjPrevPensionFile.icdoActuaryFilePensionDetail.last_name;
                aobjDecrmentFile.icdoActuaryFilePensionDetail.account_relation_value = aobjPrevPensionFile.icdoActuaryFilePensionDetail.account_relation_value;
                aobjDecrmentFile.icdoActuaryFilePensionDetail.previous_employment_status_value = aobjPrevPensionFile.icdoActuaryFilePensionDetail.previous_employment_status_value;
                aobjDecrmentFile.icdoActuaryFilePensionDetail.previous_payee_status_value = aobjPrevPensionFile.icdoActuaryFilePensionDetail.previous_payee_status_value;
                aobjDecrmentFile.icdoActuaryFilePensionDetail.previous_plan_participation_code_value = aobjPrevPensionFile.icdoActuaryFilePensionDetail.current_plan_participation_code_value;
                aobjDecrmentFile.icdoActuaryFilePensionDetail.benefit_option_value = aobjPrevPensionFile.icdoActuaryFilePensionDetail.benefit_option_value;
                aobjDecrmentFile.icdoActuaryFilePensionDetail.plan_name = aobjPrevPensionFile.icdoActuaryFilePensionDetail.plan_name;
            }
            if (aobjPrevPensionFile != null && aobjCurrentPensionFile != null)
            {
                aobjDecrmentFile.icdoActuaryFilePensionDetail.person_id = aobjCurrentPensionFile.icdoActuaryFilePensionDetail.person_id;
                aobjDecrmentFile.icdoActuaryFilePensionDetail.ssn = aobjCurrentPensionFile.icdoActuaryFilePensionDetail.ssn;
                aobjDecrmentFile.icdoActuaryFilePensionDetail.first_name = aobjCurrentPensionFile.icdoActuaryFilePensionDetail.first_name;
                aobjDecrmentFile.icdoActuaryFilePensionDetail.last_name = aobjCurrentPensionFile.icdoActuaryFilePensionDetail.last_name;
                aobjDecrmentFile.icdoActuaryFilePensionDetail.account_relation_value = aobjCurrentPensionFile.icdoActuaryFilePensionDetail.account_relation_value;
                aobjDecrmentFile.icdoActuaryFilePensionDetail.previous_employment_status_value = aobjPrevPensionFile.icdoActuaryFilePensionDetail.employment_status_value;
                aobjDecrmentFile.icdoActuaryFilePensionDetail.previous_payee_status_value = aobjPrevPensionFile.icdoActuaryFilePensionDetail.payee_status_value;
                aobjDecrmentFile.icdoActuaryFilePensionDetail.previous_plan_participation_code_value = aobjPrevPensionFile.icdoActuaryFilePensionDetail.plan_participation_status_value;
                aobjDecrmentFile.icdoActuaryFilePensionDetail.current_plan_participation_code_value = aobjCurrentPensionFile.icdoActuaryFilePensionDetail.plan_participation_status_value;
                aobjDecrmentFile.icdoActuaryFilePensionDetail.current_employment_status_value = aobjCurrentPensionFile.icdoActuaryFilePensionDetail.employment_status_value;
                aobjDecrmentFile.icdoActuaryFilePensionDetail.current_payee_status_value = aobjCurrentPensionFile.icdoActuaryFilePensionDetail.payee_status_value;
                aobjDecrmentFile.icdoActuaryFilePensionDetail.benefit_option_value = aobjCurrentPensionFile.icdoActuaryFilePensionDetail.benefit_option_value;
                aobjDecrmentFile.icdoActuaryFilePensionDetail.plan_name = aobjCurrentPensionFile.icdoActuaryFilePensionDetail.plan_name;
                aobjDecrmentFile.icdoActuaryFilePensionDetail.decrement_reason = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2603,
                    GetDecrementReason(aobjPrevPensionFile, aobjCurrentPensionFile, adtbPensionFileMembersTIAATFFRService, aobjDBCacheData));
                aobjDecrmentFile.icdoActuaryFilePensionDetail.date_of_decrement = aobjCurrentPensionFile.icdoActuaryFilePensionDetail.date_of_decrement;
            }
        }
        //Get Decrement reason as per use case
        private string GetDecrementReason(busPensionFile aobjPrevPensionFile, busPensionFile aobjCurrentPensionFile, DataTable ldtbPensionFileMembersTIAATFFRService, 
            busDBCacheData aobjDBCacheData)
        {
            string lstrCurrentPayeeStatus = string.Empty;
            string lstrPreviousPayeeStatus = string.Empty;
            string lstrReason = string.Empty;
            if (aobjPrevPensionFile.ibusPersonAccount == null)
                aobjPrevPensionFile.LoadPersonAccount();
            if (aobjPrevPensionFile.ibusMember == null)
                aobjPrevPensionFile.LoadMember();
            if (aobjPrevPensionFile.ibusPlan == null)
                aobjPrevPensionFile.LoadPlan();
            DataTable ldtbPersonTffrTiaaService = ldtbPensionFileMembersTIAATFFRService.AsEnumerable().Where(o =>
                      o.Field<int>("person_id") == aobjPrevPensionFile.icdoActuaryFilePensionDetail.person_id).AsDataTable();

            aobjPrevPensionFile.ibusPersonAccount.idtbListTFFRTIAA = ldtbPersonTffrTiaaService;
            aobjPrevPensionFile.ibusPersonAccount.LoadTotalVSC();

            if (aobjCurrentPensionFile.ibusPersonAccount == null)
                aobjCurrentPensionFile.LoadPersonAccount();
            if (aobjCurrentPensionFile.ibusMember == null)
                aobjCurrentPensionFile.LoadMember();
            if (aobjCurrentPensionFile.ibusPlan == null)
                aobjCurrentPensionFile.LoadPlan();
            aobjCurrentPensionFile.ibusPersonAccount.idtbListTFFRTIAA = ldtbPersonTffrTiaaService;
            aobjCurrentPensionFile.ibusPersonAccount.icdoPersonAccount.Total_VSC = aobjPrevPensionFile.ibusPersonAccount.icdoPersonAccount.Total_VSC;

            if (aobjPrevPensionFile.icdoActuaryFilePensionDetail.payee_status_value != null)
            {
                lstrPreviousPayeeStatus = busGlobalFunctions.GetData2ByCodeValue(2203, aobjPrevPensionFile.icdoActuaryFilePensionDetail.payee_status_value, iobjPassInfo);
            }
            if (aobjCurrentPensionFile.icdoActuaryFilePensionDetail.payee_status_value != null)
            {
                lstrCurrentPayeeStatus = busGlobalFunctions.GetData2ByCodeValue(2203, aobjCurrentPensionFile.icdoActuaryFilePensionDetail.payee_status_value, iobjPassInfo);
            }
            //set date of decrement
            if (aobjCurrentPensionFile.icdoActuaryFilePensionDetail.payee_status_value != aobjPrevPensionFile.icdoActuaryFilePensionDetail.payee_status_value)
            {//If Payee staus changed,set status effective date as decrement date
                if (aobjCurrentPensionFile.ibusPayeeAccount == null)
                    aobjCurrentPensionFile.LoadPayeeAccount();
                if (aobjCurrentPensionFile.ibusPayeeAccount.ibusPayeeAccountActiveStatus == null)
                    aobjCurrentPensionFile.ibusPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                aobjCurrentPensionFile.icdoActuaryFilePensionDetail.date_of_decrement
                    = aobjCurrentPensionFile.ibusPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_effective_date;
            }//If plan participation staus changed,set history_change_date as decrement date
            else if (aobjCurrentPensionFile.icdoActuaryFilePensionDetail.plan_participation_status_value != aobjPrevPensionFile.icdoActuaryFilePensionDetail.plan_participation_status_value)
            {
                if (aobjCurrentPensionFile.ibusPersonAccount == null)
                    aobjCurrentPensionFile.LoadPersonAccount();
                aobjCurrentPensionFile.icdoActuaryFilePensionDetail.date_of_decrement
                    = aobjCurrentPensionFile.ibusPersonAccount.icdoPersonAccount.history_change_date;
            }//If employment staus changed,set employment detail start date as decrement date
            else if (aobjCurrentPensionFile.icdoActuaryFilePensionDetail.employment_status_value != aobjPrevPensionFile.icdoActuaryFilePensionDetail.employment_status_value)
            {
                aobjCurrentPensionFile.icdoActuaryFilePensionDetail.date_of_decrement
                    = aobjCurrentPensionFile.icdoActuaryFilePensionDetail.employment_start_date;
            }
            //if member is not vested previous year and current plan participation status is Withdrwan,then decrement value is "Non-Vested Withdrawal"
            if (aobjCurrentPensionFile.icdoActuaryFilePensionDetail.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementWithDrawn)
            {
                if ((busPersonBase.CheckIsPersonVested(aobjPrevPensionFile.ibusPlan.icdoPlan.plan_id, aobjPrevPensionFile.ibusPlan.icdoPlan.plan_code,
                                                        aobjPrevPensionFile.ibusPlan.icdoPlan.benefit_provision_id, busConstant.ApplicationBenefitTypeRetirement,
                                                        aobjPrevPensionFile.ibusPersonAccount.icdoPersonAccount.Total_VSC,
                                                        aobjPrevPensionFile.getmemberage(),
                                                        ibusActuaryFileHeader.icdoActuaryFileHeader.effective_date.AddYears(1), false, ibusActuaryFileHeader.icdoActuaryFileHeader.effective_date.AddYears(1).AddMonths(-1).GetLastDayofMonth(), //PIR 14646 - Vesting logic changes
                                                        aobjPrevPensionFile.ibusPersonAccount, iobjPassInfo, aobjDBCacheData: aobjDBCacheData)))
                {

                    return busConstant.DecrementReasonVestedWithdrawal;
                }
                else  //if member is vested previous year and current plan participation status is Withdrwan,then decrement value is "Vested Withdrawal"
                {
                    return busConstant.DecrementReasonNonVestedWithdrawal;
                }
            }
            //if the current ‘Plan Participation Status’ = ‘Pre-Retirement Death’ AND Contributing ‘Employment End Date’ is on or after Date of Death
            //then decrement value is Death While Active
            if ((aobjCurrentPensionFile.icdoActuaryFilePensionDetail.employment_status_value == busConstant.EmploymentStatusContributing ||
                aobjCurrentPensionFile.icdoActuaryFilePensionDetail.employment_status_value == "TERM") &&
                aobjPrevPensionFile.icdoActuaryFilePensionDetail.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled && //as per mail from satya
                aobjCurrentPensionFile.icdoActuaryFilePensionDetail.plan_participation_status_value == busConstant.PlanParticipationStatusPreRetirementDeath &&
                aobjCurrentPensionFile.ibusMember.icdoPerson.date_of_death != DateTime.MinValue &&
                aobjCurrentPensionFile.icdoActuaryFilePensionDetail.emp_dt_chk == busConstant.EmploymentEndDateAfterDeath)
            {
                return busConstant.DecrementReasonDeathWhileActive;
            }
            // if the current ‘Payee Status’ = ‘Receiving’ AND Benefit Type = ‘Disability’

            if (!string.IsNullOrEmpty(lstrCurrentPayeeStatus) &&
                lstrCurrentPayeeStatus == busConstant.PayeeAccountStatusReceiving)
            {
                if (aobjCurrentPensionFile.ibusPayeeAccount == null)
                    aobjCurrentPensionFile.LoadPayeeAccount();
                if (aobjCurrentPensionFile.ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                {
                    aobjCurrentPensionFile.icdoActuaryFilePensionDetail.date_of_decrement
                        = aobjCurrentPensionFile.ibusPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_effective_date;
                    return busConstant.DecrementReasonDisabilityRetirement;
                }
            }
            //‘Plan Participation’ = ‘Suspended’ AND Member = ‘Vested’
            if (aobjCurrentPensionFile.icdoActuaryFilePensionDetail.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended)
            {
                if ((busPersonBase.CheckIsPersonVested(aobjCurrentPensionFile.icdoActuaryFilePensionDetail.plan_id, aobjCurrentPensionFile.ibusPlan.icdoPlan.plan_code,
                                   aobjCurrentPensionFile.ibusPlan.icdoPlan.benefit_provision_id, busConstant.ApplicationBenefitTypeRetirement,
                                   aobjCurrentPensionFile.ibusPersonAccount.icdoPersonAccount.Total_VSC,
                                   aobjCurrentPensionFile.getmemberage(),
                                   ibusActuaryFileHeader.icdoActuaryFileHeader.effective_date, false, ibusActuaryFileHeader.icdoActuaryFileHeader.effective_date.AddMonths(-1).GetLastDayofMonth(), //PIR 14646 - Vesting logic changes
                                   aobjCurrentPensionFile.ibusPersonAccount, iobjPassInfo, aobjDBCacheData: aobjDBCacheData)))
                {
                    return busConstant.DecrementReasonVestedTermNoWithdrawal;
                }

                else //‘Plan Participation’ = ‘Suspended’ AND Member = ‘Non-Vested’
                {
                    return busConstant.DecrementReasonNonVestedTermNoWithdrawal;
                }
            }
            //if previous Plan Participation Status = ‘Enrolled’ or ‘Suspended’ AND ‘Payee Status’ is NULL
            // and current ‘Payee Status’ = ‘Receiving’ AND ‘Benefit Type’ = ‘Retirement’
            //systest pir 2485 : need to check approved, receiving, suspended, review payee status
            if (aobjCurrentPensionFile.icdoActuaryFilePensionDetail.payee_status_value != null
                && aobjPrevPensionFile.icdoActuaryFilePensionDetail.payee_status_value == null)
            {
                if ((lstrCurrentPayeeStatus == busConstant.PayeeAccountStatusReceiving || lstrCurrentPayeeStatus == busConstant.PayeeAccountStatusApproved ||
                    lstrCurrentPayeeStatus == busConstant.PayeeAccountStatusReview || lstrCurrentPayeeStatus == busConstant.PayeeAccountStatusSuspended) &&
                    aobjCurrentPensionFile.ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement &&
                    (aobjPrevPensionFile.icdoActuaryFilePensionDetail.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended
                    || aobjPrevPensionFile.icdoActuaryFilePensionDetail.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled))
                {
                    return busConstant.DecrementReasonServiceRetirement;
                }
            }
            //‘Plan Participation Status’ = ‘Pre-Retirement Death’ AND ‘Employment End Date’ is before Date of Death
            if (aobjCurrentPensionFile.icdoActuaryFilePensionDetail.plan_participation_status_value == busConstant.PlanParticipationStatusPreRetirementDeath && 
                aobjPrevPensionFile.icdoActuaryFilePensionDetail.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended &&
                aobjCurrentPensionFile.icdoActuaryFilePensionDetail.emp_dt_chk == busConstant.EmploymentEndDateBeforeDeath) //as per mail from satya
            {
                return busConstant.DecrementReasonPriorToRetirement;                
            }
            //if current ‘Payee Status’ = ‘Suspended’.
            if (lstrCurrentPayeeStatus == busConstant.PayeeAccountStatusSuspended)
            {
                return busConstant.DecrementReasonSuspensionofPension;
            }
            if ((lstrPreviousPayeeStatus == busConstant.PayeeAccountStatusRetirmentRecieving
                || lstrPreviousPayeeStatus == busConstant.PayeeAccountStatusRetirmentDCRecieving)
                && (lstrCurrentPayeeStatus == busConstant.PayeeAccountStatusRetirementPaymentCompleted))
            {
                return busConstant.DecrementReasonPaymentCompleted;
            }
            // ‘Payee Account benefit Type’ = ‘Post Retirement Death’ AND ‘Payee Status’ = ‘Payment Complete’ AND ‘Date of Death’ is not NULL 
            if (lstrCurrentPayeeStatus == busConstant.PayeeAccountStatusRetirementPaymentCompleted)
            {
                if (aobjCurrentPensionFile.ibusPayeeAccount == null)
                {
                    aobjCurrentPensionFile.LoadPayeeAccount();
                }
                if (aobjCurrentPensionFile.ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath)
                {
                    if (aobjCurrentPensionFile.ibusPayeeAccount.ibusApplication == null)
                        aobjCurrentPensionFile.ibusPayeeAccount.LoadApplication();
                    if (aobjCurrentPensionFile.ibusPayeeAccount.ibusApplication.ibusPerson == null)
                    {
                        aobjCurrentPensionFile.ibusPayeeAccount.ibusApplication.ibusPerson = new busPerson();
                        aobjCurrentPensionFile.ibusPayeeAccount.ibusApplication.ibusPerson.FindPerson(aobjCurrentPensionFile.ibusPayeeAccount.ibusApplication.icdoBenefitApplication.member_person_id);
                    }
                    if (aobjCurrentPensionFile.ibusPayeeAccount.ibusApplication.ibusPerson.icdoPerson.date_of_death != DateTime.MinValue)
                    {
                        return busConstant.DecrementReasonPostRetirementDeath;
                    }
                }
            }
            // ‘Payee Account Benefit Type’ = ‘Post Retirement Death’ AND ‘Payee Account Status’ = ‘Payments Complete’  AND ‘Payee Account Benefit Option’ = ‘xx term certain’ 

            if (lstrCurrentPayeeStatus == busConstant.PayeeAccountStatusRetirementPaymentCompleted)
            {
                if (aobjCurrentPensionFile.ibusPayeeAccount == null)
                {
                    aobjCurrentPensionFile.LoadPayeeAccount();
                }
                if (aobjCurrentPensionFile.ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath)
                {
                    DataTable ldtbBenefitOption = iobjPassInfo.isrvDBCache.GetCodeDescription(aobjCurrentPensionFile.icdoActuaryFilePensionDetail.benefit_option_id,
                        aobjCurrentPensionFile.icdoActuaryFilePensionDetail.benefit_option_value);
                    if (ldtbBenefitOption.Rows.Count > 0 && ldtbBenefitOption.Rows[0]["data1"] != DBNull.Value)
                    {
                        return busConstant.DecrementReasonEndOfTermCertainPeriod;
                    }
                }
            }
            // ‘Payee Account Benefit Type’ = ‘Post Retirement Death’ AND ‘Payee Account Status’ = ‘Payments Complete’  AND ‘Payee Account Benefit Option’ = ‘xx term certain’ 

            if (lstrCurrentPayeeStatus == busConstant.PayeeAccountStatusRetirementPaymentCompleted)
            {
                if (aobjCurrentPensionFile.ibusPayeeAccount == null)
                {
                    aobjCurrentPensionFile.LoadPayeeAccount();
                }
                if (aobjCurrentPensionFile.ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value != busConstant.ApplicationBenefitTypeRefund)
                {
                    DataTable ldtbBenefitOption = iobjPassInfo.isrvDBCache.GetCodeDescription(aobjCurrentPensionFile.icdoActuaryFilePensionDetail.benefit_option_id,
                        aobjCurrentPensionFile.icdoActuaryFilePensionDetail.benefit_option_value);
                    if (ldtbBenefitOption.Rows.Count > 0 && ldtbBenefitOption.Rows[0]["data1"] == DBNull.Value)
                    {
                        return busConstant.DecrementReasonPaymentCompleted;
                    }
                }
            }
            //Transferred to DC
            if (aobjCurrentPensionFile.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusTransferDC)
            {
                return busConstant.DecrementReasonTransferredDC;
            }//Transferred to TFFR
            else if (aobjCurrentPensionFile.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusTransferToTFFR)
            {
                return busConstant.DecrementReasonTransferredTFFR;
            }//Transferred to TIAA CREF
            else if (aobjCurrentPensionFile.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementTransferredTIAACREF)
            {
                return busConstant.DecrementReasonTransferredTIAACREF;
            }//Cancelled
            else if (aobjCurrentPensionFile.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirmentCancelled ||
                lstrCurrentPayeeStatus == busConstant.PayeeAccountStatusCancelled)
            {
                return busConstant.DecrementReasonCancelled;
            }
            return lstrReason;
        }
        private void CreateBatchReports()
        {
            DataTable ldtReportResult = busBase.Select("cdoActuaryFileHeader.rptActuaryNonRetiredRecap",
                            new object[1] { ibusActuaryFileHeader.icdoActuaryFileHeader.actuary_file_header_id });
            if (ldtReportResult.Rows.Count > 0)
            {
                CreateReport("rptActuaryNonRetiredRecap.rpt", ldtReportResult);
                idlgUpdateProcessLog("Actuary Non-Retired Recap Report generated successfully", "INFO", istrProcessName);
            }

            CreatePayeeRecapreport();

            if (idtPensionFileTable.Rows.Count > 0)
            {
                CreateReport("rptActuaryPensionPlanFileReport.rpt", idtPensionFileTable);
                idlgUpdateProcessLog("Pension File Report generated successfully", "INFO", istrProcessName);
            }

            if (idtDecrementResultTable.Rows.Count > 0)
            {
                CreateReport("rptActuaryDecrementFileReport.rpt", idtDecrementResultTable);
                idlgUpdateProcessLog("Decrement File Report generated successfully", "INFO", istrProcessName);
            }
        }

        private void CreatePayeeRecapreport()
        {
            DataTable ldtReportResult1 = busBase.Select("cdoActuaryFileHeader.rptActuaryPayeeRecapReport",
                            new object[1] { ibusActuaryFileHeader.icdoActuaryFileHeader.actuary_file_header_id });
            if (ldtReportResult1.Rows.Count > 0)
            {
                if (ibusActuaryFileHeader.icdoActuaryFileHeader.file_type_value == busConstant.AdhocActuaryFileTypeRHICFile)
                    CreateReportWithPrefix("rptActuaryPayeeRecapReport.rpt", ldtReportResult1, "RHIC_");
                else
                    CreateReport("rptActuaryPayeeRecapReport.rpt", ldtReportResult1);
                idlgUpdateProcessLog("Actuary Payee Recap Report generated successfully", "INFO", istrProcessName);
            }
        }

        //Create New Data Table for Decrment file Report
        public DataTable CreateNewDecrementFileDataTable()
        {
            DataTable ldtbReportTable = new DataTable();
            DataColumn ldc1 = new DataColumn("person_id", Type.GetType("System.Int32"));
            DataColumn ldc2 = new DataColumn("ssn", Type.GetType("System.String"));
            DataColumn ldc3 = new DataColumn("account_relation_value", Type.GetType("System.String"));
            DataColumn ldc4 = new DataColumn("last_name", Type.GetType("System.String"));
            DataColumn ldc5 = new DataColumn("first_name", Type.GetType("System.String"));
            DataColumn ldc6 = new DataColumn("current_plan_participation_code_value", Type.GetType("System.String"));
            DataColumn ldc7 = new DataColumn("previous_plan_participation_code_value", Type.GetType("System.String"));
            DataColumn ldc18 = new DataColumn("current_plan_participation_value_formatted", Type.GetType("System.String"));
            DataColumn ldc19 = new DataColumn("previous_plan_participation_value_formatted", Type.GetType("System.String"));
            DataColumn ldc8 = new DataColumn("current_employment_status_value", Type.GetType("System.String"));
            DataColumn ldc9 = new DataColumn("previous_employment_status_value", Type.GetType("System.String"));
            DataColumn ldc10 = new DataColumn("current_payee_status_value", Type.GetType("System.String"));
            DataColumn ldc11 = new DataColumn("previous_payee_status_value", Type.GetType("System.String"));
            DataColumn ldc12 = new DataColumn("benefit_option_value", Type.GetType("System.String"));
            DataColumn ldc13 = new DataColumn("plan_name", Type.GetType("System.String"));
            DataColumn ldc14 = new DataColumn("plan_id", Type.GetType("System.Int32"));
            DataColumn ldc16 = new DataColumn("date_of_decrement", Type.GetType("System.DateTime"));
            DataColumn ldc17 = new DataColumn("decrement_reason", Type.GetType("System.String"));
            DataColumn ldc20 = new DataColumn("current_payee_status_data2", Type.GetType("System.String"));
            DataColumn ldc21 = new DataColumn("previous_payee_status_data2", Type.GetType("System.String"));
            ldtbReportTable.Columns.Add(ldc1);
            ldtbReportTable.Columns.Add(ldc2);
            ldtbReportTable.Columns.Add(ldc3);
            ldtbReportTable.Columns.Add(ldc4);
            ldtbReportTable.Columns.Add(ldc5);
            ldtbReportTable.Columns.Add(ldc6);
            ldtbReportTable.Columns.Add(ldc7);
            ldtbReportTable.Columns.Add(ldc8);
            ldtbReportTable.Columns.Add(ldc9);
            ldtbReportTable.Columns.Add(ldc10);
            ldtbReportTable.Columns.Add(ldc11);
            ldtbReportTable.Columns.Add(ldc12);
            ldtbReportTable.Columns.Add(ldc13);
            ldtbReportTable.Columns.Add(ldc14);
            ldtbReportTable.Columns.Add(ldc16);
            ldtbReportTable.Columns.Add(ldc17);
            ldtbReportTable.Columns.Add(ldc18);
            ldtbReportTable.Columns.Add(ldc19);
            ldtbReportTable.Columns.Add(ldc20);
            ldtbReportTable.Columns.Add(ldc21);
            ldtbReportTable.TableName = busConstant.ReportTableName;
            return ldtbReportTable;
        }

        public void AddToNewDataRowDecrmentFile(busPensionFile aobjPensionFile)
        {
            DataRow dr = idtDecrementResultTable.NewRow();
            dr["person_id"] = aobjPensionFile.icdoActuaryFilePensionDetail.person_id;
            dr["ssn"] = aobjPensionFile.icdoActuaryFilePensionDetail.ssn;
            dr["account_relation_value"] = aobjPensionFile.icdoActuaryFilePensionDetail.account_relation_value;
            dr["last_name"] = aobjPensionFile.icdoActuaryFilePensionDetail.last_name;
            dr["first_name"] = aobjPensionFile.icdoActuaryFilePensionDetail.first_name;
            dr["current_plan_participation_code_value"] = aobjPensionFile.icdoActuaryFilePensionDetail.current_plan_participation_code_value;
            dr["previous_plan_participation_code_value"] = aobjPensionFile.icdoActuaryFilePensionDetail.previous_plan_participation_code_value;
            dr["current_plan_participation_value_formatted"] = aobjPensionFile.icdoActuaryFilePensionDetail.current_plan_participation_value_formatted;
            dr["previous_plan_participation_value_formatted"] = aobjPensionFile.icdoActuaryFilePensionDetail.previous_plan_participation_value_formatted;
            dr["current_employment_status_value"] = aobjPensionFile.icdoActuaryFilePensionDetail.current_employment_status_value;
            dr["previous_employment_status_value"] = aobjPensionFile.icdoActuaryFilePensionDetail.previous_employment_status_value;
            dr["current_payee_status_value"] = aobjPensionFile.icdoActuaryFilePensionDetail.current_payee_status_value;
            dr["previous_payee_status_value"] = aobjPensionFile.icdoActuaryFilePensionDetail.previous_payee_status_value;
            dr["benefit_option_value"] = aobjPensionFile.icdoActuaryFilePensionDetail.benefit_option_value;
            dr["plan_id"] = aobjPensionFile.icdoActuaryFilePensionDetail.plan_id;
            dr["plan_name"] = aobjPensionFile.icdoActuaryFilePensionDetail.plan_name;
            dr["date_of_decrement"] = aobjPensionFile.icdoActuaryFilePensionDetail.date_of_decrement;
            dr["decrement_reason"] = aobjPensionFile.icdoActuaryFilePensionDetail.decrement_reason;
            dr["current_payee_status_data2"] = busGlobalFunctions.GetData2ByCodeValue(2203, aobjPensionFile.icdoActuaryFilePensionDetail.current_payee_status_value, iobjPassInfo);
            dr["previous_payee_status_data2"] = busGlobalFunctions.GetData2ByCodeValue(2203, aobjPensionFile.icdoActuaryFilePensionDetail.previous_payee_status_value, iobjPassInfo);
            idtDecrementResultTable.Rows.Add(dr);
        }
        //Create New Data Table for Pension file Report
        public DataTable CreateNewPensionFileDataTable()
        {
            DataTable ldtbReportTable = new DataTable();
            DataColumn ldc1 = new DataColumn("person_id", Type.GetType("System.Int32"));
            DataColumn ldc2 = new DataColumn("ssn", Type.GetType("System.String"));
            DataColumn ldc3 = new DataColumn("account_relation_value", Type.GetType("System.String"));
            DataColumn ldc4 = new DataColumn("last_name", Type.GetType("System.String"));
            DataColumn ldc5 = new DataColumn("first_name", Type.GetType("System.String"));
            DataColumn ldc6 = new DataColumn("plan_participation_status_value", Type.GetType("System.String"));
            DataColumn ldc49 = new DataColumn("plan_participation_value_formatted", Type.GetType("System.String"));
            DataColumn ldc7 = new DataColumn("application_status_value", Type.GetType("System.String"));
            DataColumn ldc8 = new DataColumn("employment_status_value", Type.GetType("System.String"));
            DataColumn ldc9 = new DataColumn("employment_type_value", Type.GetType("System.String"));
            DataColumn ldc41 = new DataColumn("employment_type_description", Type.GetType("System.String"));
            DataColumn ldc10 = new DataColumn("payee_status_value", Type.GetType("System.String"));
            DataColumn ldc11 = new DataColumn("Hourly", Type.GetType("System.String"));
            DataColumn ldc15 = new DataColumn("Seasonal", Type.GetType("System.String"));
            DataColumn ldc42 = new DataColumn("hourly_description", Type.GetType("System.String"));
            DataColumn ldc43 = new DataColumn("Seasonal_description", Type.GetType("System.String"));
            DataColumn ldc12 = new DataColumn("benefit_option_value", Type.GetType("System.String"));
            DataColumn ldc13 = new DataColumn("Planid", Type.GetType("System.String"));
            DataColumn ldc44 = new DataColumn("plan_name", Type.GetType("System.String"));
            DataColumn ldc45 = new DataColumn("person_account_id", Type.GetType("System.Int32"));
            DataColumn ldc46 = new DataColumn("payee_account_id", Type.GetType("System.Int32"));
            DataColumn ldc47 = new DataColumn("org_id", Type.GetType("System.Int32"));
            DataColumn ldc48 = new DataColumn("plan_id", Type.GetType("System.Int32"));
            DataColumn ldc16 = new DataColumn("marital_status_value", Type.GetType("System.String"));
            DataColumn ldc14 = new DataColumn("date_of_birth", Type.GetType("System.DateTime"));
            DataColumn ldc17 = new DataColumn("plan_participation_start_date", Type.GetType("System.DateTime"));
            DataColumn ldc18 = new DataColumn("pension_service_credit", Type.GetType("System.Decimal"));
            DataColumn ldc19 = new DataColumn("total_vested_service_credit", Type.GetType("System.Decimal"));
            DataColumn ldc20 = new DataColumn("spouse_dob", Type.GetType("System.DateTime"));
            DataColumn ldc21 = new DataColumn("retirement_date", Type.GetType("System.DateTime"));
            DataColumn ldc22 = new DataColumn("joint_annuitant_dob", Type.GetType("System.DateTime"));
            DataColumn ldc23 = new DataColumn("MemberGenderCode", Type.GetType("System.String"));
            DataColumn ldc24 = new DataColumn("SpouseGenderCode", Type.GetType("System.String"));
            DataColumn ldc25 = new DataColumn("org_code", Type.GetType("System.String"));
            DataColumn ldc26 = new DataColumn("total_salary", Type.GetType("System.Decimal"));
            DataColumn ldc27 = new DataColumn("account_balance", Type.GetType("System.Decimal"));
            DataColumn ldc28 = new DataColumn("total_vested_service_credit_ba", Type.GetType("System.Decimal"));
            DataColumn ldc29 = new DataColumn("pension_service_credit_ba", Type.GetType("System.Decimal"));
            DataColumn ldc30 = new DataColumn("accrued_benefit_calculation", Type.GetType("System.Decimal"));
            DataColumn ldc31 = new DataColumn("gross_current_monthly_benefit_amount", Type.GetType("System.Decimal"));
            DataColumn ldc32 = new DataColumn("purchased_service_credit_sum", Type.GetType("System.Decimal"));
            DataColumn ldc33 = new DataColumn("ssli_or_uniform_income_commencement_age", Type.GetType("System.Decimal"));
            DataColumn ldc34 = new DataColumn("estimated_ssli_benefit_amount", Type.GetType("System.Decimal"));
            DataColumn ldc35 = new DataColumn("vested_er_amount", Type.GetType("System.Decimal"));
            DataColumn ldc36 = new DataColumn("ssli_change_date", Type.GetType("System.DateTime"));
            DataColumn ldc37 = new DataColumn("travellers_base_benefit_amount", Type.GetType("System.Decimal"));
            DataColumn ldc38 = new DataColumn("travellers_cumulative_cola_amount", Type.GetType("System.Decimal"));
            DataColumn ldc39 = new DataColumn("minimum_guarantee_amount", Type.GetType("System.Decimal"));
            DataColumn ldc40 = new DataColumn("amount_paid_ltd", Type.GetType("System.Decimal"));
            DataColumn ldc50 = new DataColumn("employment_status_value_formatted", Type.GetType("System.String"));
            DataColumn ldc51 = new DataColumn("payee_status_data2", Type.GetType("System.String"));
            DataColumn ldc52 = new DataColumn("hourly_value", Type.GetType("System.String"));
            DataColumn ldc53 = new DataColumn("seasonal_value", Type.GetType("System.String"));
            DataColumn ldc54 = new DataColumn("benefit_account_type_value", Type.GetType("System.String"));
            DataColumn ldc55 = new DataColumn("benefit_sub_type_value", Type.GetType("System.String"));
            DataColumn ldc56 = new DataColumn("option_factor", Type.GetType("System.Decimal"));
            DataColumn ldc57 = new DataColumn("ben_acc_owner_perslinkID", Type.GetType("System.Int32"));
            DataColumn ldc58 = new DataColumn("graduated_benefit_option_percentage", Type.GetType("System.String"));

            
            
            ldtbReportTable.Columns.Add(ldc1);
            ldtbReportTable.Columns.Add(ldc2);
            ldtbReportTable.Columns.Add(ldc3);
            ldtbReportTable.Columns.Add(ldc4);
            ldtbReportTable.Columns.Add(ldc5);
            ldtbReportTable.Columns.Add(ldc6);
            ldtbReportTable.Columns.Add(ldc7);
            ldtbReportTable.Columns.Add(ldc8);
            ldtbReportTable.Columns.Add(ldc9);
            ldtbReportTable.Columns.Add(ldc10);
            ldtbReportTable.Columns.Add(ldc11);
            ldtbReportTable.Columns.Add(ldc12);
            ldtbReportTable.Columns.Add(ldc13);
            ldtbReportTable.Columns.Add(ldc14);
            ldtbReportTable.Columns.Add(ldc15);
            ldtbReportTable.Columns.Add(ldc16);
            ldtbReportTable.Columns.Add(ldc17);
            ldtbReportTable.Columns.Add(ldc18);
            ldtbReportTable.Columns.Add(ldc19);
            ldtbReportTable.Columns.Add(ldc20);
            ldtbReportTable.Columns.Add(ldc21);
            ldtbReportTable.Columns.Add(ldc22);
            ldtbReportTable.Columns.Add(ldc23);
            ldtbReportTable.Columns.Add(ldc24);
            ldtbReportTable.Columns.Add(ldc25);
            ldtbReportTable.Columns.Add(ldc26);
            ldtbReportTable.Columns.Add(ldc27);
            ldtbReportTable.Columns.Add(ldc28);
            ldtbReportTable.Columns.Add(ldc29);
            ldtbReportTable.Columns.Add(ldc30);
            ldtbReportTable.Columns.Add(ldc31);
            ldtbReportTable.Columns.Add(ldc32);
            ldtbReportTable.Columns.Add(ldc33);
            ldtbReportTable.Columns.Add(ldc34);
            ldtbReportTable.Columns.Add(ldc35);
            ldtbReportTable.Columns.Add(ldc36);
            ldtbReportTable.Columns.Add(ldc37);
            ldtbReportTable.Columns.Add(ldc38);
            ldtbReportTable.Columns.Add(ldc39);
            ldtbReportTable.Columns.Add(ldc40);
            ldtbReportTable.Columns.Add(ldc41);
            ldtbReportTable.Columns.Add(ldc42);
            ldtbReportTable.Columns.Add(ldc43);
            ldtbReportTable.Columns.Add(ldc44);
            ldtbReportTable.Columns.Add(ldc45);
            ldtbReportTable.Columns.Add(ldc46);
            ldtbReportTable.Columns.Add(ldc47);
            ldtbReportTable.Columns.Add(ldc48);
            ldtbReportTable.Columns.Add(ldc49);
            ldtbReportTable.Columns.Add(ldc50);
            ldtbReportTable.Columns.Add(ldc51);
            ldtbReportTable.Columns.Add(ldc52);
            ldtbReportTable.Columns.Add(ldc53);
            ldtbReportTable.Columns.Add(ldc54);
            ldtbReportTable.Columns.Add(ldc55);
            ldtbReportTable.Columns.Add(ldc56);
            ldtbReportTable.Columns.Add(ldc57);
            ldtbReportTable.Columns.Add(ldc58);
            ldtbReportTable.TableName = busConstant.ReportTableName;
            return ldtbReportTable;
        }
    }
}
