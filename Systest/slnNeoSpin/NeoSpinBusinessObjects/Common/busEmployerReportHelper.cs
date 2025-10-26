using System;
using System.Collections.ObjectModel;
using System.Data;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using System.Reflection;
using Sagitec.CustomDataObjects;
using NeoSpin.DataObjects;
using System.Linq;
using System.Linq.Expressions;

namespace NeoSpin.BusinessObjects
{
    public static class busEmployerReportHelper
    {
        /// <summary>
        /// Calcualte the Remittance Amount by given Remittance ID
        /// Check the Status of Remittance and calculate only if the status is applied else Return 0
        /// Formula : Remittance Amount - Sum of All allocated amount (Status : Allocated) from 
        /// Employer Report Remittance Allocation
        /// </summary>
        /// <param name="aintRemittanceID"></param>
        /// <returns></returns>
        public static decimal GetRemittanceAvailableAmount(int aintRemittanceID)
        {
            decimal ldecAvailableAmount = 0.00M;
            decimal ldecRemittanceAmount = 0.00M;
            decimal ldecRefundRemittanceAmount = 0.0M, ldecComputed = 0.0M, ldecOverridden = 0.0M, ldecAllocatedNegativeDeposit = 0.00M;
            DataTable ldtbRemittance = busNeoSpinBase.Select("cdoRemittance.GetAppliedRemittanceByRemittanceID",
                                                                 new object[1] { aintRemittanceID });
            if (ldtbRemittance.Rows.Count > 0)
            {
                if (ldtbRemittance.Rows[0]["remittance_amount"] != DBNull.Value)
                    ldecRemittanceAmount = (decimal)ldtbRemittance.Rows[0]["remittance_amount"];

                if (ldtbRemittance.Rows[0]["allocated_negative_deposit_amount"] != DBNull.Value)
                    ldecAllocatedNegativeDeposit = (decimal)ldtbRemittance.Rows[0]["allocated_negative_deposit_amount"];

                decimal ldecAllocatedAmount = GetRemittanceAllocatedAmount(aintRemittanceID);
                ldecAvailableAmount = ldecRemittanceAmount - ldecAllocatedAmount;

                //Getting Remittance Refund amount if any
                if (ldtbRemittance.Rows[0]["refund_status_value"] != DBNull.Value &&
                    !String.IsNullOrEmpty(ldtbRemittance.Rows[0]["refund_status_value"].ToString()))
                {
                    if (ldtbRemittance.Rows[0]["computed_refund_amount"] != DBNull.Value)
                        ldecComputed = (decimal)ldtbRemittance.Rows[0]["computed_refund_amount"];
                    if (ldtbRemittance.Rows[0]["overridden_refund_amount"] != DBNull.Value)
                        ldecOverridden = (decimal)ldtbRemittance.Rows[0]["overridden_refund_amount"];
                    ldecRefundRemittanceAmount = ldecOverridden > 0.0M ? ldecOverridden : ldecComputed;
                }

                //Available amount will be net amount after Refund amount is reduced
                ldecAvailableAmount = ldecAvailableAmount - ldecRefundRemittanceAmount - ldecAllocatedNegativeDeposit;
            }
            return ldecAvailableAmount;
        }

