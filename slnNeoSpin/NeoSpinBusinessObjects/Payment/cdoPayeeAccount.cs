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
	public class cdoPayeeAccount : doPayeeAccount
	{
		public cdoPayeeAccount() : base()
		{
		}

        public decimal TotalMinimumGuarantee
        {
            get
            {
                return minimum_guarantee_amount
                    + nontaxable_beginning_balance;
            }
        }

        #region Tax Rate Change Batch

        private decimal _current_federal_tax;
        public decimal current_federal_tax
        {
            get { return _current_federal_tax; }
            set { _current_federal_tax = value; }
        }

        private decimal _current_state_tax;
        public decimal current_state_tax
        {
            get { return _current_state_tax; }
            set { _current_state_tax = value; }
        }

        private decimal _last_federal_tax;
        public decimal last_federal_tax
        {
            get { return _last_federal_tax; }
            set { _last_federal_tax = value; }
        }

        private decimal _last_state_tax;
        public decimal last_state_tax
        {
            get { return _last_state_tax; }
            set { _last_state_tax = value; }
        }

        public decimal federal_difference
        {
            get { return _current_federal_tax - _last_federal_tax; }
        }

        public decimal state_difference
        {
            get { return _current_state_tax - _last_state_tax; }
        }

        private bool _exception_report;
        public bool exception_report
        {
            get { return _exception_report; }
            set { _exception_report = value; }
        }

        private string _fed_tax_error_message;
        public string fed_tax_error_message
        {
            get { return _fed_tax_error_message; }
            set { _fed_tax_error_message = value; }
        }
        public string state_tax_error_message { get; set; }

        public string fed_tax_type_of_error { get; set; }

        public string state_tax_type_of_error { get; set; }
        
        #endregion

        public string Current_Month
        {
            get
            {
                return DateTime.Now.ToString("MMMM");
            }
        }

        public string Next_Month
        {
            get
            {
                return DateTime.Now.AddMonths(1).ToString("MMMM");
            }
        }

        public bool IsTermCertainBenefitOption()
        {
            if ((benefit_option_value == "L05C") ||
                (benefit_option_value == "L10C") ||
                (benefit_option_value == "L20C") ||
                (benefit_option_value == "LA10") ||
                (benefit_option_value == "LA15") ||
                (benefit_option_value == "LA20") ||
                (benefit_option_value == "LB05") ||
                (benefit_option_value == "LB10") ||
                (benefit_option_value == "LB15") ||
                (benefit_option_value == "LB20") ||
                (benefit_option_value == "T10C") ||
                (benefit_option_value == "T15C") ||
                (benefit_option_value == "T20C") ||
                (benefit_option_value == "5YTL"))
                return true;                
            return false;
        }

		public string istrBenefitBegin_Formatted
        {
            get
            {
                return benefit_begin_date.ToString("MMMM") + " " + benefit_begin_date.Year.ToString();
            }
        }

        public string istrBenefitEnd_Formatted
        {
            get
            {
                return benefit_end_date.ToString("MMMM") + " " + benefit_end_date.Year.ToString();
            }
        }

        public string benefit_begin_long_date
        {
            get
            {
                return benefit_begin_date.ToString(BusinessObjects.busConstant.DateFormatLongDate);
            }
        }

        public string term_certain_long_end_date
        {
            get
            {
                return term_certain_end_date.ToString(BusinessObjects.busConstant.DateFormatLongDate);
            }
        }

        public DateTime disa_normal_effective_date_no_null
        {
            get
            {
                if (disa_normal_effective_date == DateTime.MinValue)
                    return DateTime.Today;
                else
                    return disa_normal_effective_date;
            }
        }

        //PIR 18503 - Wizard changes
        public string istrDisplayPayeeAccounts { get; set; }
        public string istrPlanName { get; set; }
        public string istrBenefitAmount { get; set; }

        
    }
} 
