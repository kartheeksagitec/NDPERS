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
    public class busPersonAccountTFFRFile
    {
        /// Properties for TFFR File out.

        private string _ssn;
        public string ssn
        {
            get { return _ssn; }
            set { _ssn = value; }
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

        private string _insurance_type;
        public string insurance_type
        {
            get { return _insurance_type; }
            set { _insurance_type = value; }
        }

        private string _old_coverage_code;
        public string old_coverage_code
        {
            get { return _old_coverage_code; }
            set { _old_coverage_code = value; }
        }

        private string _old_coverage_description;
        public string old_coverage_description
        {
            get { return _old_coverage_description; }
            set { _old_coverage_description = value; }
        }

        private string _old_rate_structure;
        public string old_rate_structure
        {
            get { return _old_rate_structure; }
            set { _old_rate_structure = value; }
        }

        private decimal _old_premium_amount;
        public decimal old_premium_amount
        {
            get { return _old_premium_amount; }
            set { _old_premium_amount = value; }
        }

        private string _new_coverage_code;
        public string new_coverage_code
        {
            get { return _new_coverage_code; }
            set { _new_coverage_code = value; }
        }

        private string _new_coverage_description;
        public string new_coverage_description
        {
            get { return _new_coverage_description; }
            set { _new_coverage_description = value; }
        }

        private string _new_rate_structure;
        public string new_rate_structure
        {
            get { return _new_rate_structure; }
            set { _new_rate_structure = value; }
        }

        private decimal _new_premium_amount;
        public decimal new_premium_amount
        {
            get { return _new_premium_amount; }
            set { _new_premium_amount = value; }
        }

        private string _enrollment_change_reason;
        public string enrollment_change_reason
        {
            get { return _enrollment_change_reason; }
            set { _enrollment_change_reason = value; }
        }

        private DateTime _effective_start_date;
        public DateTime effective_start_date
        {
            get { return _effective_start_date; }
            set { _effective_start_date = value; }
        }

        private DateTime _effective_end_date;
        public DateTime effective_end_date
        {
            get { return _effective_end_date; }
            set { _effective_end_date = value; }
        }
    }
}
