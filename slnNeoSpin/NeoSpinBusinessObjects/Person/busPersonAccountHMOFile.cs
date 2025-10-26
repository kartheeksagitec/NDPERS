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
    public class busPersonAccountHMOFile
    {
        /// Properties for HMO File out.

        private string _last_name;
        public string last_name
        {
            get 
            { 
                if(string.IsNullOrEmpty(_last_name))
                    return _last_name;
                return _last_name.Trim().ToUpper();
            }
            set { _last_name = value; }
        }

        private string _first_name;
        public string first_name
        {
            get 
            {
                if(string.IsNullOrEmpty(_first_name))
                    return _first_name;
                return _first_name.Trim().ToUpper();
            }
            set { _first_name = value; }
        }

        private string _benefit_plan;
        public string benefit_plan
        {
            get 
            {
                if(string.IsNullOrEmpty(_benefit_plan))
                    return _benefit_plan;
                return _benefit_plan.Trim().ToUpper();
            }
            set { _benefit_plan = value; }
        }

        private DateTime _coverate_begin_date;
        public DateTime coverage_begin_date
        {
            get { return _coverate_begin_date; }
            set { _coverate_begin_date = value; }
        }

        private DateTime _coverage_end_date;
        public DateTime coverage_end_date
        {
            get { return _coverage_end_date; }
            set { _coverage_end_date = value; }
        }

        private DateTime _deduction_begin_date;
        public DateTime deduction_begin_date
        {
            get { return _deduction_begin_date; }
            set { _deduction_begin_date = value; }
        }

        private string _coverage_election;
        public string coverage_election
        {
            get 
            {
                if(string.IsNullOrEmpty(_coverage_election))
                    return _coverage_election;
                return _coverage_election.Trim().ToUpper();
            }
            set { _coverage_election = value; }
        }

        private string _level_of_coverage;
        public string level_of_coverage
        {
            get 
            { 
                if(string.IsNullOrEmpty(_level_of_coverage))
                    return _level_of_coverage;
                return _level_of_coverage.Trim().ToUpper();
            }
            set { _level_of_coverage = value; }
        }

        private string _premium_amount;
        public string premium_amount
        {
            get { return _premium_amount; }
            set { _premium_amount = value; }
        }

        private string _employee_ssn;
        public string employee_ssn
        {
            get { return _employee_ssn; }
            set { _employee_ssn = value; }
        }
    }
}
