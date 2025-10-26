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
	public class cdoBenefitCalculationPayee : doBenefitCalculationPayee
	{
		public cdoBenefitCalculationPayee() : base()
		{
		}
        
        public String FullName
        {
            get
            {
                string lstrName = String.Empty;
                if (!String.IsNullOrEmpty(payee_first_name))
                {
                    lstrName = payee_first_name;
                }
                if (!String.IsNullOrEmpty(payee_middle_name))
                {
                    lstrName += " " + payee_middle_name;
                }
                if (!String.IsNullOrEmpty(payee_last_name))
                    lstrName += " " + payee_last_name;

                return lstrName;
            }
        }

        private string _payee_identifier;
        public string payee_identifier
        {
            get { return _payee_identifier; }
            set { _payee_identifier = value; }
        }

        private string _payee_benefit_option;
        public string payee_benefit_option
        {
            get { return _payee_benefit_option; }
            set { _payee_benefit_option = value; }
        }

        public decimal payee_benefit_amount { get; set; }

        public int payee_benefit_provision_benefit_option_id { get; set; }

        private string _payee_org_code;
        public string payee_org_code
        {
            get { return _payee_org_code; }
            set { _payee_org_code = value; }
        }

        private string _payee_gender;
        public string payee_gender
        {
            get { return _payee_gender; }
            set { _payee_gender = value; }
        }

        public string payee_name { get; set; }

        public string mss_account_relationship
        {
            get
            {
                if (account_relationship_value == "JANT")
                    return "Spouse";
                else
                    return account_relationship_description;
            }
        }
    }
} 
