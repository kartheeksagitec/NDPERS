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
    public class cdoPersonAccountDeferredCompProvider : doPersonAccountDeferredCompProvider
    {
        public cdoPersonAccountDeferredCompProvider()
            : base()
        {
        }
        private string _istrProviderOrgCode;

        public string istrProviderOrgCode
        {
            get { return _istrProviderOrgCode; }
            set { _istrProviderOrgCode = value; }
        }
        public DateTime end_date_no_null
        {
            get
            {
                if (end_date == DateTime.MinValue)
                    return DateTime.MaxValue;
                return end_date;
            }
        }
        private string _suppress_warning_flag;

        public string suppress_warning_flag
        {
            get { return _suppress_warning_flag; }
            set { _suppress_warning_flag = value; }
        }
        public string istrMSSEmployerName { get; set; }

        public int new_person_employment_id { get; set; }
        public string new_istrProviderOrgCode { get; set; }
        public int new_provider_agent_contact_id { get; set; }
        public decimal new_per_pay_period_contribution_amt { get; set; }
        public string new_mutual_fund_window_flag { get; set; }
        public DateTime new_start_date { get; set; }
        public DateTime new_end_date { get; set; }

        public int person_id { get; set; }
        public int org_id { get; set; }

        public string change_amount_text
        {
            get
            {
                return "Change Amount";
            }
        }
        //PIR 25920 New Plan DC 2025 // set desc to Yes or No
        public string is_apply_employer_matching_contribution_desc
        {
            get
            {
                if (is_apply_employer_matching_contribution == busConstant.Flag_Yes)
                    return busConstant.Flag_Yes_Value;
                else return busConstant.Flag_No_Value;
            }
        }
    }
}
