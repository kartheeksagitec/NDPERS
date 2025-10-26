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
using System.Text.RegularExpressions;
using NeoSpin.BusinessObjects;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

#endregion

namespace NeoSpin.BusinessObjects
{
    public class busPostingVestedERContributionBatch : busExtendBase
    {
        private string _istrAdjustmentType;
        public string istrAdjustmentType
        {
            get { return _istrAdjustmentType; }
            set { _istrAdjustmentType = value; }
        }
        /// <summary>
        /// Calculate ER Vested Amount to Post. Based on VSC calculate the amount and post it for that account id.
        /// </summary>
        /// <param name="ldecContAmt">Contribution Amount</param>
        /// <param name="ldecSalaryAmt">Salary Amount</param>
        /// <param name="lintAcctID">Person Account ID</param>
        /// <param name="ldecVSC">Vested Service Credit</param>
        /// <param name="ldtPayPeriodDate"></param>
        /// <param name="lintPlanID"></param>
        /// <param name="ldecERVestedAmt"></param>
        /// <param name="lintRefID"></param>
        /// <param name="aintEmpDtlId"></param>
        /// <returns></returns>
        public decimal CalculateERVestedAmount(decimal ldecContAmt, decimal adecTotSalaryForPayPeriod, int lintAcctID,
            decimal ldecVSC, DateTime ldtPayPeriodDate, int lintPlanID, decimal ldecERVestedAmtAlreadyPosted, int lintRefID, int aintEmpDtlId,
            DateTime adtBatchDate, string astrSubSystemValue) //PIR-18292
        {
            decimal ldecERVestedYetToBePosted = 0;
            decimal ldecPostedAmt = 0;
            decimal ldecERVestedSchedulePercentage = 0.0M;
            int lintVestedContributionPercentageRefID = 0;
            string lstrRetirementType = busConstant.PlanRetirementTypeValueDB;
            if (lintPlanID == busConstant.PlanIdDC || lintPlanID == busConstant.PlanIdDC2020 || lintPlanID == busConstant.PlanIdDC2025) //PIR 20232
            {
                lstrRetirementType = busConstant.PlanRetirementTypeValueDC;
            }
            // PIR:1683 && 1798
            ldecERVestedSchedulePercentage = busPersonBase.GetVestedERSchedulePercentage(ldtPayPeriodDate, lstrRetirementType, ldecVSC, ref lintVestedContributionPercentageRefID); //PIR-18292
            decimal ldecERVestedCalculatedFromSalary = adecTotSalaryForPayPeriod * ldecERVestedSchedulePercentage; //total til now
            decimal ldecMinERVestedAmtToBePosted = 25;
            decimal ldecERVestedAmtFinal = ldecContAmt < 25 ? 0 :
                                            (ldecERVestedCalculatedFromSalary > 0 && ldecERVestedCalculatedFromSalary <= ldecMinERVestedAmtToBePosted) ? ldecMinERVestedAmtToBePosted :
                                            (ldecERVestedCalculatedFromSalary > 0 && ldecERVestedCalculatedFromSalary >= ldecContAmt) ? ldecContAmt :
                                            (ldecERVestedCalculatedFromSalary > 0 && ldecERVestedCalculatedFromSalary < ldecContAmt) ? ldecERVestedCalculatedFromSalary : 0;
            ldecERVestedYetToBePosted = (adecTotSalaryForPayPeriod <= 0) ? (-1 * ldecERVestedAmtAlreadyPosted) : (ldecERVestedAmtFinal - ldecERVestedAmtAlreadyPosted);
            //PIR-17777
            if (ldecERVestedYetToBePosted != 0)
                ldecPostedAmt = PostERVestedAmount(lintAcctID, ldecERVestedYetToBePosted, ldtPayPeriodDate, lintPlanID, lintRefID,
                    aintEmpDtlId, adtBatchDate, lintVestedContributionPercentageRefID, astrSubSystemValue);
            return ldecPostedAmt;
        }

