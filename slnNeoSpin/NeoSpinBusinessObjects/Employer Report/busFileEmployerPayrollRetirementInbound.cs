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
using Sagitec.ExceptionPub;
using Sagitec.Interface;


#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public partial class busFileEmployerPayrollRetirementInbound : busFileBase
    {
        public busFileEmployerPayrollRetirementInbound()
        {

        }

        private static int lintHeaderGroupValue = 0;
        private string lstrHeaderGroupValue = "0";
        private static int lintCentralPayrollID = 0;


        //Setting the Header Group Values if multiple ORG Code comes in the Same File (Central Payroll)
        public override void SetHeaderGroupValue()
        {
            if (icdoFileDtl != null)
            {
                if (icdoFileDtl.transaction_code_value == "1")
                {
                    lintHeaderGroupValue++;
                    if (lintHeaderGroupValue < 10)
                        lstrHeaderGroupValue = "0" + lintHeaderGroupValue.ToString();
                    else
                        lstrHeaderGroupValue = lintHeaderGroupValue.ToString();
                }
                icdoFileDtl.header_group_value = lstrHeaderGroupValue;
            }
        }

        private busEmployerPayrollHeader _ibusEmployerPayrollHeader;
        public busEmployerPayrollHeader ibusEmployerPayrollHeader
        {
            get { return _ibusEmployerPayrollHeader; }
            set { _ibusEmployerPayrollHeader = value; }
        }

        public override busBase NewHeader()
        {
            _ibusEmployerPayrollHeader = new busEmployerPayrollHeader();
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader();
            _ibusEmployerPayrollHeader.iblnValidateDetail = true;
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.reporting_source_value = busConstant.PayrollHeaderReportingSourceWebRpt;
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.balancing_status_value = busConstant.PayrollHeaderBalancingStatusNoRemittance;
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.interest_waiver_flag = busConstant.Flag_No;
            _ibusEmployerPayrollHeader.icdoFileHdr = icdoFileHdr;
            //PROD Pir 4406
            _ibusEmployerPayrollHeader.iblnFromFile = true;
            return _ibusEmployerPayrollHeader;
        }

        public override busBase NewDetail()
        {
            if (icdoFileDtl.transaction_code_value == "1")
                return _ibusEmployerPayrollHeader;

            return _ibusEmployerPayrollHeader.CreateNewEmployerPayrollDetail();
        }

        public override void ProcessHeader()
        {
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value = busConstant.PayrollHeaderBenefitTypeRtmt;

            if (!(String.IsNullOrEmpty(_ibusEmployerPayrollHeader.istrOrgCodeId)))
            {
                _ibusEmployerPayrollHeader.ibusOrganization = new busOrganization();
                _ibusEmployerPayrollHeader.ibusOrganization.FindOrganizationByOrgCode(_ibusEmployerPayrollHeader.istrOrgCodeId);
                _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.org_id = _ibusEmployerPayrollHeader.ibusOrganization.icdoOrganization.org_id;
            }
            //Assigning Prepopulated Values into Header
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.received_date = DateTime.Now;
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.submitted_date = DateTime.Now;
            // If the File has multiple header, increment the existing max central payroll ID
            if ((lintHeaderGroupValue > 1) && (lintCentralPayrollID == 0))
            {
                lintCentralPayrollID = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoEmployerPayrollHeader.GetMaxCentralPayrollID", new object[] { },
                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                lintCentralPayrollID += 1;
            }
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.central_payroll_record_id = lintCentralPayrollID;
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.total_wages_original = _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.total_wages_reported;
            //PIR 25920 Total Contribution Amount Reported field – Add the ADEC Reported amount to this field to display
            //_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.total_contribution_reported += _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.total_adec_amount_reported; 
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.total_contribution_original = _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.total_contribution_reported;
            foreach (busEmployerPayrollDetail lobjEmployerPayrollDetail in _ibusEmployerPayrollHeader.iclbEmployerPayrollDetail)
            {
                if (lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value != busConstant.PayrollDetailRecordTypeNegativeAdjustment)
                {
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_pre_tax_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_pre_tax_reported;
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_employer_pickup_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_employer_pickup_reported;
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_contribution_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_contribution_reported;
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.er_contribution_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.er_contribution_reported;
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.rhic_ee_contribution_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.rhic_ee_contribution_reported;
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.rhic_er_contribution_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.rhic_er_contribution_reported;
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.wages_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.eligible_wages;
                    //PIR 25920 New Plan DC 2025
                    if (lobjEmployerPayrollDetail.istrPlanValue == busConstant.Plan_Code_DC_2025)
                    {
                        lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.adec_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.adec_reported;
                        lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_pretax_addl_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_pretax_addl_reported;
                        lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_post_tax_addl_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_post_tax_addl_reported;
                        lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.er_pretax_match_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.er_pretax_match_reported;

                        //_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.total_adec_amount_reported += lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.adec_reported;
                        _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.total_adec_amount_original = _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.total_adec_amount_reported;
                    }
                }
                else 
                {
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_pre_tax_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_pre_tax_reported * -1;
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_employer_pickup_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_employer_pickup_reported * -1;
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_contribution_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_contribution_reported * -1;
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.er_contribution_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.er_contribution_reported * -1;
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.rhic_ee_contribution_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.rhic_ee_contribution_reported * -1;
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.rhic_er_contribution_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.rhic_er_contribution_reported * -1;
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.wages_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.eligible_wages * -1;
                    //PIR 25920 New Plan DC 2025
                    if (lobjEmployerPayrollDetail.istrPlanValue == busConstant.Plan_Code_DC_2025)
                    {
                        lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.adec_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.adec_reported * -1;
                        lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_pretax_addl_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_pretax_addl_reported * -1;
                        lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_post_tax_addl_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_post_tax_addl_reported * -1;
                        lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.er_pretax_match_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.er_pretax_match_reported * -1;

                        //_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.total_adec_amount_reported += lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.adec_reported;
                        _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.total_adec_amount_original = _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.total_adec_amount_reported;
                    }
                }
				
            }
            base.ProcessHeader();
        }
        public override void BeforePersisitHeader()
        {

            //Inserting the Header Record
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.Insert();
            if (_ibusEmployerPayrollHeader.iclbEmployerPayrollDetail == null)
            {
                _ibusEmployerPayrollHeader.iclbEmployerPayrollDetail = new Collection<busEmployerPayrollDetail>();
            }
            foreach (busEmployerPayrollDetail lobjEmployerPayrollDetail in _ibusEmployerPayrollHeader.iclbEmployerPayrollDetail)
            {
                lobjEmployerPayrollDetail.ibusEmployerPayrollHeader = ibusEmployerPayrollHeader;
                lobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader = ibusEmployerPayrollHeader.icdoEmployerPayrollHeader;
                lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_header_id = _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.employer_payroll_header_id;

                //Plan ID
                if ((!(String.IsNullOrEmpty(lobjEmployerPayrollDetail.istrPlanValue))))
                {
                    lobjEmployerPayrollDetail.ibusPlan = new busPlan();
                    lobjEmployerPayrollDetail.ibusPlan.FindPlanByPlanCode(lobjEmployerPayrollDetail.istrPlanValue);

                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_id = lobjEmployerPayrollDetail.ibusPlan.icdoPlan.plan_id;
                    //PIR 21588 add new value PLAN_ID_ORIGINAL that should populate when reports are initially created 
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_id_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_id;
                }

                //PERSON ID, FIRST NAME, LAST NAME
                //PIR 24585 If last_name and first letter of first_name changes then the form should go to review status and should display soft error
                if (!String.IsNullOrEmpty(lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ssn))
                {
                    lobjEmployerPayrollDetail.LoadPersonBySSNAndLastName();
                    busPerson lbusPerson = new busPerson() { icdoPerson = new cdoPerson() };
                    lbusPerson.FindPerson(lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.person_id);
                    bool lblnIsLastNameDiffer = false;
                    if (!string.IsNullOrEmpty(lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.last_name) && !string.IsNullOrEmpty(lbusPerson.icdoPerson.last_name))
                    {

                        lblnIsLastNameDiffer =
                       (lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.last_name.Trim().ToLower().ReplaceWith("[^a-zA-Z0-9]", "")) ==
                       (lbusPerson.icdoPerson.last_name.Trim().ToLower().ReplaceWith("[^a-zA-Z0-9]", ""));
                        if (lblnIsLastNameDiffer == true)
                        {
                            DataTable ldtbSSN = busBase.Select<cdoPerson>(new string[2] { "ssn", "last_name" },
                                        new object[2] { lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ssn, lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.last_name }, null, null);
                            if (ldtbSSN.IsNotNull() && ldtbSSN.Rows.Count > 0)
                            {
                                if ((lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.first_name.ToLower().ReplaceWith("[^a-zA-Z0-9]", "").Substring(0, 1)) ==
                                    (Convert.ToString(ldtbSSN.Rows[0]["first_name"]).ToLower().ReplaceWith("[^a-zA-Z0-9]", "").Substring(0, 1)))
                                {
                                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.person_id = Convert.ToInt32(ldtbSSN.Rows[0]["person_id"]);
                                }
                            }
                        }

                    }
                }
                lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.suppress_warnings_flag = "N";
                //Calculate Bonus                 
                //This also calculates the Member Interest and Employer Interest for each month bonus
                if (lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus)
                {
                    lobjEmployerPayrollDetail.iclcEmployerPayrollBonusDetail = busEmployerReportHelper.CalculateBonus(lobjEmployerPayrollDetail);
                }
				//PIR 15616 - Bonus detail record's interest logic same as normal adjustment.
                //Process Interest Calculation for all the entries except Bonus Record Type
                //if (lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value != busConstant.PayrollDetailRecordTypeBonus)
                //{
                lobjEmployerPayrollDetail.ProcessInterestCalculation();
                //}
                //else
                //{
                //    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.member_interest_calculated = 0;
                //    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_interest_calculated = 0;
                //    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_rhic_interest_calculated = 0;
                //    foreach (cdoEmployerPayrollBonusDetail lcdoEmployerPayrollBonusDetail in lobjEmployerPayrollDetail.iclcEmployerPayrollBonusDetail)
                //    {
                //        lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.member_interest_calculated += lcdoEmployerPayrollBonusDetail.member_interest;
                //        lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_interest_calculated += lcdoEmployerPayrollBonusDetail.employer_interest;
                //        lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_rhic_interest_calculated += lcdoEmployerPayrollBonusDetail.employer_rhic_interest;
                //    }
                //}
                lobjEmployerPayrollDetail.UpdateCalculatedAmountForRetirement(String.Empty);
            }
        }

        public override void AfterPersisitHeader()
        {
            //Persist Bonus Details into Table
            foreach (busEmployerPayrollDetail lobjEmployerPayrollDetail in _ibusEmployerPayrollHeader.iclbEmployerPayrollDetail)
            {
                //Insert Bonus Detail Records if the record type is bonus. Delete the Old Entries
                if (lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus)
                {
                    lobjEmployerPayrollDetail.PersistBonusDetails();
                }
            }

            base.AfterPersisitHeader();
        }

        public override string BeforeFieldAssigned(string astrFieldName, string astrFieldValue)
        {
            string lstrReturnValue = astrFieldValue;
            string lstrObjectField = astrFieldName.IndexOf(".") > -1 ? astrFieldName.Substring(astrFieldName.LastIndexOf(".") + 1) : astrFieldName;

            lstrObjectField = lstrObjectField.ToLower();

            if (((lstrObjectField == busConstant.EmployerReportPayrollPaidDate) || (lstrObjectField == busConstant.EmployerReportPayPeriod)
               || (lstrObjectField == busConstant.EmployerReportPayPeriodDate)) && (astrFieldValue.Length == 6))
            {
                lstrReturnValue = astrFieldValue.Substring(0, 2) + "/01/" + astrFieldValue.Substring(2, 4);
            }

            if (((lstrObjectField == busConstant.EmployerReportPayPeriodEndMonthForBonus) || (lstrObjectField == busConstant.EmployerReportMemberStatusEffectiveDate))
                                                                            && (astrFieldValue.Length == 6))
            {
                lstrReturnValue = astrFieldValue.Substring(0, 2) + "/01/" + astrFieldValue.Substring(2, 4);
            }

            //Record Type Value for detail
            if (lstrObjectField == "record_type_value")
            {
                if ((!(String.IsNullOrEmpty(astrFieldValue))))
                {
                    if (astrFieldValue == "1")
                    {
                        lstrReturnValue = busConstant.PayrollDetailRecordTypeRegular;
                    }
                    else if (astrFieldValue == "2")
                    {
                        lstrReturnValue = busConstant.PayrollDetailRecordTypePositiveAdjustment;
                    }
                    else if (astrFieldValue == "3")
                    {
                        lstrReturnValue = busConstant.PayrollDetailRecordTypeBonus;
                    }
                    else if (astrFieldValue == "4")
                    {
                        lstrReturnValue = busConstant.PayrollDetailRecordTypeNegativeAdjustment;
                    }
                }
            }

            //Report type for Header
            if (lstrObjectField == "report_type_value")
            {
                if ((!(String.IsNullOrEmpty(astrFieldValue))))
                {
                    if (astrFieldValue == "1")
                    {
                        lstrReturnValue = busConstant.PayrollHeaderReportTypeRegular;
                    }
                    else if (astrFieldValue == "2")
                    {
                        lstrReturnValue = busConstant.PayrollHeaderReportTypeAdjustment;
                    }
                }
            }

            //Member Status Value for detail
            if (lstrObjectField == "member_status_value")
            {
                if ((!(String.IsNullOrEmpty(astrFieldValue))))
                {
                    if (astrFieldValue == "1")
                    {
                        lstrReturnValue = busConstant.EmployerPayrollDetailMemberStatusACTIVE;
                    }
                    else if (astrFieldValue == "2")
                    {
                        lstrReturnValue = busConstant.EmployerPayrollDetailMemberStatusTerminated;
                    }
                    else if (astrFieldValue == "3")
                    {
                        lstrReturnValue = busConstant.EmployerPayrollDetailMemberStatusLOA;
                    }
                    else if (astrFieldValue == "4")
                    {
                        lstrReturnValue = busConstant.EmployerPayrollDetailMemberStatusLOAM;
                    }
                    else if (astrFieldValue == "5")
                    {
                        lstrReturnValue = busConstant.EmployerPayrollDetailMemberStatusDeath;
                    }
                }
            }
            //If total wages reported is less than 0
            if (lstrObjectField == "total_wages_reported")
            {
                if ((!(String.IsNullOrEmpty(astrFieldValue))))
                {
                    if (astrFieldValue.IndexOf("-") > -1)
                    {
                        lstrReturnValue = lstrReturnValue.Substring(astrFieldValue.IndexOf("-"));
                    }
                }
            }
            //If total contribution reported is less than 0
            if (lstrObjectField == "total_contribution_reported")
            {
                if ((!(String.IsNullOrEmpty(astrFieldValue))))
                {
                    if (astrFieldValue.IndexOf("-") > -1)
                    {
                        lstrReturnValue = lstrReturnValue.Substring(astrFieldValue.IndexOf("-"));
                    }
                }
            }
            return lstrReturnValue;
        }

        public override sfwOnFileError ContinueOnValueError(string astrObjectField, out string astrValue)
        {
            astrValue = String.Empty;
            
            string lstrObjectField = astrObjectField.IndexOf(".") > -1 ? astrObjectField.Substring(astrObjectField.LastIndexOf(".") + 1) : astrObjectField;
            //NOT YET IMPLEMENTED.. BASE IS DEFINED...  
            switch (lstrObjectField.ToLower())
            {
                case busConstant.EmployerReportPayrollPaidDate:
                case busConstant.EmployerReportPayPeriodDate:
                case busConstant.EmployerReportPayPeriod:        
                case busConstant.EmployerReportPayPeriodEndMonthForBonus:
                    astrValue = String.Empty;
                    return sfwOnFileError.ContinueWithRecord;

                default: return base.ContinueOnValueError(astrObjectField, out astrValue);
            }
        }
        public override void AfterReject(int aintErrorRecNo, string astrErrorMessage)
        {
            if (icdoFileHdr.reference_id > 0) // PIR - 14286 - when file is uploaded from internal, reference ID (org_id) will be zero 
            {
                string lstrPrioityValue = string.Empty;
                busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(6, iobjPassInfo, ref lstrPrioityValue), icdoFileHdr.mailbox_file_name), lstrPrioityValue, aintOrgID: icdoFileHdr.reference_id);
                busGlobalFunctions.MoveFileFromProcessedToErrorFolder(icdoFileHdr, iobjPassInfo);
            }
        }

        public override void FinalizeFile()
        {
            lintCentralPayrollID = 0;
            lintHeaderGroupValue = 0;
            //PIR-13996 Added code to display message on Dashboard when Header Status is Review or Processed With Warnings.
            if ((icdoFileHdr != null && ((icdoFileHdr.status_value == busConstant.PayrollHeaderStatusReview) || (icdoFileHdr.status_value == busConstant.PayrollHeaderStatusProcessedWithWarnings)))
                && ((base.iarrErrors != null && base.iarrErrors.Count > 0) || (_ibusEmployerPayrollHeader.iclbDetailError != null && _ibusEmployerPayrollHeader.iclbDetailError.Count > 0) || (_ibusEmployerPayrollHeader.iclbEmployerPayrollHeaderError != null && _ibusEmployerPayrollHeader.iclbEmployerPayrollHeaderError.Count > 0)) && (icdoFileHdr.reference_id > 0))
            {
                string lstrPrioityValue = string.Empty;
                busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(7, iobjPassInfo, ref lstrPrioityValue), icdoFileHdr.mailbox_file_name), lstrPrioityValue, aintOrgID: icdoFileHdr.reference_id);
            }
            base.FinalizeFile();
        }
    }
}
