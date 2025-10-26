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

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPersonAccountEAPFile
    {
        /// Properties for EAP File out.

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
                if(string.IsNullOrEmpty(_middle_name))
                    return _middle_name;
                if (_middle_name.Length > 50)
                    return _middle_name.Trim().ToUpper().Substring(0, 50);
                return _middle_name.Trim().ToUpper();
            }
            set { _middle_name = value; }
        }

        private string _employer_name;
        public string employer_name
        {
            get 
            {
                if (string.IsNullOrEmpty(_employer_name))
                    return _employer_name;
                if (_employer_name.Length > 50)
                    return _employer_name.Trim().ToUpper().Substring(0, 50);
                return _employer_name.Trim().ToUpper(); 
            }
            set { _employer_name = value; }
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

        private string _employer_city;
        public string employer_city
        {
            get 
            {
                if (string.IsNullOrEmpty(_employer_city))
                    return _employer_city;
                if (_employer_city.Length > 50)
                    return _employer_city.Trim().ToUpper().Substring(0,50);
                return _employer_city.Trim().ToUpper();
            }
            set { _employer_city = value; }
        }

        private string _employer_state;
        public string employer_state
        {
            get { return _employer_state; }
            set { _employer_state = value; }
        }

        private string _employer_zip;
        public string employer_zip
        {
            get { return _employer_zip; }
            set { _employer_zip = value; }
        }

        private DateTime _employment_start_date;
        public DateTime employment_start_date
        {
            get { return _employment_start_date; }
            set { _employment_start_date = value; }
        }

        private DateTime _employment_end_date;
        public DateTime employment_end_date
        {
            get { return _employment_end_date; }
            set { _employment_end_date = value; }
        }
    }
}