        /// <summary>
        /// Post ER Vested Amount for person account id. Also deduct same amount from pre_tax_er_amount for that account id.
        /// </summary>
        /// <param name="lintAcctID">Person Account ID</param>
        /// <param name="ldecWagesPercentAmt">ER Vested Amount</param>
        /// <returns></returns>
        public decimal PostERVestedAmount(int lintAcctID, decimal ldecWagesPercentAmt, DateTime ldtPayPeriodDate,
            int lintPlanID, int lintRefID, int aintEmpDtlId, DateTime adtBatchDate, int lintVestedContributionPercentageRefID, string astrSubSystemValue)
        {
            busPersonAccountRetirementContribution lobjRetirementContribution = new busPersonAccountRetirementContribution();
            lobjRetirementContribution.icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution();
            lobjRetirementContribution.icdoPersonAccountRetirementContribution.person_account_id = lintAcctID;

            //if (istrAdjustmentType == busConstant.PayrollDetailRecordTypeNegativeAdjustment)
            //{
            //    ldecWagesPercentAmt = ldecWagesPercentAmt * -1;
            //}

            lobjRetirementContribution.icdoPersonAccountRetirementContribution.er_vested_amount = ldecWagesPercentAmt;
            lobjRetirementContribution.icdoPersonAccountRetirementContribution.pre_tax_er_amount = ldecWagesPercentAmt * -1;

            lobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_value = astrSubSystemValue;
            lobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_id = busConstant.SubSystemPEPId;
            lobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_ref_id = lintRefID;
            lobjRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value = busConstant.TransactionTypeVestedER;
            lobjRetirementContribution.icdoPersonAccountRetirementContribution.person_employment_dtl_id = aintEmpDtlId;
            lobjRetirementContribution.icdoPersonAccountRetirementContribution.vested_er_percentage_ref_id = lintVestedContributionPercentageRefID;

            if (lintPlanID == 8 || lintPlanID == 19)
            {
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.effective_date = ldtPayPeriodDate;
            }
            else
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.effective_date = ldtPayPeriodDate.AddMonths(1).AddDays(-1);

            lobjRetirementContribution.icdoPersonAccountRetirementContribution.transaction_date = adtBatchDate;
            lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month = ldtPayPeriodDate.Month;
            lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year = ldtPayPeriodDate.Year;
            lobjRetirementContribution.icdoPersonAccountRetirementContribution.Insert();

            return ldecWagesPercentAmt;
        }

