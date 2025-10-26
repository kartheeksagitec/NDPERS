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
	public class cdoPersonBeneficiary : doPersonBeneficiary
	{
		public cdoPersonBeneficiary() : base()
		{

		}

        private string _beneficiary_name;
        public string beneficiary_name
        {
            get { return _beneficiary_name; }
            set { _beneficiary_name = value; }
        }

        public string beneficiary_name_caps { get; set; }

        public string beneficiary_first_name { get; set; }

        public string beneficiary_last_name { get; set; }

        private string _beneficiary_org_code;
        public string beneficiary_org_code
        {
            get { return _beneficiary_org_code; }
            set { _beneficiary_org_code = value; }
        }

        private DateTime _beneficiary_DOB;
        public DateTime beneficiary_DOB
        {
            get { return _beneficiary_DOB; }
            set { _beneficiary_DOB = value; }
        }

        private string _beneficiary_Contact_No;
        public string beneficiary_Contact_No
        {
            get { return _beneficiary_Contact_No; }
            set { _beneficiary_Contact_No = value; }
        }

        private string _beneficiary_Email;
        public string beneficiary_Email
        {
            get { return _beneficiary_Email; }
            set { _beneficiary_Email = value; }
        }

        private string _beneficiary_gender;
        public string beneficiary_gender
        {
            get { return _beneficiary_gender; }
            set { _beneficiary_gender = value; }
        }

        private string _beneficiary_Marital_Status;
        public string beneficiary_Marital_Status
        {
            get { return _beneficiary_Marital_Status; }
            set { _beneficiary_Marital_Status = value; }
        }

        // Used for a Single Plan
        private decimal _beneficiary_percentage;
        public decimal beneficiary_percentage
        {
            get { return _beneficiary_percentage; }
            set { _beneficiary_percentage = value; }
        }

        private string _suppress_warning;
        public string suppress_warning
        {
            get { return _suppress_warning; }
            set { _suppress_warning = value; }
        }

        public string istrIsAddressActive { get; set; }

        //UCS 53 auto refund batch properties
        public string istrZipCode
        {
            get
            {
                string lstrZipCode = string.Empty;
                if (!String.IsNullOrEmpty(address_zip_code))
                    lstrZipCode = address_zip_code.ToString();
                if (!String.IsNullOrEmpty(address_zip_4_code))
                    lstrZipCode = lstrZipCode  +" "+ address_zip_4_code.ToString();
                return lstrZipCode;
            }
        }

        public string bene_id { get; set; } // Payee Death Letter

        public string beneficiary_address_line_1 { get; set; }

        public string beneficiary_address_line_2 { get; set; }

        public string beneficiary_city { get; set; }

        public string beneficiary_state { get; set; }

        public string beneficiary_state_abbr { get; set; }

        public string beneficiary_zip { get; set; }
    }
} 
