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
	public class cdoPersonAccountLifeHistory : doPersonAccountLifeHistory
	{
		public cdoPersonAccountLifeHistory() : base()
		{
		}

        private string _plan_name;
        public string plan_name
        {
            get { return _plan_name; }
            set { _plan_name = value; }
        }

        private string _provider_name;
        public string provider_name
        {
            get { return _provider_name; }
            set { _provider_name = value; }
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

        private decimal _premium_amount;
        public decimal premium_amount
        {
            get { return _premium_amount; }
            set { _premium_amount = value; }
        }

        public int Sequence_ID { get; set; } //PIR 10422


        //PIR 13881
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
    } 
} 
