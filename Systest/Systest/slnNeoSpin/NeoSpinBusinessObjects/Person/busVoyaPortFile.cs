using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.CustomDataObjects;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using NeoSpin.DataObjects;

namespace NeoSpin.BusinessObjects
{
    public class busVoyaPortFile
    {
        private string _ssn;
        public string ssn
        {
            get { return _ssn; }
            set { _ssn = value; }
        }

        private string _first_name;
        public string first_name
        {
            get { return _first_name; }
            set { _first_name = value; }
        }

        private string _middle_name;
        public string middle_name
        {
            get { return _middle_name; }
            set { _middle_name = value; }
        }

        private string _last_name;
        public string last_name
        {
            get { return _last_name; }
            set { _last_name = value; }
        }

        private DateTime _date_of_birth;
        public DateTime date_of_birth
        {
            get { return _date_of_birth; }
            set { _date_of_birth = value; }
        }

        private string _address_line_1;
        public string address_line_1
        {
            get { return _address_line_1; }
            set { _address_line_1 = value; }
        }

        private string _address_line_2;
        public string address_line_2
        {
            get { return _address_line_2; }
            set { _address_line_2 = value; }
        }

        private string _city;
        public string city
        {
            get { return _city; }
            set { _city = value; }
        }

        private string _state;
        public string state
        {
            get { return _state; }
            set { _state = value; }
        }

        private string _zip_code;
        public string zip_code
        {
            get { return _zip_code; }
            set { _zip_code = value; }
        }

        private string _zip_code_4;
        public string zip_code_4
        {
            get { return _zip_code_4; }
            set { _zip_code_4 = value; }
        }

        private DateTime _date_of_hire;
        public DateTime date_of_hire
        {
            get { return _date_of_hire; }
            set { _date_of_hire = value; }
        }

        private string _annual_salary_amount;
        public string annual_salary_amount
        {
            get { return _annual_salary_amount; }
            set { _annual_salary_amount = value; }
        }

        private string _group_name;
        public string group_name
        {
            get { return _group_name; }
            set { _group_name = value; }
        }

        private int _group_number;
        public int group_number
        {
            get { return _group_number; }
            set { _group_number = value; }
        }

        private int _account_number;
        public int account_number
        {
            get { return _account_number; }
            set { _account_number = value; }
        }

        private string _ee_class;
        public string ee_class
        {
            get { return _ee_class; }
            set { _ee_class = value; }
        }

        private string _tobacco_status;
        public string tobacco_status
        {
            get { return _tobacco_status; }
            set { _tobacco_status = value; }
        }

        private string _home_phone;
        public string home_phone
        {
            get { return _home_phone; }
            set { _home_phone = value; }
        }

        private DateTime _employment_termination_date;
        public DateTime employment_termination_date
        {
            get { return _employment_termination_date; }
            set { _employment_termination_date = value; }
        }

        private DateTime _insurance_termination_date;
        public DateTime insurance_termination_date
        {
            get { return _insurance_termination_date; }
            set { _insurance_termination_date = value; }
        }

        private string _additional_comment;
        public string additional_comment
        {
            get { return _additional_comment; }
            set { _additional_comment = value; }
        }

        private string _reason_for_termination;
        public string reason_for_termination
        {
            get { return _reason_for_termination; }
            set { _reason_for_termination = value; }
        }

        private decimal _employee_basic_coverage_amount;
        public decimal employee_basic_coverage_amount
        {
            get { return _employee_basic_coverage_amount; }
            set { _employee_basic_coverage_amount = value; }
        }

        private decimal _employee_supplemental_coverage_amount;
        public decimal employee_supplemental_coverage_amount
        {
            get { return _employee_supplemental_coverage_amount; }
            set { _employee_supplemental_coverage_amount = value; }
        }

