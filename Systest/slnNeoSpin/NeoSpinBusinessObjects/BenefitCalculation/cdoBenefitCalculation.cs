#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;
using NeoSpin.BusinessObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
    public class cdoBenefitCalculation : doBenefitCalculation
    {
        public cdoBenefitCalculation() : base()
        {
        }

        private int _annuitant_id;
        public int annuitant_id
        {
            get { return _annuitant_id; }
            set { _annuitant_id = value; }
        }

        private decimal _annuitant_age;
        public decimal annuitant_age
        {
            get { return _annuitant_age; }
            set { _annuitant_age = value; }
        }

        private decimal _calculation_final_average_salary;
        public decimal calculation_final_average_salary
        {
            get { return _calculation_final_average_salary; }
            set { _calculation_final_average_salary = value; }
        }

        public decimal consolidated_psc_in_years
        {
            get
            {
                return credited_psc / 12;
            }
        }

        public decimal consolidated_tvsc_in_years
        {
            get
            {
                return credited_vsc / 12;
            }
        }

        private decimal _reduced_monthly_after_plso_deduction;
        public decimal reduced_monthly_after_plso_deduction
        {
            get { return _reduced_monthly_after_plso_deduction; }
            set { _reduced_monthly_after_plso_deduction = value; }
        }

        private decimal _reduced_monthly_after_dnro_early;
        public decimal reduced_monthly_after_dnro_early
        {
            get { return _reduced_monthly_after_dnro_early; }
            set { _reduced_monthly_after_dnro_early = value; }
        }

        private decimal _standard_rhic_amount;
        public decimal standard_rhic_amount
        {
            get { return _standard_rhic_amount; }
            set { _standard_rhic_amount = value; }
        }

        public DateTime spouse_date_of_birth { get; set; }

        #region UCS-080

        public decimal disability_gross_benefit_amount { get; set; }

        public string disability_to_normal 
        {
            get
            {
                if ((disability_payee_account_id != 0) &&
                    (is_rule_or_age_conversion != 0))
                    return busConstant.Flag_Yes;
                return busConstant.Flag_No;
            }
        }

        #endregion

        public bool is_return_to_work_member
        {
            get
            {
                if (pre_rtw_payee_account_id != 0)
                    return true;
                return false;
            }
        }

        public string is_member_vested { get; set; }

        // As per Satya, exclusion ratio will always be Zero.
        public decimal exclusion_ratio { get; set; }

        public string IsNonTaxableAmountExists
        {
            get
            {
                return non_taxable_amount > 0M ? busConstant.Flag_Yes : busConstant.Flag_No;
            }
        }

        # region Correspondence related properties
        private decimal _idecSingleLifeAmount;
        public decimal idecSingleLifeAmount
        {
            get { return _idecSingleLifeAmount; }
            set { _idecSingleLifeAmount = value; }
        }

        private decimal _idec50PercentJSAmount;
        public decimal idec50PercentJSAmount
        {
            get { return _idec50PercentJSAmount; }
            set { _idec50PercentJSAmount = value; }
        }

        private decimal _idec100PercentJSAmount;
        public decimal idec100PercentJSAmount
        {
            get { return _idec100PercentJSAmount; }
            set { _idec100PercentJSAmount = value; }
        }

        private decimal _idec10YrCertainAmount;
        public decimal idec10YrCertainAmount
        {
            get { return _idec10YrCertainAmount; }
            set { _idec10YrCertainAmount = value; }
        }

        private decimal _idec20YrCertainAmount;
        public decimal idec20YrCertainAmount
        {
            get { return _idec20YrCertainAmount; }
            set { _idec20YrCertainAmount = value; }
        }

        private decimal _idec15YrCertainAmount;
        public decimal idec15YrCertainAmount
        {
            get { return _idec15YrCertainAmount; }
            set { _idec15YrCertainAmount = value; }
        }

        private decimal _idec5YrCertainAmount;
        public decimal idec5YrCertainAmount
        {
            get { return _idec5YrCertainAmount; }
            set { _idec5YrCertainAmount = value; }
        }

        private decimal _idec75PercentJSAmount;
        public decimal idec75PercentJSAmount
        {
            get { return _idec75PercentJSAmount; }
            set { _idec75PercentJSAmount = value; }
        }

        private decimal _idec55PercentJSAmount;
        public decimal idec55PercentJSAmount
        {
            get { return _idec55PercentJSAmount; }
            set { _idec55PercentJSAmount = value; }
        }

        //after SSLI
        private decimal _idecSingleLifeAmountAfterSSLI;
        public decimal idecSingleLifeAmountAfterSSLI
        {
            get { return _idecSingleLifeAmountAfterSSLI; }
            set { _idecSingleLifeAmountAfterSSLI = value; }
        }

        private decimal _idecSingleLifeAmountBeforeSSLI;
        public decimal idecSingleLifeAmountBeforeSSLI
        {
            get { return _idecSingleLifeAmountBeforeSSLI; }
            set { _idecSingleLifeAmountBeforeSSLI = value; }
        }

        private decimal _idec50PercentJSAmountAfterSSLI;
        public decimal idec50PercentJSAmountAfterSSLI
        {
            get { return _idec50PercentJSAmountAfterSSLI; }
            set { _idec50PercentJSAmountAfterSSLI = value; }
        }

        private decimal _idec100PercentJSAmountAfterSSLI;
        public decimal idec100PercentJSAmountAfterSSLI
        {
            get { return _idec100PercentJSAmountAfterSSLI; }
            set { _idec100PercentJSAmountAfterSSLI = value; }
        }

        private decimal _idec10YrCertainAmountAfterSSLI;
        public decimal idec10YrCertainAmountAfterSSLI
        {
            get { return _idec10YrCertainAmountAfterSSLI; }
            set { _idec10YrCertainAmountAfterSSLI = value; }
        }

        private decimal _idec20YrCertainAmountAfterSSLI;
        public decimal idec20YrCertainAmountAfterSSLI
        {
            get { return _idec20YrCertainAmountAfterSSLI; }
            set { _idec20YrCertainAmountAfterSSLI = value; }
        }

        private decimal _idec15YrCertainAmountAfterSSLI;
        public decimal idec15YrCertainAmountAfterSSLI
        {
            get { return _idec15YrCertainAmountAfterSSLI; }
            set { _idec15YrCertainAmountAfterSSLI = value; }
        }

        private decimal _idec5YrCertainAmountAfterSSLI;
        public decimal idec5YrCertainAmountAfterSSLI
        {
            get { return _idec5YrCertainAmountAfterSSLI; }
            set { _idec5YrCertainAmountAfterSSLI = value; }
        }

        private decimal _idec75PercentJSAmountAfterSSLI;
        public decimal idec75PercentJSAmountAfterSSLI
        {
            get { return _idec75PercentJSAmountAfterSSLI; }
            set { _idec75PercentJSAmountAfterSSLI = value; }
        }

        private decimal _idec55PercentJSAmountAfterSSLI;
        public decimal idec55PercentJSAmountAfterSSLI
        {
            get { return _idec55PercentJSAmountAfterSSLI; }
            set { _idec55PercentJSAmountAfterSSLI = value; }
        }

        private decimal _idecNormalRetBenefitAmtAfterSSLI;
        public decimal idecNormalRetBenefitAmtAfterSSLI
        {
            get { return _idecNormalRetBenefitAmtAfterSSLI; }
            set { _idecNormalRetBenefitAmtAfterSSLI = value; }
        }

        //these properties are used in correspondence
        private decimal _idecRHICOptionStandardAmt;
        public decimal idecRHICOptionStandardAmt
        {
            get { return _idecRHICOptionStandardAmt; }
            set { _idecRHICOptionStandardAmt = value; }
        }

        private decimal _idecRHICOption50PercentAmt;
        public decimal idecRHICOption50PercentAmt
        {
            get { return _idecRHICOption50PercentAmt; }
            set { _idecRHICOption50PercentAmt = value; }
        }

        private decimal _idecRHICOption100PercentAmt;
        public decimal idecRHICOption100PercentAmt
        {
            get { return _idecRHICOption100PercentAmt; }
            set { _idecRHICOption100PercentAmt = value; }
        }

        private decimal _idecNormalRetBenefitAmt;
        public decimal idecNormalRetBenefitAmt
        {
            get { return _idecNormalRetBenefitAmt; }
            set { _idecNormalRetBenefitAmt = value; }
        }

        // Normal Anniuty for benefit option

        private decimal _idec10YrCertainBenefitOptionAmt;
        public decimal idec10YrCertainBenefitOptionAmt
        {
            get { return _idec10YrCertainBenefitOptionAmt; }
            set { _idec10YrCertainBenefitOptionAmt = value; }
        }

        private decimal _idec20YrCertainBenefitOptionAmt;
        public decimal idec20YrCertainBenefitOptionAmt
        {
            get { return _idec20YrCertainBenefitOptionAmt; }
            set { _idec20YrCertainBenefitOptionAmt = value; }
        }
        private decimal _idec15YrCertainBenefitOptionAmt;
        public decimal idec15YrCertainBenefitOptionAmt
        {
            get { return _idec15YrCertainBenefitOptionAmt; }
            set { _idec15YrCertainBenefitOptionAmt = value; }
        }
        private decimal _idecStraightLifeBenefitOptionAmt;
        public decimal idecStraightLifeBenefitOptionAmt
        {
            get { return _idecStraightLifeBenefitOptionAmt; }
            set { _idecStraightLifeBenefitOptionAmt = value; }
        }
        private decimal _idec55PercentJSBenefitOptionAmt;
        public decimal idec55PercentJSBenefitOptionAmt
        {
            get { return _idec55PercentJSBenefitOptionAmt; }
            set { _idec55PercentJSBenefitOptionAmt = value; }
        }
        private decimal _idec75PercentJSBenefitOptionAmt;
        public decimal idec75PercentJSBenefitOptionAmt
        {
            get { return _idec75PercentJSBenefitOptionAmt; }
            set { _idec75PercentJSBenefitOptionAmt = value; }
        }
        private decimal _idec100PercentJSBenefitOptionAmt;
        public decimal idec100PercentJSBenefitOptionAmt
        {
            get { return _idec100PercentJSBenefitOptionAmt; }
            set { _idec100PercentJSBenefitOptionAmt = value; }
        }

        // with PLSO Amount
        private decimal _idecSingleLifeWithPLSOAmt;
        public decimal idecSingleLifeWithPLSOAmt
        {
            get { return _idecSingleLifeWithPLSOAmt; }
            set { _idecSingleLifeWithPLSOAmt = value; }
        }

        private decimal _idec10YrCertainWithPLSOAmt;
        public decimal idec10YrCertainWithPLSOAmt
        {
            get { return _idec10YrCertainWithPLSOAmt; }
            set { _idec10YrCertainWithPLSOAmt = value; }
        }

        private decimal _idec20YrCertainWithPLSOAmt;
        public decimal idec20YrCertainWithPLSOAmt
        {
            get { return _idec20YrCertainWithPLSOAmt; }
            set { _idec20YrCertainWithPLSOAmt = value; }
        }

        private decimal _idecNormalBenefitWithPLSOAmt;
        public decimal idecNormalBenefitWithPLSOAmt
        {
            get { return _idecNormalBenefitWithPLSOAmt; }
            set { _idecNormalBenefitWithPLSOAmt = value; }
        }

        private decimal _idec50PercentJSWithPLSOAmt;
        public decimal idec50PercentJSWithPLSOAmt
        {
            get { return _idec50PercentJSWithPLSOAmt; }
            set { _idec50PercentJSWithPLSOAmt = value; }
        }

        private decimal _idec100PercentJSWithPLSOAmt;
        public decimal idec100PercentJSWithPLSOAmt
        {
            get { return _idec100PercentJSWithPLSOAmt; }
            set { _idec100PercentJSWithPLSOAmt = value; }
        }
        # endregion

        #region Calculation Details - Benefit Information

        private decimal _early_retirement_percentage_decrease;
        public decimal early_retirement_percentage_decrease
        {
            get { return _early_retirement_percentage_decrease; }
            set { _early_retirement_percentage_decrease = value; }
        }

        private decimal _dnro_percentage_increase;
        public decimal dnro_percentage_increase
        {
            get { return _dnro_percentage_increase; }
            set { _dnro_percentage_increase = value; }
        }

        private decimal _member_account_balance;
        public decimal member_account_balance
        {
            get { return _member_account_balance; }
            set { _member_account_balance = value; }
        }
		//used for Correspondence
        public decimal total_member_account_balance
        {
            get
            {
                return taxable_amount + non_taxable_amount;
            }
        }
        
        #endregion

        public string calculation_identifier
        {
            get
            {
                if ((benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement) &&
                    (calculation_type_value == busConstant.CalculationTypeFinal || calculation_type_value == busConstant.CalculationTypeAdjustments
                    || calculation_type_value == busConstant.CalculationTypeSubsequent || calculation_type_value == busConstant.CalculationTypeSubsequentAdjustment))
                    return busConstant.RetirementFinal;
                if ((benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement) && 
                    (calculation_type_value == busConstant.CalculationTypeEstimate || calculation_type_value == busConstant.CalculationTypeEstimateSubsequent)) //PIR 19594
                    return busConstant.RetirementEstimate;
                if((benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund) && 
                   (calculation_type_value == busConstant.CalculationTypeFinal))
                    return busConstant.RefundFinal;
                if ((benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund) && 
                    (calculation_type_value == busConstant.CalculationTypeAdjustments))
                    return busConstant.RefundAdjustment;
                if ((benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath) &&
                    (calculation_type_value == busConstant.CalculationTypeFinal || calculation_type_value == busConstant.CalculationTypeAdjustments))
                    return busConstant.PreRetirementDeathFinal;
                if ((benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath) &&
                    (calculation_type_value == busConstant.CalculationTypeEstimate))
                    return busConstant.PreRetirementDeathEstimate;
                if ((benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability) &&
                    (calculation_type_value == busConstant.CalculationTypeEstimate))
                    return busConstant.RetirementEstimate;
                if ((benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability) &&
                    (calculation_type_value == busConstant.CalculationTypeFinal || calculation_type_value == busConstant.CalculationTypeAdjustments))
                    return busConstant.RetirementFinal;
                if (benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath)
                    return busConstant.PostRetirementDeathFinal;
                return string.Empty;
            }
        }

        public string application_identifier
        {
            get
            {
                if (benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
                    return busConstant.RetirementDisability;
                if (benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                    return busConstant.RetirementDisability;
                if (benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
                    return busConstant.RetirementDeath;
                if (benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund)
                    return busConstant.Refund;
                if (benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath)
                    return busConstant.PostRetirementDeathFinal;
                return string.Empty;
            }
        }

        public string retirement_long_date
        {
            get
            {
                return retirement_date.ToString(busConstant.DateFormatLongDate);
            }
        }

        public override bool AuditColumn(string astrOperation, string astrColumnName)
        {
            if ((astrColumnName == "overridden_final_average_salary") ||
                (astrColumnName == "reduced_benefit_option_amount"))
            {
                if (calculation_type_value == BusinessObjects.busConstant.CalculationTypeFinal || calculation_type_value == busConstant.CalculationTypeSubsequent)
                    return true;
                return false;
            }
            if ((astrColumnName == "paid_up_annuity_amount") ||
                (astrColumnName == "js_residual_mg_amount"))
            {
                if (benefit_account_type_value == BusinessObjects.busConstant.ApplicationBenefitTypePreRetirementDeath)
                    return true;
                return false;
            }
            return base.AuditColumn(astrOperation, astrColumnName);
        }

        // Correspondence Properties
        public string normal_retirement_long_date
        {
            get
            {
                return normal_retirement_date.ToString(busConstant.DateFormatLongDate);
            }
        }

        public string calculated_long_date
        {
            get
            {
                return created_date.ToString(busConstant.DateFormatMonthYear);
            }
        }

        public string termination_long_date
        {
            get
            {
                return termination_date.ToString(busConstant.DateFormatLongDate);
            }
        }

        public decimal idec55PercentJSAmountForSpouse { get; set; }

        public decimal idec55PercentJSAmountAfterSSLIForSpouse { get; set; }

        public decimal idec55PercentJSBenefitOptionAmtForSpouse { get; set; }

        public decimal idec75PercentJSAmountForSpouse { get; set; }

        public decimal idec75PercentJSAmountAfterSSLIForSpouse { get; set; }

        public decimal idec75PercentJSBenefitOptionAmtForSpouse { get; set; }

        public decimal idec100PercentJSAmountForSpouse { get; set; }

        public decimal idec100PercentJSAmountAfterSSLIForSpouse { get; set; }

        public decimal idec100PercentJSBenefitOptionAmtForSpouse { get; set; }

       // public decimal idec100PercentJSWithPLSOAmtForSpouse { get; set; }

        public string salary_month_increase_month
        {
            get
            {
                if (salary_month_increase > 0)
                {
                    DateTime ldteTempDate = new DateTime(DateTime.Now.Year, salary_month_increase, 01);
                    return ldteTempDate.ToString(busConstant.DateFormatMonth);
                }
                return string.Empty;
            }
        }

        // This field is shown in MSS Benefit Summary. To display without service purchase and projections.s
        public decimal credited_psc_from_file { get; set; }

        public decimal estimated_credited_psc { get; set; }

        //SP PIR-1054 FAS
        public bool iblnUseMainPlanFasFactors = false;

        //PIR 18053
        public bool is_rtw_member_subsequent_retirement
        {
            get
            {
                if (pre_rtw_payee_account_id != 0 && (calculation_type_value == busConstant.CalculationTypeSubsequent || calculation_type_value == busConstant.CalculationTypeSubsequentAdjustment)
                    && retirement_date >= busConstant.RTWEffectiveDate)
                    return true;
                return false;
            }
        }

        public bool is_rtw_member_recalculate;

        //PIR 23975
        public decimal idecCredited_Vsc_From_File { get; set; }
        public decimal idecCredited_Vsc_From_File_For_OtherPlans { get; set; }
        
    }
}