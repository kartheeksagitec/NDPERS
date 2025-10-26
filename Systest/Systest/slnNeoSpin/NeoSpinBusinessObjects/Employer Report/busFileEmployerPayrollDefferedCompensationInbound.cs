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

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busFileEmployerPayrollDefferedCompensationInbound : busFileBase
    {
        public busFileEmployerPayrollDefferedCompensationInbound()
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
            //PROD Pir 4406
            _ibusEmployerPayrollHeader.iblnFromFile = true;
            _ibusEmployerPayrollHeader.icdoFileHdr = icdoFileHdr;
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
            //Assigning Prepopulated Values into Header
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value = busConstant.PayrollHeaderBenefitTypeDefComp;

            if (!(String.IsNullOrEmpty(_ibusEmployerPayrollHeader.istrOrgCodeId)))
            {
                _ibusEmployerPayrollHeader.ibusOrganization = new busOrganization();
                _ibusEmployerPayrollHeader.ibusOrganization.FindOrganizationByOrgCode(_ibusEmployerPayrollHeader.istrOrgCodeId);
                _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.org_id = _ibusEmployerPayrollHeader.ibusOrganization.icdoOrganization.org_id;
            }
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
            //PIR 25920 For DEFF header types total_wages_reported should be NULL. 
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.total_wages_reported = 0;
            base.ProcessHeader();
        }

        public override void BeforePersisitHeader()
        {
            DateTime ldtPreviousRecordPayCheckDate = DateTime.MinValue;
            bool lblnIsPayCheckDateSame = true;
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

                if (ldtPreviousRecordPayCheckDate != DateTime.MinValue && ldtPreviousRecordPayCheckDate != lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_check_date)
                    lblnIsPayCheckDateSame = false;
                //Plan ID
                if ((!(String.IsNullOrEmpty(lobjEmployerPayrollDetail.istrPlanValue))))
                {
                    lobjEmployerPayrollDetail.ibusPlan = new busPlan();
                    lobjEmployerPayrollDetail.ibusPlan.FindPlanByPlanCode(lobjEmployerPayrollDetail.istrPlanValue);

                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_id = lobjEmployerPayrollDetail.ibusPlan.icdoPlan.plan_id;
                }

                //PERSON ID, FIRST NAME, LAST NAME
                if (!String.IsNullOrEmpty(lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ssn))
                {
                    DataTable ldtbSSN = busBase.Select<cdoPerson>(new string[1] { "ssn" }, new object[1] { lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ssn }, null, null);
                    if (ldtbSSN.Rows.Count > 0)
                    {
                        lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.person_id = Convert.ToInt32(ldtbSSN.Rows[0]["person_id"]);
                    }
                }

                if (!String.IsNullOrEmpty(lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id1))
                {
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_id1 = busGlobalFunctions.GetOrgIdFromOrgCode(lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id1);
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount1_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount1;
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id1_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id1;

                }
                if (!String.IsNullOrEmpty(lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id2))
                {
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_id2 = busGlobalFunctions.GetOrgIdFromOrgCode(lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id2);
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount2_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount2;
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id2_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id2;
                }
                if (!String.IsNullOrEmpty(lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id3))
                {
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_id3 = busGlobalFunctions.GetOrgIdFromOrgCode(lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id3);
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount3_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount3;
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id3_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id3;
                }
                if (!String.IsNullOrEmpty(lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id4))
                {
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_id4 = busGlobalFunctions.GetOrgIdFromOrgCode(lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id4);
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount4_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount4;
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id4_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id4;
                }
                if (!String.IsNullOrEmpty(lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id5))
                {
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_id5 = busGlobalFunctions.GetOrgIdFromOrgCode(lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id5);
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount5_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount5;
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id5_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id5;
                }
                if (!String.IsNullOrEmpty(lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id6))
                {
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_id6 = busGlobalFunctions.GetOrgIdFromOrgCode(lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id6);
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount6_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount6;
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id6_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id6;
                }
                if (!String.IsNullOrEmpty(lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id7))
                {
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_id7 = busGlobalFunctions.GetOrgIdFromOrgCode(lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id7);
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount7_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount7;
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id7_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id7;
                }
                //PIR 25920 New Plan DC 2025
                lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.er_pretax_match_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.er_pretax_match_reported;
                lobjEmployerPayrollDetail.UpdateAmtFromEnrolmentForDefComp();
                if (_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.report_type_value == busConstant.PayrollHeaderReportTypeRegular && lobjEmployerPayrollDetail.istrPlanValue == busConstant.PlanCodeOther457 && !lobjEmployerPayrollDetail.IsPledgeAmtNotAsPerFrequency())
                    lobjEmployerPayrollDetail.UpdateAmtAndProviderFromFile();
                lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.suppress_warnings_flag = "N";
                ldtPreviousRecordPayCheckDate = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_check_date;
            }
            if (lblnIsPayCheckDateSame && ldtPreviousRecordPayCheckDate != DateTime.MinValue)
            {
                ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_check_date = ldtPreviousRecordPayCheckDate;
                ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.Update();
            }
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

            if ((lstrObjectField == "pay_period_start_date") ||
                (lstrObjectField == "pay_period_end_date") ||
                (lstrObjectField == "pay_check_date") ||
                (lstrObjectField == "member_status_effective_date"))
            {

                if ((astrFieldValue.Length == 8) && (astrFieldValue.IndexOf("/") < 0))
                {
                    lstrReturnValue = astrFieldValue.Substring(0, 2) + "/" + astrFieldValue.Substring(2, 2) + "/" + astrFieldValue.Substring(4, 4);
                }
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
            //if total wages reported is Negative Amount
            if (lstrObjectField == "total_wages_reported")
            {
                if ((!(String.IsNullOrEmpty(astrFieldValue))))
                {
                    if (astrFieldValue.IndexOf("-") > -1)
                    {
                        lstrReturnValue = astrFieldValue.Substring(astrFieldValue.IndexOf("-"));
                    }
                }
            }
            //if total contribution reported is Negative Amount
            if (lstrObjectField == "total_contribution_reported")
            {
                if ((!(String.IsNullOrEmpty(astrFieldValue))))
                {
                    if (astrFieldValue.IndexOf("-") > -1)
                    {
                        lstrReturnValue = astrFieldValue.Substring(astrFieldValue.IndexOf("-"));
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
                case busConstant.EmployerReportPayPeriodStartDate:                                        
                case busConstant.EmployerReportPayPeriodEndDate:     
                case busConstant.EmployerReportPayCheckDate: 
                case busConstant.EmployerReportPayPeriodEndMonthForBonus:
                    astrValue = String.Empty;
                    return sfwOnFileError.ContinueWithRecord;

                default: return base.ContinueOnValueError(astrObjectField, out astrValue);
            }
        }

        public override void AfterReject(int aintErrorRecNo, string astrErrorMessage)
        {
            if (icdoFileHdr.reference_id > 0) // PIR - 7202
            {
                string lstrPrioityValue = string.Empty;
                busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(6, iobjPassInfo, ref lstrPrioityValue),
                            icdoFileHdr.mailbox_file_name), lstrPrioityValue, aintOrgID: icdoFileHdr.reference_id);
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
                busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(7, iobjPassInfo, ref lstrPrioityValue),
                           icdoFileHdr.mailbox_file_name), lstrPrioityValue, aintOrgID: icdoFileHdr.reference_id);
            }
            base.FinalizeFile();
        }
    }
}
