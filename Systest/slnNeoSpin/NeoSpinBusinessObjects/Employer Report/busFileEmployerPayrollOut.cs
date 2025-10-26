#region Using directives

using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CustomDataObjects;
using System.Linq;
#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busFileEmployerPayrollOut : busFileBaseOut
    {
        private busEmployerPayrollHeader _ibusEmployerPayrollHeader;
        public busEmployerPayrollHeader ibusEmployerPayrollHeader
        {
            get { return _ibusEmployerPayrollHeader; }
            set { _ibusEmployerPayrollHeader = value; }
        }
        
        public void LoadEmployerPayroll(DataTable ldtbEmployerPayroll)
        {
            if (iarrParameters[0] is int)
            {
                int lintPayrollHeaderID = Convert.ToInt32(iarrParameters[0]);
                if (_ibusEmployerPayrollHeader == null)
                    _ibusEmployerPayrollHeader = new busEmployerPayrollHeader();
                _ibusEmployerPayrollHeader.FindEmployerPayrollHeader(lintPayrollHeaderID);
                _ibusEmployerPayrollHeader.LoadEmployerPayrollDetail();
                _ibusEmployerPayrollHeader.iintCountOfDetails = _ibusEmployerPayrollHeader.iclbEmployerPayrollDetail.Count;
                _ibusEmployerPayrollHeader.istrOrgCodeId = busGlobalFunctions.GetOrgCodeFromOrgId(_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.org_id);
                _ibusEmployerPayrollHeader.LoadFileRecordType();
                foreach (busEmployerPayrollDetail lobjPayrollDetail in _ibusEmployerPayrollHeader.iclbEmployerPayrollDetail)
                {
                    busPlan lobjPlan = new busPlan();
                    lobjPlan.FindPlan(lobjPayrollDetail.icdoEmployerPayrollDetail.plan_id);
                    lobjPayrollDetail.istrPlanValue = lobjPlan.icdoPlan.plan_code;
                    lobjPayrollDetail.LoadPerson();
                    lobjPayrollDetail.LoadFileRecordType();
                    lobjPayrollDetail.istrOrgCodeId = _ibusEmployerPayrollHeader.istrOrgCodeId;
                }
            }
            else
            {
                _ibusEmployerPayrollHeader = (busEmployerPayrollHeader)iarrParameters[0];
                _ibusEmployerPayrollHeader.iintCountOfDetails = _ibusEmployerPayrollHeader.iclbEmployerPayrollDetail.Count;
                _ibusEmployerPayrollHeader.istrOrgCodeId = busGlobalFunctions.GetOrgCodeFromOrgId(_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.org_id);
                _ibusEmployerPayrollHeader.LoadFileRecordType();
                if (_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.report_type_value != busConstant.PayrollDetailRecordTypeRegular
                    &&  _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value== busConstant.PayrollHeaderBenefitTypeRtmt
                    )
                {
                    _ibusEmployerPayrollHeader.iclbEmployerPayrollDetail = 
                        _ibusEmployerPayrollHeader.iclbEmployerPayrollDetail.Where(o => o.icdoEmployerPayrollDetail.eligible_wages!=0M).ToList().ToCollection();
                  
                                             
                }
                if (_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.report_type_value != busConstant.PayrollDetailRecordTypeRegular
                            && _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                {
                    _ibusEmployerPayrollHeader.iclbEmployerPayrollDetail =
                       _ibusEmployerPayrollHeader.iclbEmployerPayrollDetail.Where(obj => obj.icdoEmployerPayrollDetail.contribution_amount1
                                                                                     + obj.icdoEmployerPayrollDetail.contribution_amount2
                                                                                     + obj.icdoEmployerPayrollDetail.contribution_amount3
                                                                                     + obj.icdoEmployerPayrollDetail.contribution_amount4
                                                                                     + obj.icdoEmployerPayrollDetail.contribution_amount5
                                                                                     + obj.icdoEmployerPayrollDetail.contribution_amount6
                                                                                     + obj.icdoEmployerPayrollDetail.contribution_amount7 != 0M).ToList().ToCollection();
                }
                foreach (busEmployerPayrollDetail lobjPayrollDetail in _ibusEmployerPayrollHeader.iclbEmployerPayrollDetail)
                {
                    if (_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.report_type_value != busConstant.PayrollDetailRecordTypeRegular
                              && _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                    {
                        //PIR 13940 & 13942 - Reporting Month to populate file should come from each detail
                        DateTime ldtPayPeriodDate;
                        if (DateTime.TryParse(lobjPayrollDetail.icdoEmployerPayrollDetail.pay_period, out ldtPayPeriodDate))
                        {
                            lobjPayrollDetail.icdoEmployerPayrollDetail.pay_period_date = ldtPayPeriodDate;
                        }
                        if (_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.report_type_description == "Bonus/Retro Pay")
                        {
                            lobjPayrollDetail.icdoEmployerPayrollDetail.record_type_value = busConstant.PayrollDetailRecordTypeBonus;
                        }
                        else if (lobjPayrollDetail.icdoEmployerPayrollDetail.eligible_wages > 0M)
                        {
                            lobjPayrollDetail.icdoEmployerPayrollDetail.record_type_value = busConstant.PayrollDetailRecordTypePositiveAdjustment;
                        }
                        else
                        {
                            lobjPayrollDetail.icdoEmployerPayrollDetail.record_type_value = busConstant.PayrollDetailRecordTypeNegativeAdjustment;
                            lobjPayrollDetail.icdoEmployerPayrollDetail.eligible_wages = -1 * lobjPayrollDetail.icdoEmployerPayrollDetail.eligible_wages;
                        }
                    }
                    if (_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.report_type_value != busConstant.PayrollDetailRecordTypeRegular
                             && _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                    {
                        if (lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount1
                                                                                     + lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount2
                                                                                     + lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount3
                                                                                     + lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount4
                                                                                     + lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount5
                                                                                     + lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount6
                                                                                     + lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount7 > 0M)
                        {
                            lobjPayrollDetail.icdoEmployerPayrollDetail.record_type_value = busConstant.PayrollDetailRecordTypePositiveAdjustment;
                        }
                        else
                        {
                            lobjPayrollDetail.icdoEmployerPayrollDetail.record_type_value = busConstant.PayrollDetailRecordTypeNegativeAdjustment;
                            lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount1 = lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount1 > 0 ? lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount1 : -1 * lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount1;
                            lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount2 = lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount2 > 0 ? lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount2 : -1 * lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount2;
                            lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount3 = lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount3 > 0 ? lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount3 : -1 * lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount3;
                            lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount4 = lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount4 > 0 ? lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount4 : -1 * lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount4;
                            lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount5 = lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount5 > 0 ? lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount5 : -1 * lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount5;
                            lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount6 = lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount6 > 0 ? lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount6 : -1 * lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount6;
                            lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount7 = lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount7 > 0 ? lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount7 : -1 * lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount7;

                        }
                    }
                    lobjPayrollDetail.iblnIsFromESS = true;
                    lobjPayrollDetail.iblnIsFromFile = true;  //PIR 14519: Save issue.
                    lobjPayrollDetail.LoadPerson();
                    lobjPayrollDetail.LoadPersonAccount();
                    lobjPayrollDetail.LoadFileRecordType();
                    lobjPayrollDetail.ibusPersonAccount.LoadPersonAccountRetirement();
                    lobjPayrollDetail.ibusPersonAccount.ibusPersonAccountRetirement.ibusHistoryAsofDate = null;
                    lobjPayrollDetail.UpdateCalculatedAmountForRetirement(string.Empty);
                    //Commented - Maik mail dated November 26, 2014
                    //_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.total_contribution_reported += lobjPayrollDetail.icdoEmployerPayrollDetail.ee_pre_tax_reported +
                    //                                                                                    lobjPayrollDetail.icdoEmployerPayrollDetail.ee_contribution_reported +
                    //                                                                                    lobjPayrollDetail.icdoEmployerPayrollDetail.ee_employer_pickup_reported +
                    //                                                                                    lobjPayrollDetail.icdoEmployerPayrollDetail.rhic_ee_contribution_reported +
                    //                                                                                    lobjPayrollDetail.icdoEmployerPayrollDetail.er_contribution_reported +
                    //                                                                                    lobjPayrollDetail.icdoEmployerPayrollDetail.rhic_er_contribution_reported;
                    lobjPayrollDetail.istrOrgCodeId = _ibusEmployerPayrollHeader.istrOrgCodeId;
                    if (_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.report_type_value != busConstant.PayrollDetailRecordTypeRegular
                             && _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                    {
                        if (!string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.pay_end_month) && !string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.pay_period))
                        {
                            DateTime ldtEndMonthForBonus;
                            if (DateTime.TryParse(lobjPayrollDetail.icdoEmployerPayrollDetail.pay_end_month, out ldtEndMonthForBonus))
                            {
                                lobjPayrollDetail.icdoEmployerPayrollDetail.pay_period_end_month_for_bonus = ldtEndMonthForBonus;
                            }
                            DateTime ldtReportingMonth;
                            if (DateTime.TryParse(lobjPayrollDetail.icdoEmployerPayrollDetail.pay_period, out ldtReportingMonth))
                            {
                                lobjPayrollDetail.icdoEmployerPayrollDetail.pay_period_date = ldtReportingMonth;
                            }
                        }
                    }
                }
            }
        }

        public override bool ValidateFile()
        {
            bool lblnFlag = true;
            if (_ibusEmployerPayrollHeader.iclbEmployerPayrollDetail.Count < 0)
            {
                this.istrError = "No Employer Payroll Details Exists.";
                lblnFlag = false;
            }
            return lblnFlag;
        }
        public override void FinalizeFile()
        {
            iarrParameters[0] = base.istrFileName;
            base.FinalizeFile();
        }
       
    }
}
