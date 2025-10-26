#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;
using System.Globalization;
#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
    public class cdoEmployerPayrollHeader : doEmployerPayrollHeader
    {
        public cdoEmployerPayrollHeader()
            : base()
        {
        }

        # region Custom Properties

        private string _pay_period;

        public string pay_period
        {
            get
            {
                return _pay_period;
            }
            set
            {
                _pay_period = value;
            }
        }

        private int _lintReportType;

        public int lintReportType
        {
            get { return _lintReportType; }
            set { _lintReportType = value; }
        }

        //This is a derived property to get the last date of given pay period
        public DateTime payroll_paid_last_date
        {
            get
            {
                DateTime ldtDate = DateTime.MinValue;
                if (payroll_paid_date != DateTime.MinValue)
                {
                    ldtDate = new DateTime(payroll_paid_date.Year, payroll_paid_date.Month, 1);
                    ldtDate = ldtDate.AddMonths(1).AddDays(-1);
                }
                return ldtDate;
            }
        }

        //Prop  used in ESS
        public int contact_id { get; set; }

        public DateTime SORT_ORDER_DATE { get; set; }
        public string Payment_Status { get; set; }

        //this property is used to suppress warning for def. comp header type
        //to suppress "Pay check date different in header and details" warning
        public string suppress_warnings_flag { get; set; }
        public decimal total_wages_from_details { get; set; }
        public decimal total_contributions_from_details { get; set; }
        public decimal idecOutstandingRetirementBalance { get; set; }
        public decimal idecOutstandingRHICBalance { get; set; }
        //PIR 25920 DC 2025 chnages
        public decimal idecOutstandingADECBalance { get; set; }
        public decimal idecOutstandingDeferredCompBalance { get; set; }

        public decimal idecInsuranceContributionByPlan { get; set; }

        public decimal idecOutstandingAmount { get; set; }
        public decimal idecRemainingBalance { get; set; }
        public decimal idecRoundingDifference
        {
            get
            {
                decimal roundDiff;

                roundDiff = Math.Abs(total_contributions_from_details - total_contribution_reported);
                return roundDiff;
            }
        
        }

        # endregion      
        public string astrRecordTypeBonus { get; set; }
        //prod pir 6795
        public string istrPayPeriodSortOrder
        {
            get
            {
                if (payroll_paid_date == DateTime.MinValue)
                    return string.Empty;
                else
                    return payroll_paid_date.Year.ToString() + (payroll_paid_date.Month < 10 ? "0" + payroll_paid_date.Month.ToString() : payroll_paid_date.Month.ToString());
            }
        }
        public decimal idecTotalContributionsAndInterestDue
        {
            get
            {
                //PIR 13996 - Used total_contributions_from_details instead of total_contribution_calculated
                return total_contributions_from_details + total_interest_calculated;
            }
        }
        public string istrPayrollPaidDateAsMonthYear
        {
            get
            {
                string lstrMonthYear = String.Empty;
                if (payroll_paid_date != DateTime.MinValue)
                    lstrMonthYear = payroll_paid_date.ToString("MM") + "/" + payroll_paid_date.Year.ToString();
                return lstrMonthYear;
            }
        }
        public string istrTotalWagesReported { get; set; }
        public string istrTotalContributionsReported { get; set; }
        public string istrTotalADECAmountReported { get; set; }		//PIR 25920 New Plan DC 2025
        public string istrlookupComment { get; set; } 		// ESS Backlog PIR - 13416
        
		//PIR 15238 
        public decimal TotalMemberInterestCalculated { get; set; }
        public decimal TotalEmployerInterestCalculated { get; set; }
        public decimal TotalEmployerRhicInterestCalculated { get; set; }
    }
}
