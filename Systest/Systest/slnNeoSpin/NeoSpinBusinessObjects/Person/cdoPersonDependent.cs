#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;
using NeoSpin.BusinessObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
	public class cdoPersonDependent : doPersonDependent
	{
		public cdoPersonDependent() : base()
		{
                        
		}

        public override int Delete()
        {
            return base.Delete();
        }

        private int _dependent_age;
        public int dependent_age
        {
            get { return _dependent_age; }
            set { _dependent_age = value; }
        }

        private string _dependent_name;
        public string dependent_name
        {
            get { return _dependent_name; }
            set { _dependent_name = value; }
        }

        public string istrDependentNameForCorrs { get; set; }

        private string _dependent_first_name;
        public string dependent_first_name
        {
            get { return _dependent_first_name; }
            set { _dependent_first_name = value; }
        }

        private string _dependent_last_name;
        public string dependent_last_name
        {
            get { return _dependent_last_name; }
            set { _dependent_last_name = value; }
        }

        private string _dependent_middle_name;
        public string dependent_middle_name
        {
            get { return _dependent_middle_name; }
            set { _dependent_middle_name = value; }
        }

        private string _dependent_prefix_name;
        public string dependent_prefix_name
        {
            get { return _dependent_prefix_name; }
            set { _dependent_prefix_name = value; }
        }

        private string _dependent_suffix_name;
        public string dependent_suffix_name
        {
            get { return _dependent_suffix_name; }
            set { _dependent_suffix_name = value; }
        }

        private string _plan_name;
        public string plan_name
        {
            get { return _plan_name; }
            set { _plan_name = value; }
        }

        private DateTime _dependent_DOB;
        public DateTime dependent_DOB
        {
            get { return _dependent_DOB; }
            set { _dependent_DOB = value; }
        }

        private string _dependent_Contact_No;
        public string dependent_Contact_No
        {
            get { return _dependent_Contact_No; }
            set { _dependent_Contact_No = value; }
        }

        private string _dependent_Email;
        public string dependent_Email
        {
            get { return _dependent_Email; }
            set { _dependent_Email = value; }
        }

        private string _dependent_ssn;
        public string dependent_ssn
        {
            get { return _dependent_ssn; }
            set { _dependent_ssn = value; }
        }

        public string dependent_last_4_digit_ssn { get; set; }

        private string _dependent_marital_status;
        public string dependent_marital_status
        {
            get { return _dependent_marital_status; }
            set { _dependent_marital_status = value; }
        }

        private string _dependent_gender;
        public string dependent_gender
        {
            get { return _dependent_gender; }
            set { _dependent_gender = value; }
        }
        private string _dependent_marital_status_description;
        public string dependent_marital_status_description
        {
            get { return _dependent_marital_status_description; }
            set { _dependent_marital_status_description = value; }
        }

        private string _dependent_gender_description;
        public string dependent_gender_description
        {
            get { return _dependent_gender_description; }
            set { _dependent_gender_description = value; }
        }

        private DateTime _dependent_date_of_death; //PROD PIR ID 6590
        public DateTime dependent_date_of_death
        {
            get { return _dependent_date_of_death; }
            set { _dependent_date_of_death = value; }
        }

        private DateTime _dependent_ms_change_date; //PROD PIR ID 6590
        public DateTime dependent_ms_change_date
        {
            get { return _dependent_ms_change_date; }
            set { _dependent_ms_change_date = value; }
        }

        public string middle_name_no_null
        {
            get
            {
                if (String.IsNullOrEmpty(middle_name))
                    return String.Empty;
                return " " + middle_name;
            }
        }

        private string _Suppress_Warning;
        public string Suppress_Warning
        {
            get { return _Suppress_Warning; }
            set { _Suppress_Warning = value; }
        }

        #region Correspondence

        // Ineligible dependent Letter
        private string _istr_dependent_Ineligible_Date;
        public string istr_dependent_Ineligible_Date
        {
            get { return _istr_dependent_Ineligible_Date; }
            set { _istr_dependent_Ineligible_Date = value; }
        }

        // Ineligible dependent Letter
        private DateTime _dependent_coverage_enddate;
        public DateTime dependent_coverage_enddate
        {
            get { return _dependent_coverage_enddate; }
            set { _dependent_coverage_enddate = value; }
        }

        public string istr_dependent_coverage_enddate { get; set; }

        // Ineligible dependent Letter
        public string dependent_coverage_renew_date { get; set; }
        // Ineligible dependent Letter
        public string istr_Guardianship { get; set; }
        public string istr_Actual_Expiration_Date { get; set; }
        
        public string istrLifePlan { get; set; }

        // Medicare Age 65 Letter
        private DateTime _CurrentYearBirthDate;
        public DateTime CurrentYearBirthDate
        {
            get { return _CurrentYearBirthDate; }
            set { _CurrentYearBirthDate = value; }
        }

        // Medicare Age 65 Letter        
        public string CurrentYearBirthDateMonth
        {
            get
            {
                string lstrMonth = String.Empty;
                if (_CurrentYearBirthDate != DateTime.MinValue)
                    lstrMonth = _CurrentYearBirthDate.ToString("MMMM");
                return lstrMonth;
            }
        }

        // Medicare Age 65 Letter
        private DateTime _coverage_enddate;
        public string coverage_enddate
        {
            get
            {
                // Last day of the Bday month
                if (_CurrentYearBirthDate != DateTime.MinValue)
                {
                    if (_CurrentYearBirthDate.Day == 1)
                    {
                        //Prod PIR:4369
                        //The health plan end date for covered retirees/dependents turning 65 is as follows:  Plan end date is the last day of the month preceeding age 65, except if the birthday is on the first day of the month, then the plan end date is the last day of the month day of the prior month.
                        //example: DOB is January 2, the plan end date is 12/31.  If the DOB is January 1, the plan end date is 11/30.
                        _coverage_enddate = _CurrentYearBirthDate.AddMonths(-2).GetLastDayofMonth();
                    }
                    else
                    {

                        _coverage_enddate = _CurrentYearBirthDate.AddMonths(-1).GetLastDayofMonth();
                    }
                }
                return _coverage_enddate.ToString(BusinessObjects.busConstant.DateFormatLongDate);
            }
        }

        // Medicare Age 65 Letter
        public string PriorMonthToNotice
        {
            get
            {
                if (_coverage_enddate != DateTime.MinValue)
                    return _coverage_enddate.ToString("MMMM"); //PROD PIR ID 6393
                return string.Empty;
            }
        }

        // Medicare Age 65 Letter
        private string _full_name;
        public string full_name
        {
            get { return _full_name; }
            set { _full_name = value; }
        }

        #endregion

        public bool is_medicare_split
        {
            get
            {
                if (string.IsNullOrEmpty(medicare_claim_no))
                    return false;
                return true;
            }
        }

        public string is_selected_flag { get; set; }

        public bool is_subscriber_dependent { get; set; }

        public string splitted_coverage_code { get; set; }

        public string subscriber_dependent_ssn { get; set; }

        public int mss_person_dependent_id { get; set; }

        //PIR 11841
        public bool iblnNewResult { get; set; } 

        public int iintPlanId { get; set; } 

        public DateTime start_date { get; set; } 

        //PIR 26111
        public string istrMedicareAge65LetterSentFlagDependent { get; set; }

    }
} 
