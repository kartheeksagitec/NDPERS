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
    public class cdoWebServiceAddress
    {   
        private string _addr_line_1;
        public string addr_line_1
        {
            get
            {
                return _addr_line_1;
            }

            set
            {
                _addr_line_1 = value;
            }
        }

        private string _addr_line_2;
        public string addr_line_2
        {
            get
            {
                return _addr_line_2;
            }

            set
            {
                _addr_line_2 = value;
            }
        }

        private string _addr_city;
        public string addr_city
        {
            get
            {
                return _addr_city;
            }

            set
            {
                _addr_city = value;
            }
        }

        private int _addr_state_id;
        public int addr_state_id
        {
            get
            {
                return _addr_state_id;
            }

            set
            {
                _addr_state_id = value;
            }
        }

        private string _addr_state_description;
        public string addr_state_description
        {
            get
            {
                return _addr_state_description;
            }

            set
            {
                _addr_state_description = value;
            }
        }

        private string _addr_state_value;
        public string addr_state_value
        {
            get
            {
                return _addr_state_value;
            }

            set
            {
                _addr_state_value = value;
            }
        }

        private int _addr_county_id;
        public int addr_county_id
        {
            get
            {
                return _addr_county_id;
            }

            set
            {
                _addr_county_id = value;
            }
        }

        private string _addr_county_description;
        public string addr_county_description
        {
            get
            {
                return _addr_county_description;
            }

            set
            {
                _addr_county_description = value;
            }
        }

        private string _addr_county_value;
        public string addr_county_value
        {
            get
            {
                return _addr_county_value;
            }

            set
            {
                _addr_county_value = value;
            }
        }

        private int _addr_country_id;
        public int addr_country_id
        {
            get
            {
                return _addr_country_id;
            }

            set
            {
                _addr_country_id = value;
            }
        }

        private string _addr_country_description;
        public string addr_country_description
        {
            get
            {
                return _addr_country_description;
            }

            set
            {
                _addr_country_description = value;
            }
        }

        private string _addr_country_value;
        public string addr_country_value
        {
            get
            {
                return _addr_country_value;
            }

            set
            {
                _addr_country_value = value;
            }
        }

        private string _addr_zip_code;
        public string addr_zip_code
        {
            get
            {
                return _addr_zip_code;
            }

            set
            {
                _addr_zip_code = value;
            }
        }

        private string _addr_zip_4_code;
        public string addr_zip_4_code
        {
            get
            {
                return _addr_zip_4_code;
            }

            set
            {
                _addr_zip_4_code = value;
            }
        }
        private string _address_validate_flag;
        public string address_validate_flag
        {
            get
            {
                return _address_validate_flag;
            }

            set
            {
                _address_validate_flag = value;
            }
        }

        private string _address_validate_error;
        public string address_validate_error
        {
            get
            {
                return _address_validate_error;
            }

            set
            {
                _address_validate_error = value;
            }
        }
    }
}