		//PIR 15673
        public static decimal GetRemittanceAvailableAmountForIBS(int aintRemittanceID)
        {
            decimal ldecAvailableAmount = 0.00M;
            decimal ldecRemittanceAmount = 0.00M;
            decimal ldecRefundRemittanceAmount = 0.0M, ldecComputed = 0.0M, ldecOverridden = 0.0M, ldecAllocatedNegativeDeposit = 0.00M;
            DataTable ldtbRemittance = busNeoSpinBase.Select("cdoRemittance.GetAppliedRemittanceByRemittanceID",
                                                                 new object[1] { aintRemittanceID });
            if (ldtbRemittance.Rows.Count > 0)
            {
                if (ldtbRemittance.Rows[0]["remittance_amount"] != DBNull.Value)
                    ldecRemittanceAmount = (decimal)ldtbRemittance.Rows[0]["remittance_amount"];

                if (ldtbRemittance.Rows[0]["allocated_negative_deposit_amount"] != DBNull.Value)
                    ldecAllocatedNegativeDeposit = (decimal)ldtbRemittance.Rows[0]["allocated_negative_deposit_amount"];

                decimal ldecAllocatedAmount = 0;

                DataTable ldtIBSAllocationList =
                    busBase.Select<cdoIbsRemittanceAllocation>(new string[1] { "remittance_id" }, new object[1] { aintRemittanceID }, null, null);
                if (ldtIBSAllocationList.Rows.Count > 0)
                {
                    foreach (DataRow dr in ldtIBSAllocationList.Rows)
                    {
                        if (!(String.IsNullOrEmpty(dr["ALLOCATED_AMOUNT"].ToString())))
                            ldecAllocatedAmount = ldecAllocatedAmount + Convert.ToDecimal(dr["ALLOCATED_AMOUNT"]);
                    }
                }

                ldecAvailableAmount = ldecRemittanceAmount - ldecAllocatedAmount;

                //Getting Remittance Refund amount if any
                if (ldtbRemittance.Rows[0]["refund_status_value"] != DBNull.Value &&
                    !String.IsNullOrEmpty(ldtbRemittance.Rows[0]["refund_status_value"].ToString()))
                {
                    if (ldtbRemittance.Rows[0]["computed_refund_amount"] != DBNull.Value)
                        ldecComputed = (decimal)ldtbRemittance.Rows[0]["computed_refund_amount"];
                    if (ldtbRemittance.Rows[0]["overridden_refund_amount"] != DBNull.Value)
                        ldecOverridden = (decimal)ldtbRemittance.Rows[0]["overridden_refund_amount"];
                    ldecRefundRemittanceAmount = ldecOverridden > 0.0M ? ldecOverridden : ldecComputed;
                }

                //Available amount will be net amount after Refund amount is reduced
                ldecAvailableAmount = ldecAvailableAmount - ldecRefundRemittanceAmount - ldecAllocatedNegativeDeposit;
            }
            return ldecAvailableAmount;
        }
        /// <summary>
        /// Get the Total Allocated Amount for the Given Remittance ID
        /// 1) Remittance Status Should be Applied
        /// 2) Loop through the Remittance Allocation and Sum up all the allocated Amount. 
        /// Remiitance Allocation Status should be allocated
        /// </summary>
        /// <param name="aintRemittanceID"></param>
        /// <returns></returns>
        public static decimal GetRemittanceAllocatedAmount(int aintRemittanceID, busRemittance aobjRemittance = null)
        {
            decimal ldecTotalAllocatedAmount = 0.00M;
            //If the Remittance Status must be Applied to Calculate the Amount
            busRemittance lobjRemittance = new busRemittance { icdoRemittance = new cdoRemittance() };
            if (aobjRemittance.IsNotNull() && aobjRemittance.icdoRemittance.IsNotNull())
                lobjRemittance = aobjRemittance;
            else
                lobjRemittance.FindRemittance(aintRemittanceID);

            if (lobjRemittance.ibusDeposit.IsNull())
                lobjRemittance.LoadDeposit();

            if (lobjRemittance.ibusDeposit.icdoDeposit.status_value == busConstant.DepositDetailStatusApplied)
            {
                DataTable ldtbAllocatedAmountList =
                    busBase.Select<cdoEmployerRemittanceAllocation>(
                        new string[2] { "remittance_id", "payroll_allocation_status_value" },
                        new object[2]
                            {
                                aintRemittanceID,
                                busConstant.RemittanceAllocationStatusAllocated
                            }, null, null);
                if (ldtbAllocatedAmountList.Rows.Count > 0)
                {
                    foreach (DataRow dr in ldtbAllocatedAmountList.Rows)
                    {
                        if (!(String.IsNullOrEmpty(dr["allocated_amount"].ToString())))
                            ldecTotalAllocatedAmount = ldecTotalAllocatedAmount + Convert.ToDecimal(dr["allocated_amount"]);
                        //prod pir 4079 : new column added for difference amount
                        //--Start--//
                        if (dr["difference_amount"] != DBNull.Value && dr["difference_type_value"] != DBNull.Value &&
                            (Convert.ToString(dr["difference_type_value"]) == busConstant.GLItemTypeUnderContribution ||
                            Convert.ToString(dr["difference_type_value"]) == busConstant.GLItemTypeUnderRHIC))
                        {
                            ldecTotalAllocatedAmount = ldecTotalAllocatedAmount + Convert.ToDecimal(dr["difference_amount"]);
                        }
                        //--End--//
                    }
                }

                // Changes for Use Case 044 : Service Purchase, consider the payment allocation gone for Service Purchase as well.
                DataTable ldtbServicePurchaseAllocationList =
                    busBase.Select<cdoServicePurchasePaymentAllocation>(new string[1] { "remittance_id" }, new object[1] { aintRemittanceID }, null, null);
                if (ldtbServicePurchaseAllocationList.Rows.Count > 0)
                {
                    foreach (DataRow dr in ldtbServicePurchaseAllocationList.Rows)
                    {
                        if (!(String.IsNullOrEmpty(dr["applied_amount"].ToString())))
                            ldecTotalAllocatedAmount = ldecTotalAllocatedAmount + Convert.ToDecimal(dr["applied_amount"]);
                    }
                }
                // Checking Available Amount for IBS A/R Batch
                DataTable ldtIBSAllocationList =
                    busBase.Select<cdoIbsRemittanceAllocation>(new string[1] { "remittance_id" }, new object[1] { aintRemittanceID }, null, null);
                if (ldtIBSAllocationList.Rows.Count > 0)
                {
                    foreach (DataRow dr in ldtIBSAllocationList.Rows)
                    {
                        if (!(String.IsNullOrEmpty(dr["ALLOCATED_AMOUNT"].ToString())))
                            ldecTotalAllocatedAmount = ldecTotalAllocatedAmount + Convert.ToDecimal(dr["ALLOCATED_AMOUNT"]);
                    }
                }
                //Pir-7432
                DataTable ldtSeminarAttPytAllocationList =
                    busBase.Select<cdoSeminarAttendeePaymentAllocation>(new string[1] { "remittance_id" }, new object[1] { aintRemittanceID }, null, null);
                if (ldtSeminarAttPytAllocationList.Rows.Count > 0)
                {
                    foreach (DataRow dr in ldtSeminarAttPytAllocationList.Rows)
                    {
                        if (!(String.IsNullOrEmpty(dr["APPLIED_AMOUNT"].ToString())))
                            ldecTotalAllocatedAmount = ldecTotalAllocatedAmount + Convert.ToDecimal(dr["APPLIED_AMOUNT"]);
                    }
                }
                //checking availble amount for BenefitPayback
                DataTable ldtPaymentRecoveryHistory =
                    busBase.Select<cdoPaymentRecoveryHistory>(new string[1] { enmPaymentRecoveryHistory.remittance_id.ToString() },
                    new object[1] { aintRemittanceID }, null, null);
                ldecTotalAllocatedAmount += ldtPaymentRecoveryHistory.AsEnumerable()
                                            .Sum(o => o.Field<decimal>("principle_amount_paid") +
                                                o.Field<decimal>("amortization_interest_paid"));
            }                
            return ldecTotalAllocatedAmount;
        }

        public static string GetToItemTypeForEmployerReportBucket(string astrBucket)
        {
            string lstrResult = string.Empty;
            switch (astrBucket)
            {
                case busConstant.ItemTypeEEContribution:
                case busConstant.ItemTypeEEPreTax:
                case busConstant.ItemTypeEEEmpPickup:
                case busConstant.ItemTypeERContribution:
                case busConstant.ItemTypeEmployerInterest:
                case busConstant.ItemTypeMemberInterest:
                    lstrResult = busConstant.ItemTypeContribution;
                    break;
                case busConstant.ItemTypeRHICEEContribution:
                case busConstant.ItemTypeRHICERContribution:
                    lstrResult = busConstant.ItemTypeRHICContribution;
                    break;
                case busConstant.ItemTypeDefeCompAmount:
                    lstrResult = busConstant.ItemTypeDefeCompDeposit;
                    break;
                case busConstant.ItemTypeGroupHealthPremium:
                    lstrResult = busConstant.ItemTypeGroupHealthDeposit;
                    break;
                case busConstant.ItemTypeHMOPremium:
                    lstrResult = busConstant.ItemTypeHMODeposit;
                    break;
                case busConstant.ItemTypeGroupLifePremium:
                    lstrResult = busConstant.ItemTypeGroupLifeDeposit;
                    break;
                case busConstant.ItemTypeEAPPremium:
                    lstrResult = busConstant.ItemTypeEAPDeposit;
                    break;
                case busConstant.ItemTypeLTCPremium:
                    lstrResult = busConstant.ItemTypeLTCDeposit;
                    break;
                case busConstant.ItemTypeGroupDentalPremium:
                    lstrResult = busConstant.ItemTypeGroupDentalDeposit;
                    break;
                case busConstant.ItemTypeGroupVisionPremium:
                    lstrResult = busConstant.ItemTypeGroupVisionDeposit;
                    break;
                case busConstant.ItemTypeMedicarePremium:
                    lstrResult = busConstant.ItemTypeMedicareDeposit;
                    break;
            }
            return lstrResult;
        }
        public static string GetBenefitTypeForEmployerHeaderType(string astrHeaderTypeValue)
        {
            string lstrResult = string.Empty;
            switch (astrHeaderTypeValue)
            {
                case busConstant.PayrollHeaderBenefitTypeDefComp:
                    lstrResult = busConstant.PlanBenefitTypeDeferredComp;
                    break;
                case busConstant.PayrollHeaderBenefitTypeRtmt:
                case busConstant.PayrollHeaderBenefitTypePurchases:
                    lstrResult = busConstant.PlanBenefitTypeRetirement;
                    break;
                case busConstant.PayrollHeaderBenefitTypeInsr:
                    lstrResult = busConstant.PlanBenefitTypeInsurance;
                    break;
            }
            return lstrResult;
        }

