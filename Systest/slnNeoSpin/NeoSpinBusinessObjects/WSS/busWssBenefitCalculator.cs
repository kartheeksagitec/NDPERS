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
using System.Linq;

#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busWssBenefitCalculator:
    /// Inherited from busWssBenefitCalculatorGen, the class is used to customize the business object busWssBenefitCalculatorGen.
    /// </summary>
    [Serializable]
    public class busWssBenefitCalculator : busWssBenefitCalculatorGen
    {
        public decimal idecMonthlyNonTaxableAmount { get; set; }
        public bool iblnIsPersonEligibleForEarly { get; set; }
        public Collection<busDeductionCalculation> iclbDeductionCalculationWeb { get; set; }
        public decimal idecUnusedSickLeaveInMonths { get; set; }
        public DateTime ldtSpouseDateOfBirth { get; set; }

        public string benefit_tier_description_display { get; set; }

        public decimal idecTotalVestingServiceCredit { get; set; }
        //public Collection<busPersonTffrTiaaService> iclbTffrTiaaService
        //{ get;
        //  set;
        //}
        public void LoadMember(int aintPersonID)
        {
            if (ibusMember.IsNull())
                ibusMember = new busPerson();
            ibusMember.FindPerson(aintPersonID);
        }

        public void LoadPlan()
        {
            if (ibusPlan.IsNull())
                ibusPlan = new busPlan();
            ibusPlan.FindPlan(icdoWssBenefitCalculator.plan_id);
        }

         public busBenefitDroApplication ibusBenefitDroApplication { get; set; }
         public void LoadBenefitDroApplication()
        {
            if (ibusBenefitDroApplication == null)
                ibusBenefitDroApplication = new busBenefitDroApplication();
            ibusBenefitDroApplication.FindBenefitDroApplicationBypersonID(ibusMember.icdoPerson.person_id);
        }

         //public void LoadTffrTiaaService(int aintPersonID)
         //{
         //    iclbTffrTiaaService = null;
         //    DataTable ldtbList = Select<cdoPersonTffrTiaaService>(
         //        new string[1] { "person_id" },
         //        new object[1] { aintPersonID }, null, null);
         //    iclbTffrTiaaService = GetCollection<busPersonTffrTiaaService>(ldtbList, "icdoPersonTffrTiaaService");
         //}
         public bool IsStatusQualified()
         {
             bool lbnResult;
             LoadBenefitDroApplication();
             lbnResult = ibusBenefitDroApplication.IsStatusQualified();
             //if (ibusBenefitDroApplication.icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusQualified)
             //{
             //    if (iobjPassInfo.istrUserID != ibusBenefitDroApplication.icdoBenefitDroApplication.qualified_by_user)
             //    {
             //        return true;
             //    }
             //}
             return lbnResult;
         }
        public busOrganization ibusLatestEmployer { get; set; }
        //load latest Employer
        public void LoadLatestEmployer()
        {
            if (ibusLatestEmployer.IsNull())
                ibusLatestEmployer = new busOrganization();

            DateTime idtTermiantionDate = DateTime.MinValue;
            int lintOrgId = ibusRetirementBenefitCalculation.GetOrgIdAsLatestEmploymentOrgId(ibusRetirementBenefitCalculation.ibusPersonAccount.icdoPersonAccount.person_account_id,
                ibusRetirementBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value, ref idtTermiantionDate);

            ibusLatestEmployer.FindOrganization(lintOrgId);
            if (idtTermiantionDate != DateTime.MinValue)
                ibusRetirementBenefitCalculation.icdoBenefitCalculation.termination_date = idtTermiantionDate;
            busPersonAccount lbusPersonAccount = new busPersonAccount() { icdoPersonAccount = new cdoPersonAccount() };
            lbusPersonAccount.FindPersonAccount(ibusRetirementBenefitCalculation.ibusPersonAccount.icdoPersonAccount.person_account_id);
            lbusPersonAccount.ibusPersonAccountRetirement = new busPersonAccountRetirement();
            lbusPersonAccount.ibusPersonAccountRetirement.FindPersonAccountRetirement(lbusPersonAccount.icdoPersonAccount.person_account_id);
            benefit_tier_description_display = lbusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_description_display;
        }
        //summary
        public decimal idecEstimatedSCForVesting { get; set; }
        public decimal idecEstimatedSCYearlyForVesting { get; set; }
        public decimal idecEstimatedSCForBenCal { get; set; }
        public decimal idecEstimatedSCYearlyForBenCal { get; set; }

        public string idecEstimatedSCYearlyForBenCal_formatted
        {
            get
            {
                decimal ldecValue = ibusRetirementBenefitCalculation.icdoBenefitCalculation.estimated_credited_psc;
                if (ldecValue < 0)
                    return String.Format("{0} Years {1} Months", Math.Ceiling(ldecValue / 12).ToString(),
                                     Math.Round((ldecValue % 12), 4, MidpointRounding.AwayFromZero).ToString());

                return String.Format("{0} Years {1} Months", Math.Floor(ldecValue / 12).ToString(),
                                     Math.Round((ldecValue % 12), 4, MidpointRounding.AwayFromZero).ToString());
            }
        }
        
        public decimal idecLastSalary { get; set; }

        public decimal idecServiceAge { get; set; }

        public string idecServiceAge_formatted
        {
            get
            {
                decimal ldecValue = ((ibusRetirementBenefitCalculation.icdoBenefitCalculation.estimated_credited_psc / 12) + ibusRetirementBenefitCalculation.idecMemberAgeBasedOnRetirementDate) * 12;
                if (ldecValue < 0)
                    return String.Format("{0} Years {1} Months", Math.Ceiling(ldecValue / 12).ToString(), Math.Round((ldecValue % 12), 4, MidpointRounding.AwayFromZero).ToString());

                return String.Format("{0} Years {1} Months", Math.Floor(ldecValue / 12).ToString(), Math.Round((ldecValue % 12), 4, MidpointRounding.AwayFromZero).ToString());
            }
        }

        public void SetEstimatedServiceCredit()
        {
            idecEstimatedSCForVesting = ibusRetirementBenefitCalculation.icdoBenefitCalculation.credited_vsc
                                        + icdoWssBenefitCalculator.tffr_tiaa_service_credit
                                        + ibusConsolidatedServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.RoundedTotalTimeOfPurchaseExcludeFreeService
                                        + ibusUnusedServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.total_time_to_purchase;

            idecEstimatedSCYearlyForVesting = Math.Round((idecEstimatedSCForVesting / 12), 2, MidpointRounding.AwayFromZero);

            idecEstimatedSCForBenCal = ibusRetirementBenefitCalculation.icdoBenefitCalculation.credited_psc
                                         + ibusConsolidatedServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.RoundedTotalTimeOfPurchaseExcludeFreeService
                                        + ibusUnusedServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.total_time_to_purchase;

            idecEstimatedSCYearlyForBenCal = idecEstimatedSCForBenCal / 12;
                //Math.Round((idecEstimatedSCForBenCal / 12), 2, MidpointRounding.AwayFromZero);

            idecServiceAge = ibusRetirementBenefitCalculation.idecMemberAgeBasedOnRetirementDate + idecEstimatedSCYearlyForBenCal;
            //PIR - 2451                     
            //icdoWssBenefitCalculator.idecUnusedSickLeaveInMonths = busGlobalFunctions.c ibusUnusedServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.unused_sick_leave_hours
        }
      

        public void LoadLastSalary()
        {
            DataTable ldtbResult = busBase.Select("cdoBenefitCalculationFasMonths.LoadLastSalaryRecord",
                                                                 new object[3] { ibusRetirementBenefitCalculation.ibusPersonAccount.icdoPersonAccount.person_id,
                                                            ibusRetirementBenefitCalculation.ibusPersonAccount.icdoPersonAccount.plan_id,
                                                            0 });
            if (ldtbResult.Rows.Count > 0)
            {
                idecLastSalary = Convert.ToDecimal(ldtbResult.Rows[0]["SALARY_AMOUNT"]);
            }
        }

        public decimal idecEstimatedYearlyLastSalary
        {
            get
            {
                decimal ldecResult = 0M;
                ldecResult = idecLastSalary * 12;
                return ldecResult;
            }
        }

        public decimal idecProjectedYearlySalary
        {
            get
            {
                decimal ldecResult = 0M;
                ldecResult = idecEstimatedYearlyLastSalary * (ibusRetirementBenefitCalculation.icdoBenefitCalculation.percentage_salary_increase / 100);
                return ldecResult;
            }
        }

        public DateTime idtDateLevelIncomeEffective { get; set; }
        public void SetDateLevelIncomeEffective()
        {
            DateTime ldtReturnDate = DateTime.MinValue;
            DateTime ldtDateofBirth = ibusMember.icdoPerson.date_of_birth;
            int lintYearsToBeAdded = 0;
            int lintMonthsToBeAdded = 0;

            DataTable ldtbList = iobjPassInfo.isrvDBCache.GetCodeValues(1912, null, null, null);

            var lOverlappingYears = ldtbList.AsEnumerable().Where(lrow => (ldtDateofBirth.Year > (Convert.ToInt32(lrow.Field<String>("DATA1")))
                && (Convert.ToInt32(lrow.Field<String>("DATA2")) >= ldtDateofBirth.Year)));

            if (lOverlappingYears.Count() > 0)
            {
                string lstr = (lOverlappingYears.AsDataTable().Rows[0]["data3"]).ToString();

                string[] lstrarray = busGlobalFunctions.Split(lstr, ".");
                
                if(lstrarray[0].ToString().IsNotNullOrEmpty())
                lintYearsToBeAdded = Convert.ToInt32(lstrarray[0]) * 12;
                if (lstrarray.Count() == 2)
                {
                    if (lstrarray[1].ToString().IsNotNullOrEmpty())
                        lintMonthsToBeAdded = Convert.ToInt32(Convert.ToDecimal(lstrarray[1]) * 12);
                }
            }

            idtDateLevelIncomeEffective = ldtDateofBirth.AddMonths(lintYearsToBeAdded + lintMonthsToBeAdded);
        }

        public void LoadDeductionWeb()
        {
            if (iclbDeductionCalculationWeb.IsNull())
                iclbDeductionCalculationWeb = new Collection<busDeductionCalculation>();

            if (ibusRetirementBenefitCalculation.iclbBenefitCalculationOptions.IsNull())
                ibusRetirementBenefitCalculation.LoadBenefitCalculationOptions();

            iclbDeductionCalculationWeb = new Collection<busDeductionCalculation>();

            foreach (busBenefitCalculationOptions lobjBenefitOptions in ibusRetirementBenefitCalculation.iclbBenefitCalculationOptions)
            {
                busDeductionCalculation lobjNewDeductionCalculations = new busDeductionCalculation { icdoBenefitCalculation = new cdoBenefitCalculation() };
                lobjNewDeductionCalculations.icdoBenefitCalculation = ibusRetirementBenefitCalculation.icdoBenefitCalculation;
                lobjNewDeductionCalculations.ibusBenefitCalculationOptions = lobjBenefitOptions;
                lobjNewDeductionCalculations.ibusPerson = ibusMember;
                lobjNewDeductionCalculations.LoadBenefitDeductionSummary();
                lobjNewDeductionCalculations.LoadBenefitDentalDeduction();
                lobjNewDeductionCalculations.LoadBenefitHealthDeduction();
                lobjNewDeductionCalculations.LoadBenefitVisionDeduction();
                lobjNewDeductionCalculations.LoadBenefitLifeDeductions();
                //if(icdoWssBenefitCalculator.skip_Deductions_flag.Equals("Y"))
                //lobjNewDeductionCalculations.idecTotalLifePremium = 0;

                //ltc member
                lobjNewDeductionCalculations.LoadBenefitLtcMemberDeductions();
                lobjNewDeductionCalculations.LoadBenefitLtcSpouseDeductions();
                lobjNewDeductionCalculations.LoadBenefitPayeeFedTaxWithholding();
                lobjNewDeductionCalculations.LoadBenefitPayeeStateTaxWithholding();

                lobjNewDeductionCalculations.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.gross_monthly_benefit_amount =
                    lobjBenefitOptions.icdoBenefitCalculationOptions.benefit_option_amount;
                //lobjNewDeductionCalculations.ibusBenefitCalculationOptions.icdoBenefitCalculationOptions.taxable_amount =
                //    lobjBenefitOptions.icdoBenefitCalculationOptions.benefit_option_amount;
                if (lobjNewDeductionCalculations.ibusBenefitCalculationOptions.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value == busConstant.BenefitOptionRefund || 
                    lobjNewDeductionCalculations.ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_option_value == busConstant.TaxOptionFedTaxwithheld)
                    lobjNewDeductionCalculations.ProcessFedTax();
                if (lobjNewDeductionCalculations.ibusBenefitCalculationOptions.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value == busConstant.BenefitOptionRefund ||
                    lobjNewDeductionCalculations.ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_option_value == busConstant.TaxOptionStateTaxwithheld)
                    lobjNewDeductionCalculations.ProcessStateTax();
                
                lobjBenefitOptions.idecTotalDeductions = lobjNewDeductionCalculations.ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.computed_tax_amount +
                            lobjNewDeductionCalculations.ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.computed_tax_amount +
                            ((lobjNewDeductionCalculations.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.net_health_insurance_premium_amount > 0) ?
                            lobjNewDeductionCalculations.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.net_health_insurance_premium_amount : 0) +
                            lobjNewDeductionCalculations.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.vision_overridden_amount +
                            lobjNewDeductionCalculations.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.dental_overridden_amount +
                            lobjNewDeductionCalculations.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.life_overridden_amount +
                            lobjNewDeductionCalculations.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ltc_overridden_amount +
                            lobjNewDeductionCalculations.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.miscellaneous_deduction_amount;

                lobjBenefitOptions.iblnIsPersonEligibleForEarly = ibusRetirementBenefitCalculation.IsPersonEligibleForEarly();
                lobjBenefitOptions.idecBenefitAmountAfterDeductions = lobjBenefitOptions.icdoBenefitCalculationOptions.benefit_option_amount - lobjBenefitOptions.idecTotalDeductions;

                iclbDeductionCalculationWeb.Add(lobjNewDeductionCalculations);
            }
        }

        // PROD PIR 6895
        public bool IsDCMember()
        {
            if (ibusMember.IsNotNull())
            {
                if (ibusMember.IsDCPersonAccountExists())
                    return true;
            }
            return false;
        }

        public void LoadBenefitMulitiplier()
        {
            if (ibusRetirementBenefitCalculation.IsNull()) 
                LoadRetirementBenefitCalculation();
            if (ibusRetirementBenefitCalculation.iclbBenefitMultiplier.IsNull()) 
                ibusRetirementBenefitCalculation.LoadBenefitMultiplier();
            foreach (busBenefitMultiplier lobjMultiplier in ibusRetirementBenefitCalculation.iclbBenefitMultiplier)
                lobjMultiplier.icdoBenefitMultiplier.final_average_salary = ibusRetirementBenefitCalculation.icdoBenefitCalculation.calculation_final_average_salary; // PIR 9428
        }

        // PIR 9894
        public bool IsDCTextVisible()
        {
            if (icdoWssBenefitCalculator.plan_id == busConstant.PlanIdDC ||
               icdoWssBenefitCalculator.plan_id == busConstant.PlanIdDC2020 ||//PIR 20232
                icdoWssBenefitCalculator.plan_id == busConstant.PlanIdDC2025) //PIR 25920
                return true;
            if (icdoWssBenefitCalculator.plan_id != busConstant.PlanIdJudges &&
                icdoWssBenefitCalculator.plan_id != busConstant.PlanIdHP)
            {
                return IsDCMember();
            }
            return false;
        }

        public string idecEstimatedSCYearlyForVesting_formatted
        {
            get
            {
                decimal ldecValue = idecEstimatedSCForVesting;
                if (ldecValue < 0)
                    return String.Format("{0} Years {1} Months", Math.Ceiling(ldecValue / 12).ToString(),
                                     Math.Round((ldecValue % 12), 4, MidpointRounding.AwayFromZero).ToString());

                return String.Format("{0} Years {1} Months", Math.Floor(ldecValue / 12).ToString(),
                                     Math.Round((ldecValue % 12), 4, MidpointRounding.AwayFromZero).ToString());
            }
        }
    }
}
