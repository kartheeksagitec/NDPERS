using System;
using System.Collections.Generic;
using System.Text;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using System.Collections;
using System.Collections.ObjectModel;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CorBuilder;
using Sagitec.DataObjects;
using System.Data;
using Sagitec.CustomDataObjects;
using NeoSpin.CustomDataObjects;
using System.Linq;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busHIPAAHealthBCBSFileOut : busFileBaseOut
    {
        public DateTime ldtTodaysDate
        {
            get { return DateTime.Now; }
        }

        public override void InitializeFile()
        {
            istrFileName = "834.NDPERS." + _lstrProviderOrgCode + "." + ldtTodaysDate.ToString(busConstant.DateFormat) + busConstant.FileFormattxt;
        }

        private int _lintTransactionSetCount;
        public int lintTransactionSetCount
        {
            get { return _lintTransactionSetCount; }
            set { _lintTransactionSetCount = value; }
        }

        public string lstrTransactionSetCount
        {
            get { return Convert.ToString(_lintTransactionSetCount); }
        }

        // Unique number everytime file generates. 
        private string _lstrInterchangeControlNo;
        public string lstrInterchangeControlNo
        {
            get { return _lstrInterchangeControlNo; }
            set { _lstrInterchangeControlNo = value; }
        }

        public DataTable idtbHealthData { get; set; }
        public Collection<int> iclbHealth { get; set; }

        // Loads the number from System constant Code value 52 and increment it.
        // Updates the InterchangeControl number in Constants code value.
        private void LoadAndUpdatesInterchangeControlNo()
        {
            cdoCodeValue lcdoCodeValue = new cdoCodeValue();
            lcdoCodeValue = busGlobalFunctions.GetCodeValueDetails(busConstant.SystemConstantCodeID, busConstant.SystemConstantISAHealthBCBS);
            int lintISA = Convert.ToInt32(lcdoCodeValue.data1) + 1;
            _lstrInterchangeControlNo = Convert.ToString(lintISA).PadLeft(9, '0');
            lcdoCodeValue.data1 = _lstrInterchangeControlNo;
            lcdoCodeValue.Update();
        }

        private string _lstrProviderOrgCode;
        public string lstrProviderOrgCode
        {
            get { return _lstrProviderOrgCode; }
            set { _lstrProviderOrgCode = value; }
        }

        private void LoadHealthProviderOrgCode(DataRow adrRow)
        {
            // Since we have single provider for Health
            busPersonAccount lbusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            lbusPersonAccount.icdoPersonAccount.LoadData(adrRow);

            DataTable ldtbResult = busBase.Select("cdoPersonAccount.LoadActiveProviderByPlan",
                                      new object[2] { lbusPersonAccount.icdoPersonAccount.plan_id, ldtTodaysDate });
            if (ldtbResult.Rows.Count > 0)
            {
                if (!Convert.IsDBNull(ldtbResult.Rows[0]["org_code"]))
                    _lstrProviderOrgCode = ldtbResult.Rows[0]["org_code"].ToString();
            }
        }

        public int iintBCBS_ST_Count { get; set; }

        public int iintSTCount { get; set; }

        public int iintTransactionSetControlNumber { get; set; }

        public string istrIsCoverageNeedsToSplit { get; set; }

        public string istrTransactionSetControlNumber
        {
            get
            {
                return iintTransactionSetControlNumber.ToString().PadLeft(4, '0');
            }
        }

        public bool iblnSTFlag { get; set; }

        public bool iblnIsDetail { get; set; }

        public string istrGroupNumber { get; set; }

        public string istrMaitenanceCode { get; set; }

        private void LoadMaintenanceCode(busPersonAccountGhdv abusHealth)
        {
            istrMaitenanceCode = string.Empty;
            if (abusHealth.icdoPersonAccountGhdv.is_level_of_coverage_modified == busConstant.Flag_Yes)
                istrMaitenanceCode = "001";
            else if (abusHealth.icdoPersonAccountGhdv.reason_value == busConstant.ChangeReasonNewHire)
                istrMaitenanceCode = "021";
            else if ((abusHealth.icdoPersonAccount.header_plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended) ||
                (abusHealth.icdoPersonAccount.header_plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceCancelled))
                istrMaitenanceCode = "024";
            else if (abusHealth.icdoPersonAccountGhdv.is_reinstatement == busConstant.Flag_Yes) // Re-instatement, Current Status Enrolled and Previous Status Suspended.
                istrMaitenanceCode = "025";
            else
                istrMaitenanceCode = "030";
        }

        private busBase iobjBase { get; set; }

        private DataTable idtAllDependents { get; set; }

        private DataTable idtAllAddressess { get; set; }

        private DataTable idtAllEmployments { get; set; }

        private DataTable idtAllOrgPlan { get; set; }

        private DataTable idtAllDependentMember { get; set; }

        public DataTable idtbCachedRateRef { get; set; }

        private DataTable idtbCachedRateStructureRef { get; set; }

        private bool iblnIsLayout5010 { get; set; }

        private void LoadDBCache()
        {
            idtbCachedRateRef = busGlobalFunctions.LoadHealthRateRefCacheData(iobjPassInfo);
            idtbCachedRateStructureRef = busGlobalFunctions.LoadHealthRateStructureCacheData(iobjPassInfo);
        }

        public void LoadHealthBCBS(DataTable adtbHealthBCBS)
        {
            iclbHealth = (Collection<int>)iarrParameters[0];
            idtbHealthData = (DataTable)iarrParameters[1];
            idtAllDependents = (DataTable)iarrParameters[2];
            idtAllAddressess = (DataTable)iarrParameters[3];
            idtAllEmployments = (DataTable)iarrParameters[4];
            idtAllOrgPlan = (DataTable)iarrParameters[5];
            idtAllDependentMember = (DataTable)iarrParameters[6];
            iintBCBS_ST_Count = Convert.ToInt32(busGlobalFunctions.GetData1ByCodeValue(52, "BCST", iobjPassInfo));
            istrIsCoverageNeedsToSplit = Convert.ToString(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.CoverageCodeSplitFlag, iobjPassInfo));
            iobjBase = new busBase();
            LoadDBCache();
            iintSTCount = 0;
            iintTransactionSetControlNumber = 1;
            LoadAndUpdatesInterchangeControlNo();
            if (idtbHealthData.Rows.Count > 0)
            {
                LoadHealthProviderOrgCode(idtbHealthData.Rows[0]);
            }
            if (Convert.ToString(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.New5010HIPAALayoutForBCBS, iobjPassInfo)) == busConstant.Flag_Yes) //pir 8570
                iblnIsLayout5010 = true;
        }

        // SYSTEST PIR ID - 1505 - Update the flag so that the file wont pick this record next time.
        public override void FinalizeFile()
        {
            base.FinalizeFile();

            // Write Footer Details
            istrRecord = string.Empty;

            if (iblnSTFlag)
                istrRecord += GetSTFooter() + "\r\n";
            istrRecord += GetFooterDetails();
            iswrOut.Write(istrRecord);

            // UAT PIR ID 2170
            CreateBCBSReport();

            // ***** No More Flag *****
            /// Perfomance issue results in mass update of flag
            /// Enrolled Accounts should sent to file everytime. Other status records should sent to file only once. 
            /// Updates only the other than Enrolled status record's IS_MODIFIED_AFTER_BCBS_FILE_SENT_FLAG to "N"
            DBFunction.DBNonQuery("cdoPersonAccountGhdv.Update_HIPAA_BCBS_Flag", new object[1] { busConstant.PlanIdGroupHealth }, iobjPassInfo.iconFramework,
                            iobjPassInfo.itrnFramework);
        }

        public override void BeforeWriteRecord()
        {
            if (iobjDetail is int)
            {
                if (!iblnIsDetail)
                {
                    // Printing the Header Section only once.
                    istrRecord = GetHeaderDetails();
                    iblnIsDetail = true;
                }

                // Pir 6015
                DataRow ldrRow = idtbHealthData.Rows[int.Parse(iobjDetail.ToString())];
                busPersonAccountGhdv lbusHealth = new busPersonAccountGhdv
                {
                    icdoPersonAccount = new cdoPersonAccount(),
                    icdoPersonAccountGhdv = new cdoPersonAccountGhdv(),
                    ibusPerson = new busPerson { icdoPerson = new cdoPerson() },
                    ibusPlan = new busPlan { icdoPlan = new cdoPlan() },
                    ibusOrgPlan = new busOrgPlan { icdoOrgPlan = new cdoOrgPlan() },
                    iclbPersonAccountDependent = new Collection<busPersonAccountDependent>(),
                    ibusPersonEmploymentDetail = new busPersonEmploymentDetail
                    {
                        icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail(),
                        ibusPersonEmployment = new busPersonEmployment
                        {
                            icdoPersonEmployment = new cdoPersonEmployment(),
                            ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() }
                        }
                    },
                    ibusPaymentElection = new busPersonAccountPaymentElection { icdoPersonAccountPaymentElection = new cdoPersonAccountPaymentElection() }
                };

                lbusHealth.icdoPersonAccount.LoadData(ldrRow);
                lbusHealth.icdoPersonAccountGhdv.LoadData(ldrRow);
                lbusHealth.ibusPerson.icdoPerson.LoadData(ldrRow);
                lbusHealth.ibusPlan.icdoPlan.LoadData(ldrRow);
                lbusHealth.ibusPaymentElection.icdoPersonAccountPaymentElection.LoadData(ldrRow);

                // Load the Dependents
                DataRow[] ldtbDependents = busGlobalFunctions.FilterTable(idtAllDependents,
                                            busConstant.DataType.Numeric, "PERSON_ACCOUNT_ID", lbusHealth.icdoPersonAccount.person_account_id);
                lbusHealth.ibusPerson.iclbPersonDependentByDependent = new Collection<busPersonDependent>();
                foreach (DataRow ldtrRow in ldtbDependents)
                {
                    busPersonAccountDependent lobjPADependent = new busPersonAccountDependent
                    {
                        icdoPersonAccountDependent = new cdoPersonAccountDependent(),
                        icdoPersonDependent = new cdoPersonDependent()
                    };
                    lobjPADependent.icdoPersonAccountDependent.LoadData(ldtrRow);
                    lobjPADependent.icdoPersonDependent.LoadData(ldtrRow);
                    lbusHealth.iclbPersonAccountDependent.Add(lobjPADependent);
                }
                lbusHealth.iclbPersonAccountDependent = lbusHealth.iclbPersonAccountDependent.Where(lobj =>
                                                        lobj.icdoPersonAccountDependent.is_bcbs_file_sent_flag != busConstant.Flag_Yes).ToList().ToCollection();

                // Load all the Dependent member
                //PROD PIR ID 6761  -- Start
                DataRow[] ldtbDependentMember = busGlobalFunctions.FilterTable(idtAllDependentMember,
                                            busConstant.DataType.Numeric, "DEPENDENT_PERSLINK_ID", lbusHealth.icdoPersonAccount.person_id);
                lbusHealth.ibusPerson.iclbPersonDependentByDependent = new Collection<busPersonDependent>();
                foreach (DataRow ldtr in ldtbDependentMember)
                {
                    busPersonDependent lobjDependent = new busPersonDependent { icdoPersonDependent = new cdoPersonDependent() };
                    lbusHealth.ibusMemberGHDVForDependent = new busPersonAccountGhdv { icdoPersonAccountGhdv = new cdoPersonAccountGhdv(), icdoPersonAccount = new cdoPersonAccount() };
                    lbusHealth.ibusMemberGHDVForDependent.icdoPersonAccountGhdv.LoadData(ldtr);
                    lbusHealth.ibusMemberGHDVForDependent.icdoPersonAccount.LoadData(ldtr);
                    lobjDependent.icdoPersonDependent.LoadData(ldtr);
                    if (!lbusHealth.ibusPerson.iclbPersonDependentByDependent.Contains(lobjDependent))
                        lbusHealth.ibusPerson.iclbPersonDependentByDependent.Add(lobjDependent);
                }
                foreach (busPersonDependent lobjDependent in lbusHealth.ibusPerson.iclbPersonDependentByDependent)
                {
                    lobjDependent.iclbPersonAccountDependent = new Collection<busPersonAccountDependent>();
                    foreach (DataRow ldtr in ldtbDependentMember)
                    {
                        // The Member can be dependent to more than one person
                        if (lobjDependent.icdoPersonDependent.person_id == Convert.ToInt32(ldtr["PERSON_ID"]))
                        {
                            busPersonAccountDependent lobjPADependent = new busPersonAccountDependent { icdoPersonAccountDependent = new cdoPersonAccountDependent() };
                            lobjPADependent.icdoPersonAccountDependent.LoadData(ldtr);
                            lobjDependent.iclbPersonAccountDependent.Add(lobjPADependent);
                        }
                    }
                }
                //PROD PIR ID 6761  -- End

                // Load the addresses
                DataRow[] ldtrAddress = busGlobalFunctions.FilterTable(idtAllAddressess, busConstant.DataType.Numeric, "PERSON_ID", lbusHealth.icdoPersonAccount.person_id);
                lbusHealth.ibusPerson.iclbPersonAddress = iobjBase.GetCollection<busPersonAddress>(ldtrAddress, "icdoPersonAddress");
                
                // PIR 9029 - get latest address irrespective of end date
                lbusHealth.ibusPerson.ibusPersonCurrentAddress = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };
                if (lbusHealth.ibusPerson.iclbPersonAddress.Count > 0)
                    lbusHealth.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress = lbusHealth.ibusPerson.iclbPersonAddress[0].icdoPersonAddress;

                // UAT PIR ID 1431
                // Load the Employment
                DataRow[] ldtrPAEmployment = busGlobalFunctions.FilterTable(idtAllEmployments,
                                                busConstant.DataType.Numeric, "PERSON_ACCOUNT_ID", lbusHealth.icdoPersonAccount.person_account_id);
                if (ldtrPAEmployment.Count() > 0)
                {
                    lbusHealth.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.LoadData(ldtrPAEmployment[0]);
                    lbusHealth.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.LoadData(ldtrPAEmployment[0]);
                    lbusHealth.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.LoadData(ldtrPAEmployment[0]);
                }
                if (lbusHealth.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id > 0)
                    lbusHealth.icdoPersonAccount.person_employment_dtl_id = lbusHealth.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;
                else if (lbusHealth.icdoPersonAccount.from_person_account_id > 0)
                {
                    // SYS PIR ID 2647 - For Dependent COBRA case the employment should be loaded from the Member's
                    busPersonAccountGhdv lobjMemberGHDV = new busPersonAccountGhdv { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };
                    lobjMemberGHDV.FindPersonAccount(lbusHealth.icdoPersonAccount.from_person_account_id);
                    lobjMemberGHDV.LoadPersonAccountGHDV();
                    lbusHealth.LoadEmploymentDetailByDate(ldtTodaysDate, lobjMemberGHDV, true, true);

                    lbusHealth.LoadPersonEmploymentDetail();
                    lbusHealth.ibusPersonEmploymentDetail.LoadPersonEmployment();
                    lbusHealth.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                }
                lbusHealth.iclbAccountEmploymentDetail = new Collection<busPersonAccountEmploymentDetail>();
                foreach (DataRow ldtr in ldtrPAEmployment)
                {
                    busPersonAccountEmploymentDetail lobjPAEmpDtl = new busPersonAccountEmploymentDetail
                    {
                        icdoPersonAccountEmploymentDetail = new cdoPersonAccountEmploymentDetail(),
                        ibusEmploymentDetail = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() }
                    };
                    lobjPAEmpDtl.icdoPersonAccountEmploymentDetail.LoadData(ldtr);
                    lobjPAEmpDtl.ibusEmploymentDetail.icdoPersonEmploymentDetail.LoadData(ldtr);
                    lbusHealth.iclbAccountEmploymentDetail.Add(lobjPAEmpDtl);
                }

                // Load OrgPlan
                DataRow[] ldtrOrgPlan = busGlobalFunctions.FilterTable(idtAllOrgPlan,
                                        busConstant.DataType.Numeric, "ORG_ID", lbusHealth.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id);
                if (ldtrOrgPlan.Count() > 0)
                    lbusHealth.ibusOrgPlan.icdoOrgPlan.LoadData(ldtrOrgPlan[0]);

                lbusHealth.idtbCachedRateRef = idtbCachedRateRef;
                lbusHealth.idtbCachedRateStructureRef = idtbCachedRateStructureRef;

                //PROD PIR ID 6761  -- Start
                lbusHealth.LoadPlanEffectiveDate();
                lbusHealth.iblnIsActiveMember = false; lbusHealth.iblnIsCobraMember = false; lbusHealth.iblnIsDependentCobra = false;
                lbusHealth.iblnIsActiveMember = lbusHealth.LoadEmploymentDetailByDate(lbusHealth.idtPlanEffectiveDate, false);
                if (!lbusHealth.iblnIsActiveMember)
                    lbusHealth.iblnIsCobraMember = lbusHealth.LoadEmploymentDetailByDate(lbusHealth.idtPlanEffectiveDate, true);
                var lcdoMemberPersonAccount = new cdoPersonAccount();
                if (!lbusHealth.iblnIsActiveMember)
                    lbusHealth.iblnIsDependentCobra = lbusHealth.ibusPerson.IsDependentCobra(busConstant.PlanIdGroupHealth, lbusHealth.idtPlanEffectiveDate, ref lcdoMemberPersonAccount);
                //PROD PIR ID 6761  -- End                    
                //lbusHealth.DetermineEnrollmentAndLoadObjects(lbusHealth.idtPlanEffectiveDate, false); //PROD PIR ID 6761

                if (lbusHealth.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                    lbusHealth.LoadRateStructureForUserStructureCode();
                else
                    lbusHealth.LoadRateStructure();

                #region PROD PIR ID 933
                foreach (busPersonAccountDependent lobjPADependent in lbusHealth.iclbPersonAccountDependent)
                    lobjPADependent.LoadDependentInfo();

                if (lbusHealth.IsCoverageNeedsToSplit() && istrIsCoverageNeedsToSplit == busConstant.Flag_Yes)
                {
                    int iintMedicareSplitCount = 0, iintNonMedicareSplitCount = 0;
                    string lstrMedicareCode = string.Empty, lstrNonMedicareCode = string.Empty;
                    if (lbusHealth.icdoPersonAccountGhdv.is_medicare_split)
                        iintMedicareSplitCount = 1;
                    else
                        iintNonMedicareSplitCount = 1;
                    foreach (busPersonAccountDependent lobjPADependent in lbusHealth.iclbPersonAccountDependent)
                    {
                        if (lobjPADependent.icdoPersonDependent.is_medicare_split)
                            iintMedicareSplitCount += 1;
                        else
                            iintNonMedicareSplitCount += 1;
                    }
                    lbusHealth.SplitCoverage(iintMedicareSplitCount, iintNonMedicareSplitCount, ref lstrMedicareCode, ref lstrNonMedicareCode);

                    if (lbusHealth.icdoPersonAccountGhdv.is_medicare_split)
                        lbusHealth.icdoPersonAccountGhdv.splitted_coverage_code = lstrMedicareCode;
                    else
                        lbusHealth.icdoPersonAccountGhdv.splitted_coverage_code = lstrNonMedicareCode;
                    
                    foreach (busPersonAccountDependent lobjPADependent in lbusHealth.iclbPersonAccountDependent)
                    {
                        
                        // Split Coverage Code
                        if (lobjPADependent.icdoPersonDependent.is_medicare_split)
                            lobjPADependent.icdoPersonDependent.splitted_coverage_code = lstrMedicareCode;
                        else
                            lobjPADependent.icdoPersonDependent.splitted_coverage_code = lstrNonMedicareCode;

                        // Set Subscriber's SSN
                        if (lobjPADependent.icdoPersonDependent.is_medicare_split == lbusHealth.icdoPersonAccountGhdv.is_medicare_split)
                            lobjPADependent.icdoPersonDependent.subscriber_dependent_ssn = lbusHealth.ibusPerson.icdoPerson.ssn;
                        else
                        {
                            //  First Hierarchy to Spouse
                            var lvarDependent = lbusHealth.iclbPersonAccountDependent.Where(lobj =>
                                            lobj.icdoPersonDependent.relationship_value == busConstant.PersonBeneficiaryRelationshipSpouse &&
                                            lobj.icdoPersonDependent.dependent_ssn.IsNotNullOrEmpty() &&
                                            lobj.icdoPersonDependent.is_medicare_split != lbusHealth.icdoPersonAccountGhdv.is_medicare_split
                                            ).FirstOrDefault();

                            var lvarEldestDependent = lbusHealth.iclbPersonAccountDependent.Where(lobj =>
                                                    lobj.icdoPersonDependent.dependent_ssn.IsNotNullOrEmpty() &&
                                                    lobj.icdoPersonDependent.is_medicare_split != lbusHealth.icdoPersonAccountGhdv.is_medicare_split).
                                                    OrderBy(lobj => lobj.icdoPersonDependent.dependent_DOB).FirstOrDefault();

                            if (lvarDependent.IsNotNull())// iclbPersonAccountDependent
                            {
                                if (lvarDependent.icdoPersonDependent.person_dependent_id == lobjPADependent.icdoPersonAccountDependent.person_dependent_id)
                                {
                                    lobjPADependent.icdoPersonDependent.is_subscriber_dependent = true;
                                                                      
                                }
                                lobjPADependent.icdoPersonDependent.subscriber_dependent_ssn = lvarDependent.icdoPersonDependent.dependent_ssn;
                            }
                            else if(lvarEldestDependent.IsNotNull())
                            {
                                if (lvarEldestDependent.icdoPersonDependent.person_dependent_id == lobjPADependent.icdoPersonAccountDependent.person_dependent_id)
                                {
                                    lobjPADependent.icdoPersonDependent.is_subscriber_dependent = true;

                                }
                                lobjPADependent.icdoPersonDependent.subscriber_dependent_ssn = lvarEldestDependent.icdoPersonDependent.dependent_ssn;
                            }
                        }
                    }
                    // The sort needs to be done only if any of the dependent has medicare info as Member enrollment
                    if (lbusHealth.iclbPersonAccountDependent.Where(o => o.icdoPersonDependent.is_medicare_split == lbusHealth.icdoPersonAccountGhdv.is_medicare_split).Any())
                        lbusHealth.iclbPersonAccountDependent = busGlobalFunctions.Sort<busPersonAccountDependent>(
                                ("icdoPersonDependent.is_medicare_split " + (lbusHealth.icdoPersonAccountGhdv.is_medicare_split ? "desc" : "asc")), lbusHealth.iclbPersonAccountDependent);
                }
                else
                {
                    foreach (busPersonAccountDependent lobjPADependent in lbusHealth.iclbPersonAccountDependent)
                        lobjPADependent.icdoPersonDependent.subscriber_dependent_ssn = lbusHealth.ibusPerson.icdoPerson.ssn;
                }


                #endregion
                if (iintSTCount < iintBCBS_ST_Count)
                {
                    if (!iblnSTFlag)
                    {
                        istrRecord += GetSTHeader();
                        iblnSTFlag = true;
                    }
                    // SYS PIR ID 2605: If the Count increases the BCBS ST Count, that should come in the next ST-SE section.
                    if ((iintSTCount + 1 + lbusHealth.iclbPersonAccountDependent.Count) > iintBCBS_ST_Count)
                    {
                        iintSTCount = 0;
                        istrRecord += GetSTFooter();
                        _lintTransactionSetCount = 0;
                        iintTransactionSetControlNumber += 1;
                        istrRecord += "\r\n";
                        istrRecord += GetSTHeader();
                    }
                }
                else
                {
                    iintSTCount = 0;
                    istrRecord += GetSTFooter();
                    _lintTransactionSetCount = 0;
                    iintTransactionSetControlNumber += 1;
                    istrRecord += "\r\n";
                    istrRecord += GetSTHeader();
                }
                // Load the Segments for the Subscriber ie, Member
                istrRecord += GetSubscribersSegments(lbusHealth);
                foreach (busPersonAccountDependent lobjPADependent in lbusHealth.iclbPersonAccountDependent)
                {
                    // Load the Segments for all the Dependents
                    istrRecord += "\r\n";
                    istrRecord += GetDependentsSegments(lobjPADependent, lbusHealth);

                    // SYS PIR ID 2614                            
                    if (lobjPADependent.icdoPersonAccountDependent.end_date_no_null.Date <= ldtTodaysDate.Date)
                    {
                        lobjPADependent.icdoPersonAccountDependent.is_bcbs_file_sent_flag = busConstant.Flag_Yes;
                        lobjPADependent.icdoPersonAccountDependent.Update();
                    }
                }


            }
        }

        private string GetHeaderDetails()
        {
            string lstrHeader = string.Empty;
            lstrHeader = "ISA*00*          *00*          *ZZ*ND00913        *30*450173185      *" + ldtTodaysDate.ToString("yyMMdd") + "*" + ldtTodaysDate.ToString("HHmm") + "*" +
                        (iblnIsLayout5010 ? "^" : "U") + "*" + (iblnIsLayout5010 ? "00501" : "00401") + "*" + lstrInterchangeControlNo + "*1*P*:~" + "\r\n";
            lstrHeader += "GS*BE*ND00913*" + (iblnIsLayout5010 ? "0057*" : "0001*") + ldtTodaysDate.ToString("yyyyMMdd") + "*" + ldtTodaysDate.ToString("HHmm") + "*1*X*" +
                        (iblnIsLayout5010 ? "005010X220A1" : "004010X095A1") + "~" + "\r\n";
            return lstrHeader;
        }

        private string GetFooterDetails()
        {
            string lstrFooter = string.Empty;
            _lintTransactionSetCount += 8; // includes the header sections' count
            lstrFooter += "GE*" + iintTransactionSetControlNumber.ToString() + "*1~" + "\r\n";
            lstrFooter += "IEA*1*" + lstrInterchangeControlNo + "~";
            return lstrFooter;
        }

        private string GetSTHeader()
        {
            string lstrSTHeader = string.Empty;
            lstrSTHeader += "ST*834*" + istrTransactionSetControlNumber + (iblnIsLayout5010 ? "*005010X220A1" : "") + "~\r\n";
            lstrSTHeader += "BGN*00*" + istrTransactionSetControlNumber + "*" + ldtTodaysDate.ToString("yyyyMMdd") + "*" + ldtTodaysDate.ToString("HHmmssdd") + "*CS***4~" + "\r\n";
            lstrSTHeader += "REF*38*NPER" + "~\r\n";
            lstrSTHeader += "DTP*382*D8*" + ldtTodaysDate.ToString("yyyyMMdd") + "~\r\n";
            lstrSTHeader += "N1*P5*NDPERS*FI*450282090~" + "\r\n";
            lstrSTHeader += "N1*IN*BCBSND and Affiliated Companies*FI*450173185" + "~\r\n";
            lstrSTHeader += "N1*TV*BCBSND and Affiliated Companies*FI*450173185" + "~\r\n";
            _lintTransactionSetCount += 7;
            return lstrSTHeader;
        }

        private string GetSTFooter()
        {
            string lstrSTFooter = string.Empty;
            _lintTransactionSetCount += 1;
            lstrSTFooter += "SE*" + lstrTransactionSetCount + "*" + istrTransactionSetControlNumber + "~";
            return lstrSTFooter;
        }

        // Add Employee's Segments
        private string GetSubscribersSegments(busPersonAccountGhdv abusHealth)
        {
            string lstrResult = string.Empty;

            lstrResult = GetINS(true, abusHealth.IsCOBRAValueSelected(), null, abusHealth, false);
            lstrResult += GetREFSubsriber(abusHealth.ibusPerson.icdoPerson.ssn);
            lstrResult += GetREFMemberPolicy(abusHealth, null, true);
            if (abusHealth.icdoPersonAccountGhdv.medicare_claim_no.IsNotNullOrEmpty())
                lstrResult += GetREFMedicare(abusHealth.icdoPersonAccountGhdv.medicare_claim_no);
            lstrResult += GetDTPMedicare(true, null, abusHealth);
            lstrResult += GetNM1(true, null, abusHealth);
            lstrResult += GetN3(abusHealth);
            lstrResult += GetN4(abusHealth);
            lstrResult += GetDMG(true, null, abusHealth);
            lstrResult += GetHD(abusHealth);
            lstrResult += GetDTP(true, abusHealth);
            lstrResult += GetDTP303(true, abusHealth);
            //lstrResult += "~\r\n";
            return lstrResult;
        }

        // Add Dependent's Segments
        private string GetDependentsSegments(busPersonAccountDependent aobjPADependent, busPersonAccountGhdv abusHealth)
        {
            string lstrResult = string.Empty;
            if (aobjPADependent.icdoPersonDependent.is_subscriber_dependent)
                lstrResult += GetINS(false, false, aobjPADependent, abusHealth, true);
            else
                lstrResult += GetINS(false, false, aobjPADependent, abusHealth, false);
            lstrResult += GetREFSubsriber(aobjPADependent.icdoPersonDependent.subscriber_dependent_ssn);
            lstrResult += GetREFMemberPolicy(abusHealth, aobjPADependent, false);
            if (aobjPADependent.icdoPersonDependent.medicare_claim_no.IsNotNullOrEmpty())
            {
                lstrResult += GetREFMedicare(aobjPADependent.icdoPersonDependent.medicare_claim_no);
                lstrResult += GetDTPMedicare(false, aobjPADependent, abusHealth);
            }
            lstrResult += GetNM1(false, aobjPADependent, abusHealth);
            if (aobjPADependent.icdoPersonDependent.is_subscriber_dependent)
            {
                lstrResult += GetN3(abusHealth);
                lstrResult += GetN4(abusHealth);
            }
            lstrResult += GetDMG(false, aobjPADependent, abusHealth);
            lstrResult += GetHD(abusHealth);
            lstrResult += GetDTP(false, abusHealth, aobjPADependent);
            lstrResult += GetDTP303(false, abusHealth, aobjPADependent);
            //lstrResult += "~";
            return lstrResult;
        }

        #region Segment Methods

        // INS - Member Level Detail
        private string GetINS(bool ablnIsSubscriber, bool ablnIsCOBRA, busPersonAccountDependent aobjPADependent, busPersonAccountGhdv abusHealth, bool ablnIsSubscriberDependent)
        {
            iintSTCount += 1;
            _lintTransactionSetCount += 1;
            string lstrINS = "INS*";
            // Response Code
            // Individual Relationship Code
            if (ablnIsSubscriber || ablnIsSubscriberDependent)
            {
                lstrINS += busConstant.Flag_Yes + "*";
                lstrINS += "18*";
            }
            else
            {
                lstrINS += busConstant.Flag_No + "*";
                lstrINS += GetINS02(aobjPADependent.icdoPersonDependent.relationship_value);
            }

            // Maintenance Type Code
            LoadMaintenanceCode(abusHealth);
            lstrINS += istrMaitenanceCode + "*";

            //PIR 11940 - Maintenance Reason Code - Changing the logic according to Change Reason
            if (abusHealth.icdoPersonAccountGhdv.reason_value == busConstant.BCBSFileCodeDivorce) 
                lstrINS += "01*";
            else if (abusHealth.icdoPersonAccountGhdv.reason_value == busConstant.BCBSFileCodeDeath)
                lstrINS += "03*";
            else if (abusHealth.icdoPersonAccountGhdv.reason_value == busConstant.BCBSFileCodeEligibleRetirement)
                lstrINS += "04*";
            else if (abusHealth.icdoPersonAccountGhdv.reason_value == busConstant.BCBSFileCodeEndofCOBRAPeriod)
                lstrINS += "09*";
            else if (abusHealth.icdoPersonAccountGhdv.reason_value == busConstant.BCBSFileCodeMemberCancellation)
                lstrINS += "26*";
            else if (abusHealth.icdoPersonAccountGhdv.reason_value == busConstant.BCBSFileCodeRemoveDependent)
                lstrINS += "31*"; 
            else if (abusHealth.icdoPersonAccountGhdv.reason_value == busConstant.BCBSFileCodeLeaveofAbsence)
                lstrINS += "37*";
            else if (abusHealth.icdoPersonAccountGhdv.reason_value == busConstant.BCBSFileCodeUnpaidLeaveofAbsence)
                lstrINS += "38*";
            else if (abusHealth.icdoPersonAccountGhdv.reason_value == busConstant.BCBSFileCodeNonPayment)
                lstrINS += "59*";
            else if (abusHealth.icdoPersonAccountGhdv.reason_value == busConstant.ReasonValueMarriage) //Logic changed for married according to change reason
                lstrINS += "32*";
            else if (abusHealth.icdoPersonAccountGhdv.reason_value == busConstant.BCBSEmploymentTermination)
                lstrINS += "08*";
            else
                lstrINS += "XN*";
            
            // Benefit Status Code
            if (ablnIsCOBRA)
                lstrINS += "C*";
            else
                lstrINS += "A*";

            // Medicare Plan Code
            if (ablnIsSubscriber)
            {
                if (abusHealth.icdoPersonAccountGhdv.medicare_part_a_effective_date != DateTime.MinValue)
                {
                    if (abusHealth.icdoPersonAccountGhdv.medicare_part_b_effective_date != DateTime.MinValue)
                        lstrINS += "C*";
                    else
                        lstrINS += "A*";
                }
                else if (abusHealth.icdoPersonAccountGhdv.medicare_part_b_effective_date != DateTime.MinValue)
                    lstrINS += "B*";
                else
                    lstrINS += "E*";
            }
            else
            {
                if (aobjPADependent.icdoPersonDependent.medicare_part_a_effective_date != DateTime.MinValue)
                {
                    if (aobjPADependent.icdoPersonDependent.medicare_part_b_effective_date != DateTime.MinValue)
                        lstrINS += "C*";
                    else
                        lstrINS += "A*";
                }
                else if (aobjPADependent.icdoPersonDependent.medicare_part_b_effective_date != DateTime.MinValue)
                    lstrINS += "B*";
                else
                    lstrINS += "E*";
            }

            // COBRA Qualifying
            if (ablnIsCOBRA)
                lstrINS += "1*";
            else
                lstrINS += "*";

            if (ablnIsSubscriber || ablnIsSubscriberDependent)
            {
                // Employment Status Code
                if (abusHealth.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent)
                    lstrINS += "FT*";
                else if (abusHealth.icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeRetiree)
                    lstrINS += "RT*";
                else
                    lstrINS += "*";

                lstrINS += "**";

                // INS12 Date Time Period
                if (abusHealth.ibusPerson.icdoPerson.date_of_death != DateTime.MinValue)
                    lstrINS += "D8*" + abusHealth.ibusPerson.icdoPerson.date_of_death.ToString(busConstant.DateFormatD8);
                //PROD PIR - 4668 :- Remove the below field
                //else if (abusHealth.ibusPerson.icdoPerson.ms_change_date != DateTime.MinValue)
                //    lstrINS += "D8*" + abusHealth.ibusPerson.icdoPerson.ms_change_date.ToString(busConstant.DateFormatD8);
                else
                    lstrINS += "*";
            }
            else
            {
                // Employment Status Code
                lstrINS += "*";

                // Student Status
                // PROD PIR ID 7366
                lstrINS += "*";
                //if (aobjPADependent.icdoPersonDependent.full_time_student_flag == busConstant.Flag_Yes)
                //    lstrINS += "F*";
                //else
                //    lstrINS += "N*";

                // Handicapped Response Code
                if (aobjPADependent.icdoPersonDependent.relationship_value == busConstant.DependentRelationshipDisabledChild)
                    lstrINS += busConstant.Flag_Yes + "*";
                else
                    lstrINS += busConstant.Flag_No + "*";
            }
            while (busGlobalFunctions.IsLastCharacterAsterisk(lstrINS))
                lstrINS = busGlobalFunctions.RemoveLastCharacter(lstrINS);
            lstrINS += "~\r\n";
            return lstrINS;
        }

        // PROD PIR ID 5746
        private string GetINS02(string astrRelationshipValue)
        {
            string lstrINS02 = string.Empty;
            if (astrRelationshipValue == busConstant.DependentRelationshipSpouse)
                lstrINS02 = "01*";
            else if (astrRelationshipValue == busConstant.DependentRelationshipGrandChild)
                lstrINS02 = "05*";
            else if (astrRelationshipValue == busConstant.DependentRelationshipAdoptiveChild)
                lstrINS02 = "09*";
            else if (astrRelationshipValue == busConstant.DependentRelationshipStepChild)
                lstrINS02 = "17*";
            else if (astrRelationshipValue == busConstant.DependentRelationshipChild)
                lstrINS02 = "19*";
            else if (astrRelationshipValue == busConstant.DependentRelationshipExSpouse)
                lstrINS02 = "25*";
            else if (astrRelationshipValue == busConstant.DependentRelationshipDisabledChild)
                lstrINS02 = "38*";
            else if (astrRelationshipValue == busConstant.DependentRelationshipOthers)
                lstrINS02 = "26*";
            else if (astrRelationshipValue == busConstant.DependentRelationshipLegalGuardian)//pir 7269
                lstrINS02 = "31*";
            else
                lstrINS02 = "G8*";
            return lstrINS02;
        }

        // REF - Medicare
        private string GetREFMedicare(string astrMediclaimNo)
        {
            _lintTransactionSetCount += 1;
            string lstrREF = "REF*"; // Segment Header
            lstrREF += "F6*";   // Reference Identification Qualifier
            lstrREF += astrMediclaimNo;  // Reference Identification          
            if (busGlobalFunctions.IsLastCharacterAsterisk(lstrREF))
                lstrREF = busGlobalFunctions.RemoveLastCharacter(lstrREF);
            lstrREF += "~\r\n";
            return lstrREF;
        }

        // REF - Subsriber Number
        private string GetREFSubsriber(string astrSSN)
        {
            _lintTransactionSetCount += 1;
            string lstrREF = "REF*"; // Segment Header
            lstrREF += "0F*";   // Reference Identification Qualifier
            lstrREF += astrSSN;    // Reference Identification
            if (busGlobalFunctions.IsLastCharacterAsterisk(lstrREF))
                lstrREF = busGlobalFunctions.RemoveLastCharacter(lstrREF);
            lstrREF += "~\r\n";
            return lstrREF;
        }

        // NM1 - Member Name
        private string GetNM1(bool ablnIsSubscriber, busPersonAccountDependent aobjPADependent, busPersonAccountGhdv abusHealth)
        {
            _lintTransactionSetCount += 1;
            // Segment Header
            string lstrNM1 = "NM1*";

            // Entity Identifier Code
            lstrNM1 += "IL*";

            // Entity Type Qualifier
            lstrNM1 += "1*";

            if (ablnIsSubscriber)
            {
                // Last Name
                if (!string.IsNullOrEmpty(abusHealth.ibusPerson.icdoPerson.last_name))
                    lstrNM1 += abusHealth.ibusPerson.icdoPerson.last_name.Trim().ToUpper() + "*";
                else
                    lstrNM1 += "*";

                // First Name
                if (!string.IsNullOrEmpty(abusHealth.ibusPerson.icdoPerson.first_name))
                    lstrNM1 += abusHealth.ibusPerson.icdoPerson.first_name.Trim().ToUpper() + "*";
                else
                    lstrNM1 += "*";

                // Middle Name
                if (!string.IsNullOrEmpty(abusHealth.ibusPerson.icdoPerson.middle_name))
                    lstrNM1 += abusHealth.ibusPerson.icdoPerson.middle_name.Trim().ToUpper() + "*";
                else
                    lstrNM1 += "*";

                // Prefix Name
                if (!string.IsNullOrEmpty(abusHealth.ibusPerson.icdoPerson.name_prefix_description))
                    lstrNM1 += abusHealth.ibusPerson.icdoPerson.name_prefix_description.Trim().ToUpper() + "*";
                else
                    lstrNM1 += "*";

                // Suffix Name
                if (!string.IsNullOrEmpty(abusHealth.ibusPerson.icdoPerson.name_suffix_description))
                    lstrNM1 += abusHealth.ibusPerson.icdoPerson.name_suffix_description.Trim().ToUpper() + "*";
                else
                    lstrNM1 += "*";

                // Identification Code Qualifier
                if (!string.IsNullOrEmpty(abusHealth.ibusPerson.icdoPerson.ssn))
                    lstrNM1 += "34*" + abusHealth.ibusPerson.icdoPerson.ssn;
            }
            else
            {
                // Dependent's Last Name
                if (!string.IsNullOrEmpty(aobjPADependent.icdoPersonDependent.dependent_last_name))
                    lstrNM1 += aobjPADependent.icdoPersonDependent.dependent_last_name.Trim().ToUpper() + "*";
                else
                    lstrNM1 += "*";

                // Dependent's First Name
                if (!string.IsNullOrEmpty(aobjPADependent.icdoPersonDependent.dependent_first_name))
                    lstrNM1 += aobjPADependent.icdoPersonDependent.dependent_first_name.Trim().ToUpper() + "*";
                else
                    lstrNM1 += "*";

                // Dependent's Middle Name
                if (!string.IsNullOrEmpty(aobjPADependent.icdoPersonDependent.dependent_middle_name))
                    lstrNM1 += aobjPADependent.icdoPersonDependent.dependent_middle_name.Trim().ToUpper() + "*";
                else
                    lstrNM1 += "*";

                // Dependent's Prefix Name
                if (!string.IsNullOrEmpty(aobjPADependent.icdoPersonDependent.dependent_prefix_name))
                    lstrNM1 += aobjPADependent.icdoPersonDependent.dependent_prefix_name.Trim().ToUpper() + "*";
                else
                    lstrNM1 += "*";

                // Dependent's Suffix Name
                if (!string.IsNullOrEmpty(aobjPADependent.icdoPersonDependent.dependent_suffix_name))
                    lstrNM1 += aobjPADependent.icdoPersonDependent.dependent_suffix_name.Trim().ToUpper() + "*";
                else
                    lstrNM1 += "*";

                // Dependent's SSN
                if (!string.IsNullOrEmpty(aobjPADependent.icdoPersonDependent.dependent_ssn))
                    lstrNM1 += "34*" + aobjPADependent.icdoPersonDependent.dependent_ssn.ToUpper();
            }
            while (busGlobalFunctions.IsLastCharacterAsterisk(lstrNM1))
                lstrNM1 = busGlobalFunctions.RemoveLastCharacter(lstrNM1);
            lstrNM1 += "~\r\n";
            return lstrNM1;
        }

        // N3 - Member Residence Street Address
        private string GetN3(busPersonAccountGhdv abusHealth)
        {
            _lintTransactionSetCount += 1;
            string lstrN3 = "N3";
            if (!string.IsNullOrEmpty(abusHealth.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_1))
                lstrN3 += "*" + abusHealth.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_1.ToUpper().Trim();
            if (!string.IsNullOrEmpty(abusHealth.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_2))
                lstrN3 += "*" + abusHealth.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_2.ToUpper().Trim();
            if (busGlobalFunctions.IsLastCharacterAsterisk(lstrN3))
                lstrN3 = busGlobalFunctions.RemoveLastCharacter(lstrN3);
            lstrN3 += "~\r\n";
            return lstrN3;
        }

        // N4 - Member Residence City, State, Zipcode
        private string GetN4(busPersonAccountGhdv abusHealth)
        {
            _lintTransactionSetCount += 1;
            string lstrN4 = "N4*";

            // N401 City
            if (!string.IsNullOrEmpty(abusHealth.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_city))
                lstrN4 += abusHealth.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_city.ToUpper().Trim() + "*";
            else
                lstrN4 += "*";

            // N402 State        
            if (abusHealth.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_value == busConstant.US_Code_ID ||
                abusHealth.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_value == busConstant.Canada_Code_ID)
            {
                if (!string.IsNullOrEmpty(abusHealth.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_state_value))
                    lstrN4 += abusHealth.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_state_value.Trim() + "*";
                else
                    lstrN4 += "*";
            }
            else
                lstrN4 += "*";

            // N403
            string lstrAddr4ZipCode = string.IsNullOrEmpty(abusHealth.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_4_code) ?
                                        string.Empty : abusHealth.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_4_code.Trim();
            if (!string.IsNullOrEmpty(abusHealth.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_code))
                lstrN4 += busGlobalFunctions.GetValidZipCode(abusHealth.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_code, lstrAddr4ZipCode) + "*";
            else if (!string.IsNullOrEmpty(abusHealth.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.foreign_postal_code))
                lstrN4 += abusHealth.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.foreign_postal_code.Trim() + "*";
            else
                lstrN4 += "*";

            // N404
            if ((!string.IsNullOrEmpty(abusHealth.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_value)) &&
                (abusHealth.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_value != busConstant.US_Code_ID))
            {
                string lstrCountryCode = busGlobalFunctions.GetData1ByCodeValue(abusHealth.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_id,
                                                                abusHealth.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_value, iobjPassInfo);
                if (!string.IsNullOrEmpty(lstrCountryCode))
                    lstrN4 += lstrCountryCode.Trim() + "*";
            }

            while (busGlobalFunctions.IsLastCharacterAsterisk(lstrN4))
                lstrN4 = busGlobalFunctions.RemoveLastCharacter(lstrN4);
            lstrN4 += "~\r\n";
            return lstrN4;
        }

        // DMG - Member Demographics
        private string GetDMG(bool ablnIsSubscriber, busPersonAccountDependent aobjPADependent, busPersonAccountGhdv abusHealth)
        {
            _lintTransactionSetCount += 1;
            string lstrDMG = "DMG*";
            // Date Time Period Format Qualifier
            lstrDMG += "D8*";
            if (ablnIsSubscriber)
            {
                // Date Time Period
                if (abusHealth.ibusPerson.icdoPerson.date_of_birth != DateTime.MinValue)
                    lstrDMG += abusHealth.ibusPerson.icdoPerson.date_of_birth.ToString(busConstant.DateFormatD8) + "*";
                else
                    lstrDMG += "*";

                // Gender Code
                if (abusHealth.ibusPerson.icdoPerson.gender_value == busConstant.GenderTypeFemale)
                    lstrDMG += "F*";
                else if (abusHealth.ibusPerson.icdoPerson.gender_value == busConstant.GenderTypeMale)
                    lstrDMG += "M*";
                else
                    lstrDMG += "U*";

                if (abusHealth.ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried)
                    lstrDMG += "M";
                else if (abusHealth.ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusSingle)
                    lstrDMG += "S";
                else if (abusHealth.ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusDivorced)
                    lstrDMG += "D"; // PROD PIR 7095
            }
            else
            {
                // Date Time Period
                if (aobjPADependent.icdoPersonDependent.dependent_DOB != DateTime.MinValue)
                    lstrDMG += aobjPADependent.icdoPersonDependent.dependent_DOB.ToString(busConstant.DateFormatD8) + "*";
                else
                    lstrDMG += "*";

                // Gender Code
                if (aobjPADependent.icdoPersonDependent.dependent_gender == busConstant.GenderTypeFemale)
                    lstrDMG += "F*";
                else if (aobjPADependent.icdoPersonDependent.dependent_gender == busConstant.GenderTypeMale)
                    lstrDMG += "M*";
                else
                    lstrDMG += "U*";
            }
            if (busGlobalFunctions.IsLastCharacterAsterisk(lstrDMG))
                lstrDMG = busGlobalFunctions.RemoveLastCharacter(lstrDMG);
            lstrDMG += "~\r\n";
            return lstrDMG;
        }

        // HD - Health Coverage
        private string GetHD(busPersonAccountGhdv abusHealth)
        {
            _lintTransactionSetCount += 1;
            string lstrHD = "HD*";
            // Maintenance Type Code
            lstrHD += "001**";
            // Insurance Line Code
            lstrHD += "HLT*";
            // Coverage Level Code
            if (abusHealth.icdoPersonAccountGhdv.level_of_coverage_value == busConstant.VisionLevelofCoverageFamily)
                lstrHD += "FAM";
            else if (abusHealth.icdoPersonAccountGhdv.level_of_coverage_value == busConstant.VisionLevelofCoverageIndividual)
                lstrHD += "EMP";
            else if (abusHealth.icdoPersonAccountGhdv.level_of_coverage_value == busConstant.VisionLevelofCoverageIndividualChild)
                lstrHD += "ECH";
            else if (abusHealth.icdoPersonAccountGhdv.level_of_coverage_value == busConstant.VisionLevelofCoverageIndividualSpouse)
                lstrHD += "ESP";
            if (busGlobalFunctions.IsLastCharacterAsterisk(lstrHD))
                lstrHD = busGlobalFunctions.RemoveLastCharacter(lstrHD);
            lstrHD += "~\r\n";
            return lstrHD;
        }

        // DTP - Medicare
        private string GetDTPMedicare(bool ablnIsSubscriber, busPersonAccountDependent aobjPADependent, busPersonAccountGhdv abusHealth)
        {
            _lintTransactionSetCount += 1;
            string lstrDTP = "DTP*";
            if (ablnIsSubscriber)
            {
                if (abusHealth.icdoPersonAccountGhdv.medicare_claim_no.IsNotNullOrEmpty())
                {
                    DateTime ldteMedicaredate = busGlobalFunctions.GetMax(abusHealth.icdoPersonAccountGhdv.medicare_part_a_effective_date,
                                                                            abusHealth.icdoPersonAccountGhdv.medicare_part_b_effective_date);
                    if (ldteMedicaredate != DateTime.MinValue)
                    {
                        lstrDTP += "338*";  // Date Time Qualifier
                        lstrDTP += "D8*";   // Date Time Format Qualifier
                        lstrDTP += ldteMedicaredate.ToString(busConstant.DateFormatD8);
                    }
                    else
                    {
                        _lintTransactionSetCount -= 1; //pir 8458
                        return string.Empty;
                    }
                }
                else if (abusHealth.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date != DateTime.MinValue)
                {
                    lstrDTP += "336*";  // Date Time Qualifier                            
                    lstrDTP += "D8*";   // Date Time Format Qualifier
                    lstrDTP += abusHealth.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date.ToString(busConstant.DateFormatD8);
                }
                else
                {
                    _lintTransactionSetCount -= 1;
                    return string.Empty;
                }
            }
            else
            {
                DateTime ldteMedicaredate = busGlobalFunctions.GetMax(aobjPADependent.icdoPersonDependent.medicare_part_a_effective_date,
                                                                            aobjPADependent.icdoPersonDependent.medicare_part_b_effective_date);
                if (ldteMedicaredate != DateTime.MinValue)
                {
                    lstrDTP += "338*";  // Date Time Qualifier
                    lstrDTP += "D8*";   // Date Time Format Qualifier
                    lstrDTP += ldteMedicaredate.ToString(busConstant.DateFormatD8);
                }
                else
                {
                    _lintTransactionSetCount -= 1;
                    return string.Empty;
                }
            }

            if (busGlobalFunctions.IsLastCharacterAsterisk(lstrDTP))
                lstrDTP = busGlobalFunctions.RemoveLastCharacter(lstrDTP);
            lstrDTP += "~\r\n";
            return lstrDTP;
        }

        // DTP - Health Coverage Dates
        private string GetDTP(bool ablnIsSubscriber, busPersonAccountGhdv abusHealth, busPersonAccountDependent aobjPADependent = null)
        {
            _lintTransactionSetCount += 1;
            string lstrDTP = "DTP*";
            if (ablnIsSubscriber)
            {
                // Person Account Begin Date is the Latest date
                if (abusHealth.icdoPersonAccount.header_plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                {
                    lstrDTP += "348*";  // Date Time Qualifier                           
                    lstrDTP += "D8*";   // Date Time Format Qualifier
                    //UAT PIR : 2462 later of history change date or employment start date
                    DateTime ldtDTPDate = abusHealth.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date > abusHealth.icdoPersonAccount.current_plan_start_date ? abusHealth.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date : abusHealth.icdoPersonAccount.current_plan_start_date;
                    lstrDTP += ldtDTPDate.ToString(busConstant.DateFormatD8);
                }
                else if (abusHealth.ibusPerson.icdoPerson.date_of_death != DateTime.MinValue) // PROD PIR ID 6590
                {
                    lstrDTP += "349*";  // Date Time Qualifier                            
                    lstrDTP += "D8*";   // Date Time Format Qualifier
                    lstrDTP += abusHealth.ibusPerson.icdoPerson.date_of_death.ToString(busConstant.DateFormatD8);
                }
                else if (abusHealth.icdoPersonAccount.header_plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended ||
                        abusHealth.icdoPersonAccount.header_plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceCancelled) // PROD PIR ID 4242
                {
                    if ((abusHealth.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                        && (DateTime.Today.AddDays(60) < abusHealth.icdoPersonAccount.end_date))
                    {
                        lstrDTP += "348*";  // Date Time Qualifier                           
                        lstrDTP += "D8*";   // Date Time Format Qualifier
                        DateTime ldtDTPDate = abusHealth.icdoPersonAccount.start_date;
                        lstrDTP += ldtDTPDate.ToString(busConstant.DateFormatD8);
                    }
                    else
                    {
                        lstrDTP += "349*";  // Date Time Qualifier                            
                        lstrDTP += "D8*";   // Date Time Format Qualifier
                        lstrDTP += abusHealth.icdoPersonAccount.history_change_date_no_null.AddDays(-1).ToString(busConstant.DateFormatD8);
                    }
                }
            }
            else
            {
                if (aobjPADependent.icdoPersonDependent.dependent_date_of_death != DateTime.MinValue) // PROD PIR ID 6590
                {
                    lstrDTP += "349*";  // Date Time Qualifier                    
                    lstrDTP += "D8*";   // Date Time Format Qualifier
                    lstrDTP += aobjPADependent.icdoPersonDependent.dependent_date_of_death.ToString(busConstant.DateFormatD8);
                }
                else if (aobjPADependent.icdoPersonAccountDependent.end_date == DateTime.MinValue)
                {
                    lstrDTP += "348*";  // Date Time Qualifier                    
                    lstrDTP += "D8*";   // Date Time Format Qualifier
                    lstrDTP += aobjPADependent.icdoPersonAccountDependent.start_date.ToString(busConstant.DateFormatD8);
                }
                else if (DateTime.Today.AddDays(60) > aobjPADependent.icdoPersonAccountDependent.end_date)//PIR 13179
                {
                    lstrDTP += "349*";  // Date Time Qualifier                    
                    lstrDTP += "D8*";   // Date Time Format Qualifier
                    lstrDTP += aobjPADependent.icdoPersonAccountDependent.end_date.ToString(busConstant.DateFormatD8);
                }
                else
                {
                    lstrDTP += "348*";  // Date Time Qualifier                    
                    lstrDTP += "D8*";   // Date Time Format Qualifier
                    lstrDTP += aobjPADependent.icdoPersonAccountDependent.start_date.ToString(busConstant.DateFormatD8);
                }
            }
            if (busGlobalFunctions.IsLastCharacterAsterisk(lstrDTP))
                lstrDTP = busGlobalFunctions.RemoveLastCharacter(lstrDTP);
            lstrDTP += "~";
            return lstrDTP;
        }

        // DTP 303 - Married & Divorce Date Segement
        // PROD PIR ID 6590
        private string GetDTP303(bool ablnIsSubscriber, busPersonAccountGhdv abusHealth, busPersonAccountDependent aobjPADependent = null)
        {
            string lstrDTP = string.Empty;
            if (ablnIsSubscriber)
            {
                if ((abusHealth.ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried ||
                     abusHealth.ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusDivorced) &&
                     abusHealth.ibusPerson.icdoPerson.ms_change_date != DateTime.MinValue)
                {
                    _lintTransactionSetCount += 1;
                    lstrDTP += "\r\n";
                    lstrDTP += "DTP*";
                    lstrDTP += "303*";
                    lstrDTP += "D8*";
                    lstrDTP += abusHealth.ibusPerson.icdoPerson.ms_change_date.ToString(busConstant.DateFormatD8);
                    lstrDTP += "~";
                }
            }
            // Commented as part of Pir 8473
            /*else
            {
                if ((aobjPADependent.icdoPersonDependent.dependent_marital_status == busConstant.PersonMaritalStatusMarried ||
                     aobjPADependent.icdoPersonDependent.dependent_marital_status == busConstant.PersonMaritalStatusDivorced) &&
                     aobjPADependent.icdoPersonDependent.dependent_ms_change_date != DateTime.MinValue)
                {
                    _lintTransactionSetCount += 1;
                    lstrDTP += "\r\n";
                    lstrDTP += "DTP*";
                    lstrDTP += "303*";          
                    lstrDTP += "D8*";
                    lstrDTP += aobjPADependent.icdoPersonDependent.dependent_ms_change_date.ToString(busConstant.DateFormatD8);
                    lstrDTP += "~";
                }
            }*/
            return lstrDTP;
        }

        // REF - Member Policy Number
        private string GetREFMemberPolicy(busPersonAccountGhdv abusHealth, busPersonAccountDependent aobjPADependent, bool ablnIsSubscriber)
        {
            _lintTransactionSetCount += 1;
            // Segment Header
            string lstrREF = "REF*";

            // Reference Identification Qualifier
            lstrREF += "1L*";

            // Structure or Plan Code
            if ((abusHealth.icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeRetiree) ||
                (abusHealth.icdoPersonAccountGhdv.is_health_cobra) || (abusHealth.IsGroupNumber06ConditionSatisfied()))
            {
                istrGroupNumber = abusHealth.GetGroupNumberForBCBS();
                lstrREF += istrGroupNumber;
            }
            else
                lstrREF += abusHealth.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.org_code;
            if (abusHealth.icdoPersonAccountGhdv.derived_rate_structure_code.Length >= 4)
                lstrREF += abusHealth.icdoPersonAccountGhdv.derived_rate_structure_code.Substring(2, 2);
            if (ablnIsSubscriber)
            {
                if (abusHealth.icdoPersonAccountGhdv.splitted_coverage_code.IsNullOrEmpty())
                    lstrREF += abusHealth.icdoPersonAccountGhdv.coverage_code.Substring(2, 2);
                else
                    lstrREF += abusHealth.icdoPersonAccountGhdv.splitted_coverage_code;
            }
            else
            {
                if (aobjPADependent.icdoPersonDependent.splitted_coverage_code.IsNullOrEmpty())
                    lstrREF += abusHealth.icdoPersonAccountGhdv.coverage_code.Substring(2, 2); // Invalid split has to send the existing Coverage
                else
                    lstrREF += aobjPADependent.icdoPersonDependent.splitted_coverage_code;
            }

            if (busGlobalFunctions.IsLastCharacterAsterisk(lstrREF))
                lstrREF = busGlobalFunctions.RemoveLastCharacter(lstrREF);
            lstrREF += "~\r\n";
            return lstrREF;
        }

        #endregion

        #region BCBS Report
        /// UAT PIR ID 2170

        public DataTable idtbBCBS { get; set; }

        /// <summary>
        /// Initialize the New DataTable only once.
        /// </summary>
        public void InitializeColumns()
        {
            idtbBCBS = new DataTable(busConstant.ReportTableName);

            DataColumn ldc1 = new DataColumn("EMPLOYEE_ID", Type.GetType("System.Int32"));
            DataColumn ldc2 = new DataColumn("EMPLOYEE_NAME", Type.GetType("System.String"));
            DataColumn ldc3 = new DataColumn("SSN", Type.GetType("System.String"));
            DataColumn ldc4 = new DataColumn("COVERAGE_START_DATE", Type.GetType("System.DateTime"));
            DataColumn ldc5 = new DataColumn("COVERAGE_END_DATE", Type.GetType("System.DateTime"));
            DataColumn ldc6 = new DataColumn("DEPENDENT_NAME", Type.GetType("System.String"));
            DataColumn ldc7 = new DataColumn("MEDICARE_CLAIM_NO", Type.GetType("System.String"));
            DataColumn ldc8 = new DataColumn("MEDICARE_PART_A_DATE", Type.GetType("System.DateTime"));
            DataColumn ldc9 = new DataColumn("MEDICARE_PART_B_DATE", Type.GetType("System.DateTime"));
            DataColumn ldc10 = new DataColumn("DATE_OF_DEATH", Type.GetType("System.DateTime"));

            idtbBCBS.Columns.Add(ldc1);
            idtbBCBS.Columns.Add(ldc2);
            idtbBCBS.Columns.Add(ldc3);
            idtbBCBS.Columns.Add(ldc4);
            idtbBCBS.Columns.Add(ldc5);
            idtbBCBS.Columns.Add(ldc6);
            idtbBCBS.Columns.Add(ldc7);
            idtbBCBS.Columns.Add(ldc8);
            idtbBCBS.Columns.Add(ldc9);
            idtbBCBS.Columns.Add(ldc10);
        }

        public void CreateBCBSReport()
        {
            int lintEmployeeID = 1;
            InitializeColumns();
            Collection<busPersonAccountDependent> lclbTobeUpdated = new Collection<busPersonAccountDependent>();

            DataTable ldtbResult = busBase.Select("cdoPersonAccountGhdv.GetMedicareReportData", new object[] { });
            foreach (DataRow ldtrMember in ldtbResult.Rows)
            {
                DataRow[] ldtrDependents = idtAllDependents.FilterTable(busConstant.DataType.Numeric, "PERSON_ACCOUNT_ID", Convert.ToInt32(ldtrMember["PERSON_ACCOUNT_ID"]));
                if (ldtrDependents.Count() > 0)
                {
                    // Employee Detail
                    DataRow ldtrMbr = idtbBCBS.NewRow();
                    ldtrMbr["EMPLOYEE_ID"] = lintEmployeeID; // Used to group in the report.
                    // Member full Name
                    ldtrMbr["EMPLOYEE_NAME"] = Convert.ToString(ldtrMember["FIRST_NAME"]) + " " + Convert.ToString(ldtrMember["MIDDLE_NAME"]) + " " + Convert.ToString(ldtrMember["LAST_NAME"]);
                    ldtrMbr["SSN"] = Convert.ToString(ldtrMember["SSN"]);
                    if (ldtrMember["START_DATE"] != DBNull.Value)
                        ldtrMbr["COVERAGE_START_DATE"] = Convert.ToDateTime(ldtrMember["START_DATE"]);
                    if (ldtrMember["PLAN_PARTICIPATION_STATUS_VALUE"] != DBNull.Value &&
                        Convert.ToString(ldtrMember["PLAN_PARTICIPATION_STATUS_VALUE"]) == busConstant.PlanParticipationStatusInsuranceSuspended &&
                        ldtrMember["HISTORY_CHANGE_DATE"] != DBNull.Value)
                    {
                        DateTime ldteEndDate = Convert.ToDateTime(ldtrMember["HISTORY_CHANGE_DATE"]);
                        ldtrMbr["COVERAGE_END_DATE"] = ldteEndDate.AddDays(-1);
                    }
                    ldtrMbr["DEPENDENT_NAME"] = Convert.ToString(ldtrMember["FIRST_NAME"]) + " " + Convert.ToString(ldtrMember["MIDDLE_NAME"]) + " " + Convert.ToString(ldtrMember["LAST_NAME"]);
                    ldtrMbr["MEDICARE_CLAIM_NO"] = Convert.ToString(ldtrMember["MEDICARE_CLAIM_NO"]);
                    if (ldtrMember["MEDICARE_PART_A_EFFECTIVE_DATE"] != DBNull.Value)
                        ldtrMbr["MEDICARE_PART_A_DATE"] = Convert.ToDateTime(ldtrMember["MEDICARE_PART_A_EFFECTIVE_DATE"]);
                    if (ldtrMember["MEDICARE_PART_B_EFFECTIVE_DATE"] != DBNull.Value)
                        ldtrMbr["MEDICARE_PART_B_DATE"] = Convert.ToDateTime(ldtrMember["MEDICARE_PART_B_EFFECTIVE_DATE"]);
                    if (ldtrMember["DATE_OF_DEATH"] != DBNull.Value)
                        ldtrMbr["DATE_OF_DEATH"] = Convert.ToDateTime(ldtrMember["DATE_OF_DEATH"]);
                    idtbBCBS.Rows.Add(ldtrMbr);

                    // Dependents Detail
                    foreach (DataRow ldtrDependent in ldtrDependents)
                    {
                        DataRow ldtrDepnt = idtbBCBS.NewRow();
                        ldtrDepnt["EMPLOYEE_ID"] = lintEmployeeID;
                        ldtrDepnt["SSN"] = Convert.ToString(ldtrDependent["SSN"]);
                        ldtrDepnt["DEPENDENT_NAME"] = Convert.ToString(ldtrDependent["DEP_NAME"]);
                        ldtrDepnt["MEDICARE_CLAIM_NO"] = Convert.ToString(ldtrDependent["MEDICARE_CLAIM_NO"]);
                        if (ldtrDependent["START_DATE"] != DBNull.Value)
                            ldtrDepnt["COVERAGE_START_DATE"] = Convert.ToDateTime(ldtrDependent["START_DATE"]);
                        if (ldtrDependent["END_DATE"] != DBNull.Value)
                            ldtrDepnt["COVERAGE_END_DATE"] = Convert.ToDateTime(ldtrDependent["END_DATE"]);
                        if (ldtrDependent["MEDICARE_PART_A_EFFECTIVE_DATE"] != DBNull.Value)
                            ldtrDepnt["MEDICARE_PART_A_DATE"] = Convert.ToDateTime(ldtrDependent["MEDICARE_PART_A_EFFECTIVE_DATE"]);
                        if (ldtrDependent["MEDICARE_PART_B_EFFECTIVE_DATE"] != DBNull.Value)
                            ldtrDepnt["MEDICARE_PART_B_DATE"] = Convert.ToDateTime(ldtrDependent["MEDICARE_PART_B_EFFECTIVE_DATE"]);
                        if (ldtrDependent["DATE_OF_DEATH"] != DBNull.Value)
                            ldtrDepnt["DATE_OF_DEATH"] = Convert.ToDateTime(ldtrDependent["DATE_OF_DEATH"]);
                        idtbBCBS.Rows.Add(ldtrDepnt);

                        if (ldtrDependent["IS_MODIFIED_AFTER_BCBS_FILE_SENT_FLAG"].ToString() == busConstant.Flag_Yes)
                        {
                            busPersonAccountDependent lobjPADep = new busPersonAccountDependent { icdoPersonAccountDependent = new cdoPersonAccountDependent() };
                            lobjPADep.icdoPersonAccountDependent.LoadData(ldtrDependent);
                            lclbTobeUpdated.Add(lobjPADep);
                        }
                    }
                    lintEmployeeID += 1;
                }
            }

            if (idtbBCBS.Rows.Count > 0)
            {
                DataSet ldtsBCBSReport = new DataSet();
                ldtsBCBSReport.Tables.Add(idtbBCBS);
                busNeoSpinBase lobjBase = new busNeoSpinBase();
                lobjBase.CreateReport("rptBCBS.rpt", ldtsBCBSReport, string.Empty);
            }

            // Update Modified flag for Dependent screen
            foreach (busPersonAccountDependent lobjPADep in lclbTobeUpdated)
            {
                lobjPADep.icdoPersonAccountDependent.is_modified_after_bcbs_file_sent_flag = busConstant.Flag_No;
                lobjPADep.icdoPersonAccountDependent.Update();
            }
        }

        #endregion
    }
}
