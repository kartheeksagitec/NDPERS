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
    public class cdoEmployerPayrollDetail : doEmployerPayrollDetail
    {
        public cdoEmployerPayrollDetail()
            : base()
        {
        }

        # region Custom Properties
        private string _lstrPayPeriodMonth;

        public string lstrPayPeriodMonth
        {
            get { return _lstrPayPeriodMonth; }
            set { _lstrPayPeriodMonth = value; }
        }

        private string _lstrPayPeriodEndMonth;

        public string lstrPayPeriodEndMonth
        {
            get { return _lstrPayPeriodEndMonth; }
            set { _lstrPayPeriodEndMonth = value; }
        }

        private int _lintRecordTypeValue;

        public int lintRecordTypeValue
        {
            get { return _lintRecordTypeValue; }
            set { _lintRecordTypeValue = value; }
        }
        private int _lintMemberStatusValue;

        public int lintMemberStatusValue
        {
            get { return _lintMemberStatusValue; }
            set { _lintMemberStatusValue = value; }
        }
        # endregion

        //This is a derived property to get the last date of given pay period
        public DateTime pay_period_last_date
        {
            get
            {
                DateTime ldtDate = DateTime.MinValue;
                if (pay_period_date != DateTime.MinValue)
                {
                    ldtDate = new DateTime(pay_period_date.Year, pay_period_date.Month, 1);
                    ldtDate = ldtDate.AddMonths(1).AddDays(-1);
                }
                return ldtDate;
            }
        }

        // PIR 9647
        public DateTime pay_period_last_date_for_bonus
        {
            get
            {
                DateTime ldtDate = DateTime.MinValue;
                if (pay_period_end_month_for_bonus != DateTime.MinValue)
                {
                    ldtDate = new DateTime(pay_period_end_month_for_bonus.Year, pay_period_end_month_for_bonus.Month, 1);
                    ldtDate = ldtDate.AddMonths(1).AddDays(-1);
                }
                return ldtDate;
            }
        }

        public decimal eligible_wages_rounded
        {
            get
            {
                return Math.Round(eligible_wages, 0, MidpointRounding.AwayFromZero);
            }
        }

        public bool is_provider1_linked_with_member;
        public bool is_provider2_linked_with_member;
        public bool is_provider3_linked_with_member;
        public bool is_provider4_linked_with_member;
        public bool is_provider5_linked_with_member;
        public bool is_provider6_linked_with_member;
        public bool is_provider7_linked_with_member;

        public bool ibln_provider1_linked_with_member { get { return is_provider1_linked_with_member; } }
        public bool ibln_provider2_linked_with_member { get { return is_provider2_linked_with_member; } }
        public bool ibln_provider3_linked_with_member { get { return is_provider3_linked_with_member; } }
        public bool ibln_provider4_linked_with_member { get { return is_provider4_linked_with_member; } }
        public bool ibln_provider5_linked_with_member { get { return is_provider5_linked_with_member; } }
        public bool ibln_provider6_linked_with_member { get { return is_provider6_linked_with_member; } }
        public bool ibln_provider7_linked_with_member { get { return is_provider7_linked_with_member; } }

        public string LastFourDigitsOfSSN
        {
            get
            {
                if ((ssn != null) && (ssn.Length == 9))
                {
                    return ssn.Substring(5);
                }
                return string.Empty;
            }
        }

        public string istrPeopleSoftID { get; set; }

        //prod pir 6077
        public string istrProviderOrgCode { get; set; }

        //prod pir 5640 : to remove duplicate while sorting in grid
        public string istrLastNameEmpDetailID
        {
            get
            {
                return (string.IsNullOrEmpty(last_name) ? "" : last_name) + employer_payroll_detail_id.ToString();
            }
        }

        //prod pir 5640 : to remove duplicate while sorting in grid
        public string istrFirstNameEmpDetailID
        {
            get
            {
                return (string.IsNullOrEmpty(first_name) ? "" : first_name) + employer_payroll_detail_id.ToString();
            }
        }
        public int plan_id_display { get; set; }
        public string first_name_display { get; set; }
        public string last_name_display { get; set; }
        public string ssn_display { get; set; }

        public string ssn_display_defComp { get; set; }
        public string first_name_display_defComp { get; set; }
        public string last_name_display_defComp { get; set; }
        public string istrlookupComment { get; set; } 		// ESS Backlog PIR - 13416
        //code optimization changes
        public string org_code { get; set; }
        public string org_name { get; set; }

        public string plan_name { get; set; }

        public string pay_period { get; set; }

        public string pay_end_month { get; set; }

        public decimal eligible_wages_original { get; set; }

        public decimal idecLow_Income_Credit { get; set; }
        public int iintPremiumForPersonId { get; set; }//Org to bill
        public string provider_org_code { get; set; }
    }
}
