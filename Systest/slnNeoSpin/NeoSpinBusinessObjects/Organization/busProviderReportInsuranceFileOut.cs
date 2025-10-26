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
using System.Linq.Expressions;
using Sagitec.ExceptionPub;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busProviderReportInsuranceFileOut : busFileBaseOut
    {
        public busProviderReportInsuranceFileOut()
        {
        }
        private bool iblnIsCurrentMonthRegularRecordExists = true;
        private string lstrProviderOrgCodeID = string.Empty;
        private string istrProviderOrgName = string.Empty; // PIR 10448
        private bool iblnIsProviderDelta = false; // PIR 10448
        private int iintMemberCount = 0;
        private int iintCounter = 0;
        private int lintProviderOrgID = 0;

        DateTime ldtEffectiveDate = DateTime.Today;

        public string istrStructureCode { get; set; }
        public string istrIdentificationCode { get; set; }
        public string istrVisionStructureCode { get; set; }

        //PIR 8872 
        private int lintCignaOrgID { 
            get {
                return  busGlobalFunctions.GetOrgIdFromOrgCode(busGlobalFunctions.GetData1ByCodeValue(1213, busConstant.CIGNAProviderCodeValue, iobjPassInfo));
                } 
            }
        public string istrPayerName { get; set; }
        public string istrPaymentMethodCode { get; set; }
        //prod pir 6944
        //-- start --//

        public string istrRepetitionSeparator
        {
            get
            {
                if (iblnIsNewLayout) //pir 8570
                {
                    return busConstant.New820RepetitionSeparator;
                }
                else
                {
                    return busConstant.Old820RepetitionSeparator; 
                }
            }
        }

        public string istrRepetitionSeparatorSanford
        {
            get
            {
                return busConstant.New820RepetitionSeparator;
            }
        }

        public string istrInterchangeControlVersionNumber
        {
            get
            {
                if (iblnIsNewLayout) //pir 8570
                {
                    return busConstant.New820InterchangeControlVersionNumber;
                }
                else
                {
                    return busConstant.Old820InterchangeControlVersionNumber;
                }
            }
        }

        public string istrInterchangeControlVersionNumberSanford
        {
            get
            { 
                return busConstant.New820InterchangeControlVersionNumber;
            }
        }

        public string istrApplicationReceiversCode
        {
            get
            {
                if (iblnIsNewLayout) //pir 8570
                {
                    return busConstant.New820ApplicationReceiversCode;
                }
                else
                {
                    return busConstant.Old820ApplicationReceiversCode;
                }
            }
        }

        public string istrApplicationReceiversCodeSanford
        {
            get
            {
                return "Sanford Health Plan";
            }
        }

        public string istrVersionReleaseIndustryIdentifierCode
        {
            get
            {
                if (iblnIsNewLayout) //pir 8570
                {
                    return busConstant.New820VersionReleaseIndustryIdentifierCode;
                }
                else
                {
                    return busConstant.Old820VersionReleaseIndustryIdentifierCode;
                }
            }
        }

        public string istrImplementationConventionReference
        {
            get
            {
                if (iblnIsNewLayout) //pir 8570
                {
                    return "0001" + "*" + busConstant.New820ImplementationConventionReference;
                }
                else
                {
                    return "0001";
                }
            }
        }

        public string istrImplementationConventionReferenceSanford
        {
            get
            {
                return "0001" + "*" + busConstant.New820ImplementationConventionReference;
            }
        }

        public string istrOriginatingCompanyIdentifier
        {
            get
            {
                if (iblnIsNewLayout) //pir 8570
                {
                    return busConstant.New820OriginatingCompanyIdentifier;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string istrOriginatingCompanyIdentifierSanford
        {
            get
            {
                return busConstant.New820OriginatingCompanyIdentifier;
            }
        }

        //-- end --//

        private DateTime _ldtTodaysDate;
        public DateTime ldtTodaysDate
        {
            get
            {
                _ldtTodaysDate = DateTime.Now;
                return _ldtTodaysDate;
            }
        }

        private decimal _ldclTotalAmount;
        public decimal ldclTotalAmount
        {
            get { return _ldclTotalAmount; }
            set { _ldclTotalAmount = value; }
        }

        public void LoadTotalAmount()
        {
            /*if (_iclbProviderReportDataInsurance != null)
                foreach (busProviderReportDataInsurance lobjInsurance in _iclbProviderReportDataInsurance)
                    _ldclTotalAmount += lobjInsurance.icdoProviderReportDataInsurance.premium_amount;*/

            DataTable ldtbTotalPremiumData = busBase.Select("cdoProviderReportDataInsurance.GetTotalPremiumAmount",
               new object[2] { ldtEffectiveDate, lintProviderOrgID });
             if (ldtbTotalPremiumData.Rows.Count > 0)
             {
                 if (ldtbTotalPremiumData.Rows[0]["TOTAL_PREMIUM_AMOUNT"] != DBNull.Value)
                     _ldclTotalAmount = Convert.ToDecimal(ldtbTotalPremiumData.Rows[0]["TOTAL_PREMIUM_AMOUNT"]);
             }
        }

        private int _lintHIPPARangeNo;
        public int lintHIPPARangeNo
        {
            get { return _lintHIPPARangeNo; }
            set { _lintHIPPARangeNo = value; }
        }

        private string _lstrHIPPARangeNo;
        public string lstrHIPPARangeNo
        {
            get
            {
                _lstrHIPPARangeNo = Convert.ToString(_lintHIPPARangeNo);
                return _lstrHIPPARangeNo.PadLeft(9, '0');
            }
        }

        public string lstrProviderName { get; set; }
        public string lstrProviderID { get; set; }
        public string lstrInterchangeReceiverID
        {
            get
            {
                return lstrProviderOrgCodeID.PadLeft(15, '0');
            }
        }


        public string lstrBPRFiller
        {
            get
            {
                string lstrBPRFillerAfterFormatChange = "****";
                return lstrBPRFillerAfterFormatChange;
            }
        }
        //prod pir 6944
        public string lstrBPRFillerAfterCompanyIdentifier
        {
            get
            {
                string lstrBPRFillerAfterFormatChange = "****";
                return lstrBPRFillerAfterFormatChange;
            }
        }

        public string lstrDTMFiller
        {
            get
            {
                string lstrDTMFillerAfterFormatChange = "**";
                return lstrDTMFillerAfterFormatChange;
            }
        }

        public string lstrNumberOfSegments
        {
            get
            {
                return lintNumberofSegments.ToString();
            }
        }
        public void LoadHIPAARanges()
        {
            DataTable ldtbList = busBase.Select("cdoProviderReportHipaaRange.GetDistinctPlanID", new object[1] { lintProviderOrgID });
            foreach (DataRow dr in ldtbList.Rows)
            {
                int lintPlanID = Convert.ToInt32(dr["PLAN_ID"]);
                DataTable ldtHIPAA = busBase.Select<cdoProviderReportHipaaRange>(new string[1] { "plan_id" },
                                            new object[1] { lintPlanID }, null, null);
                if (ldtHIPAA.Rows.Count == 0)
                {
                    cdoProviderReportHipaaRange lobjHIPAARange = new cdoProviderReportHipaaRange();
                    lobjHIPAARange.provider_org_code_id = lstrProviderOrgCodeID;
                    lobjHIPAARange.plan_id = lintPlanID;
                    lobjHIPAARange.last_sequence += 1;
                    lobjHIPAARange.Insert();
                }
                else
                {
                    cdoProviderReportHipaaRange lobjHIPAARange = new cdoProviderReportHipaaRange();
                    lobjHIPAARange.LoadData(ldtHIPAA.Rows[0]);
                    lobjHIPAARange.last_sequence += 1;
                    lobjHIPAARange.Update();
                }
            }
        }

        /// Since Each Provider is providing a single plan, this code works fine.
        public int GetHIPAARange()
        {
            int lintHIPPARange = 0;
            if ((_iclbProviderReportDataInsurance != null) && (_iclbProviderReportDataInsurance.Count > 0))
            {
                DataTable ldtbList = busBase.Select<cdoProviderReportHipaaRange>(new string[1] { "plan_id" },
                                        new object[1] { _iclbProviderReportDataInsurance[0].icdoProviderReportDataInsurance.plan_id }, null, null);
                if (ldtbList.Rows.Count > 0)
                {
                    cdoProviderReportHipaaRange lobjHIPAARange = new cdoProviderReportHipaaRange();
                    lobjHIPAARange.LoadData(ldtbList.Rows[0]);
                    lintHIPPARange = lobjHIPAARange.last_sequence;
                }
            }
            return lintHIPPARange;
        }

        /// Date Time Period in DTM Segment
        private string _lstrDateTimePeriod;
        public string lstrDateTimePeriod
        {
            get
            {
                //datetime period changed to vendor payment request effective date range as per Satya, Sep 23, 2010
                _lstrDateTimePeriod = string.Empty;
                DateTime ldtStartDate = new DateTime(ldtEffectiveDate.Year, ldtEffectiveDate.Month, 1);
                DateTime ldtEndDate = new DateTime(ldtEffectiveDate.Year, ldtEffectiveDate.Month, DateTime.DaysInMonth(ldtEffectiveDate.Year, ldtEffectiveDate.Month));
                _lstrDateTimePeriod = ldtStartDate.ToString("yyyyMMdd") + "-" + ldtEndDate.ToString("yyyyMMdd");
                return _lstrDateTimePeriod;
            }
        }

        private Collection<busProviderReportDataInsurance> _iclbProviderReportDataInsurance;
        public Collection<busProviderReportDataInsurance> iclbProviderReportDataInsurance
        {
            get { return _iclbProviderReportDataInsurance; }
            set { _iclbProviderReportDataInsurance = value; }
        }

        public override void InitializeFile()
        {
            string lstrFileName = DateTime.Now.ToString(busConstant.DateFormat) + "_" + lstrProviderOrgCodeID + busConstant.FileFormattxt;
            istrFileName = "DF.ER308010_" + lstrFileName;
        }

        public override void FinalizeFile()
        {
            Collection<busCodeValue> lclbCodeValue = busGlobalFunctions.LoadCodeValueByData1(5012, lstrProviderOrgCodeID);
            if (lclbCodeValue.Count > 0)
            {
                try
                {
                    // PROD PIR ID 512
                    busGlobalFunctions.SendMail(busGlobalFunctions.GetSysManagementEmailNotification(), lclbCodeValue[0].icdoCodeValue.data2,
                                                    istrFileName + " file is generated.", lclbCodeValue[0].icdoCodeValue.comments, true, true);
                }
                catch (Exception Ex)
                {
                    ExceptionManager.Publish(Ex);
                }
            }
            base.FinalizeFile();
        }

        private string istrFromEmail;

        private static int _lintAssignedNo;

        public bool iblnIsSuperiorVision { get; set; }

        public Collection<cdoCodeValue> iclcCodeValue { get; set; }
        public void LoadCodeValueForHIPAA()
        {
            iclcCodeValue = new Collection<cdoCodeValue>();
            DataTable ldtbList = iobjPassInfo.isrvDBCache.GetCodeValues(1213);
            iclcCodeValue = Sagitec.DataObjects.doBase.GetCollection<cdoCodeValue>(ldtbList);
        }

        public Collection<busProviderReportDataInsurance> iclbPersonWithMultipleRegRecords { get; set; }
        
        public bool iblnIsNewLayout { get; set; } //8570
        
        public void LoadProviderReportDataInsurance(DataTable ldtbProviderInsurance)
        {
            //uat pir 359
            /// SE Segment count, 8 segments are from layout (including SE and ST)
            _lintNumberofSegments = 8;
            /// Reset the ENT Count
            _lintAssignedNo = 0;
            lintProviderOrgID = Convert.ToInt32(iarrParameters[0]);
            ldtEffectiveDate = Convert.ToDateTime(iarrParameters[1]);
            istrFromEmail = Convert.ToString(iarrParameters[2]);

            // PIR 10448
            istrProviderOrgName = busGlobalFunctions.GetOrgNameByOrgID(lintProviderOrgID);
            if (istrProviderOrgName.Contains("Delta"))
            {
                iblnIsProviderDelta = true;
            }
            else
            {
                iblnIsProviderDelta = false;
            }

            lstrProviderOrgCodeID = busGlobalFunctions.GetOrgCodeFromOrgId(lintProviderOrgID);
            istrStructureCode = busGlobalFunctions.GetData1ByCodeValue(busConstant.SystemConstantCodeID, "CIGN", iobjPassInfo);
            istrVisionStructureCode = busGlobalFunctions.GetData1ByCodeValue(busConstant.SystemConstantCodeID, "VISN", iobjPassInfo);
            if (iclcCodeValue == null)
                LoadCodeValueForHIPAA();
            //assigning identification code in Ref section to NDPERS by default
            istrIdentificationCode = busConstant.IdentificationCodeNDPERS;
            foreach (cdoCodeValue lobjCodeValue in iclcCodeValue)
            {
                // PIR ID 267
                // Assign Provider Name and ID
                if (lstrProviderOrgCodeID == busGlobalFunctions.GetData1ByCodeValue(1213, lobjCodeValue.code_value, iobjPassInfo))
                {
                   
                    lstrProviderID = busGlobalFunctions.GetData2ByCodeValue(1213, lobjCodeValue.code_value, iobjPassInfo);
                    //pir 8570
                    if ((lobjCodeValue.code_value == busConstant.CIGNAProviderCodeValue && Convert.ToString(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.New5010HIPAA820LayoutForCigna, iobjPassInfo)) == busConstant.Flag_Yes)
                        || (lobjCodeValue.code_value == busConstant.DELTAProviderCodeValue && Convert.ToString(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.New5010HIPAA820LayoutForDelta, iobjPassInfo)) == busConstant.Flag_Yes) // PIR 10448
                        || (lobjCodeValue.code_value == busConstant.SuperiorVisionProviderCodeValue && Convert.ToString(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.New5010HIPAA820LayoutForSuperVision, iobjPassInfo)) == busConstant.Flag_Yes)
                        || (lobjCodeValue.code_value == busConstant.BCBSProviderCodeValue && Convert.ToString(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.New5010HIPAA820LayoutForBCBS, iobjPassInfo)) == busConstant.Flag_Yes)
                        || (lobjCodeValue.code_value == busConstant.SanfordProviderCodeValue))
                        iblnIsNewLayout = true;
                    //PIR 8872
                    if (lobjCodeValue.code_value == busConstant.CIGNAProviderCodeValue && iblnIsNewLayout)
                        lstrProviderName = "";
                    else
                        lstrProviderName = busGlobalFunctions.GetData3ByCodeValue(1213, lobjCodeValue.code_value, iobjPassInfo);
                    //assigning identification code in Ref section for dental
                    if (lobjCodeValue.code_value == busConstant.CIGNAProviderCodeValue)
                        istrIdentificationCode = busConstant.IdentificationCodeDental;
                    //checking whether file is for superior vision
                    if (lobjCodeValue.code_value == busConstant.SuperiorVisionProviderCodeValue)
                        iblnIsSuperiorVision = true;
                    else
                        iblnIsSuperiorVision = false;
                }
            }
            //pir 8872
            if (lintProviderOrgID == lintCignaOrgID && iblnIsNewLayout)
            {
                istrPayerName = "";
                istrPaymentMethodCode = "NON";
            }
            else
            {
                istrPayerName = "NDPERS";
                istrPaymentMethodCode = "FWT";
            }
            _iclbProviderReportDataInsurance = new Collection<busProviderReportDataInsurance>();
            iclbPersonWithMultipleRegRecords = new Collection<busProviderReportDataInsurance>();
            /// Get all Current month Regular data Group by Person
            DataTable ldtbProviderReportData = busBase.Select("cdoProviderReportDataInsurance.GetCurrentMonthRegularDataByProvider",
                new object[2] { lintProviderOrgID, ldtEffectiveDate });
            int lintPersonID = 0;
            if (ldtbProviderReportData.Rows.Count > 0)
            {
                foreach (DataRow dr in ldtbProviderReportData.Rows)
                {
                    busProviderReportDataInsurance lobjInsurance = new busProviderReportDataInsurance();
                    lobjInsurance.icdoProviderReportDataInsurance = new cdoProviderReportDataInsurance();
                    lobjInsurance.icdoProviderReportDataInsurance.LoadData(dr);

                    if (dr["person_id"] != DBNull.Value && lintPersonID != Convert.ToInt32(dr["person_id"]))
                    {
                        _lintAssignedNo += 1;
                        lobjInsurance.lintAssignedNumber = _lintAssignedNo;
                        _iclbProviderReportDataInsurance.Add(lobjInsurance);
                        lintPersonID = lobjInsurance.icdoProviderReportDataInsurance.person_id;
                    }
                    else
                    {
                        iclbPersonWithMultipleRegRecords.Add(lobjInsurance);
                    }
                }
                iintMemberCount = _iclbProviderReportDataInsurance.Count;
            }
            else
            {
                iblnIsCurrentMonthRegularRecordExists = false;
                /*/// Get only current month Adjustment Records for the persons who doesnt have Regular Records.
                DataTable ldtbOnlyCurrentMonthAdjustments = busBase.Select("cdoProviderReportDataInsurance.GetOnlyCurrentMonthAdjustments", new object[2] { ldtEffectiveDate, lintProviderOrgID });
                foreach (DataRow dr in ldtbOnlyCurrentMonthAdjustments.Rows)
                {
                    busProviderReportDataInsurance lobjInsurance = new busProviderReportDataInsurance();
                    lobjInsurance.icdoProviderReportDataInsurance = new cdoProviderReportDataInsurance();
                    lobjInsurance.icdoProviderReportDataInsurance.LoadData(dr);
                    _lintAssignedNo += 1;
                    lobjInsurance.lintAssignedNumber = _lintAssignedNo;
                    _iclbProviderReportDataInsurance.Add(lobjInsurance);
                }
                /// Get only Prior month Adjustment Records for the persons who doesnt have Regular Records.
                DataTable ldtbOnlyPriorMonthAdjustments = busBase.Select("cdoProviderReportDataInsurance.GetOnlyPriorMonthAdjustments", new object[2] { ldtEffectiveDate, lintProviderOrgID });
                foreach (DataRow dr in ldtbOnlyPriorMonthAdjustments.Rows)
                {
                    busProviderReportDataInsurance lobjInsurance = new busProviderReportDataInsurance();
                    lobjInsurance.icdoProviderReportDataInsurance = new cdoProviderReportDataInsurance();
                    lobjInsurance.icdoProviderReportDataInsurance.LoadData(dr);
                    _lintAssignedNo += 1;
                    lobjInsurance.lintAssignedNumber = _lintAssignedNo;
                    _iclbProviderReportDataInsurance.Add(lobjInsurance);
                }*/

                DataTable ldtbDistinctPersonAdjustments = busBase.Select("cdoProviderReportDataInsurance.GetDistinctPersonAdjustments",
                    new object[2] { lintProviderOrgID, ldtEffectiveDate });
                foreach (DataRow dr in ldtbDistinctPersonAdjustments.Rows)
                {
                    busProviderReportDataInsurance lobjInsurance = new busProviderReportDataInsurance();
                    lobjInsurance.icdoProviderReportDataInsurance = new cdoProviderReportDataInsurance();
                    lobjInsurance.icdoProviderReportDataInsurance.LoadData(dr);
                    _lintAssignedNo += 1;
                    lobjInsurance.lintAssignedNumber = _lintAssignedNo;
                    _iclbProviderReportDataInsurance.Add(lobjInsurance);
                }
            }
            /// Load Total Contributions
            LoadTotalAmount();

            /// Add HIPPA Range for Provider and Plan Combination if not exists
            LoadHIPAARanges();

            /// Get the HIPPA Range for the Provider, Plan Combination
            _lintHIPPARangeNo = GetHIPAARange();
        }
               
        public override void AfterWriteRecord()
        {
            if (iobjDetail is busProviderReportDataInsurance)
            {                
                if (iblnIsCurrentMonthRegularRecordExists)
                {
                    StringBuilder lsbrHIPAADetail = new StringBuilder();
                    busProviderReportDataInsurance lobjProviderReportDataInsurance = (busProviderReportDataInsurance)iobjDetail;
                    cdoProviderReportDataInsurance lcdoData = lobjProviderReportDataInsurance.icdoProviderReportDataInsurance;

                    // NM1 Segment.
                    lsbrHIPAADetail.Append(GetNM1(lcdoData));
                    // RMR1 Segment.
                    lsbrHIPAADetail.Append(GetRMR1(lcdoData));

                    // Add RMR1 Segment for the Person having Current Month Adjustments
                    DataTable ldtbCurrentMonthAdjustments = busBase.Select("cdoProviderReportDataInsurance.GetCurrentMonthAdjustmentsDataByMember",
                                                        new object[4] { lcdoData.person_id, lcdoData.plan_id, ldtEffectiveDate, lintProviderOrgID });
                    IEnumerable<busProviderReportDataInsurance> lenmPRDI = iclbPersonWithMultipleRegRecords.Where(o => o.icdoProviderReportDataInsurance.person_id == lcdoData.person_id);
                    foreach (busProviderReportDataInsurance lobjIns in lenmPRDI)
                    {
                        DataRow dr = ldtbCurrentMonthAdjustments.NewRow();
                        dr["person_id"] = lobjIns.icdoProviderReportDataInsurance.person_id;
                        dr["ssn"] = lobjIns.icdoProviderReportDataInsurance.ssn;
                        dr["plan_id"] = lobjIns.icdoProviderReportDataInsurance.plan_id;
                        dr["first_name"] = lobjIns.icdoProviderReportDataInsurance.first_name;
                        dr["last_name"] = lobjIns.icdoProviderReportDataInsurance.last_name;
                        dr["record_type_value"] = busConstant.RecordTypePositiveAdjustment;
                        dr["effective_date"] = lobjIns.icdoProviderReportDataInsurance.effective_date;
                        dr["premium_amount"] = lobjIns.icdoProviderReportDataInsurance.premium_amount;
                        //prod pir 6076 & 6077 - Removal of person account ghdv history id
                        //dr["person_account_ghdv_history_id"] = lobjIns.icdoProviderReportDataInsurance.person_account_ghdv_history_id;
                        dr["group_number"] = lobjIns.icdoProviderReportDataInsurance.group_number;
                        dr["coverage_code"] = lobjIns.icdoProviderReportDataInsurance.coverage_code;
                        dr["rate_structure_code"] = lobjIns.icdoProviderReportDataInsurance.rate_structure_code;
                        ldtbCurrentMonthAdjustments.Rows.Add(dr);
                    }
                    ldtbCurrentMonthAdjustments.AcceptChanges();
                    lsbrHIPAADetail.Append(GetRMR2(ldtbCurrentMonthAdjustments));

                    // Add RMR2, DTM and ADX Segment for Person having Prior Month Adjustments
                    DataTable ldtbPriorMonthAdjustments = busBase.Select("cdoProviderReportDataInsurance.GetPriorMonthAdjustmentsDataByMember",
                                                            new object[4] { lcdoData.person_id, lcdoData.plan_id, ldtEffectiveDate, lintProviderOrgID });
                    
                    lsbrHIPAADetail.Append(GetRMR2(ldtbPriorMonthAdjustments));

                    iintCounter++;
                    // Executes only at the last loop
                    if (_iclbProviderReportDataInsurance.Count == iintCounter)
                    {
                        // Append only Prior month adjustment
                        DataTable ldtbOnlMonthAdjustments = busBase.Select("cdoProviderReportDataInsurance.GetAllAdjustments", new object[2] { lintProviderOrgID, ldtEffectiveDate });
                        int lintPersonId = 0;
                        
                        foreach (DataRow dr in ldtbOnlMonthAdjustments.Rows)
                        {
                            busProviderReportDataInsurance lobjInsurance = new busProviderReportDataInsurance();
                            lobjInsurance.icdoProviderReportDataInsurance = new cdoProviderReportDataInsurance();
                            lobjInsurance.icdoProviderReportDataInsurance.LoadData(dr);
                            if (lintPersonId != lobjInsurance.icdoProviderReportDataInsurance.person_id)
                            {
                                // Updates the ENT Segment
                                _lintAssignedNo += 1;
                                lobjInsurance.lintAssignedNumber = _lintAssignedNo;

                                string lstrOldValue = istrRecord.Split(new char[1] { '*' })[1];
                                string lstrNewValue = lobjInsurance.lstrAssignedNumber.PadRight(lstrOldValue.Length, ' ');
                                string lstrOldSSN = istrRecord.Split(new char[1] { '*' })[4];
                                string lstrNewSSN = lobjInsurance.icdoProviderReportDataInsurance.ssn;
                                istrRecord = istrRecord.Replace("ENT*" + lstrOldValue + "*", "ENT*" + lstrNewValue + "*");
                                istrRecord = istrRecord.Replace(lstrOldSSN, lstrNewSSN) + "~";
                                lsbrHIPAADetail.Append(istrRecord); // ENT Segment
                                lsbrHIPAADetail.Append(GetNM1(lobjInsurance.icdoProviderReportDataInsurance));
                                //lsbrHIPAADetail.Append("RMR****~"); // Blank RMR1
                                //_lintNumberofSegments += 1;

                                DataTable ldtInsurance = ldtbOnlMonthAdjustments.AsEnumerable()
                                    .Where(o => o.Field<int>("person_id") == lobjInsurance.icdoProviderReportDataInsurance.person_id).AsDataTable();
                                //prod pir 4753
                                //820 file changes                                
                                DataRow ldrCurrentMonth = null;
                                bool lblnCurrentMonthExists = false;
                                foreach (DataRow ldrIns in ldtInsurance.Rows)
                                {
                                    if (ldrIns["effective_date"] != DBNull.Value && Convert.ToDateTime(ldrIns["effective_date"]).Month == ldtEffectiveDate.Month &&
                                        Convert.ToDateTime(ldrIns["effective_date"]).Year == ldtEffectiveDate.Year)
                                    {
                                        ldrCurrentMonth = ldrIns;
                                        lblnCurrentMonthExists = true;
                                        break;
                                    }
                                }

                                if (lblnCurrentMonthExists && ldrCurrentMonth != null)
                                {
                                    cdoProviderReportDataInsurance lcdoIns = new cdoProviderReportDataInsurance();
                                    lcdoIns.LoadData(ldrCurrentMonth);
                                    lsbrHIPAADetail.Append(GetRMR1(lcdoIns));
                                    ldtInsurance.Rows.Remove(ldrCurrentMonth);
                                    ldtInsurance.AcceptChanges();
                                    lblnCurrentMonthExists = false;
                                }
                                                                
                                lsbrHIPAADetail.Append(GetRMR2(ldtInsurance));
                            }                            
                            lintPersonId = lobjInsurance.icdoProviderReportDataInsurance.person_id;                            
                        }
                    }
                    // Appends all the details and finally write in file.
                    iswrOut.Write(lsbrHIPAADetail);
                }
                else
                {
                    StringBuilder lsbrHIPAADetail = new StringBuilder();
                    busProviderReportDataInsurance lobjProviderReportDataInsurance = (busProviderReportDataInsurance)iobjDetail;
                    cdoProviderReportDataInsurance lcdoData = lobjProviderReportDataInsurance.icdoProviderReportDataInsurance;

                    lsbrHIPAADetail.Append(GetNM1(lcdoData));

                    // Add RMR1 Segment for the Person having Current Month Adjustments
                    DataTable ldtbCurrentMonthAdjustments = busBase.Select("cdoProviderReportDataInsurance.GetCurrentMonthAdjustmentsDataByMember",
                                                        new object[4] { lcdoData.person_id, lcdoData.plan_id, ldtEffectiveDate, lintProviderOrgID });

                    //prod pir 4753
                    if (ldtbCurrentMonthAdjustments.Rows.Count > 0)
                    {
                        DataRow ldrCurrFirstRow = null;
                        foreach (DataRow ldrCurr in ldtbCurrentMonthAdjustments.Rows)
                        {
                            ldrCurrFirstRow = ldrCurr;
                            break;
                        }

                        cdoProviderReportDataInsurance lcdoIns = new cdoProviderReportDataInsurance();
                        lcdoIns.LoadData(ldrCurrFirstRow);
                        lsbrHIPAADetail.Append(GetRMR1(lcdoIns));

                        if (ldtbCurrentMonthAdjustments.Rows.Count > 1)
                        {                            
                            ldtbCurrentMonthAdjustments.Rows.Remove(ldrCurrFirstRow);
                            ldtbCurrentMonthAdjustments.AcceptChanges();
                            lsbrHIPAADetail.Append(GetRMR2(ldtbCurrentMonthAdjustments));
                        }
                    }
                    // Add RMR2, DTM and ADX Segment for Person having Prior Month Adjustments
                    DataTable ldtbPriorMonthAdjustments = busBase.Select("cdoProviderReportDataInsurance.GetPriorMonthAdjustmentsDataByMember",
                                                            new object[4] { lcdoData.person_id, lcdoData.plan_id, ldtEffectiveDate, lintProviderOrgID });
                    
                    lsbrHIPAADetail.Append(GetRMR2(ldtbPriorMonthAdjustments));

                    iswrOut.Write(lsbrHIPAADetail);
                }
            }
        }

        private static int _lintNumberofSegments;
        public int lintNumberofSegments
        {
            get { return _lintNumberofSegments; }
            set { _lintNumberofSegments = value; }
        }

        /// Get the section, NM1 - INDIVIDUAL NAME
        public string GetNM1(cdoProviderReportDataInsurance acdoInsurance)
        {
            _lintNumberofSegments += 2; // Includes ENT and NM1
            string lstrLastName = string.Empty;
            string lstrFirstName = string.Empty;
            string lstrNM1 = string.Empty;
            if (acdoInsurance.last_name != null)
                lstrLastName = acdoInsurance.last_name.Trim().ToUpper();
            if (acdoInsurance.first_name != null)
                lstrFirstName = acdoInsurance.first_name.Trim().ToUpper();

            //PROD PIR 5308
            //New file for Superior Vision
            if (iblnIsSuperiorVision)
                lstrNM1 = "NM1*EY*1*" + lstrLastName + "*" + lstrFirstName + "**34*" + acdoInsurance.ssn + "~";
            else
                lstrNM1 = "NM1*EY*1*" + lstrLastName + "*" + lstrFirstName + "****34*" + acdoInsurance.ssn + "~";
            return lstrNM1;
        }

        /// Get the section , RMR1 - INDIVIDUAL PREMIUM REMITTANCE DETAIL
        public string GetRMR1(cdoProviderReportDataInsurance acdoInsurance)
        {
            _lintNumberofSegments += 1;
            string lstrRMR1 = "RMR*11*";
            //PIR 8872
            if(istrIdentificationCode == busConstant.IdentificationCodeDental && iblnIsNewLayout)
            {
                lstrRMR1 = "RMR*AZ*";
            }
            // PIR 10448 - RMR02 code for Delta
            if (iblnIsProviderDelta)
            {
                DataTable ldtbPersonAccount = busNeoSpinBase.Select<cdoPersonAccount>(
                    new string[2] { "person_id", "plan_id" },
                    new object[2] { acdoInsurance.person_id, acdoInsurance.plan_id }, null, null);

                if (ldtbPersonAccount.Rows.Count > 0)
                {
                    DataTable ldtbPersonAccountGhdv = busNeoSpinBase.Select<cdoPersonAccountGhdv>(
                        new string[1] { "person_account_id" },
                        new object[1] { Convert.ToInt32(ldtbPersonAccount.Rows[0]["PERSON_ACCOUNT_ID"]) }, null, null);

                    if (ldtbPersonAccountGhdv.Rows.Count > 0)
                    {
                        busPersonAccountGhdv lbusPersonAccountGhdv = new busPersonAccountGhdv() { icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };
                        lbusPersonAccountGhdv.icdoPersonAccountGhdv.LoadData(ldtbPersonAccountGhdv.Rows[0]);

                        if (lbusPersonAccountGhdv.icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeActive)
                            lstrRMR1 += "9005374820001";
                        else if (lbusPersonAccountGhdv.icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeRetiree)
                            lstrRMR1 += "9005374820002";
                        else if (lbusPersonAccountGhdv.icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeCOBRA)
                            lstrRMR1 += "9005374829272";
                    }
                }
            }
            else
            {
                lstrRMR1 += GetOrgCodeAndRateStructureAndCoverageCode(acdoInsurance);
            }
            lstrRMR1 += "**" + acdoInsurance.premium_amount + "~";
            return lstrRMR1;
        }

        private string GetOrgCodeAndRateStructureAndCoverageCode(cdoProviderReportDataInsurance acdoInsurance)
        {
            string lstrRMR = string.Empty;
            busPerson lbusPerson = new busPerson();

            if (lbusPerson.FindPerson(acdoInsurance.person_id))
            {
                //loading rate structure code and coverage code
                if (acdoInsurance.plan_id == busConstant.PlanIdMedicarePartD || acdoInsurance.plan_id == busConstant.PlanIdGroupHealth)
                {
                    if (!string.IsNullOrEmpty(acdoInsurance.group_number))
                        lstrRMR += acdoInsurance.group_number;

                    if (!string.IsNullOrEmpty(acdoInsurance.rate_structure_code))
                    {
                        switch (acdoInsurance.rate_structure_code.Length)
                        {
                            case 0:
                                break;
                            case 1:
                                lstrRMR += acdoInsurance.rate_structure_code.PadLeft(2, '0');
                                break;
                            default:
                                lstrRMR += acdoInsurance.rate_structure_code.Substring(acdoInsurance.rate_structure_code.Length - 2, 2);
                                break;
                        }
                    }
                    if (!string.IsNullOrEmpty(acdoInsurance.coverage_code))
                    {
                        switch (acdoInsurance.coverage_code.Length)
                        {
                            case 0:
                                break;
                            case 1:
                                lstrRMR += acdoInsurance.coverage_code.PadLeft(2, '0');
                                break;
                            default:
                                lstrRMR += acdoInsurance.coverage_code.Substring(acdoInsurance.coverage_code.Length - 2, 2);
                                break;
                        }
                    }
                }
                else if (acdoInsurance.plan_id == busConstant.PlanIdVision)
                {
                    if (iblnIsSuperiorVision)
                        lstrRMR += string.IsNullOrEmpty(acdoInsurance.group_number) ? "" : acdoInsurance.group_number;
                    else
                        lstrRMR += istrVisionStructureCode + (string.IsNullOrEmpty(acdoInsurance.group_number) ? "" : acdoInsurance.group_number);
                }
                else if (acdoInsurance.plan_id == busConstant.PlanIdDental)
                {
                    // PIR 10448: RMR02 group number for Delta
                    if (iblnIsProviderDelta)
                    {
                        lstrRMR += (string.IsNullOrEmpty(acdoInsurance.group_number) ? "" : acdoInsurance.group_number);
                    }
                    else
                    {
                        lstrRMR += istrStructureCode + (string.IsNullOrEmpty(acdoInsurance.group_number) ? "" : acdoInsurance.group_number);
                    }
                }
                
            }
            return lstrRMR;
        }

        /// Get the section, RMR2 - INDIVIDUAL PREMIUM REMITTANCE DETAIL FOR ADJUSTMENT
        public string GetRMR2(DataTable adtInsurance)
        {
            string lstrFinal = string.Empty;
            string lstrPrevRMR2 = string.Empty;
            string lstrPrevDTM = string.Empty;            
            decimal ldecTotalPremium = 0.0M;
            foreach (DataRow ldrIns in adtInsurance.Rows)
            {
                cdoProviderReportDataInsurance lcdoInsurance = new cdoProviderReportDataInsurance();
                lcdoInsurance.LoadData(ldrIns);
                
                string lstrRMR2 = "RMR*11*";
                //PIR 8872
                if( istrIdentificationCode == busConstant.IdentificationCodeDental && iblnIsNewLayout)
                {
                    lstrRMR2 = "RMR*AZ*";
                }
                lstrRMR2 += GetOrgCodeAndRateStructureAndCoverageCode(lcdoInsurance);
                string lstrDTM = string.Empty;
                lstrDTM = "DTM*582****RD8*" + lcdoInsurance.lstrDateTimePeriod + "~";
                if (lstrPrevRMR2 != lstrRMR2 || lstrPrevDTM != lstrDTM)
                {
                    if (lstrFinal.IndexOf("(AMTPREM)") > -1)
                        lstrFinal = lstrFinal.Replace("(AMTPREM)", ldecTotalPremium.ToString());
                    _lintNumberofSegments += 2;
                    lstrFinal += lstrRMR2 + "**" + "(AMTPREM)" + "~" + lstrDTM + GetADX(lcdoInsurance);
                    ldecTotalPremium = 0.0M;
                    ldecTotalPremium += lcdoInsurance.premium_amount;
                }
                else
                {
                    lstrFinal += GetADX(lcdoInsurance);
                    ldecTotalPremium += lcdoInsurance.premium_amount;
                }
                lstrPrevRMR2 = lstrRMR2;
                lstrPrevDTM = lstrDTM;

                lcdoInsurance = null;
            }
            if (lstrFinal.IndexOf("(AMTPREM)") > -1)
                lstrFinal = lstrFinal.Replace("(AMTPREM)", ldecTotalPremium.ToString());
            return lstrFinal;
        }

        public string GetADX(cdoProviderReportDataInsurance acdoInsurance)
        {
            _lintNumberofSegments += 1; // Includes ADX
            string lstrAdjustmentType = string.Empty;
            string lstrFinal = string.Empty;

            if (acdoInsurance.record_type_value == busConstant.RecordTypePositiveAdjustment)
                lstrAdjustmentType = "53";
            else if (acdoInsurance.record_type_value == busConstant.RecordTypeNegativeAdjustment)
                lstrAdjustmentType = "52";

            lstrFinal = "ADX*" + acdoInsurance.premium_amount.ToString() + "*" + lstrAdjustmentType + "~";
            return lstrFinal;
        }

        // REF - Member Policy Number - NOT USED
        private string GetRMRSectionForDental(busPersonAccountGhdv abusGHDV)
        {

            string lstrREF = string.Empty;

            // Structure or Plan Code
            lstrREF += istrStructureCode;
            lstrREF += GetBranch(abusGHDV);
            lstrREF += GetBenefitOptionCode(abusGHDV);
            
            return lstrREF;
        }

        private string GetBranch(busPersonAccountGhdv abusGHDV)
        {
            if (abusGHDV != null)
            {
                if (abusGHDV.IsCOBRAValueSelected())
                    return "199   ";
                else
                {
                    abusGHDV.LoadPlanEffectiveDate();
                    abusGHDV.icdoPersonAccount.person_employment_dtl_id = abusGHDV.GetEmploymentDetailID();
                    // Employment is there
                    if (abusGHDV.icdoPersonAccount.person_employment_dtl_id != 0)
                    {
                        DataTable ldtbResult = busBase.Select("cdoPersonEmployment.LoadEmploymentByEmpDtlID",
                                                    new object[1] { abusGHDV.icdoPersonAccount.person_employment_dtl_id });
                        if (ldtbResult.Rows.Count > 0)
                        {
                            abusGHDV.ibusPersonEmploymentDetail = new busPersonEmploymentDetail();
                            abusGHDV.ibusPersonEmploymentDetail.ibusPersonEmployment = new busPersonEmployment();
                            abusGHDV.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization = new busOrganization();
                            abusGHDV.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization = new cdoOrganization();
                            abusGHDV.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.LoadData(ldtbResult.Rows[0]);
                            if (!string.IsNullOrEmpty(abusGHDV.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.hipaa_branch_id))
                                return abusGHDV.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.hipaa_branch_id.PadRight(6, ' ');
                        }
                    }
                    else // No Employment 
                        return "102   ";
                }
            }
            return "101   "; // All other cases - Referred to NDPERS
        }

        private string GetBenefitOptionCode(busPersonAccountGhdv abusGHDV)
        {
            string lstrBOC = string.Empty;
            if (abusGHDV.ibusPerson != null)
            {
                if (abusGHDV.ibusPerson.ibusPersonCurrentAddress == null)
                    abusGHDV.ibusPerson.LoadPersonCurrentAddress();
                if (abusGHDV.ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress != null)
                    lstrBOC = abusGHDV.ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.lstr_Benefit_Option_Code;
            }
            return lstrBOC;
        }

        //NOT USED
        private string GetRMRSectionForVision(busPersonAccountGhdv abusGHDV)
        {
            // Segment Header
            string lstrREF = string.Empty;
            
            // Structure or Plan Code
            lstrREF += "010350308";
            lstrREF += GetHIPAAReferenceID(abusGHDV);
            
            return lstrREF;
        }

        // Load HIPAA Reference ID from Organization Table.
        private string GetHIPAAReferenceID(busPersonAccountGhdv abusGHDV)
        {
            if (abusGHDV != null)
            {
                if (abusGHDV.IsCOBRAValueSelected())
                    return "00999";
                else
                {
                    abusGHDV.LoadPlanEffectiveDate();
                    abusGHDV.icdoPersonAccount.person_employment_dtl_id = abusGHDV.GetEmploymentDetailID();
                    // Employment is there
                    if (abusGHDV.icdoPersonAccount.person_employment_dtl_id != 0)
                    {
                        DataTable ldtbResult = busBase.Select("cdoPersonEmployment.LoadEmploymentByEmpDtlID",
                                                    new object[1] { abusGHDV.icdoPersonAccount.person_employment_dtl_id });
                        if (ldtbResult.Rows.Count > 0)
                        {
                            // Employment Detail and Employment is not Loaded, If necessary we can load
                            abusGHDV.ibusPersonEmploymentDetail = new busPersonEmploymentDetail();
                            abusGHDV.ibusPersonEmploymentDetail.ibusPersonEmployment = new busPersonEmployment();
                            abusGHDV.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization = new busOrganization();
                            abusGHDV.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization = new cdoOrganization();
                            abusGHDV.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.LoadData(ldtbResult.Rows[0]);
                            if (!string.IsNullOrEmpty(abusGHDV.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.hipaa_reference_id))
                                return abusGHDV.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.hipaa_reference_id;
                        }
                    }
                    else // No Employment 
                        return "00002";
                }
            }
            return "00001"; // All other cases - Referred to NDPERS
        }
    }
}
