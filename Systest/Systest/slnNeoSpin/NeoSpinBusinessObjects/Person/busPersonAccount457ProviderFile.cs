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
    public class busPersonAccount457ProviderFile
    {
        private string _employee_ssn;
        public string employee_ssn
        {
            get { return _employee_ssn; }
            set { _employee_ssn = value; }
        }

        private string _employee_last_name;
        public string employee_last_name
        {
            get { return _employee_last_name; }
            set { _employee_last_name = value; }
        }

        private string _employee_first_name;
        public string employee_first_name
        {
            get { return _employee_first_name; }
            set { _employee_first_name = value; }
        }

        private string _org_code;
        public string org_code
        {
            get { return _org_code; }
            set { _org_code = value; }
        }

        private DateTime _deduction_begin_date;
        public DateTime deduction_begin_date
        {
            get { return _deduction_begin_date; }
            set { _deduction_begin_date = value; }
        }

        private DateTime _deduction_end_date;
        public DateTime deduction_end_date
        {
            get { return _deduction_end_date; }
            set { _deduction_end_date = value; }
        }

        private decimal _deduction_amount;
        public decimal deduction_amount
        {
            get { return _deduction_amount; }
            set { _deduction_amount = value; }
        }

        private DateTime _election_date;
        public DateTime election_date
        {
            get { return _election_date; }
            set { _election_date = value; }
        }        
    }
}
