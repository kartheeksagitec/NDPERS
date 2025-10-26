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
    public class busASIEligibilityFile
    {
        private int _person_id;
        public int person_id
        {
            get
            {
                return _person_id;
            }
            set { _person_id = value; }
        }

        private int _division_code;
        public int division_code
        {
            get
            {
                return _division_code;
            }
            set
            {
                _division_code = value;
            }
        }

        private string _last_name;
        public string last_name
        {
            get
            {
                if (string.IsNullOrEmpty(_last_name))
                    return _last_name;
                if (_last_name.Length > 50)
                    return _last_name.Trim().ToUpper().Substring(0, 50);
                return _last_name.Trim().ToUpper();
            }
            set { _last_name = value; }
        }

        private string _first_name;
        public string first_name
        {
            get
            {
                if (string.IsNullOrEmpty(_first_name))
                    return _first_name;
                if (_first_name.Length > 50)
                    return _first_name.Trim().ToUpper().Substring(0, 50);
                return _first_name.Trim().ToUpper();
            }
            set { _first_name = value; }
        }

        private string _middle_name;
        public string middle_name
        {
            get
            {
                if (string.IsNullOrEmpty(_middle_name))
                    return _middle_name;
                if (_middle_name.Length > 50)
                    return _middle_name.Trim().ToUpper().Substring(0, 50);
                return _middle_name.Trim().ToUpper();
            }
            set { _middle_name = value; }
        }

        private string _address_line_1;
        public string address_line_1
        {
            get
            {
                if (string.IsNullOrEmpty(_address_line_1))
                    return _address_line_1;
                if (_address_line_1.Length > 60)
                    return _address_line_1.Trim().ToUpper().Substring(0, 60);
                return _address_line_1.Trim().ToUpper();
            }
            set { _address_line_1 = value; }
        }

        private string _address_line_2;
        public string address_line_2
        {
            get
            {
                if (string.IsNullOrEmpty(_address_line_2))
                    return _address_line_2;
                if (_address_line_2.Length > 60)
                    return _address_line_2.Trim().ToUpper().Substring(0, 60);
                return _address_line_2.Trim().ToUpper();
            }
            set { _address_line_2 = value; }
        }

        private string _city;
        public string city
        {
            get
            {
                if (string.IsNullOrEmpty(_city))
                    return _city;
                if (_city.Length > 50)
                    return _city.Trim().ToUpper().Substring(0, 50);
                return _city.Trim().ToUpper();
            }
            set { _city = value; }
        }

        private string _addr_state;
        public string addr_state
        {
            get { return _addr_state; }
            set { _addr_state = value; }
        }

        private string _zip_code;
        public string zip_code
        {
            get { return _zip_code; }
            set { _zip_code = value; }
        }

        private string _addr_country;
        public string addr_country
        {
            get { return _addr_country; }
            set { _addr_country = value; }
        }

        private string _foreign_province;
        public string foreign_province
        {
            get { return _foreign_province; }
            set { _foreign_province = value; }
        }

        private string _foreign_postal_code;
        public string foreign_postal_code
        {
            get { return _foreign_postal_code; }
            set { _foreign_postal_code = value; }
        }

        private decimal _rhic_amount;
        public decimal rhic_amount
        {
            get { return _rhic_amount; }
            set { _rhic_amount = value; }
        }

        private DateTime _rhic_start_date;
        public DateTime rhic_start_date
        {
            get { return _rhic_start_date; }
            set { _rhic_start_date = value; }
        }

        //PIR 18271
        private string _routing_no;
        public string routing_no
        {
            get { return _routing_no; }
            set { _routing_no = value; }
        }
        private string _account_number;
        public string account_number
        {
            get { return _account_number; }
            set { _account_number = value; }
        }
        private string _bank_account_type_value;
        public string bank_account_type_value
        {
            get { return _bank_account_type_value; }
            set { _bank_account_type_value = value; }
        }

        public DateTime date_of_death { get; set; } //PIR 19290
    }
}
