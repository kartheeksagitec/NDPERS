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
    public class busHIPAADentalDeltaFileOut : busFileBaseOut
    {
        public DateTime ldtTodaysDate
        {
            get { return DateTime.Now; }
        }

        public override void InitializeFile()
        {
            istrFileName = "834.NDPERS." + "700072" + "." + ldtTodaysDate.ToString(busConstant.DateFormat) + busConstant.FileFormattxt;
        }

        // Unique number every time file generates. 
        private string _lstrInterchangeControlNo;
        public string lstrInterchangeControlNo
        {
            get { return _lstrInterchangeControlNo; }
            set { _lstrInterchangeControlNo = value; }
        }

        // Loads the number from System constant Code value 52 and increment it.
        // Updates the InterchangeControl number in Constants code value.
        private void LoadAndUpdatesInterchangeControlNo()
        {
            cdoCodeValue lcdoCodeValue = new cdoCodeValue();
            lcdoCodeValue = busGlobalFunctions.GetCodeValueDetails(busConstant.SystemConstantCodeID, busConstant.SystemConstantISADentalDelta);
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

        private void LoadProviderOrgCode()
        {
            DataTable ldtbResult = busBase.Select("cdoPersonAccount.LoadActiveProviderByPlan",
                                      new object[2] { busConstant.PlanIdDental, ldtTodaysDate });
            if (ldtbResult.Rows.Count > 0)
            {
                if (!Convert.IsDBNull(ldtbResult.Rows[0]["org_code"]))
                    _lstrProviderOrgCode = ldtbResult.Rows[0]["org_code"].ToString();
            }
        }

        private string GetBranch(busPersonAccountGhdv aobjDental)
        {
            if (aobjDental != null)
            {
                if (aobjDental.IsCOBRAValueSelected())
                    return "199   ";
                else
                {
                    if (aobjDental.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id != 0)
                    {
                        if (!string.IsNullOrEmpty(aobjDental.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.hipaa_branch_id))
                            return aobjDental.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.hipaa_branch_id.PadRight(6, ' ');
                    }
                    else // No Employment 
                        return "102   ";
                }
            }
            return "101   "; // All other cases - Referred to NDPERS
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

        public DataTable idtbDentalData { get; set; }

        public Collection<int> iclbDental { get; set; }

        public bool iblnIsDetail { get; set; }

        private busBase iobjBase { get; set; }

        private DataTable idtbAllPersonAccountDependent { get; set; }

        private DataTable idtbAllPersonAddress { get; set; }

        private DataTable idtbAllPersonEmployment { get; set; }

        private bool iblnIsLayout5010 { get; set; }

        public void LoadDentalDELTA(DataTable adtbDentalDELTA)
        {
            iobjBase = new busBase();
            iclbDental = (Collection<int>)iarrParameters[0];
            idtbDentalData = (DataTable)iarrParameters[1];
            idtbAllPersonAccountDependent = (DataTable)iarrParameters[2];
            idtbAllPersonAddress = (DataTable)iarrParameters[3];
            idtbAllPersonEmployment = (DataTable)iarrParameters[4];
            LoadAndUpdatesInterchangeControlNo();
            LoadProviderOrgCode();
            if (Convert.ToString(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.New5010HIPAALayoutForDelta, iobjPassInfo)) == busConstant.Flag_Yes)
                iblnIsLayout5010 = true;
        }

        public override void FinalizeFile()
        {
            base.FinalizeFile();

            // Write Footer Details
            istrRecord = string.Empty;
            istrRecord += GetFooterDetails();
            iswrOut.Write(istrRecord);

            DBFunction.DBNonQuery("cdoPersonAccountGhdv.Update_HIPAA_BCBS_Flag", new object[1] { busConstant.PlanIdDental }, iobjPassInfo.iconFramework,
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

                DataRow ldrRow = idtbDentalData.Rows[int.Parse(iobjDetail.ToString())];
                busPersonAccountGhdv lbusDental = new busPersonAccountGhdv
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
                    }
                };

                lbusDental.icdoPersonAccount.LoadData(ldrRow);
                lbusDental.icdoPersonAccountGhdv.LoadData(ldrRow);
                lbusDental.ibusPerson.icdoPerson.LoadData(ldrRow);
                lbusDental.ibusPlan.icdoPlan.LoadData(ldrRow);

                // Load the Dependents
                DataRow[] ldtbDependents = busGlobalFunctions.FilterTable(idtbAllPersonAccountDependent,
                                           busConstant.DataType.Numeric, "PERSON_ACCOUNT_ID", lbusDental.icdoPersonAccount.person_account_id);
                foreach (DataRow ldtrRow in ldtbDependents)
                {
                    busPersonAccountDependent lobjPADependent = new busPersonAccountDependent
                    {
                        icdoPersonAccountDependent = new cdoPersonAccountDependent(),
                        icdoPersonDependent = new cdoPersonDependent()
                    };
                    lobjPADependent.icdoPersonAccountDependent.LoadData(ldtrRow);
                    lobjPADependent.icdoPersonDependent.LoadData(ldtrRow);
                    lbusDental.iclbPersonAccountDependent.Add(lobjPADependent);
                }

                // Load the addresses
                DataRow[] ldtrAddress = busGlobalFunctions.FilterTable(idtbAllPersonAddress, busConstant.DataType.Numeric, "PERSON_ID", lbusDental.icdoPersonAccount.person_id);
                lbusDental.ibusPerson.iclbPersonAddress = iobjBase.GetCollection<busPersonAddress>(ldtrAddress, "icdoPersonAddress");

                // PIR 9029 - get latest address irrespective of end date
                lbusDental.ibusPerson.ibusPersonCurrentAddress = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };
                if (lbusDental.ibusPerson.iclbPersonAddress.Count > 0)
                    lbusDental.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress = lbusDental.ibusPerson.iclbPersonAddress[0].icdoPersonAddress;

                // Load the Employment
                DataRow[] ldtrPAEmployment = busGlobalFunctions.FilterTable(idtbAllPersonEmployment,
                                             busConstant.DataType.Numeric, "PERSON_ACCOUNT_ID", lbusDental.icdoPersonAccount.person_account_id);
                if (ldtrPAEmployment.Count() > 0)
                {
                    lbusDental.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.LoadData(ldtrPAEmployment[0]);
                    lbusDental.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.LoadData(ldtrPAEmployment[0]);
                    lbusDental.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.LoadData(ldtrPAEmployment[0]);
                }
                lbusDental.icdoPersonAccount.person_employment_dtl_id = lbusDental.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;

                // Load the Segments for the Subscriber ie, Member
                istrRecord += GetSubscribersSegments(lbusDental);
                foreach (busPersonAccountDependent lobjPADependent in lbusDental.iclbPersonAccountDependent)
                {
                    istrRecord += "\r\n";
                    // Load the Segments for all the Dependents
                    lobjPADependent.LoadDependentInfo();
                    istrRecord += GetDependentsSegments(lobjPADependent, lbusDental);

                    if (lobjPADependent.icdoPersonAccountDependent.end_date_no_null.Date <= ldtTodaysDate.Date)
                    {
                        lobjPADependent.icdoPersonAccountDependent.is_bcbs_file_sent_flag = busConstant.Flag_Yes;
                        lobjPADependent.icdoPersonAccountDependent.iblnUpdateModifiedBy = false;//PIR 17178 - Modified date shound not be updated.
                        lobjPADependent.icdoPersonAccountDependent.Update();
                    }
                }
            }
        }

        private string GetHeaderDetails()
        {
            string lstrHeader = string.Empty;
            DataTable ldtbList = iobjPassInfo.isrvDBCache.GetCodeDescription(1213, "HICG");
            string lstrORgIdentifier = ldtbList.Rows.Count > 0 ? ldtbList.Rows[0]["data2"].ToString() : string.Empty;
            lstrHeader = "ISA*00*          *00*          *30*450282090      *" + (iblnIsLayout5010 ? "30" : "20") + "*410952670      *" + ldtTodaysDate.ToString("yyMMdd") + "*" + ldtTodaysDate.ToString("HHmm") + "*" +
                            (iblnIsLayout5010 ? "^" : "U") + "*" + (iblnIsLayout5010 ? "00501" : "00401") + "*" + lstrInterchangeControlNo + "*1*P*>~" + "\r\n";
            lstrHeader += "GS*BE*450282090*410952670*" + ldtTodaysDate.ToString("yyyyMMdd") + "*" + ldtTodaysDate.ToString("HHmm") + "*1*X*" +
                            (iblnIsLayout5010 ? "005010X220A1" : "004010X095") + "~" + "\r\n";
            lstrHeader += "ST*834*0001" + (iblnIsLayout5010 ? "*005010X220A1" : "") + "~" + "\r\n";
            lstrHeader += "BGN*00*1*" + ldtTodaysDate.ToString("yyyyMMdd") + "*" + ldtTodaysDate.ToString("HHmmssdd") + "****4~" + "\r\n";
            lstrHeader += "N1*P5*NDPERS*FI*450282090~" + "\r\n";
            lstrHeader += "N1*IN*DENTAL PLAN*FI*410952670~" + "\r\n";
            return lstrHeader;
        }

        private string GetFooterDetails()
        {
            string lstrFooter = string.Empty;
            _lintTransactionSetCount += 5;
            lstrFooter += "SE*" + lstrTransactionSetCount + "*0001~" + "\r\n";
            lstrFooter += "GE*1*1~" + "\r\n";
            lstrFooter += "IEA*1*" + lstrInterchangeControlNo + "~";
            return lstrFooter;
        }

        // Add Employee's Segments
        private string GetSubscribersSegments(busPersonAccountGhdv aobjDental)
        {
            string lstrResult = string.Empty;
            lstrResult = GetINS(true, aobjDental.icdoPersonAccountGhdv.current_dental_insurance_type_value == busConstant.DentalInsuranceTypeCOBRA ? true : false, null, aobjDental);
            lstrResult += GetREFSubsriber(aobjDental);
            lstrResult += GetREFMemberPolicy(aobjDental);
            //lstrResult += GetDTP1(true, aobjDental); // PIR 10349 - remove 336* segment
            lstrResult += GetNM1(true, null, aobjDental);
            lstrResult += GetN3(aobjDental);
            lstrResult += GetN4(aobjDental);
            lstrResult += GetDMG(true, null, aobjDental);
            lstrResult += GetHD(aobjDental);
            lstrResult += GetDTP2(true, aobjDental);
            return lstrResult;
        }

        // Add Dependent's Segments
        private string GetDependentsSegments(busPersonAccountDependent aobjPADependent, busPersonAccountGhdv aobjDental)
        {
            string lstrResult = string.Empty;
            lstrResult += GetINS(false, false, aobjPADependent, aobjDental);
            lstrResult += GetREFSubsriber(aobjDental);
            lstrResult += GetREFMemberPolicy(aobjDental);
            lstrResult += GetNM1(false, aobjPADependent, aobjDental);
            lstrResult += GetDMG(false, aobjPADependent, aobjDental);
            lstrResult += GetHD(aobjDental);
            lstrResult += GetDTP2(false, aobjDental, aobjPADependent);
            return lstrResult;
        }

        #region Segment Methods

        // INS - Member Level Detail
        private string GetINS(bool ablnIsSubscriber, bool ablnIsCOBRA, busPersonAccountDependent aobjPADependent, busPersonAccountGhdv aobjDental)
        {
            _lintTransactionSetCount += 1;
            string lstrINS = "INS*";
            // Response Code
            // Individual Relationship Code
            if (ablnIsSubscriber)
            {
                lstrINS += busConstant.Flag_Yes + "*";
                lstrINS += "18*";
            }
            else
            {
                lstrINS += busConstant.Flag_No + "*";
                if (aobjPADependent.icdoPersonDependent.relationship_value == busConstant.DependentRelationshipSpouse)
                    lstrINS += "01*";
                else
                    lstrINS += "19*";
            }

            // Maintenance Type Code
            lstrINS += "030*";

            // Maintenance Reason Code
            lstrINS += "XN*";

            // Benefit Status Code
            if (ablnIsCOBRA)
                lstrINS += "C**1*";
            else
                lstrINS += "A***";

            if (ablnIsSubscriber)
            {
                // Employment Status Code
                aobjDental.LoadPersonEmploymentDetail();
                if (aobjDental.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent)
                    lstrINS += "FT*";
                else if (aobjDental.icdoPersonAccountGhdv.current_dental_insurance_type_value == busConstant.DentalInsuranceTypeRetiree)
                    lstrINS += "RT*";
                else
                {
                    if (iblnIsLayout5010)
                        lstrINS += "TE*";
                    else
                        lstrINS += "*";
                }
            }
            else
            {
                // Employment Status Code
                lstrINS += "*";

                // Student Status
                if (aobjPADependent.icdoPersonDependent.full_time_student_flag == busConstant.Flag_Yes)
                    lstrINS += "F*";
                else
                    lstrINS += "*";

                // Handicapped Response Code
                if (aobjPADependent.icdoPersonDependent.relationship_value == busConstant.DependentRelationshipDisabledChild)
                    lstrINS += "Y*";
                else
                    lstrINS += "*";
            }

            while (busGlobalFunctions.IsLastCharacterAsterisk(lstrINS))
                lstrINS = busGlobalFunctions.RemoveLastCharacter(lstrINS);
            lstrINS += "~\r\n";
            return lstrINS;
        }

        // REF - Subsriber Number
        private string GetREFSubsriber(busPersonAccountGhdv aobjDental)
        {
            _lintTransactionSetCount += 1;
            // Segment Header
            string lstrREF = "REF*";

            // Reference Identification Qualifier
            lstrREF += "0F*";

            // Reference Identification - SSN
            lstrREF += aobjDental.ibusPerson.icdoPerson.ssn;
            if (busGlobalFunctions.IsLastCharacterAsterisk(lstrREF))
                lstrREF = busGlobalFunctions.RemoveLastCharacter(lstrREF);
            lstrREF += "~\r\n";
            return lstrREF;
        }

        // NM1 - Member Name
        private string GetNM1(bool ablnIsSubscriber, busPersonAccountDependent aobjPADependent, busPersonAccountGhdv aobjDental)
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
                if (!string.IsNullOrEmpty(aobjDental.ibusPerson.icdoPerson.last_name))
                    lstrNM1 += aobjDental.ibusPerson.icdoPerson.last_name.Trim().ToUpper() + "*";
                else
                    lstrNM1 += "*";

                // First Name
                if (!string.IsNullOrEmpty(aobjDental.ibusPerson.icdoPerson.first_name))
                    lstrNM1 += aobjDental.ibusPerson.icdoPerson.first_name.Trim().ToUpper() + "*";
                else
                    lstrNM1 += "*";

                // Middle Name
                if (!string.IsNullOrEmpty(aobjDental.ibusPerson.icdoPerson.middle_name))
                    lstrNM1 += aobjDental.ibusPerson.icdoPerson.middle_name.Trim().ToUpper() + "*";
                else
                    lstrNM1 += "*";

                // Prefix Name
                if (!string.IsNullOrEmpty(aobjDental.ibusPerson.icdoPerson.name_prefix_description))
                    lstrNM1 += aobjDental.ibusPerson.icdoPerson.name_prefix_description.Trim().ToUpper() + "*";
                else
                    lstrNM1 += "*";

                // Suffix Name
                if (!string.IsNullOrEmpty(aobjDental.ibusPerson.icdoPerson.name_suffix_description))
                    lstrNM1 += aobjDental.ibusPerson.icdoPerson.name_suffix_description.Trim().ToUpper() + "*";
                else
                    lstrNM1 += "*";

                // Member's SSN
                if (!string.IsNullOrEmpty(aobjDental.ibusPerson.icdoPerson.ssn))
                    lstrNM1 += "34*" + aobjDental.ibusPerson.icdoPerson.ssn.ToUpper();
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
        private string GetN3(busPersonAccountGhdv aobjDental)
        {
            _lintTransactionSetCount += 1;
            string lstrN3 = "N3";
            if (!string.IsNullOrEmpty(aobjDental.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_1))
                lstrN3 += "*" + aobjDental.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_1.ToUpper().Trim();
            if (!string.IsNullOrEmpty(aobjDental.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_2))
                lstrN3 += "*" + aobjDental.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_2.ToUpper().Trim();
            if (busGlobalFunctions.IsLastCharacterAsterisk(lstrN3))
                lstrN3 = busGlobalFunctions.RemoveLastCharacter(lstrN3);
            lstrN3 += "~\r\n";
            return lstrN3;
        }

        // N4 - Member Residence City, State, Zipcode
        private string GetN4(busPersonAccountGhdv aobjDental)
        {
            _lintTransactionSetCount += 1;
            string lstrN4 = "N4*";

            // N401 City
            if (!string.IsNullOrEmpty(aobjDental.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_city))
                lstrN4 += aobjDental.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_city.ToUpper().Trim() + "*";
            else
                lstrN4 += "*";

            // N402 State        
            if (aobjDental.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_value == busConstant.US_Code_ID ||
                aobjDental.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_value == busConstant.Canada_Code_ID)
            {
                if (!string.IsNullOrEmpty(aobjDental.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_state_value))
                    lstrN4 += aobjDental.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_state_value.Trim() + "*";
                else
                    lstrN4 += "*";
            }
            else
                lstrN4 += "*";

            // N403
            string lstrAddr4ZipCode = string.IsNullOrEmpty(aobjDental.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_4_code) ?
                                        string.Empty : aobjDental.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_4_code.Trim();
            if (!string.IsNullOrEmpty(aobjDental.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_code))
                lstrN4 += busGlobalFunctions.GetValidZipCode(aobjDental.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_code.Trim(), lstrAddr4ZipCode) + "*";
            else if (!string.IsNullOrEmpty(aobjDental.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.foreign_postal_code))
                lstrN4 += aobjDental.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.foreign_postal_code.Trim() + "*";
            else
                lstrN4 += "*";

            // N404
            if ((!string.IsNullOrEmpty(aobjDental.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_value)) &&
                (aobjDental.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_value != busConstant.US_Code_ID))
            {
                string lstrCountryCode = busGlobalFunctions.GetData1ByCodeValue(aobjDental.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_id,
                                                                aobjDental.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_value, iobjPassInfo);
                if (!string.IsNullOrEmpty(lstrCountryCode))
                    lstrN4 += lstrCountryCode.Trim() + "*";
            }

            while (busGlobalFunctions.IsLastCharacterAsterisk(lstrN4))
                lstrN4 = busGlobalFunctions.RemoveLastCharacter(lstrN4);
            lstrN4 += "~\r\n";
            return lstrN4;
        }

        // DMG - Member Demographics
        private string GetDMG(bool ablnIsSubscriber, busPersonAccountDependent aobjPADependent, busPersonAccountGhdv aobjDental)
        {
            _lintTransactionSetCount += 1;
            string lstrDMG = "DMG*";
            lstrDMG += "D8*";
            if (ablnIsSubscriber)
            {
                if (aobjDental.ibusPerson.icdoPerson.date_of_birth != DateTime.MinValue)
                    lstrDMG += aobjDental.ibusPerson.icdoPerson.date_of_birth.ToString(busConstant.DateFormatD8) + "*";

                // Gender Code
                if (aobjDental.ibusPerson.icdoPerson.gender_value == busConstant.GenderTypeFemale)
                    lstrDMG += "F*";
                else if (aobjDental.ibusPerson.icdoPerson.gender_value == busConstant.GenderTypeMale)
                    lstrDMG += "M*";
                else
                    lstrDMG += "U*";

                // Marital Status
                if (aobjDental.ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried)
                    lstrDMG += "M";
                else if (aobjDental.ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusSingle)
                    lstrDMG += "I";
            }
            else
            {
                if (aobjPADependent.icdoPersonDependent.dependent_DOB != DateTime.MinValue)
                    lstrDMG += aobjPADependent.icdoPersonDependent.dependent_DOB.ToString(busConstant.DateFormatD8) + "*";

                // Gender                
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

        // HD - Dental Coverage
        private string GetHD(busPersonAccountGhdv aobjDental)
        {
            _lintTransactionSetCount += 1;
            string lstrHD = "HD*";
            // Maintenance Type Code
            lstrHD += "030**";
            // Insurance Line Code
            lstrHD += "DEN**";
            // Coverage Level Code
            if (aobjDental.icdoPersonAccountGhdv.current_level_of_coverage_value == busConstant.DentalLevelofCoverageFamily)
                lstrHD += "FAM";
            else if (aobjDental.icdoPersonAccountGhdv.current_level_of_coverage_value == busConstant.DentalLevelofCoverageIndividual)
                lstrHD += "EMP";
            else if (aobjDental.icdoPersonAccountGhdv.current_level_of_coverage_value == busConstant.DentalLevelofCoverageIndividualChild)
                lstrHD += "ECH";
            else if (aobjDental.icdoPersonAccountGhdv.current_level_of_coverage_value == busConstant.DentalLevelofCoverageIndividualSpouse)
                lstrHD += "ESP";
            if (busGlobalFunctions.IsLastCharacterAsterisk(lstrHD))
                lstrHD = busGlobalFunctions.RemoveLastCharacter(lstrHD);
            lstrHD += "~\r\n";
            return lstrHD;
        }


        // DTP2 - Dental Coverage Dates
        private string GetDTP2(bool ablnIsSubscriber, busPersonAccountGhdv abusDental, busPersonAccountDependent aobjPADependent = null)
        {
            _lintTransactionSetCount += 1;
            string lstrDTP = "DTP*";
            if (ablnIsSubscriber)
            {
                if (abusDental.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                {
                    // Date Time Qualifier
                    lstrDTP += "348*";
                    // Date Time Format Qualifier
                    lstrDTP += "D8*";
                    lstrDTP += abusDental.icdoPersonAccountGhdv.current_plan_start_date_from_history.ToString(busConstant.DateFormatD8);
                }
                else if (abusDental.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended ||
                        abusDental.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceCancelled)
                {
                    // Date Time Qualifier
                    lstrDTP += "348*";
                    // Date Time Format Qualifier
                    lstrDTP += "D8*";
                    if (abusDental.icdoPersonAccount.header_plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended)
                    {
                        lstrDTP += abusDental.icdoPersonAccountGhdv.previous_history_start_date.ToString(busConstant.DateFormatD8);
                    }
                    else
                    {
                        lstrDTP += abusDental.icdoPersonAccountGhdv.current_plan_start_date_from_history.ToString(busConstant.DateFormatD8);
                    }

                    if (busGlobalFunctions.IsLastCharacterAsterisk(lstrDTP))
                        lstrDTP = busGlobalFunctions.RemoveLastCharacter(lstrDTP);
                    lstrDTP += "~\r\n";

                    _lintTransactionSetCount += 1;
                    lstrDTP += "DTP*";

                    // Date Time Qualifier
                    lstrDTP += "349*";
                    // Date Time Format Qualifier
                    lstrDTP += "D8*";
                    if (abusDental.icdoPersonAccountGhdv.current_plan_end_date_from_history != DateTime.MinValue)
                    {
                        lstrDTP += abusDental.icdoPersonAccountGhdv.current_plan_end_date_from_history.ToString(busConstant.DateFormatD8);
                    }
                    else
                    {
                        lstrDTP += abusDental.icdoPersonAccount.history_change_date_no_null.AddDays(-1).ToString(busConstant.DateFormatD8);
                    }
                }
            }
            else
            {
                if (aobjPADependent.icdoPersonAccountDependent.end_date == DateTime.MinValue)
                {
                    // Date Time Qualifier
                    lstrDTP += "348*";
                    // Date Time Format Qualifier
                    lstrDTP += "D8*";
                    lstrDTP += aobjPADependent.icdoPersonAccountDependent.start_date.ToString(busConstant.DateFormatD8);
                }
                else
                {
                    // Date Time Qualifier
                    lstrDTP += "348*";
                    // Date Time Format Qualifier
                    lstrDTP += "D8*";
                    lstrDTP += aobjPADependent.icdoPersonAccountDependent.start_date.ToString(busConstant.DateFormatD8);

                    //PIR 15696 - Modified date is considered instead of Effective date for the 30 days rule.
                    if (((aobjPADependent.icdoPersonAccountDependent.end_date < DateTime.Today)
                        && (aobjPADependent.icdoPersonAccountDependent.modified_date.AddDays(31) >= DateTime.Today || 
                            aobjPADependent.icdoPersonAccountDependent.end_date.AddDays(31) >= DateTime.Today))
                        ||
                        ((aobjPADependent.icdoPersonAccountDependent.end_date >= DateTime.Today)
                        && busGlobalFunctions.CheckDateOverlapping(aobjPADependent.icdoPersonAccountDependent.end_date, DateTime.Today, DateTime.Today.AddDays(31))))
                    {
                        if (busGlobalFunctions.IsLastCharacterAsterisk(lstrDTP))
                            lstrDTP = busGlobalFunctions.RemoveLastCharacter(lstrDTP);
                        lstrDTP += "~\r\n";
                        _lintTransactionSetCount += 1;
                        lstrDTP += "DTP*";
                        // Date Time Qualifier
                        lstrDTP += "349*";
                        // Date Time Format Qualifier
                        lstrDTP += "D8*";
                        lstrDTP += aobjPADependent.icdoPersonAccountDependent.end_date.ToString(busConstant.DateFormatD8);
                    }
                }
            }
            if (busGlobalFunctions.IsLastCharacterAsterisk(lstrDTP))
                lstrDTP = busGlobalFunctions.RemoveLastCharacter(lstrDTP);
            lstrDTP += "~";
            return lstrDTP;
        }

        // REF - Member Policy Number
        private string GetREFMemberPolicy(busPersonAccountGhdv aobjDental)
        {
            _lintTransactionSetCount += 1;
            // Segment Header
            string lstrREF = "REF*";

            // Reference Identification Qualifier
            lstrREF += "1L*";

            if (aobjDental.icdoPersonAccountGhdv.current_dental_insurance_type_value == busConstant.DentalInsuranceTypeActive)
                lstrREF += "9005374820001*";
            else if (aobjDental.icdoPersonAccountGhdv.current_dental_insurance_type_value == busConstant.DentalInsuranceTypeRetiree)
                lstrREF += "9005374820002*";
            else if (aobjDental.icdoPersonAccountGhdv.current_dental_insurance_type_value == busConstant.DentalInsuranceTypeCOBRA)
                lstrREF += "9005374829272*";

            if (busGlobalFunctions.IsLastCharacterAsterisk(lstrREF))
                lstrREF = busGlobalFunctions.RemoveLastCharacter(lstrREF);
            lstrREF += "~\r\n";
            return lstrREF;
        }

        #endregion
    }
}
