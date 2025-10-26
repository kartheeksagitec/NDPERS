#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
    public class cdoPayeeAccountTaxWithholding : doPayeeAccountTaxWithholding
    {
        public cdoPayeeAccountTaxWithholding()
            : base()
        {
        }
        //as we are having different drop down list for benefit distribution type monthly benfit,plso and refund
        //we need to have the below properties
        private string _monthly_benefit_tax_option;

        public string monthly_benefit_tax_option
        {
            get { return _monthly_benefit_tax_option; }
            set { _monthly_benefit_tax_option = value; }
        }
        //Used for   posting person id in audit log
        public int person_id { get; set; }

        private string _plso_tax_option;

        public string plso_tax_option
        {
            get { return _plso_tax_option; }
            set { _plso_tax_option = value; }
        }
        private string _refund_tax_option;

        public string refund_tax_option
        {
            get { return _refund_tax_option; }
            set { _refund_tax_option = value; }
        }
        //This string property is used for tax allowance field as it needs to accept '0' as a valuable number
        private string _no_of_tax_allowance;

        public string no_of_tax_allowance
        {
            get { return _no_of_tax_allowance; }
            set { _no_of_tax_allowance = value; }
        }
        private string _monthlybenefit_suppress_warning;

        public string monthlybenefit_suppress_warning
        {
            get { return _monthlybenefit_suppress_warning; }
            set { _monthlybenefit_suppress_warning = value; }
        }
        private string _plso_supress_warning;

        public string plso_supress_warning
        {
            get { return _plso_supress_warning; }
            set { _plso_supress_warning = value; }
        }
        private string _refund_supress_warning;
        public string refund_supress_warning
        {
            get { return _refund_supress_warning; }
            set { _refund_supress_warning = value; }
        }

        public int no_of_periods { get; set; }
        public decimal tax_payer_deduction_amt { get; set; }
        public decimal annual_benefit_amt { get; set; }

        public decimal net_benefit_amt { get; set; }

        public decimal net_deduction_amt { get; set; }
        public decimal post_ded_benefit_amt { get; set; }
        public decimal adj_annual_benefit_amt { get; set; }
        public DateTime idteTaxCalcultionEffectiveDate { get; set; }

        public decimal total_income_and_pension_amt { get; set; }
        public decimal step2c_computed_net_amt { get; set; }
        public decimal adjusted_taxable_income_tax_rate_amount { get; set; }
        public decimal exceeding_tax_amt { get; set; }
        public decimal tentative_tax_amt { get; set; }
        public decimal tax_percentage { get; set; }
        public decimal calc_taxable_amt { get; set; }
        public decimal calc_taxable_perc_amt { get; set; }
        public decimal tentative_annual_withhold_amt { get; set; }
        public decimal final_tentative_annual_withhold_amt { get; set; }
        public decimal part2_total_income_and_pension_amt { get; set; }
        public decimal step2k_computed_partii_amt { get; set; }
        public decimal part_2_exceeding_tax_amt { get; set; }
        public decimal part_2_tentative_tax_amt { get; set; }
        public decimal part_2_tax_percentage { get; set; }
        public decimal part_2_calc_taxable_amt { get; set; }
        public decimal part_2_calc_taxable_perc_amt { get; set; }

        public decimal part_2_tentative_annual_withhold_amt { get; set; }
        public decimal part_2_comp_tentative_annual_withhold_amt { get; set; }
        public decimal taxable_credit_amt { get; set; }
        public decimal calc_withhold_amt { get; set; }
        public decimal fed_tax_amt { get; set; }
        public decimal calc_allowance_amt { get; set; }
        public decimal post_allowance_benefit_amt { get; set; }
        public decimal monthly_taxable_amt { get; set; }
        public string three_first_line { get; set; }
        public string three_second_line { get; set; }
        public string three_third_line { get; set; }
        public string two_tip { get; set; }
        public decimal additional_withholding_Amt
        {
            get
            {
                if (string.IsNullOrEmpty(this.tax_ref))
                    return additional_tax_amount;
                else
                    return four_c; //captured from 2022 W4P
            }
        }

    }
}