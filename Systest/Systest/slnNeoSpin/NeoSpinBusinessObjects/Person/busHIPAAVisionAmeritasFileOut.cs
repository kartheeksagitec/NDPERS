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
    public class busHIPAAVisionAmeritasFileOut : busFileBaseOut
    {
        public DateTime ldtTodaysDate
        {
            get { return DateTime.Now; }
        }

        public override void InitializeFile()
        {
            istrFileName = "834.NDPERS." + _lstrProviderOrgCode + "." + ldtTodaysDate.ToString(busConstant.DateFormat) + busConstant.FileFormattxt;
        }

        // Unique number everytime file generates. 
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
            lcdoCodeValue = busGlobalFunctions.GetCodeValueDetails(busConstant.SystemConstantCodeID,
                iblnIsSuperiorVision ? busConstant.SystemConstantISASuperiorVision : busConstant.SystemConstantISAVisionAmeritas);
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
            if (iblnIsSuperiorVision)
                _lstrProviderOrgCode = busConstant.SuperiorVisionOrgCode;
            else
            {
                DataTable ldtbResult = busBase.Select("cdoPersonAccount.LoadActiveProviderByPlan",
                    new object[2] { busConstant.PlanIdVision, ldtTodaysDate });
                if (ldtbResult.Rows.Count > 0)
                {
                    if (!Convert.IsDBNull(ldtbResult.Rows[0]["org_code"]))
                        _lstrProviderOrgCode = ldtbResult.Rows[0]["org_code"].ToString();
                }
            }
        }

        // Load HIPAA Reference ID from Organization Table.
        private string GetHIPAAReferenceID(busPersonAccountGhdv aobjVision)
        {
            if (aobjVision != null)
            {
                if (aobjVision.IsCOBRAValueSelected())
                    return "00999";
                else
                {
                    if (aobjVision.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id != 0)
                    {
                        if (!string.IsNullOrEmpty(aobjVision.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.hipaa_reference_id))
                            return aobjVision.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.hipaa_reference_id;
                    }
                    else // No Employment 
                        return "00002";
                }
            }
            return "00001"; // All other cases - Referred to NDPERS
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

        public DataTable idtbVisionData { get; set; }

        public Collection<int> iclbVision { get; set; }

        public bool iblnIsDetail { get; set; }

        private busBase iobjBase { get; set; }

        private DataTable idtbAllPersonAccountDependent { get; set; }

        private DataTable idtbAllPersonAddress { get; set; }

        private DataTable idtbAllPersonEmployment { get; set; }

        private bool iblnIsSuperiorVision { get; set; }

        private bool iblnIsLayout5010 { get; set; }

        public void LoadVisionAmeritas(DataTable adtbVisionAmeritas)
        {
            iobjBase = new busBase();
            iclbVision = (Collection<int>)iarrParameters[0];
            idtbVisionData = (DataTable)iarrParameters[1];
            idtbAllPersonAccountDependent = (DataTable)iarrParameters[2];
            idtbAllPersonAddress = (DataTable)iarrParameters[3];
            idtbAllPersonEmployment = (DataTable)iarrParameters[4];
            iblnIsSuperiorVision = (bool)iarrParameters[5];
            LoadAndUpdatesInterchangeControlNo();
            LoadProviderOrgCode();
            if (Convert.ToString(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.New5010HIPAALayoutForSuperVision, iobjPassInfo)) == busConstant.Flag_Yes) //pir 8570
                iblnIsLayout5010 = true;
        }

        public override void FinalizeFile()
        {
            base.FinalizeFile();

            // Write Footer Details
            istrRecord = string.Empty;
            istrRecord += GetFooterDetails();
            iswrOut.Write(istrRecord);

            DBFunction.DBNonQuery("cdoPersonAccountGhdv.Update_HIPAA_BCBS_Flag", new object[1] { busConstant.PlanIdVision }, iobjPassInfo.iconFramework,
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
                DataRow ldrRow = idtbVisionData.Rows[int.Parse(iobjDetail.ToString())];
                busPersonAccountGhdv lbusVision = new busPersonAccountGhdv
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
                lbusVision.icdoPersonAccount.LoadData(ldrRow);
                lbusVision.icdoPersonAccountGhdv.LoadData(ldrRow);
                lbusVision.ibusPerson.icdoPerson.LoadData(ldrRow);
                lbusVision.ibusPlan.icdoPlan.LoadData(ldrRow);

                // Load the Dependents
                DataRow[] ldtbDependents = busGlobalFunctions.FilterTable(idtbAllPersonAccountDependent,
                                           busConstant.DataType.Numeric, "PERSON_ACCOUNT_ID", lbusVision.icdoPersonAccount.person_account_id);
                foreach (DataRow ldtrRow in ldtbDependents)
                {
                    busPersonAccountDependent lobjPADependent = new busPersonAccountDependent
                    {
                        icdoPersonAccountDependent = new cdoPersonAccountDependent(),
                        icdoPersonDependent = new cdoPersonDependent()
                    };
                    lobjPADependent.icdoPersonAccountDependent.LoadData(ldtrRow);
                    lobjPADependent.icdoPersonDependent.LoadData(ldtrRow);
                    lbusVision.iclbPersonAccountDependent.Add(lobjPADependent);
                }
                lbusVision.iclbPersonAccountDependent = lbusVision.iclbPersonAccountDependent.Where(lobj =>
                                        lobj.icdoPersonAccountDependent.is_bcbs_file_sent_flag != busConstant.Flag_Yes).ToList().ToCollection();

                // Load the addresses
                DataRow[] ldtrAddress = busGlobalFunctions.FilterTable(idtbAllPersonAddress, busConstant.DataType.Numeric, "PERSON_ID", lbusVision.icdoPersonAccount.person_id);
                lbusVision.ibusPerson.iclbPersonAddress = iobjBase.GetCollection<busPersonAddress>(ldtrAddress, "icdoPersonAddress");
                
                // PIR 9029 - get latest address irrespective of end date
                lbusVision.ibusPerson.ibusPersonCurrentAddress = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };
                if (lbusVision.ibusPerson.iclbPersonAddress.Count > 0)
                    lbusVision.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress = lbusVision.ibusPerson.iclbPersonAddress[0].icdoPersonAddress;

                // Load the Employment
                DataRow[] ldtrPAEmployment = busGlobalFunctions.FilterTable(idtbAllPersonEmployment,
                                             busConstant.DataType.Numeric, "PERSON_ACCOUNT_ID", lbusVision.icdoPersonAccount.person_account_id);
                if (ldtrPAEmployment.Count() > 0)
                {
                    lbusVision.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.LoadData(ldtrPAEmployment[0]);
                    lbusVision.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.LoadData(ldtrPAEmployment[0]);
                    lbusVision.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.LoadData(ldtrPAEmployment[0]);
                }
                lbusVision.icdoPersonAccount.person_employment_dtl_id = lbusVision.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;

                // Load the Segments for the Subscriber ie, Member
                istrRecord += GetSubscribersSegments(lbusVision);
                int lintCount = 0;
                foreach (busPersonAccountDependent lobjPADependent in lbusVision.iclbPersonAccountDependent)
                {
                    lintCount += 1;
                    if (lintCount == 1)
                        istrRecord += "\r\n";
                    // Load the Segments for all the Dependents
                    lobjPADependent.LoadDependentInfo();
                    istrRecord += GetDependentsSegments(lbusVision, lobjPADependent);

                    // no line break for the last dependent
                    if (lbusVision.iclbPersonAccountDependent.Count != lintCount)
                        istrRecord += "\r\n";

                    // SYS PIR ID 2614
                    if (lobjPADependent.icdoPersonAccountDependent.end_date_no_null.Date <= ldtTodaysDate.Date)
                    {
                        lobjPADependent.icdoPersonAccountDependent.is_bcbs_file_sent_flag = busConstant.Flag_Yes;
                        lobjPADependent.icdoPersonAccountDependent.iblnUpdateModifiedBy = false;//PIR 17178 - Modified date shound not be updated.
                        lobjPADependent.icdoPersonAccountDependent.Update();
                    }
                }
            }
        }

        private string istrReceiverID
        {
            get
            {
                if (iblnIsSuperiorVision)
                    return "133741352";
                else
                    return "470098400";
            }
        }

        private string istrInsurerName
        {
            get
            {
                if (iblnIsSuperiorVision)
                    return "SuperiorVision";
                else
                    return "Ameritas Group Vision";
            }
        }

        private string istrIndustryIdentifierCode
        {
            get
            {
                if (iblnIsLayout5010)
                    return "005010X220A1";
                else
                {
                    if (iblnIsSuperiorVision)
                        return "004010X095A1";
                    else
                        return "004010X095";
                }
            }
        }

        private string GetHeaderDetails()
        {
            string lstrHeader = string.Empty;
            DataTable ldtbList = iobjPassInfo.isrvDBCache.GetCodeDescription(1213, "HIAM");
            string lstrORgIdentifier = ldtbList.Rows.Count > 0 ? ldtbList.Rows[0]["data2"].ToString() : string.Empty;
            lstrHeader = "ISA*00*          *00*          *30*450282090      *30*" + istrReceiverID + "      *" + ldtTodaysDate.ToString("yyMMdd") + "*" + ldtTodaysDate.ToString("HHmm") 
                        + "*" + (iblnIsLayout5010 ? "^" : "U") + "*" + (iblnIsLayout5010 ? "00501" : "00401") + "*" + lstrInterchangeControlNo + "*1*P*>~" + "\r\n";
            lstrHeader += "GS*BE*" + (iblnIsSuperiorVision ? "NDPERS*" : "450282090*") + (iblnIsSuperiorVision ? "SuperiorVision" : "133741352") + "*" + ldtTodaysDate.ToString("yyyyMMdd") 
                        + "*" + ldtTodaysDate.ToString("HHmm") + "*1*X*" + istrIndustryIdentifierCode + "~" + "\r\n";
            lstrHeader += "ST*834*0001" + (iblnIsLayout5010 ? "*005010X220A1" : "") + "~" + "\r\n";
            lstrHeader += "BGN*00*1*" + ldtTodaysDate.ToString("yyyyMMdd") + "*" + ldtTodaysDate.ToString("HHmmssdd") + "****4~" + "\r\n";
            if(iblnIsSuperiorVision)
                lstrHeader += "REF*38*2985401~" + "\r\n";
            lstrHeader += "N1*P5*NDPERS*FI*450282090~" + "\r\n";
            lstrHeader += "N1*IN*" + istrInsurerName + "*FI*" + istrReceiverID + "~" + "\r\n";
            return lstrHeader;
        }

        private string GetFooterDetails()
        {
            string lstrFooter = string.Empty;
            _lintTransactionSetCount += 6; // PIR 10634
            lstrFooter += "SE*" + lstrTransactionSetCount + "*0001~" + "\r\n";
            lstrFooter += "GE*1*1~" + "\r\n";
            lstrFooter += "IEA*1*" + lstrInterchangeControlNo + "~";
            return lstrFooter;
        }

        // Add Employee's Segments
        private string GetSubscribersSegments(busPersonAccountGhdv aobjVision)
        {
            string lstrResult = string.Empty;
            lstrResult = GetINS(true, aobjVision.IsCOBRAValueSelected(), null, aobjVision);
            lstrResult += GetREFSubsriber(aobjVision);
            if (iblnIsSuperiorVision)
            {
                lstrResult += GetREFMemberPolicy(aobjVision);
                lstrResult += "\r\n" + GetREFMemberSuperiorPolicy(aobjVision);
                lstrResult += "\r\n";
                //lstrResult += "\r\n" + GetDTPForSuperior(aobjVision); PROD PIR ID 6007
            }
            lstrResult += GetNM1(true, null, aobjVision);
            lstrResult += GetN3(aobjVision);
            lstrResult += GetN4(aobjVision);
            lstrResult += GetDMG(true, null, aobjVision);
            lstrResult += GetHD(aobjVision);
            lstrResult += GetDTP(true, aobjVision);
            if (!iblnIsSuperiorVision)
                lstrResult += GetREFMemberPolicy(aobjVision);
            return lstrResult;
        }

        // Add Dependent's Segments
        private string GetDependentsSegments(busPersonAccountGhdv aobjVision, busPersonAccountDependent aobjPADependent)
        {
            string lstrResult = string.Empty;
            lstrResult = GetINS(false, false, aobjPADependent, aobjVision);
            lstrResult += GetREFSubsriber(aobjVision);
            if (iblnIsSuperiorVision)
            {
                lstrResult += GetREFMemberPolicy(aobjVision) + "\r\n";
                lstrResult += GetREFMemberSuperiorPolicy(aobjVision) + "\r\n";
            }
            lstrResult += GetNM1(false, aobjPADependent, aobjVision);
            lstrResult += GetDMG(false, aobjPADependent, aobjVision);
            lstrResult += GetHD(aobjVision);
            lstrResult += GetDTP(false, aobjVision, aobjPADependent);
            if (!iblnIsSuperiorVision)
                lstrResult += GetREFMemberPolicy(aobjVision);                
            return lstrResult;
        }

        #region Segment Methods

        // INS - Member Level Detail
        private string GetINS(bool ablnIsSubscriber, bool ablnIsCOBRA, busPersonAccountDependent aobjPADependent, busPersonAccountGhdv aobjVision)
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

            if (iblnIsSuperiorVision)
            {
                // Benefit Status Code
                if (ablnIsCOBRA)
                    lstrINS += "C**1*";
                else
                    lstrINS += "A***";
            }
            else
            {
                // Benefit Status Code
                if (ablnIsCOBRA)
                    lstrINS += "C**1*";
                else
                    lstrINS += "A***";
            }

            if (ablnIsSubscriber)
            {
                // Employment Status Code
                aobjVision.LoadPersonEmploymentDetail();
                if (aobjVision.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent)
                    lstrINS += "FT*";
                else if (aobjVision.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                    lstrINS += "PT*";
                else if (iblnIsSuperiorVision && aobjVision.icdoPersonAccountGhdv.vision_insurance_type_value == busConstant.VisionInsuranceTypeRetiree)
                    lstrINS += "RT*";
                else
                    lstrINS += "*";
            }
            else
            {
                // Employment Status Code
                lstrINS += "*";

                // Student Status
                if (aobjPADependent.icdoPersonDependent.full_time_student_flag == busConstant.Flag_Yes)
                    lstrINS += "F*";
                else
                    lstrINS += "N*";
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

        // REF - Subscriber Number
        private string GetREFSubsriber(busPersonAccountGhdv aobjVision)
        {
            _lintTransactionSetCount += 1;
            // Segment Header
            string lstrREF = "REF*";

            // Reference Identification Qualifier
            lstrREF += "0F*";

            // Reference Identification - SSN
            lstrREF += aobjVision.ibusPerson.icdoPerson.ssn;
            if (busGlobalFunctions.IsLastCharacterAsterisk(lstrREF))
                lstrREF = busGlobalFunctions.RemoveLastCharacter(lstrREF);
            lstrREF += "~\r\n";
            return lstrREF;
        }

        // NM1 - Member Name
        private string GetNM1(bool ablnIsSubscriber, busPersonAccountDependent aobjPADependent, busPersonAccountGhdv aobjVision)
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
                if (!string.IsNullOrEmpty(aobjVision.ibusPerson.icdoPerson.last_name))
                    lstrNM1 += aobjVision.ibusPerson.icdoPerson.last_name.Trim().ToUpper() + "*";
                else
                    lstrNM1 += "*";

                // First Name
                if (!string.IsNullOrEmpty(aobjVision.ibusPerson.icdoPerson.first_name))
                    lstrNM1 += aobjVision.ibusPerson.icdoPerson.first_name.Trim().ToUpper() + "*";
                else
                    lstrNM1 += "*";

                // Middle Name
                if (!string.IsNullOrEmpty(aobjVision.ibusPerson.icdoPerson.middle_name))
                    lstrNM1 += aobjVision.ibusPerson.icdoPerson.middle_name.Trim().ToUpper() + "*";
                else
                    lstrNM1 += "*";

                // Prefix Name
                if (!string.IsNullOrEmpty(aobjVision.ibusPerson.icdoPerson.name_prefix_description))
                    lstrNM1 += aobjVision.ibusPerson.icdoPerson.name_prefix_description.Trim().ToUpper() + "*";
                else
                    lstrNM1 += "*";

                // Suffix Name
                if (!string.IsNullOrEmpty(aobjVision.ibusPerson.icdoPerson.name_suffix_description))
                    lstrNM1 += aobjVision.ibusPerson.icdoPerson.name_suffix_description.Trim().ToUpper() + "*";
                else
                    lstrNM1 += "*";

                // Member's SSN
                if (!string.IsNullOrEmpty(aobjVision.ibusPerson.icdoPerson.ssn))
                    lstrNM1 += "34*" + aobjVision.ibusPerson.icdoPerson.ssn.ToUpper();
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
        private string GetN3(busPersonAccountGhdv aobjVision)
        {
            _lintTransactionSetCount += 1;
            string lstrN3 = "N3";
            if (!string.IsNullOrEmpty(aobjVision.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_1))
                lstrN3 += "*" + aobjVision.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_1.ToUpper().Trim();
            if (!string.IsNullOrEmpty(aobjVision.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_2))
                lstrN3 += "*" + aobjVision.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_2.ToUpper().Trim();
            if (busGlobalFunctions.IsLastCharacterAsterisk(lstrN3))
                lstrN3 = busGlobalFunctions.RemoveLastCharacter(lstrN3);
            lstrN3 += "~\r\n";
            return lstrN3;
        }

        // N4 - Member Residence City, State, Zip-code
        private string GetN4(busPersonAccountGhdv aobjVision)
        {
            _lintTransactionSetCount += 1;
            string lstrN4 = "N4*";

            // N401
            if (!string.IsNullOrEmpty(aobjVision.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_city))
                lstrN4 += aobjVision.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_city.ToUpper().Trim() + "*";
            else
                lstrN4 += "*";

            // N402
            if (aobjVision.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_value == busConstant.US_Code_ID ||
                aobjVision.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_value == busConstant.Canada_Code_ID)
            {
                if (!string.IsNullOrEmpty(aobjVision.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_state_value))
                    lstrN4 += aobjVision.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_state_value.Trim() + "*";
                else
                    lstrN4 += "*";
            }
            else
                lstrN4 += "*";

            // N403
            string lstrAddr4ZipCode = string.IsNullOrEmpty(aobjVision.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_4_code) ?
                                        string.Empty : aobjVision.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_4_code.Trim();
            if (!string.IsNullOrEmpty(aobjVision.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_code))
                lstrN4 += busGlobalFunctions.GetValidZipCode(aobjVision.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_code.Trim(), lstrAddr4ZipCode) + "*";
            else if (!string.IsNullOrEmpty(aobjVision.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.foreign_postal_code))
                lstrN4 += aobjVision.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.foreign_postal_code.Trim() + "*";
            else
                lstrN4 += "*";

            // N404
            if ((!string.IsNullOrEmpty(aobjVision.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_value)) &&
                (aobjVision.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_value != busConstant.US_Code_ID))
            {
                string lstrCountryCode = busGlobalFunctions.GetData1ByCodeValue(aobjVision.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_id,
                                                                aobjVision.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_value, iobjPassInfo);
                if (!string.IsNullOrEmpty(lstrCountryCode))
                    lstrN4 += lstrCountryCode.Trim() + "*";
            }

            while (busGlobalFunctions.IsLastCharacterAsterisk(lstrN4))
                lstrN4 = busGlobalFunctions.RemoveLastCharacter(lstrN4);
            lstrN4 += "~\r\n";
            return lstrN4;
        }

        // DMG - Member Demographics
        private string GetDMG(bool ablnIsSubscriber, busPersonAccountDependent aobjPADependent, busPersonAccountGhdv aobjVision)
        {
            _lintTransactionSetCount += 1;
            string lstrDMG = "DMG*";
            lstrDMG += "D8*";
            // Subscriber or Member
            if (ablnIsSubscriber)
            {
                // Date of Birth
                if (aobjVision.ibusPerson.icdoPerson.date_of_birth != DateTime.MinValue)
                    lstrDMG += aobjVision.ibusPerson.icdoPerson.date_of_birth.ToString(busConstant.DateFormatD8) + "*";
                // Gender
                if (aobjVision.ibusPerson.icdoPerson.gender_value == busConstant.GenderTypeFemale)
                    lstrDMG += "F*";
                else
                    lstrDMG += "M*";
                // Marital Status
                if (aobjVision.ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried)
                    lstrDMG += "M";
                else if (aobjVision.ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusSingle)
                    lstrDMG += "I";
            }
            else // Dependent
            {
                // Date of Birth
                if (aobjPADependent.icdoPersonDependent.dependent_DOB != DateTime.MinValue)
                    lstrDMG += aobjPADependent.icdoPersonDependent.dependent_DOB.ToString(busConstant.DateFormatD8) + "*";
                // Gender
                if (string.IsNullOrEmpty(aobjPADependent.icdoPersonDependent.dependent_gender))
                    lstrDMG += "U*";
                else if (aobjPADependent.icdoPersonDependent.dependent_gender == busConstant.GenderTypeFemale)
                    lstrDMG += "F*";
                else if (aobjPADependent.icdoPersonDependent.dependent_gender == busConstant.GenderTypeMale)
                    lstrDMG += "M*";
            }
            if (busGlobalFunctions.IsLastCharacterAsterisk(lstrDMG))
                lstrDMG = busGlobalFunctions.RemoveLastCharacter(lstrDMG);
            lstrDMG += "~\r\n";
            return lstrDMG;
        }

        // HD - Vision Coverage
        private string GetHD(busPersonAccountGhdv aobjVision)
        {
            _lintTransactionSetCount += 1;
            string lstrHD = "HD*";
            // Maintenance Type Code
            lstrHD += "030**";
            // Insurance Line Code
            lstrHD += "VIS*";
            // Plan coverage Description
            lstrHD += "01*";
            // Coverage Level Code
            if (aobjVision.icdoPersonAccountGhdv.level_of_coverage_value == busConstant.VisionLevelofCoverageFamily)
                lstrHD += "FAM";
            else if (aobjVision.icdoPersonAccountGhdv.level_of_coverage_value == busConstant.VisionLevelofCoverageIndividual)
                lstrHD += (iblnIsSuperiorVision ? "EMP" : "IND");
            else if (aobjVision.icdoPersonAccountGhdv.level_of_coverage_value == busConstant.VisionLevelofCoverageIndividualChild)
                lstrHD += "ECH";
            else if (aobjVision.icdoPersonAccountGhdv.level_of_coverage_value == busConstant.VisionLevelofCoverageIndividualSpouse)
                lstrHD += "ESP";

            if (busGlobalFunctions.IsLastCharacterAsterisk(lstrHD))
                lstrHD = busGlobalFunctions.RemoveLastCharacter(lstrHD);
            lstrHD += "~\r\n";
            return lstrHD;
        }

        // DTP - Vision Coverage Dates
        private string GetDTP(bool ablnIsSubscriber, busPersonAccountGhdv abusVision, busPersonAccountDependent aobjPADependent = null)
        {
            _lintTransactionSetCount += 1;
            string lstrDTP = "DTP*";
            if (ablnIsSubscriber)
            {
                if (abusVision.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                {
                    // Date Time Qualifier
                    lstrDTP += "348*";
                    // Date Time Format Qualifier
                    lstrDTP += "D8*";
                    lstrDTP += abusVision.icdoPersonAccount.history_change_date_no_null.ToString(busConstant.DateFormatD8);
                }
                else if (abusVision.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended ||
                        abusVision.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceCancelled)  // PROD PIR ID 4242
                {
                    lstrDTP += "348*";
                    lstrDTP += "D8*";
                    if (abusVision.icdoPersonAccount.header_plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended)
                    {
                        lstrDTP += abusVision.icdoPersonAccountGhdv.previous_history_start_date.ToString(busConstant.DateFormatD8);
                    }
                    else
                    {
                        lstrDTP += abusVision.icdoPersonAccountGhdv.current_plan_start_date_from_history.ToString(busConstant.DateFormatD8);
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
                    if (abusVision.icdoPersonAccountGhdv.current_plan_end_date_from_history != DateTime.MinValue)
                    {
                        lstrDTP += abusVision.icdoPersonAccountGhdv.current_plan_end_date_from_history.ToString(busConstant.DateFormatD8);
                    }
                    else
                    {
                        lstrDTP += abusVision.icdoPersonAccount.history_change_date_no_null.AddDays(-1).ToString(busConstant.DateFormatD8);
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
            if (busGlobalFunctions.IsLastCharacterAsterisk(lstrDTP))
                lstrDTP = busGlobalFunctions.RemoveLastCharacter(lstrDTP);
            lstrDTP += "~";
            if (!iblnIsSuperiorVision)
                lstrDTP += "\r\n";
            return lstrDTP;
        }

        // DTP - Vision Coverage Dates
        private string GetDTPForSuperior(busPersonAccountGhdv abusVision)
        {
            _lintTransactionSetCount += 1;
            string lstrDTP = "DTP*";
            if (abusVision.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date != DateTime.MinValue)
            {
                lstrDTP += "337*";  // Date Time Qualifier                            
                lstrDTP += "D8*";   // Date Time Format Qualifier
                lstrDTP += abusVision.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date.ToString(busConstant.DateFormatD8);
            }
            else
            {
                lstrDTP += "336*";  // Date Time Qualifier                            
                lstrDTP += "D8*";   // Date Time Format Qualifier
                lstrDTP += abusVision.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date.ToString(busConstant.DateFormatD8);
            }
            if (busGlobalFunctions.IsLastCharacterAsterisk(lstrDTP))
                lstrDTP = busGlobalFunctions.RemoveLastCharacter(lstrDTP);
            lstrDTP += "~\r\n";
            return lstrDTP;
        }

        // REF - Member Policy Number
        private string GetREFMemberPolicy(busPersonAccountGhdv aobjVision)
        {
            _lintTransactionSetCount += 1;
            // Segment Header
            string lstrREF = "REF*";

            // Reference Identification Qualifier
            lstrREF += "1L*";

            // Structure or Plan Code
            if (iblnIsSuperiorVision)
            {
                lstrREF += "029854-01";
            }
            else
            {
                lstrREF += "010350308";
                lstrREF += GetHIPAAReferenceID(aobjVision);
            }

            if (busGlobalFunctions.IsLastCharacterAsterisk(lstrREF))
                lstrREF = busGlobalFunctions.RemoveLastCharacter(lstrREF);

            lstrREF += "~";
            return lstrREF;
        }

        // REF - Member Policy Number
        private string GetREFMemberSuperiorPolicy(busPersonAccountGhdv aobjVision)
        {
            _lintTransactionSetCount += 1;
            // Segment Header
            string lstrREF = "REF*";

            // Reference Identification Qualifier
            lstrREF += "DX*";

            // Reference Identification
            // Structure or Plan Code
            if (aobjVision.icdoPersonAccountGhdv.vision_insurance_type_value == busConstant.VisionInsuranceTypeActive)
                lstrREF += aobjVision.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.org_code;
            else
                lstrREF += aobjVision.GetGroupNumber();

            if (busGlobalFunctions.IsLastCharacterAsterisk(lstrREF))
                lstrREF = busGlobalFunctions.RemoveLastCharacter(lstrREF);

            lstrREF += "~";
            return lstrREF;
        }

        #endregion
    }
}