        public static string GetToItemTypeByPlan(string astrPlanCode)
        {
            string lstrResult = string.Empty;
            switch (astrPlanCode)
            {
                case busConstant.PlanCodeGroupHealth:
                    lstrResult = busConstant.ItemTypeGroupHealthDeposit;
                    break;
                case busConstant.PlanCodeHMO:
                    lstrResult = busConstant.ItemTypeHMODeposit;
                    break;
                case busConstant.PlanCodeGroupLife:
                    lstrResult = busConstant.ItemTypeGroupLifeDeposit;
                    break;
                case busConstant.PlanCodeEAP:
                    lstrResult = busConstant.ItemTypeEAPDeposit;
                    break;
                case busConstant.PlanCodeLTC:
                    lstrResult = busConstant.ItemTypeLTCDeposit;
                    break;
                case busConstant.PlanCodeDental:
                    lstrResult = busConstant.ItemTypeGroupDentalDeposit;
                    break;
                case busConstant.PlanCodeVision:
                    lstrResult = busConstant.ItemTypeGroupVisionDeposit;
                    break;
                case busConstant.PlanCodeMedicarePartD:
                    lstrResult = busConstant.ItemTypeMedicareDeposit;
                    break;
            }
            return lstrResult;
        }

        public static string GetToItemTypeByPlanID(int aintPlanID)
        {
            string lstrResult = string.Empty;
            switch (aintPlanID)
            {
                case busConstant.PlanIdGroupHealth:
                    lstrResult = busConstant.ItemTypeGroupHealthDeposit;
                    break;
                case busConstant.PlanIdHMO:
                    lstrResult = busConstant.ItemTypeHMODeposit;
                    break;
                case busConstant.PlanIdGroupLife:
                    lstrResult = busConstant.ItemTypeGroupLifeDeposit;
                    break;
                case busConstant.PlanIdEAP:
                    lstrResult = busConstant.ItemTypeEAPDeposit;
                    break;
                case busConstant.PlanIdLTC:
                    lstrResult = busConstant.ItemTypeLTCDeposit;
                    break;
                case busConstant.PlanIdDental:
                    lstrResult = busConstant.ItemTypeGroupDentalDeposit;
                    break;
                case busConstant.PlanIdVision:
                    lstrResult = busConstant.ItemTypeGroupVisionDeposit;
                    break;
                case busConstant.PlanIdMedicarePartD:
                    lstrResult = busConstant.ItemTypeMedicareDeposit;
                    break;
            }
            return lstrResult;
        }

        public static string GetFromItemTypeByPlan(int aintPlanID)
        {
            string lstrResult = string.Empty;
            switch (aintPlanID)
            {
                case busConstant.PlanIdGroupHealth:
                    lstrResult = busConstant.ItemTypeGroupHealthPremium;
                    break;
                case busConstant.PlanIdHMO:
                    lstrResult = busConstant.ItemTypeHMOPremium;
                    break;
                case busConstant.PlanIdGroupLife:
                    lstrResult = busConstant.ItemTypeGroupLifePremium;
                    break;
                case busConstant.PlanIdEAP:
                    lstrResult = busConstant.ItemTypeEAPPremium;
                    break;
                case busConstant.PlanIdLTC:
                    lstrResult = busConstant.ItemTypeLTCPremium;
                    break;
                case busConstant.PlanIdDental:
                    lstrResult = busConstant.ItemTypeGroupDentalPremium;
                    break;
                case busConstant.PlanIdVision:
                    lstrResult = busConstant.ItemTypeGroupVisionPremium; // PIR 9986
                    break;
                case busConstant.PlanIdMedicarePartD:
                    lstrResult = busConstant.ItemTypeMedicarePremium;
                    break;
            }
            return lstrResult;
        }