        /// <summary>
        /// Calculate PEP and adjustment for given person id. 
        /// </summary>
        /// <param name="ldecEligibleWages">Eligible Wages or Salary Amount based on plan id</param>
        /// <param name="lintPlanID">Plan id, either retirement or defferred compensation</param>
        /// <param name="lintPersonID">Person Id</param>
        /// <param name="ldtPayPeriodDate">Either pay period date or end of pay period date</param>
        /// <param name="lintRefID"></param>
        /// <param name="lstrAdjustmentType"></param>
        /// <param name="aintEmpDtlId"></param>
        public void CalculatePEPAdjustment(int lintPlanID,
            int lintPersonID, DateTime ldtPayPeriodDate, int lintRefID, string lstrAdjustmentType,
            int aintEmpDtlId, DateTime adtBatchDate, int aintPersonAccountID, string astrSubSystemValue)
        {
            decimal ldecContributionAmt = 0;
            decimal ldecPostedAmt = 0;
            decimal ldecVSC = 0;
            decimal ldecERVestedAmtAlreadyPostedForPayPeriodDate = 0;
            decimal ldecTotalSalaryForPayPeriodDate = 0;
            decimal ldecTotContAmtForPayPeriodDate = 0;
            decimal ldecInterestAmount = 0.00M;

            ////PIR-18292
            //      decimal ldecEligibleWagesPrv = 0.00M;
            //      decimal ldecEligibleWagesCurrent = 0;

            istrAdjustmentType = lstrAdjustmentType;

            busPostingInterestBatch lobjInterestBatch = new busPostingInterestBatch();
            ldecVSC = CalculateVSC(lintPersonID, ldtPayPeriodDate);

            if ((lintPlanID == 1) || (lintPlanID == 2) || (lintPlanID == 3) || (lintPlanID == 20)
                 || (lintPlanID == busConstant.PlanIdBCILawEnf)//pir 7943
                || (lintPlanID == busConstant.PlanIdMain2020) //PIR 20232
                || (lintPlanID == busConstant.PlanIdStatePublicSafety)) // PIR 25729
            {
                if (aintPersonAccountID > 0)
                {
                    ldecInterestAmount = 0.00M;
                    DataTable ldtbContAmt =
                        busNeoSpinBase.Select("cdoPersonAccountDeferredCompContribution.GetContributionAmtForDefComp",
                                              new object[2] { lintPersonID, ldtPayPeriodDate });
                    if (ldtbContAmt.Rows.Count > 0)
                        for (int i = 0; i < ldtbContAmt.Rows.Count; i++)
                        {
                            ldecContributionAmt += Convert.ToDecimal(ldtbContAmt.Rows[i]["Contribution_Amount"].ToString());
                        }
                    DataTable ldtbEligibleWages =
                        busNeoSpinBase.Select(
                            "cdoPersonAccountRetirementContribution.GetAcctIDAndSalaryForRetirementCont",
                            new object[2] { ldtPayPeriodDate, lintPersonID });
                    if (ldtbEligibleWages.Rows.Count > 0)
                    {
                        foreach (DataRow dr in ldtbEligibleWages.Rows)
                        {
                            if (Convert.ToInt32(dr["person_account_id"].ToString()) == aintPersonAccountID)
                            {
                                ldecERVestedAmtAlreadyPostedForPayPeriodDate = Convert.ToDecimal(dr["Vested_Amount"].ToString());
                                ldecTotalSalaryForPayPeriodDate += Convert.ToDecimal(dr["salary_amount"].ToString());
                            }
                        }
                    }

                    ldecPostedAmt = CalculateERVestedAmount(ldecContributionAmt, ldecTotalSalaryForPayPeriodDate, aintPersonAccountID, ldecVSC,
                        ldtPayPeriodDate, lintPlanID, ldecERVestedAmtAlreadyPostedForPayPeriodDate, lintRefID, aintEmpDtlId, adtBatchDate, astrSubSystemValue); //PIR-18292
                    if (ldecPostedAmt != 0)
                    {
                        ldecInterestAmount = lobjInterestBatch.CalculateInterestAdjustment(lintPlanID, lintPersonID,
                            ldtPayPeriodDate, ldecPostedAmt, lintRefID, adtBatchDate, aintPersonAccountID, astrSubSystemValue);
                        if (astrSubSystemValue == busConstant.SubSystemValueServicePurchase)
                            UpdateBenefitAccountAndPayeeAccountInfo(lintPersonID, aintPersonAccountID, ldecPostedAmt, ldecInterestAmount);
                    }
                }
            }
            else if ((lintPlanID == 8) || (lintPlanID == 19))
            {
                DataTable ldtbContAmt = busNeoSpinBase.Select("cdoPersonAccountDeferredCompContribution.GetContributionAmtForDefComp", new object[2] { lintPersonID, ldtPayPeriodDate });
                if (ldtbContAmt.Rows.Count > 0)
                {
                    for (int i = 0; i < ldtbContAmt.Rows.Count; i++)
                    {
                        ldecTotContAmtForPayPeriodDate += Convert.ToDecimal(ldtbContAmt.Rows[i]["Contribution_Amount"].ToString());
                    }
                }
                DataTable ldtbEligibleWages = busNeoSpinBase.Select("cdoPersonAccountRetirementContribution.GetAcctIDAndSalaryForRetirementCont", new object[2] { ldtPayPeriodDate, lintPersonID });
                if (ldtbEligibleWages.Rows.Count > 0)
                {
                    ldecInterestAmount = 0.00M;
                    decimal ldecPostedSalaryAmt = 0.0M;
                    decimal ldecPostedERAmtPerAccount = 0.00M;
                    foreach (DataRow dr in ldtbEligibleWages.Rows)
                    {
                        ldecTotalSalaryForPayPeriodDate = Convert.ToDecimal(dr["salary_amount"].ToString()) + ldecPostedSalaryAmt;
                        ldecERVestedAmtAlreadyPostedForPayPeriodDate = Convert.ToDecimal(dr["Vested_Amount"].ToString()) + ldecPostedERAmtPerAccount;
                        ldecPostedAmt = CalculateERVestedAmount(ldecTotContAmtForPayPeriodDate, ldecTotalSalaryForPayPeriodDate,
                            Convert.ToInt32(dr["person_account_id"].ToString()), ldecVSC, ldtPayPeriodDate, lintPlanID, ldecERVestedAmtAlreadyPostedForPayPeriodDate,
                            lintRefID, aintEmpDtlId, adtBatchDate, astrSubSystemValue); //PIR-18292
                        if (ldecPostedAmt != 0)
                        {
                            ldecInterestAmount = lobjInterestBatch.CalculateInterestAdjustment(Convert.ToInt32(dr["Plan_Id"].ToString()), lintPersonID,
                                ldtPayPeriodDate, ldecPostedAmt, lintRefID, adtBatchDate, Convert.ToInt32(dr["person_account_id"].ToString()), astrSubSystemValue);
                            if (ldecTotContAmtForPayPeriodDate > 0 || lstrAdjustmentType == busConstant.RecordTypePositiveAdjustment)
                            {
                                ldecPostedERAmtPerAccount += ldecPostedAmt;
                                ldecPostedSalaryAmt += ldecTotalSalaryForPayPeriodDate;
                            }

                        }
                    }
                }
            }
        }
        /// <summary>
        /// Calculate Vested Service Credit for a person id
        /// </summary>
        /// <param name="lintPersonID">Person ID</param>
        /// <returns></returns>
        public decimal CalculateVSC(int lintPersonID, DateTime adtePayPeriodDate)
        {
            decimal ldecVSC = 0;

            DataTable ldtPSC = busNeoSpinBase.Select("cdoPersonAccountRetirementContribution.CalculateVSC", new object[2] { lintPersonID, adtePayPeriodDate.GetLastDayofMonth() });
            if (ldtPSC.Rows.Count > 0)
                ldecVSC = Convert.ToDecimal(ldtPSC.Rows[0]["Vested_service_credit"].ToString());

            return ldecVSC;
        }

