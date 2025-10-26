using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busESIMedicarePartDEnrollmentFile
    {
        private string _person_id;
        public string person_id
        {
            get { return _person_id; }
            set { _person_id = value; }
        }

        private string _record_type;
        public string record_type
        {
            get { return _record_type; }
            set { _record_type = value; }
        }

        private string _last_name;
        public string last_name
        {
            get
            {
                if (string.IsNullOrEmpty(_last_name))
                    return _last_name;
                if (_last_name.Length > 35)
                    return _last_name.Trim().ToUpper().Substring(0, 35);
                return _last_name.Trim().ToUpper();
            }
            set { _last_name = value; }
        }

        private string _first_name;
        public string first_name
        {
            get
            {
                //if (string.IsNullOrEmpty(_first_name))
                //    return _first_name;
                //if (_first_name.Length > 25)
                //    return _first_name.Trim().ToUpper().Substring(0, 25);
                //return _first_name.Trim().ToUpper();
                return _first_name;
            }
            set { _first_name = value; }
        }

        private string _mi;
        public string mi
        {
            get
            {
                if (string.IsNullOrEmpty(_mi))
                    return _mi;
                if (_mi.Length > 50)
                    return _mi.Trim().ToUpper().Substring(0, 50);
                return _mi.Trim().ToUpper();
            }
            set { _mi = value; }
        }

        private string _date_of_birth;
        public string date_of_birth
        {
            get { return _date_of_birth; }
            set { _date_of_birth = value; }
        }

        private string _gender;
        public string gender
        {
            get { return _gender; }
            set { _gender = value; }
        }

        private string _ssn;
        public string ssn
        {
            get { return _ssn; }
            set { _ssn = value; }
        }

        private string _medicare_claim_no;
        public string medicare_claim_no
        {
            get { return _medicare_claim_no; }
            set { _medicare_claim_no = value; }
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

        private string _zip_4_code;
        public string zip_4_code
        {
            get { return _zip_4_code; }
            set { _zip_4_code = value; }
        }

        private string _home_phone_number;
        public string home_phone_number
        {
            get { return _home_phone_number; }
            set { _home_phone_number = value; }
        }

        private string _disenrollment_reason;
        public string disenrollment_reason
        {
            get { return _disenrollment_reason; }
            set { _disenrollment_reason = value; }
        }

        private string _disenrollment_date;
        public string disenrollment_date
        {
            get { return _disenrollment_date; }
            set { _disenrollment_date = value; }
        }

        private string _current_date;
        public string current_date
        {
            get { return _current_date; }
            set { _current_date = value; }
        }

        private string _initial_enroll_date;
        public string initial_enroll_date
        {
            get { return _initial_enroll_date; }
            set { _initial_enroll_date = value; }
        }

    }
}