        /// <summary>
        /// Calculate Bonus
        /// </summary>
        /// <param name="aobjEmployerPayrollDetail"></param>
        /// <returns></returns>
        public static Collection<cdoEmployerPayrollBonusDetail> CalculateBonus(busEmployerPayrollDetail aobjEmployerPayrollDetail)
        {
            Collection<cdoEmployerPayrollBonusDetail> lclbEmployerPayrollBonusDetail = new Collection<cdoEmployerPayrollBonusDetail>();
            decimal ldecEEContributionAmount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_contribution_reported;
            decimal ldecEEPreTaxAmount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_pre_tax_reported;
            decimal ldecEEempPickupAmount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_employer_pickup_reported;
            decimal ldecERContributionAmount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.er_contribution_reported;
            decimal ldecRHICEEContributionAmount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.rhic_ee_contribution_reported;
            decimal ldecRHICERContributionAmount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.rhic_er_contribution_reported;
            decimal ldecEligibleWages = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.eligible_wages;

            //If no amount is defined, return
            if ((ldecEEContributionAmount + ldecEEempPickupAmount + ldecEEPreTaxAmount +
                ldecERContributionAmount + ldecRHICEEContributionAmount + ldecRHICERContributionAmount + ldecEligibleWages) > 0)  //PIR-17777
            {
                DateTime ldtBonusStartDate = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_date;
                DateTime ldtBonusEndDate = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_end_month_for_bonus;
                DateTime ldtModifiedBonusEndDate = new DateTime(ldtBonusEndDate.Year, ldtBonusEndDate.Month, 1);
                DateTime ldttmpBonusEndDate = ldtModifiedBonusEndDate;
                int lintTotalNoOfMonths = 0;
                while (ldttmpBonusEndDate >= ldtBonusStartDate)
                {
                    lintTotalNoOfMonths++;
                    ldttmpBonusEndDate = ldttmpBonusEndDate.AddMonths(-1);
                }

                if (lintTotalNoOfMonths > 0)
                {
                    //Create the Collection with the Amount Zero by default
                    for (int lintLoop = 0; lintLoop < lintTotalNoOfMonths; lintLoop++)
                    {
                        cdoEmployerPayrollBonusDetail lcdoEmployerPayrollBonusDetail = new cdoEmployerPayrollBonusDetail();
                        lcdoEmployerPayrollBonusDetail.employer_payroll_detail_id = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_detail_id;
                        lcdoEmployerPayrollBonusDetail.bonus_period = ldtBonusStartDate.AddMonths(lintLoop);
                        lcdoEmployerPayrollBonusDetail.ee_contribution = 0;
                        lcdoEmployerPayrollBonusDetail.ee_pre_tax = 0;
                        lcdoEmployerPayrollBonusDetail.ee_employer_pickup = 0;
                        lcdoEmployerPayrollBonusDetail.er_contribution = 0;
                        lcdoEmployerPayrollBonusDetail.rhic_ee_contribution = 0;
                        lcdoEmployerPayrollBonusDetail.rhic_er_contribution = 0;
                        lcdoEmployerPayrollBonusDetail.eligible_wages = 0;
                        lcdoEmployerPayrollBonusDetail.member_interest = 0;
                        lcdoEmployerPayrollBonusDetail.employer_interest = 0;
                        lcdoEmployerPayrollBonusDetail.employer_rhic_interest = 0;
                        lclbEmployerPayrollBonusDetail.Add(lcdoEmployerPayrollBonusDetail);
                    }
					//PIR 15616 - We are anyway posting the contributions from detail, not from bonus detail, so commented these function calls
                    //ee contribution
                    //CalculateBonusBucket(lclbEmployerPayrollBonusDetail,
                    //                    BonusBucket.ee_contribution,
                    //                    aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_contribution_reported,
                    //                    lintTotalNoOfMonths,
                    //                    ldtBonusStartDate,
                    //                    ldtModifiedBonusEndDate);
                    ////ee pretax
                    //CalculateBonusBucket(lclbEmployerPayrollBonusDetail,
                    //                    BonusBucket.ee_pre_tax,
                    //                    aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_pre_tax_reported,
                    //                    lintTotalNoOfMonths,
                    //                    ldtBonusStartDate,
                    //                    ldtModifiedBonusEndDate);

                    ////ee emp pickup
                    //CalculateBonusBucket(lclbEmployerPayrollBonusDetail,
                    //                    BonusBucket.ee_employer_pickup,
                    //                    aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_employer_pickup_reported,
                    //                    lintTotalNoOfMonths,
                    //                    ldtBonusStartDate,
                    //                    ldtModifiedBonusEndDate);

                    ////er contribution
                    //CalculateBonusBucket(lclbEmployerPayrollBonusDetail,
                    //                    BonusBucket.er_contribution,
                    //                    aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.er_contribution_reported,
                    //                    lintTotalNoOfMonths,
                    //                    ldtBonusStartDate,
                    //                    ldtModifiedBonusEndDate);

                    ////rhic ee contribution
                    //CalculateBonusBucket(lclbEmployerPayrollBonusDetail,
                    //                    BonusBucket.rhic_ee_contribution,
                    //                    aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.rhic_ee_contribution_reported,
                    //                    lintTotalNoOfMonths,
                    //                    ldtBonusStartDate,
                    //                    ldtModifiedBonusEndDate);

                    ////rhic er contribution
                    //CalculateBonusBucket(lclbEmployerPayrollBonusDetail,
                    //                    BonusBucket.rhic_er_contribution,
                    //                    aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.rhic_er_contribution_reported,
                    //                    lintTotalNoOfMonths,
                    //                    ldtBonusStartDate,
                    //                    ldtModifiedBonusEndDate);

                    //Eligible Wages
                    CalculateBonusBucket(lclbEmployerPayrollBonusDetail,
                                        BonusBucket.eligible_wages,
                                        aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.eligible_wages,
                                        lintTotalNoOfMonths,
                                        ldtBonusStartDate,
                                        ldtModifiedBonusEndDate);
                }

                ////PIR UAT R2 132 
                ////Calculate Member Interest, Employer Interest From the Splitted Bonus, only if waiver flag is not checked
                ////PIR 9538
                //if (aobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.interest_waiver_flag != busConstant.Flag_Yes
                //    && aobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt
                //    && aobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.report_type_value == busConstant.PayrollHeaderReportTypeAdjustment
                //    && DateTime.Parse(aobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period) > ldtModifiedBonusEndDate.AddMonths(3))
                //{
                //    foreach (cdoEmployerPayrollBonusDetail lcdoEmployerPayrollBonusDetail in lclbEmployerPayrollBonusDetail)
                //    {
                //        decimal ldecMemberContribution = lcdoEmployerPayrollBonusDetail.ee_contribution +
                //                                         lcdoEmployerPayrollBonusDetail.ee_pre_tax +
                //                                         lcdoEmployerPayrollBonusDetail.ee_employer_pickup;

                //        decimal ldecEmployerContribution = lcdoEmployerPayrollBonusDetail.er_contribution;

                //        decimal ldecEmployerRHICContribution = lcdoEmployerPayrollBonusDetail.rhic_ee_contribution + lcdoEmployerPayrollBonusDetail.rhic_er_contribution;

                //        DateTime ldtBenefitBeginDate = aobjEmployerPayrollDetail.GetBenefitBeginDate();
                //        utlPassInfo lobjPassInfo = utlPassInfo.iobjPassInfo;

                //        lcdoEmployerPayrollBonusDetail.member_interest =
                //            CalculateMemberInterest(aobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.submitted_date,
                //                                    lcdoEmployerPayrollBonusDetail.bonus_period, ldecMemberContribution, lobjPassInfo, ldtBenefitBeginDate, aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_id);

                //        lcdoEmployerPayrollBonusDetail.employer_interest =
                //            CalculateEmployerInterest(aobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.submitted_date,
                //                                    lcdoEmployerPayrollBonusDetail.bonus_period,
                //                                    ldecMemberContribution,
                //                                    ldecEmployerContribution
                //                                    , lobjPassInfo, ldtBenefitBeginDate, aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_id);

                //        lcdoEmployerPayrollBonusDetail.employer_rhic_interest =
                //               CalculateEmployerRHICInterest(aobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.submitted_date,
                //                                       lcdoEmployerPayrollBonusDetail.bonus_period, ldecEmployerRHICContribution, lobjPassInfo, ldtBenefitBeginDate, aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_id);

                //    }
                //}
            }

            return lclbEmployerPayrollBonusDetail;
        }