        //prod pir 6586 : benefit account info
        //--start--//
        public DataTable idtBenefitAccountInfo { get; set; }

        public busDBCacheData ibusDBCacheData { get; set; }

        private void UpdateBenefitAccountAndPayeeAccountInfo(int aintPersonID, int aintPersonAccountID, decimal adecVestedERAmount, decimal adecInterestAmount)
        {
            busBenefitAccount lobjBenefitAccount = null;
            busPersonAccount lobjPersonAccount = new busPersonAccount();
            bool lblnPayeeAccountFound = false;

            lobjPersonAccount.FindPersonAccount(aintPersonAccountID);
            lobjPersonAccount.LoadPerson();
            lobjPersonAccount.ibusPerson.LoadBenefitApplication();
            
            foreach (busBenefitApplication lobjBA in lobjPersonAccount.ibusPerson.iclbBenefitApplication)
            {
                lblnPayeeAccountFound = false;
                lobjBA.LoadBenefitApplicationPersonAccount();
                if (lobjBA.iclbBenefitApplicationPersonAccounts.Where(o => o.icdoBenefitApplicationPersonAccount.person_account_id == aintPersonAccountID &&
                    o.icdoBenefitApplicationPersonAccount.is_application_person_account_flag == busConstant.Flag_Yes).Any())
                {
                    lobjBA.LoadPayeeAccount();
                    foreach (busPayeeAccount lobjPayeeAccount in lobjBA.iclbPayeeAccount)
                    {
                        lobjPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                        lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 =
                                    busGlobalFunctions.GetData2ByCodeValue(2203, lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo);
                        if (lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 != busConstant.PayeeAccountStatusCancelled &&
                            lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 != busConstant.PayeeAccountStatusPaymentComplete)
                        {
                            lobjBenefitAccount = new busBenefitAccount();
                            lobjBenefitAccount.FindBenefitAccount(lobjPayeeAccount.icdoPayeeAccount.benefit_account_id);
                            lblnPayeeAccountFound = true;
                            break;
                        }
                    }
                    if (lblnPayeeAccountFound)
                        break;
                }
            }            

            if (lobjBenefitAccount != null)
            {
                lobjBenefitAccount.icdoBenefitAccount.starting_taxable_amount = lobjBenefitAccount.icdoBenefitAccount.starting_taxable_amount
                                                                                + adecVestedERAmount + adecInterestAmount;
                if (lobjPersonAccount.icdoPersonAccount.person_account_id > 0)
                {                    
                    if (lobjPersonAccount.ibusPerson.icdoPerson.person_id > 0)
                    {
                        if (lobjPersonAccount.ibusPerson.iclbPayeeAccount == null)
                            lobjPersonAccount.ibusPerson.LoadPayeeAccount();

                        busPayeeAccount lbusLatestPayeeAccount = lobjPersonAccount.ibusPerson.iclbPayeeAccount.Where(o =>
                                                    o.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund &&
                                                    o.icdoPayeeAccount.benefit_account_id == lobjBenefitAccount.icdoBenefitAccount.benefit_account_id &&
                                                    (o.icdoPayeeAccount.rhic_ee_amount_refund_flag == busConstant.Flag_No ||
                                                    o.icdoPayeeAccount.rhic_ee_amount_refund_flag == null))
                                                    .OrderByDescending(o => o.icdoPayeeAccount.payee_account_id).FirstOrDefault();
                        if (lbusLatestPayeeAccount != null && lbusLatestPayeeAccount.icdoPayeeAccount.payee_account_id > 0)
                        {
                            lbusLatestPayeeAccount.idtbPaymentItemType = ibusDBCacheData.idtbCachedPaymentItemType;
                            if (lbusLatestPayeeAccount.ibusApplication == null)
                                lbusLatestPayeeAccount.LoadApplication();

                            if (lbusLatestPayeeAccount.ibusPayeeAccountActiveStatus == null)
                                lbusLatestPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                            busBenefitRefundApplication lbusBenefitRefundApplication = new busBenefitRefundApplication();
                            lbusBenefitRefundApplication.icdoBenefitApplication = lbusLatestPayeeAccount.ibusApplication.icdoBenefitApplication;
                            if (lbusBenefitRefundApplication.iclbBenefitRefundCalculation == null)
                                lbusBenefitRefundApplication.LoadRefundBenefitCalculation();
                            
                            if (lbusLatestPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusRefundProcessed)
                            {
                                if (!lbusBenefitRefundApplication.IsApplicationCancelledOrDenied())
                                {
                                    lbusBenefitRefundApplication.CreateAdjustmentRefund(null, DateTime.Now, false, adecAdditionalInterestAmount: adecInterestAmount,
                                    adecERVestedAmount: adecVestedERAmount);
                                }
                            }
                            else if (lbusLatestPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusRefundApprovedOrRefundReview())
                            {
                                decimal ldecERVestedAmount = lbusLatestPayeeAccount.LoadLatestPAPITAmount(busConstant.RefundPaymentItemVestedERContributionAmount);
                                lbusLatestPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemVestedERContributionAmount,
                                                               (adecVestedERAmount + ldecERVestedAmount), string.Empty, 0,
                                                               busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(1), DateTime.MinValue, false);

                                decimal ldecAdditionalEEInterestAmount = lbusLatestPayeeAccount.LoadLatestPAPITAmount(busConstant.RefundPaymentItemAdditionalEEInterestAmount);
                                lbusLatestPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemAdditionalEEInterestAmount,
                                                               (adecInterestAmount + ldecAdditionalEEInterestAmount), string.Empty, 0, busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(1), DateTime.MinValue);
                                
                                if (lbusLatestPayeeAccount.iclbActiveRolloverDetails == null)
                                    lbusLatestPayeeAccount.LoadActiveRolloverDetail();

                                if (lbusLatestPayeeAccount.iclbActiveRolloverDetails.Count > 0)
                                {
                                    lbusLatestPayeeAccount.CreateRolloverAdjustment();
                                }
                                else
                                {
                                    lbusLatestPayeeAccount.CalculateAdjustmentTax(false);
                                }
                            }
                        }
                    }
                }
                lobjBenefitAccount.icdoBenefitAccount.Update();

