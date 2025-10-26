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
	public class cdoPersonAccountLifeOption : doPersonAccountLifeOption
	{
		public cdoPersonAccountLifeOption() : base()
		{
		}

        private int _Sequence_ID;
        public int Sequence_ID
        {
            get { return _Sequence_ID; }
            set { _Sequence_ID = value; }
        }

        private bool _lblnValueEntered;
        public bool lblnValueEntered
        {
            get { return _lblnValueEntered; }
            set { _lblnValueEntered = value; }
        }

        public bool lblnValueRemoved { get; set; }

        private decimal _Employee_Premium_Amount;
        public decimal Employee_Premium_Amount
        {
            get { return _Employee_Premium_Amount; }
            set { _Employee_Premium_Amount = value; }
        }

        private decimal _Employer_Premium_Amount;
        public decimal Employer_Premium_Amount
        {
            get { return _Employer_Premium_Amount; }
            set { _Employer_Premium_Amount = value; }
        }

        public decimal Monthly_Premium
        {
            get 
            {
                return _Employee_Premium_Amount + _Employer_Premium_Amount; 
            }
        }

        public DateTime effective_end_date_no_null
        {
            get
            {
                if (effective_end_date == DateTime.MinValue)
                    return DateTime.MaxValue;
                else
                    return effective_end_date;
            }
        }

        public string istrEffectiveEndDateFormatted
        {
            get
            {
                if (effective_end_date == DateTime.MinValue)
                    return string.Empty;
                else
                    return effective_end_date.ToString(BusinessObjects.busConstant.DateFormatLongDate);
            }
        }
        public decimal ActualCoverageAmount { get; set; } //PIR 10422
        public decimal NewReducedCoverageAmount { get; set; } //PIR 18493 App Wizard This is used to value for supllemental and spouse text new amount
        public int enroll_req_plan_id { get; set; } //PIR 18493
        public string dependent_coverage_option_value { get; set; }//PIR 18493 This is used to value for dependant Drop down
    } 
} 