        private static void CalculateBonusBucket(Collection<cdoEmployerPayrollBonusDetail> aclbEmployerPayrollBonusDetail,
                                          BonusBucket aenuBonusBucket,
                                          decimal adecBucketAmount,
                                          int aintTotalNoOfMonths,
                                          DateTime adtBonusStartDate,
                                          DateTime adtBonusEndDate)
        {
            //Calculate EE Contribution Bonus                            
            if (adecBucketAmount > 0) //PIR-17777
            {
				//PIR 11235 - When calculating wages/contributions for each month, do not round to nearest dollar.  It should be the reported wages divided by the number of months of the bonus period.
                int i = 1;
                decimal BucketAmountSplitted = Math.Round((adecBucketAmount / aintTotalNoOfMonths), 2, MidpointRounding.AwayFromZero);
                decimal ldecTotalAmountAllocated = 0.0M;
                foreach (cdoEmployerPayrollBonusDetail lcdoEmployerPayrollBonusDetail in aclbEmployerPayrollBonusDetail)
                {
                    switch (aenuBonusBucket)
                    {
                        //case BonusBucket.ee_contribution:
                        //    lcdoEmployerPayrollBonusDetail.ee_contribution = (i == aclbEmployerPayrollBonusDetail.Count) ? (adecBucketAmount - ldecTotalAmountAllocated) : BucketAmountSplitted;
                        //    ldecTotalAmountAllocated += BucketAmountSplitted;
                        //    break;
                        //case BonusBucket.ee_pre_tax:
                        //    lcdoEmployerPayrollBonusDetail.ee_pre_tax = (i == aclbEmployerPayrollBonusDetail.Count) ? (adecBucketAmount - ldecTotalAmountAllocated) : BucketAmountSplitted;
                        //    ldecTotalAmountAllocated += BucketAmountSplitted;
                        //    break;
                        //case BonusBucket.ee_employer_pickup:
                        //    lcdoEmployerPayrollBonusDetail.ee_employer_pickup = (i == aclbEmployerPayrollBonusDetail.Count) ? (adecBucketAmount - ldecTotalAmountAllocated) : BucketAmountSplitted;
                        //    ldecTotalAmountAllocated += BucketAmountSplitted;
                        //    break;
                        //case BonusBucket.er_contribution:
                        //    lcdoEmployerPayrollBonusDetail.er_contribution = (i == aclbEmployerPayrollBonusDetail.Count) ? (adecBucketAmount - ldecTotalAmountAllocated) : BucketAmountSplitted;
                        //    ldecTotalAmountAllocated += BucketAmountSplitted;
                        //    break;
                        //case BonusBucket.rhic_ee_contribution:
                        //    lcdoEmployerPayrollBonusDetail.rhic_ee_contribution = (i == aclbEmployerPayrollBonusDetail.Count) ? (adecBucketAmount - ldecTotalAmountAllocated) : BucketAmountSplitted;
                        //    ldecTotalAmountAllocated += BucketAmountSplitted;
                        //    break;
                        //case BonusBucket.rhic_er_contribution:
                        //    lcdoEmployerPayrollBonusDetail.rhic_er_contribution = (i == aclbEmployerPayrollBonusDetail.Count) ? (adecBucketAmount - ldecTotalAmountAllocated) : BucketAmountSplitted;
                        //    ldecTotalAmountAllocated += BucketAmountSplitted;
                        //    break;
                        case BonusBucket.eligible_wages:
                            lcdoEmployerPayrollBonusDetail.eligible_wages = (i == aclbEmployerPayrollBonusDetail.Count) ? (adecBucketAmount - ldecTotalAmountAllocated) : BucketAmountSplitted;
                            ldecTotalAmountAllocated += BucketAmountSplitted;
                            break;
                        default:
                            break;
                    }
                    i++;                                    
                }
				//PIR 11235 
                //if (adecBucketAmount < aintTotalNoOfMonths)
                //{
                //    //Just Allocate 1$ in each month if the Total Amount is less than Total No Of Months (Rare Case)
                //    for (int lintLoop = 0; lintLoop < adecBucketAmount; lintLoop++)
                //    {
                //        cdoEmployerPayrollBonusDetail lcdoEmployerPayrollBonusDetail =
                //            GetEmployerPayrollBonusDetailByPeriod(aclbEmployerPayrollBonusDetail,
                //                                                  adtBonusStartDate.AddMonths(lintLoop));
                //        ApplyBonusAmountInBucket(lcdoEmployerPayrollBonusDetail, aenuBonusBucket, 1);
                //    }
                //}
                //else
                //{
                //    //Allocating the Amount for each month
                //    decimal ldecSplitedAmount = 0;
                //    decimal ldecAppliedAmount = 0;
                //    for (int lintLoop = 0; lintLoop < aintTotalNoOfMonths; lintLoop++)
                //    {
                //        ldecSplitedAmount = Math.Floor(adecBucketAmount / aintTotalNoOfMonths);

                //        cdoEmployerPayrollBonusDetail lcdoEmployerPayrollBonusDetail =
                //            GetEmployerPayrollBonusDetailByPeriod(aclbEmployerPayrollBonusDetail,
                //                                                  adtBonusStartDate.AddMonths(lintLoop));
                //        ApplyBonusAmountInBucket(lcdoEmployerPayrollBonusDetail, aenuBonusBucket, ldecSplitedAmount);
                //        ldecAppliedAmount += ldecSplitedAmount;
                //    }

                //    //Allocating the Remaining amount as each 1$ in Reverse Order                            
                //    for (int lintLoop = 0; lintLoop < adecBucketAmount - ldecAppliedAmount; lintLoop++)
                //    {
                //        cdoEmployerPayrollBonusDetail lcdoEmployerPayrollBonusDetail =
                //            GetEmployerPayrollBonusDetailByPeriod(aclbEmployerPayrollBonusDetail,
                //                                                  adtBonusEndDate.AddMonths(-lintLoop));
                //        ApplyBonusAmountInBucket(lcdoEmployerPayrollBonusDetail, aenuBonusBucket, 1);
                //    }
                //}
            }
        }

        private static void ApplyBonusAmountInBucket(cdoEmployerPayrollBonusDetail acdoEmployerPayrollBonusDetail, BonusBucket aenuBonusBucket, decimal adecAmount)
        {
            switch (aenuBonusBucket)
            {
                case BonusBucket.ee_contribution:
                    acdoEmployerPayrollBonusDetail.ee_contribution += adecAmount;
                    break;
                case BonusBucket.ee_pre_tax:
                    acdoEmployerPayrollBonusDetail.ee_pre_tax += adecAmount;
                    break;
                case BonusBucket.ee_employer_pickup:
                    acdoEmployerPayrollBonusDetail.ee_employer_pickup += adecAmount;
                    break;
                case BonusBucket.er_contribution:
                    acdoEmployerPayrollBonusDetail.er_contribution += adecAmount;
                    break;
                case BonusBucket.rhic_ee_contribution:
                    acdoEmployerPayrollBonusDetail.rhic_ee_contribution += adecAmount;
                    break;
                case BonusBucket.rhic_er_contribution:
                    acdoEmployerPayrollBonusDetail.rhic_er_contribution += adecAmount;
                    break;
                case BonusBucket.eligible_wages:
                    acdoEmployerPayrollBonusDetail.eligible_wages += adecAmount;
                    break;
            }
        }

        private static cdoEmployerPayrollBonusDetail GetEmployerPayrollBonusDetailByPeriod(Collection<cdoEmployerPayrollBonusDetail> aclbEmployerPayrollBonusDetail,
                                                                                    DateTime adtBonusPeriod)
        {
            cdoEmployerPayrollBonusDetail lcdotmpEmployerPayrollBonusDetail = new cdoEmployerPayrollBonusDetail();
            foreach (cdoEmployerPayrollBonusDetail lcdoEmployerPayrollBonusDetail in aclbEmployerPayrollBonusDetail)
            {
                if (lcdoEmployerPayrollBonusDetail.bonus_period == adtBonusPeriod)
                {
                    lcdotmpEmployerPayrollBonusDetail = lcdoEmployerPayrollBonusDetail;
                }
            }

            return lcdotmpEmployerPayrollBonusDetail;
        }

        private enum BonusBucket
        {
            ee_contribution,
            ee_pre_tax,
            ee_employer_pickup,
            er_contribution,
            rhic_ee_contribution,
            rhic_er_contribution,
            eligible_wages
        }

        /// <summary>
        /// To Get the Premium rates for the Insurance Plans.
        /// </summary>
        /// <param name="aintOrgPlanID">Org Plan ID</param>
        /// <param name="aintPlanID">Plan ID</param>
        /// <param name="adtEffectiveDate">Effective Date</param>
        /// <returns></returns>
        public static decimal GetInsurancePlansPremiumRate(int aintOrgPlanID, int aintPlanID, DateTime adtEffectiveDate)
        {
            decimal ldecPremiumRate = 0.00M;
            return ldecPremiumRate;
        }

