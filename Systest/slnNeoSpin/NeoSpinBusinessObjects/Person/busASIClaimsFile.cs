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
    public class busASIClaimsFile
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

        private decimal _total_premium;
        public decimal total_premium
        {
            get { return _total_premium; }
            set { _total_premium = value; }
        }

        private DateTime _billing_month_and_year;
        public DateTime billing_month_and_year
        {
            get { return _billing_month_and_year; }
            set { _billing_month_and_year = value; }
        }
    }
}
