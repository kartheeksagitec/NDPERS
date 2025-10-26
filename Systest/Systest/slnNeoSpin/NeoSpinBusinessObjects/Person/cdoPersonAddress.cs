#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;
using Sagitec.BusinessObjects;
using NeoSpin.BusinessObjects;
#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
    public class cdoPersonAddress : doPersonAddress
    {
        public cdoPersonAddress()
            : base()
        {
        }

        private string _county;
        public string county
        {
            get
            {
                return _county;
            }

            set
            {
                _county = value;
            }
        }

        public string foriegn_address_flag
        {
            get
            {
                if ((foreign_postal_code != null) || (foreign_postal_code != null))
                {
                    return busConstant.Flag_Yes;
                }
                else
                {
                    return busConstant.Flag_No;
                }
            }
        }

        // Used in HIPAA - CIGNA, Benefit Option Code
        public string lstr_Benefit_Option_Code
        {
            get
            {
                if (addr_state_value == busConstant.AddressStateTexas)
                    return "25DEN";
                else if (addr_state_value == busConstant.AddressStateUtah)
                    return "26DEN";
                return "DENT";
            }
        }

        public string istrAddressStreet1_CAPS
        {
            get
            {
                return addr_line_1.IsNullOrEmpty() ?addr_line_1 : addr_line_1.ToUpper();
            }
        }
        public string istrAddressStreet2_CAPS
        {
            get
            {
                return addr_line_2.IsNullOrEmpty() ? addr_line_2 : addr_line_2.ToUpper();
            }
        }
        public string istrAddressCity_CAPS
        {
            get
            {
                return addr_city.IsNullOrEmpty() ? addr_city : addr_city.ToUpper();
            }
        }
        //pir 8540
        public string istrCountryDescription_CAPS
        { 
            get 
            { 
                return string.IsNullOrEmpty(addr_country_description) ? string.Empty : addr_country_description.ToUpper(); 
            } 
        }
        public string istrStateDescription_CAPS
        {
            get
            {
                return string.IsNullOrEmpty(addr_state_description) ? string.Empty : addr_state_description.ToUpper();
            }
        }
        public string istrPersonAddress
        {
            get
            {
                string lstrValue = string.Empty;
                if (!string.IsNullOrEmpty(addr_line_1))
                    lstrValue = addr_line_1;
                if (!string.IsNullOrEmpty(addr_line_2))
                    lstrValue += " " + addr_line_2;
                return lstrValue;
            }
        }
        public string istrCityStateZIPAddress
        {
            get
            {
                string lstrValue = string.Empty;
                if (!string.IsNullOrEmpty(addr_city))
                    lstrValue = addr_city;
                if (!string.IsNullOrEmpty(addr_state_description))
                    lstrValue += ", " + addr_state_description;
                if (!string.IsNullOrEmpty(addr_zip_code))
                    lstrValue += ", and " + addr_zip_code;
                if (!string.IsNullOrEmpty(addr_zip_4_code))
                    lstrValue += " " + addr_zip_4_code;
                return lstrValue;
            }
        }
    }
}