        public static decimal CalculateMemberInterest(DateTime adtLastPostedInterestDate, DateTime adtPayPeriodDate,
            decimal adecMemberContributionAmount, utlPassInfo aobjPassInfo, DateTime adtBenefitBeginDate, int aintPlanID)
        {
            decimal ldecMemberInterest = 0.00M;
            decimal ldecInterest = 0.0M;
            //int lintTotalDueMonths = 0;           
            //if (adtBenefitBeginDate == DateTime.MinValue)         //PIR-8773 interest calculation is based on Last interest posted date and pay period date.
            //{
            // lintTotalDueMonths = GetTotalDueMonths(adtLastPostedInterestDate, adtPayPeriodDate);
            //}
            //else
            //{
            //    //uat pir 1384 : as per satya no need to add one month to payperiod date for calculating due months
            //    lintTotalDueMonths = GetTotalDueMonths(adtBenefitBeginDate.AddDays(-1), adtPayPeriodDate);
            //}
            //Calculating Actuarial Percentage
            /*  decimal ldecActualPercenatge = GetActuarialInterestPercenatge(aintPlanID, adtPayPeriodDate, aobjPassInfo);

              decimal ldecAnnualRate = (((ldecActualPercenatge - 0.5M) / 100) / 12M);

              //double ldblMemberContributionAmount = Convert.ToDouble(Math.Round(adecMemberContributionAmount, 6,
              //MidpointRounding.AwayFromZero));
              double ldblMemberContributionAmount = Convert.ToDouble(adecMemberContributionAmount);

              //double ldblAnnualRate = Convert.ToDouble(Math.Round(ldecAnnualRate, 6, MidpointRounding.AwayFromZero));
              double ldblAnnualRate = Convert.ToDouble(ldecAnnualRate);

              ldecMemberInterest = Convert.ToDecimal((ldblMemberContributionAmount *
                                                      Math.Pow(1 + ldblAnnualRate,
                                                               Convert.ToDouble(lintTotalDueMonths))) -
                                                     ldblMemberContributionAmount);
           * */
            #region PIR-17512
            //PIR 26457 uncommented this part again after discussion with Maik
            //int lintMonths = (adtLastPostedInterestDate.Month - adtPayPeriodDate.Month) + 1;
            //int lintYears = adtLastPostedInterestDate.Year - adtPayPeriodDate.Year;
            //lintMonths += lintYears * 12;
            //DateTime ldateEffectiveDate = new DateTime(adtPayPeriodDate.Year, adtPayPeriodDate.Month, 1).AddMonths(1);
            //PIR 26010                       
            int lintNumberOfMonths = busGlobalFunctions.DateDiffByMonth(adtPayPeriodDate, adtLastPostedInterestDate);
            //int lintMonths = lintNumberOfMonths + 1;
            DateTime ldateEffectiveDate = new DateTime(adtPayPeriodDate.Year, adtPayPeriodDate.Month, 1).AddMonths(1);

            for (int i = 0; i < lintNumberOfMonths; i++)
            {
                ldecInterest = busInterestCalculationHelper.CalculateInterest(adecMemberContributionAmount, aintPlanID, ldateEffectiveDate.AddDays(-1)); //PIR 17512 interest(interest rate based on effective date)  POINT 3                    
                adecMemberContributionAmount += ldecInterest;     //Add Interest to balance for next month.
                ldateEffectiveDate = ldateEffectiveDate.AddMonths(1); //Add 1 to effective date for next month.   
                ldecMemberInterest += ldecInterest;
            }
            #endregion PIR-17512
               return ldecMemberInterest;

        }

        public static decimal CalculateEmployerInterest(DateTime adtLastPostedInterestDate, DateTime adtPayPeriodDate,
            decimal adecMemberContributionAmount, decimal adecEmployerContributionAmount, utlPassInfo aobjPassInfo, DateTime adtBenefitBeginDate, int aintPlanID)
        {
            decimal ldecEmployerInterest = 0.00M;
            //int lintTotalDueMonths = 0;
            decimal ldecEREmployerInterest = 0.0M;
            decimal ldecActualPercenatge = 0.0M;
            decimal ldecERAnnualRate = 0.0M;                     
            double ldblEmployerContributionAmount;
            //lintTotalDueMonths = GetTotalDueMonths(adtSubmittedDate, adtPayPeriodDate);

            #region pir-17512
            ldblEmployerContributionAmount = Convert.ToDouble(Math.Round(adecEmployerContributionAmount, 6,
                                                              MidpointRounding.AwayFromZero));
            decimal ldecInterest = 0.0M;
            //PIR 26457 uncommented this part again after discussion with Maik
            //int lintMonths = (adtLastPostedInterestDate.Month - adtPayPeriodDate.Month) + 1;
            //int lintYears = adtLastPostedInterestDate.Year - adtPayPeriodDate.Year;
            //lintMonths += lintYears * 12;
            ////PIR 26010                       
            int lintNumberOfMonths = busGlobalFunctions.DateDiffByMonth(adtPayPeriodDate, adtLastPostedInterestDate);
            //int lintMonths = lintNumberOfMonths + 1;
            DateTime ldateEffectiveDate = new DateTime(adtPayPeriodDate.Year, adtPayPeriodDate.Month, 1).AddMonths(1);
            for (int i = 0; i < lintNumberOfMonths; i++)
            {
                ldecActualPercenatge = GetActuarialInterestPercenatge(aintPlanID, ldateEffectiveDate.AddDays(-1), aobjPassInfo);
                ldecERAnnualRate = ((ldecActualPercenatge / 100) / 12M);
                ldecInterest = Convert.ToDecimal(ldblEmployerContributionAmount * Convert.ToDouble(ldecERAnnualRate));
                ldecEREmployerInterest += ldecInterest;
                ldblEmployerContributionAmount += Convert.ToDouble(ldecInterest);     

            }
            #endregion pir-17512
            //Calculating Actuarial Percentage
            //decimal ldecActualPercenatge = GetActuarialInterestPercenatge(aintPlanID, adtPayPeriodDate, aobjPassInfo);

            //decimal ldecERAnnualRate = ((ldecActualPercenatge / 100) / 12M);

            //double ldblEmployerContributionAmount = Convert.ToDouble(Math.Round(adecEmployerContributionAmount, 6, MidpointRounding.AwayFromZero));
            //double ldblERAnnualRate = Convert.ToDouble(Math.Round(ldecERAnnualRate, 6, MidpointRounding.AwayFromZero));
            //double ldblERAnnualRate = Convert.ToDouble(ldecERAnnualRate);


            ////decimal ldecEREmployerInterest = Convert.ToDecimal((ldblEmployerContributionAmount *
            //                                                    Math.Pow(1 + ldblERAnnualRate,
            //                                                             Convert.ToDouble(lintTotalDueMonths))) -
                                                               //ldblEmployerContributionAmount);
            decimal ldecEEAnnualRate = ((0.5M / 100) / 12M);
            decimal ldecEEEmployerInterest = 0.0m;
            ldecEEEmployerInterest = GetEEEmployerInterest(adecMemberContributionAmount, lintNumberOfMonths < 0 ? 0 : lintNumberOfMonths, ldecEEAnnualRate);//PIR-8773 interest calculation is based on Last interest posted date and pay period date.
           /* //if (adtBenefitBeginDate == DateTime.MinValue)   
            {
                ldecEEEmployerInterest = GetEEEmployerInterest(adecMemberContributionAmount, lintTotalDueMonths, ldecEEAnnualRate);
            }
            else
            {//BR-79-08 - The system must calculate interest till last interest batch run date for ‘Employer Interest’
                int lintTotalDueMonthsTillBenefitBeginDate = GetTotalDueMonths(adtBenefitBeginDate.AddDays(-1), adtPayPeriodDate);
                ldecEEEmployerInterest = GetEEEmployerInterest(adecMemberContributionAmount, lintTotalDueMonthsTillBenefitBeginDate, ldecEEAnnualRate);
                decimal ldecEEEmployerInterestAdditional = 0.0m;
                if (adtLastPostedInterestDate > adtBenefitBeginDate)
                {
                    int lintTotalDueMonthsTillInterestBatchDate = GetTotalDueMonths(adtLastPostedInterestDate, adtBenefitBeginDate);
                    ldecEEEmployerInterestAdditional = GetEEEmployerInterest(adecMemberContributionAmount, lintTotalDueMonthsTillInterestBatchDate, ldecERAnnualRate);
                }
                ldecEEEmployerInterestAdditional = ldecEEEmployerInterestAdditional <= 0.0m ? 0.0m : ldecEEEmployerInterestAdditional;
                ldecEEEmployerInterest = ldecEEEmployerInterest + ldecEEEmployerInterestAdditional;
            }*/
            ldecEmployerInterest = ldecEREmployerInterest + ldecEEEmployerInterest;
            return ldecEmployerInterest;
        }

