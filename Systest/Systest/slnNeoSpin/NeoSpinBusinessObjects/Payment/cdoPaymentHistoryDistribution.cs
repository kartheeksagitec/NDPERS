#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;
using Sagitec.BusinessObjects;
using NeoSpin.BusinessObjects;
using System.Text.RegularExpressions;
#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
    public class cdoPaymentHistoryDistribution : doPaymentHistoryDistribution
    {
        public cdoPaymentHistoryDistribution()
            : base()
        {

        }

        private string _addr_description;
        public string addr_description
        {
            get
            {
                _addr_description = string.Empty;
                if (addr_line_1 != null)
                {
                    _addr_description += addr_line_1 + ", ";
                }
                if (addr_line_2 != null)
                {
                    _addr_description += addr_line_2 + ", ";
                }
                if (addr_country_value == busConstant.US_Code_ID)
                {
                    if (addr_city != null)
                    {
                        _addr_description += addr_city + ", ";
                    }
                    if (addr_state_value != null)
                    {
                        _addr_description += addr_state_value + " ";
                    }
                    if (addr_zip_code != null)
                    {
                        _addr_description += addr_zip_code;
                    }
                    if (addr_zip_4_code != null)
                    {
                        _addr_description += "-" + addr_zip_4_code;
                    }
                }
                else
                {
                    if (addr_state_description != null)
                    {
                        _addr_description += addr_state_description + ", ";
                    }

                    if (!String.IsNullOrEmpty(foreign_province))
                    {
                        _addr_description += foreign_province + ", ";
                    }

                    if (!String.IsNullOrEmpty(foreign_postal_code))
                    {
                        _addr_description += foreign_postal_code + " ";
                    }

                    if (addr_country_description != null)
                    {
                        _addr_description += addr_country_description;
                    }
                }
                return _addr_description;
            }
        }
        public string status_value { get; set; }
        public string status_description { get; set; }
        public string status_effective_date { get; set; }

        //For MSS Layout change
        public string scrambled_account_number
        {
            get
            {
                string acc_number_part = String.Empty;
                string scrambled_part = String.Empty;
                string scrambled = String.Empty;

                if (!String.IsNullOrEmpty(account_number))
                {
                    //If Account Number's length is less than 4, then pad it with leading 0's. 
                    //Scramble the remaining Account Number upto 8 chars.
                    if (account_number.Length < 4)
                        account_number = account_number.PadLeft(4, '0');
                    acc_number_part = account_number.Trim().Right(4);
                    scrambled_part = account_number.Trim().Left(account_number.Trim().Length - 4);
                    scrambled = acc_number_part.PadLeft(12, 'X');
                }

                return scrambled;

            }
        }
    }
}