                if (lobjBenefitAccount.icdoBenefitAccount.benefit_account_id > 0)
                {
                    lobjBenefitAccount.LoadPayeeAccounts();
                    IEnumerable<busPayeeAccount> lenmPA = lobjBenefitAccount.iclbPayeeAccount
                        .Where(o => (o.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath ||
                            o.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement) &&
                            o.icdoPayeeAccount.benefit_option_value != busConstant.BenefitOption10YearCertain &&
                            o.icdoPayeeAccount.benefit_option_value != busConstant.BenefitOption15YearCertain &&
                            o.icdoPayeeAccount.benefit_option_value != busConstant.BenefitOption20YearCertain &&
                            o.icdoPayeeAccount.benefit_option_value != busConstant.BenefitOption5YearTermLife &&
                            o.icdoPayeeAccount.application_id > 0);
                    foreach (busPayeeAccount lobjPA in lenmPA)
                    {
                        lobjPA.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                        lobjPA.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 =
                            busGlobalFunctions.GetData2ByCodeValue(2203, lobjPA.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo);
                        if (lobjPA.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 != busConstant.PayeeAccountStatusCancelled &&
                            lobjPA.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 != busConstant.PayeeAccountStatusPaymentComplete)
                        {
                            decimal ldecRegularERVestedAmount = (adecVestedERAmount + adecInterestAmount);
                            lobjPA.icdoPayeeAccount.minimum_guarantee_amount += ldecRegularERVestedAmount;
                            lobjPA.icdoPayeeAccount.Update();
                        }
                    }
                }
            }
        }
        //--end--//

    }
}