        public static decimal CalculateEmployerRHICInterest(DateTime adtLastPostedInterestDate, DateTime adtPayPeriodDate,
            decimal adecContributionAmount, utlPassInfo aobjPassInfo, DateTime adtBenefitBeginDate, int aintPlanID)
        {
            decimal ldecEmployerRHICInterest = 0.00M;
			decimal ldecActualPercenatge = 0.0M;
            decimal ldecAnnualRate = 0.0M;
            //int lintTotalDueMonths = 0;            
            //PIR-8773 interest calculation is based on Last interest posted date and pay period date.
            //if (adtBenefitBeginDate == DateTime.MinValue)
            //{
            
            //lintTotalDueMonths = GetTotalDueMonths(adtLastPostedInterestDate, adtPayPeriodDate);
            //}
            //else
            //{
            //    //uat pir 1384 : as per satya no need to add one month to payperiod date for calculating due months
            //    lintTotalDueMonths = GetTotalDueMonths(adtBenefitBeginDate.AddDays(-1), adtPayPeriodDate);
            //}
            //Calculating Actuarial Percentage
          /*  decimal ldecActualPercenatge = GetActuarialInterestPercenatge(aintPlanID, adtPayPeriodDate, aobjPassInfo);

            decimal ldecAnnualRate = (((ldecActualPercenatge) / 100) / 12M);

            double ldblContributionAmount = Convert.ToDouble(adecContributionAmount);

            //double ldblAnnualRate = Convert.ToDouble(Math.Round(ldecAnnualRate, 6, MidpointRounding.AwayFromZero));
            double ldblAnnualRate = Convert.ToDouble(ldecAnnualRate);

            ldecEmployerRHICInterest = Convert.ToDecimal((ldblContributionAmount *
                                                    Math.Pow(1 + ldblAnnualRate,
                                                             Convert.ToDouble(lintTotalDueMonths))) -
                                                   ldblContributionAmount);
            */
            //PIR- 17512 interest 
            #region PIR-17512
            decimal ldecInterest = 0.0M;
            //int lintMonths = (adtLastPostedInterestDate.Month - adtPayPeriodDate.Month) + 1;
            //int lintYears = adtLastPostedInterestDate.Year - adtPayPeriodDate.Year;
            //lintMonths += lintYears * 12;
            //PIR 26010
            int lintNumberOfMonths = busGlobalFunctions.DateDiffByMonth(adtPayPeriodDate, adtLastPostedInterestDate);
            //int lintMonths = lintNumberOfMonths + 1;
            DateTime ldateEffectiveDate = new DateTime(adtPayPeriodDate.Year, adtPayPeriodDate.Month, 1).AddMonths(1);
            for (int i = 0; i < lintNumberOfMonths; i++)
            {
                ldecActualPercenatge = GetActuarialInterestPercenatge(aintPlanID, ldateEffectiveDate.AddDays(-1), aobjPassInfo);
                ldecAnnualRate = (((ldecActualPercenatge) / 100) / 12M);
                ldecInterest = (ldecAnnualRate * adecContributionAmount);
                adecContributionAmount += ldecInterest;     //Add Interest to balance for next month.
                ldateEffectiveDate = ldateEffectiveDate.AddMonths(1); //Add 1 to effective date for next month.   
                ldecEmployerRHICInterest += ldecInterest;
            }
                #endregion PIR-17512
                return ldecEmployerRHICInterest;

        }

        private static decimal GetEEEmployerInterest(decimal adecMemberContributionAmount, int lintTotalDueMonths, decimal ldecEEAnnualRate)
        {
            //double ldblMemberContributionAmount = Convert.ToDouble(Math.Round(adecMemberContributionAmount, 6,
            //                                                              MidpointRounding.AwayFromZero));

            double ldblMemberContributionAmount = Convert.ToDouble(adecMemberContributionAmount);

            //double ldblEEAnnualRate = Convert.ToDouble(Math.Round(ldecEEAnnualRate, 6, MidpointRounding.AwayFromZero));
            double ldblEEAnnualRate = Convert.ToDouble(ldecEEAnnualRate);


            decimal ldecEEEmployerInterest =
                Convert.ToDecimal((ldblMemberContributionAmount *
                                   Math.Pow(1 + ldblEEAnnualRate, Convert.ToDouble(lintTotalDueMonths))) -
                                  ldblMemberContributionAmount);
            return ldecEEEmployerInterest;
        }

        private static decimal GetActuarialInterestPercenatge(int aintPlanID, DateTime adtEffectiveDate, utlPassInfo aobjPassInfo)
        {
            decimal ldecActualPercenatge = 0;

            Collection<cdoCodeValue> lclbActurialInterest = new Collection<cdoCodeValue>();
            utlPassInfo lobjPassInfo = utlPassInfo.iobjPassInfo;
            //Filter by the plan id
            DataTable ldtbActurialInterest = lobjPassInfo.isrvDBCache.GetCodeValues(2324, null, null, aintPlanID.ToString());
            var lenuList = ldtbActurialInterest.AsEnumerable().Where(i => Convert.ToDateTime(i.Field<string>("DATA1")) <= adtEffectiveDate)
                                                              .OrderByDescending(i => Convert.ToDateTime(i.Field<string>("DATA1")));
            DataTable ldtbResult = lenuList.AsDataTable();
            if (ldtbResult.Rows.Count > 0)
            {
                ldecActualPercenatge = Convert.ToDecimal(ldtbResult.Rows[0].Field<string>("data2"));
            }
            return ldecActualPercenatge;
        }

