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
	public class cdoPersonAccountDeferredCompContribution : doPersonAccountDeferredCompContribution
	{
		public cdoPersonAccountDeferredCompContribution() : base()
		{
		}
        private string _Payroll_Detail_ID;

        public string Payroll_Detail_ID
        {
            get 
            {
                if (subsystem_ref_id != 0)
                {
                    _Payroll_Detail_ID = subsystem_ref_id.ToString();
                }
                else
                {
                    _Payroll_Detail_ID = string.Empty;
                }
                return _Payroll_Detail_ID; 
            }
            set { _Payroll_Detail_ID = value; }
        }

        //Property to return YYYYMM of pay period start date
        public string PayPeriodYearMonth
        {
            get
            {
                return pay_period_start_date.Year.ToString() + ((pay_period_start_date.Month.ToString().Length == 1) ?
                    "0" + pay_period_start_date.Month.ToString() : pay_period_start_date.Month.ToString());
            }
        }
        
        //Property to return Month part of pay period start date
        public int PayPeriodMonth
        {
            get
            {
                return pay_period_start_date.Month;
            }
        }

        //Property to return Year part of pay period start date
        public int PayPeriodYear
        {
            get
            {
                return pay_period_start_date.Year;
            }
        }

        //Property to contain formatted Pay period start date
        public string istrPayPeriodStartDateFormatted { get; set; }

        //Property to contain formatted Pay Period end date
        public string istrPayPeriodEndDateFormatted { get; set; }

        //this is used in UCS-041        
        public string istrIsRecordSelected { get; set; }

    } 
} 