        private decimal _employee_basic_ad_d_coverage_amount;
        public decimal employee_basic_ad_d_coverage_amount
        {
            get { return _employee_basic_ad_d_coverage_amount; }
            set { _employee_basic_ad_d_coverage_amount = value; }
        }

        private decimal _employee_supplemental_ad_d_coverage_amount;
        public decimal employee_supplemental_ad_d_coverage_amount
        {
            get { return _employee_supplemental_ad_d_coverage_amount; }
            set { _employee_supplemental_ad_d_coverage_amount = value; }
        }

        private string _additional_coverage_amount_ee;
        public string additional_coverage_amount_ee
        {
            get { return _additional_coverage_amount_ee; }
            set { _additional_coverage_amount_ee = value; }
        }

        private string _additional_coverage_amount_fam;
        public string additional_coverage_amount_fam
        {
            get { return _additional_coverage_amount_fam; }
            set { _additional_coverage_amount_fam = value; }
        }

        private string _sp_last_name;
        public string sp_last_name
        {
            get { return _sp_last_name; }
            set { _sp_last_name = value; }
        }

        private string _sp_first_name;
        public string sp_first_name
        {
            get { return _sp_first_name; }
            set { _sp_first_name = value; }
        }

        private string _sp_ssn;
        public string sp_ssn
        {
            get { return _sp_ssn; }
            set { _sp_ssn = value; }
        }

        private DateTime _sp_date_of_birth;
        public DateTime sp_date_of_birth
        {
            get { return _sp_date_of_birth; }
            set { _sp_date_of_birth = value; }
        }

        private string _sp_tobacco_status;
        public string sp_tobacco_status
        {
            get { return _sp_tobacco_status; }
            set { _sp_tobacco_status = value; }
        }

        private string _spouse_basic_coverage_amount;
        public string spouse_basic_coverage_amount
        {
            get { return _spouse_basic_coverage_amount; }
            set { _spouse_basic_coverage_amount = value; }
        }

        private decimal _spouse_supplemental_coverage_amount;
        public decimal spouse_supplemental_coverage_amount
        {
            get { return _spouse_supplemental_coverage_amount; }
            set { _spouse_supplemental_coverage_amount = value; }
        }

        private string _spouse_basic_ad_d_coverage_amount;
        public string spouse_basic_ad_d_coverage_amount
        {
            get { return _spouse_basic_ad_d_coverage_amount; }
            set { _spouse_basic_ad_d_coverage_amount = value; }
        }

        private string _spouse_supplemental_ad_d_coverage_amount;
        public string spouse_supplemental_ad_d_coverage_amount
        {
            get { return _spouse_supplemental_ad_d_coverage_amount; }
            set { _spouse_supplemental_ad_d_coverage_amount = value; }
        }

        private string _child_basic_coverage_amount;
        public string child_basic_coverage_amount
        {
            get { return _child_basic_coverage_amount; }
            set { _child_basic_coverage_amount = value; }
        }

        private decimal _child_supplemental_coverage_amount;
        public decimal child_supplemental_coverage_amount
        {
            get { return _child_supplemental_coverage_amount; }
            set { _child_supplemental_coverage_amount = value; }
        }

        private string _child_basic_ad_d_coverage_amount;
        public string child_basic_ad_d_coverage_amount
        {
            get { return _child_basic_ad_d_coverage_amount; }
            set { _child_basic_ad_d_coverage_amount = value; }
        }

        private string _child_supplemental_ad_d_coverage_amount;
        public string child_supplemental_ad_d_coverage_amount
        {
            get { return _child_supplemental_ad_d_coverage_amount; }
            set { _child_supplemental_ad_d_coverage_amount = value; }
        }

        private string _additional_info_field_1;
        public string additional_info_field_1
        {
            get { return _additional_info_field_1; }
            set { _additional_info_field_1 = value; }
        }

        private string _additional_info_field_2;
        public string additional_info_field_2
        {
            get { return _additional_info_field_2; }
            set { _additional_info_field_2 = value; }
        }

        
    }
}