        public static int GetTotalDueMonths(DateTime adtLastPostedInterestDate, DateTime adtPayPeriodDate)
        {
            //Calculate Total Months Difference
            int lintTotalDueMonths = busGlobalFunctions.DateDiffByMonth(adtPayPeriodDate, adtLastPostedInterestDate);

            if (lintTotalDueMonths < 0)
            {
                //If Minus Records comes for Future Date, Reset the Due Months to Zero
                lintTotalDueMonths = 0;
            }
            return lintTotalDueMonths;
        }

        /// <summary>
        /// Getting the End Date for the Given Report Frequency //PIR 19701 Day_Of_Month from Org Plan Maintenance to be used for verify validation 4690
        /// </summary>
        /// <param name="adtStartDate"></param>
        /// <param name="astrReportFrequency"></param>
        /// <param name="aintDayOfMonth"></param>
        /// <returns></returns>
        public static DateTime GetEndDateByReportFrequency(DateTime adtStartDate, string astrReportFrequency, int aintDayOfMonth = 1)
        {
            DateTime ldtResult = DateTime.MinValue;
            switch (astrReportFrequency)
            {
                case busConstant.DeffCompFrequencyMonthly:
                    {

                        ldtResult = new DateTime(adtStartDate.Year, adtStartDate.Month, aintDayOfMonth);
                        ldtResult = ldtResult.AddMonths(1).AddDays(-1);
                        break;
                    }
                case busConstant.DeffCompFrequencyWeekly:
                    {
                        ldtResult = adtStartDate.AddDays(6);
                        break;
                    }
                case busConstant.DeffCompFrequencySemiMonthly:
                    {
                        if (aintDayOfMonth == 1)
                        {
                            if (adtStartDate.Day > 15)
                            {
                                ldtResult = new DateTime(adtStartDate.Year, adtStartDate.Month, 1);
                                ldtResult = ldtResult.AddMonths(1).AddDays(-1);
                            }
                            else
                            {
                                ldtResult = new DateTime(adtStartDate.Year, adtStartDate.Month, 15);
                            }
                        }
                        else
                        {
                            if (aintDayOfMonth > 14)
                            {
                                if (adtStartDate.Day > 15)
                                {
                                    ldtResult = new DateTime(adtStartDate.Year, adtStartDate.Month, aintDayOfMonth);
                                    ldtResult = ldtResult.AddDays(14);
                                }
                                else
                                {
                                    ldtResult = new DateTime(adtStartDate.Year, adtStartDate.Month, aintDayOfMonth);
                                    ldtResult = ldtResult.AddDays(-1);
                                }
                            }
                            else
                            {
                                if (adtStartDate.Day > 15)
                                {
                                    ldtResult = new DateTime(adtStartDate.Year, adtStartDate.Month, aintDayOfMonth);
                                    ldtResult = ldtResult.AddMonths(1).AddDays(-1);
                                }
                                else
                                {
                                    ldtResult = new DateTime(adtStartDate.Year, adtStartDate.Month, aintDayOfMonth);
                                    ldtResult = ldtResult.AddDays(14);
                                }
                            }
                        }
                        break;
                    }
                case busConstant.DeffCompFrequencyBiWeekly:
                    {
                        ldtResult = adtStartDate.AddDays(13);
                        break;
                    }
            }
            return ldtResult;
        }        

        /// <summary>
        /// Get Total balance amount for Purchase
        /// </summary>
        /// <param name="aintServicePurchaseHeaderID"></param>
        /// <param name="adecTotalContractAmount"></param>
        /// <returns></returns>
        public static decimal GetTotalBalanceAmountForPurchase(int aintServicePurchaseHeaderID, decimal adecTotalContractAmount)
        {
            decimal ldecBalanceAmount = 0.00M;
            decimal ldecTotalAppliedAmount = 0.00M;
            //get total of applied amount for purchase header id
            DataTable ldtbGetPaymentAllocation = busBase.Select<cdoServicePurchasePaymentAllocation>(new string[1] { "service_purchase_header_id" },
                                                                                            new object[1] { aintServicePurchaseHeaderID }, null, null);

            if (ldtbGetPaymentAllocation != null)
            {
                foreach (DataRow dr in ldtbGetPaymentAllocation.Rows)
                {
                    if (!String.IsNullOrEmpty((dr["applied_amount"]).ToString()))
                    {
                        if (Convert.ToDecimal(dr["applied_amount"]) != 0.00M)
                        {
                            ldecTotalAppliedAmount = +Convert.ToDecimal(dr["applied_amount"]);
                        }
                    }
                }
            }
            if (ldecTotalAppliedAmount != 0.00M)
            {
                ldecBalanceAmount = adecTotalContractAmount - ldecTotalAppliedAmount;
            }
            return ldecBalanceAmount;
        }

        /// <summary>
        /// Load All the Org Plan by Benefit Type
        /// </summary>       
        /// <param name="astrBenefitType"></param>
        /// <returns>Collection</returns>
        public static Collection<busOrgPlan> LoadActiveEmployerOrgPlanByBenefitType(string astrBenefitType, DateTime adtEffectiveDate)
        {
            Collection<busOrgPlan> lclbOrgPlan = new Collection<busOrgPlan>();
            DataTable ldtbResult = busBase.Select("cdoOrgPlan.LoadActiveEmployerOrgPlanByBenefitType", new object[2] { astrBenefitType, adtEffectiveDate });
            foreach (DataRow adrRow in ldtbResult.Rows)
            {
                busOrgPlan lbusOrgPlan = new busOrgPlan { icdoOrgPlan = new cdoOrgPlan() };
                lbusOrgPlan.icdoOrgPlan.LoadData(adrRow);

                lbusOrgPlan.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                lbusOrgPlan.ibusOrganization.icdoOrganization.LoadData(adrRow);

                lbusOrgPlan.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
                lbusOrgPlan.ibusPlan.icdoPlan.LoadData(adrRow);

                lclbOrgPlan.Add(lbusOrgPlan);
            }
            return lclbOrgPlan;
        }

        public static string GetEmploymentMemberStatusByPayrollMemberStatus(string astrPayrollMemberStatus)
        {
            string lstrResult = string.Empty;

            switch (astrPayrollMemberStatus)
            {
                case busConstant.EmployerPayrollDetailMemberStatusACTIVE:
                    lstrResult = busConstant.EmploymentStatusContributing;
                    break;
                case busConstant.EmployerPayrollDetailMemberStatusDeath:
                case busConstant.EmployerPayrollDetailMemberStatusTerminated:
                    lstrResult = busConstant.EmploymentStatusNonContributing;
                    break;
                case busConstant.EmployerPayrollDetailMemberStatusLOA:
                    lstrResult = busConstant.EmploymentStatusLOA;
                    break;
                case busConstant.EmploymentStatusLOAM:
                    lstrResult = busConstant.EmploymentStatusLOAM;
                    break;
            }
            return lstrResult;
        }
    }
}
