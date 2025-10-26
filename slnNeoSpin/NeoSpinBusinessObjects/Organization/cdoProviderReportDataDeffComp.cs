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
	public class cdoProviderReportDataDeffComp : doProviderReportDataDeffComp
	{
		public cdoProviderReportDataDeffComp() : base()
		{
		}

        private string _lstrSymetraContributionAmount;
        public string lstrSymetraContributionAmount
        {
            get 
            {
                decimal ldclTemp = 0.0M;
                if (contribution_amount < 0)
                {
                    ldclTemp = contribution_amount * (-1);
                    _lstrSymetraContributionAmount = ldclTemp.ToString();
                    _lstrSymetraContributionAmount = _lstrSymetraContributionAmount.PadLeft(10, '0');
                    _lstrSymetraContributionAmount = "-" + _lstrSymetraContributionAmount;
                }
                else
                {
                    _lstrSymetraContributionAmount = contribution_amount.ToString();
                    _lstrSymetraContributionAmount = _lstrSymetraContributionAmount.PadLeft(11, '0');
                }
                return _lstrSymetraContributionAmount; 
            }
        }

        private string _lstrJacksonContributionAmount;
        public string lstrJacksonContributionAmount
        {
            get
            {
                decimal ldclTemp = 0.0M;
                if (contribution_amount < 0)
                {
                    ldclTemp = contribution_amount * (-100);
                    _lstrJacksonContributionAmount = ldclTemp.ToString("#");
                    _lstrJacksonContributionAmount = _lstrJacksonContributionAmount.PadLeft(6, '0');
                    _lstrJacksonContributionAmount = "-" + _lstrJacksonContributionAmount;
                }
                else
                {
                    ldclTemp = contribution_amount * 100;
                    _lstrJacksonContributionAmount = ldclTemp.ToString("#");
                    _lstrJacksonContributionAmount = _lstrJacksonContributionAmount.PadLeft(7, '0');
                }
                return _lstrJacksonContributionAmount;
            }
        }

        private string _lstrKemperContributionAmount;
        public string lstrKemperContributionAmount
        {
            get
            {
                decimal ldclTemp = 0.00M;
                if (contribution_amount < 0)
                {
                    ldclTemp = contribution_amount * (-1);
                    _lstrKemperContributionAmount = ldclTemp.ToString();
                    _lstrKemperContributionAmount = _lstrKemperContributionAmount.PadLeft(9, '0');
                    _lstrKemperContributionAmount = "-" + _lstrKemperContributionAmount;
                }
                else
                {
                    _lstrKemperContributionAmount = contribution_amount.ToString();
                    _lstrKemperContributionAmount = _lstrKemperContributionAmount.PadLeft(10, '0');
                }
                return _lstrKemperContributionAmount;
            }
        }

        private string _lstrLincolnContributionAmount;
        public string lstrLincolnContributionAmount
        {
            get
            {
                decimal ldclTemp = 0.0M;
                if (contribution_amount < 0)
                {
                    ldclTemp = contribution_amount * (-100);
                    _lstrLincolnContributionAmount = ldclTemp.ToString("#");
                    _lstrLincolnContributionAmount = _lstrLincolnContributionAmount.PadLeft(6, '0');
                    _lstrLincolnContributionAmount = "-" + _lstrLincolnContributionAmount;
                }
                else
                {
                    ldclTemp = contribution_amount * 100;
                    _lstrLincolnContributionAmount = ldclTemp.ToString("#");
                    _lstrLincolnContributionAmount = _lstrLincolnContributionAmount.PadLeft(7, '0');
                }
                return _lstrLincolnContributionAmount;
            }
        }

        private string _lstrHartFordContributionAmount;
        public string lstrHartFordContributionAmount
        {
            get
            {
                decimal ldclTemp = 0.0M;
                if (contribution_amount < 0)
                {
                    ldclTemp = contribution_amount * (-1);
                    _lstrHartFordContributionAmount = ldclTemp.ToString();
                    _lstrHartFordContributionAmount = _lstrHartFordContributionAmount.PadLeft(9, '0');
                    _lstrHartFordContributionAmount = "-" + _lstrHartFordContributionAmount;
                }
                else
                {
                    _lstrHartFordContributionAmount = contribution_amount.ToString();
                    _lstrHartFordContributionAmount = _lstrHartFordContributionAmount.PadLeft(10, '0');
                }
                return _lstrHartFordContributionAmount;
            }
        }

        private string _lstrKansasContributionAmount;
        public string lstrKansasContributionAmount
        {
            get
            {
                decimal ldclTemp = 0.0M;
                if (contribution_amount < 0)
                {
                    ldclTemp = contribution_amount * (-100);
                    _lstrKansasContributionAmount = ldclTemp.ToString("#");
                    _lstrKansasContributionAmount = _lstrKansasContributionAmount.PadLeft(14, '0');
                    _lstrKansasContributionAmount = "-" + _lstrKansasContributionAmount;
                }
                else
                {
                    ldclTemp = contribution_amount * 100;
                    _lstrKansasContributionAmount = ldclTemp.ToString("#");
                    _lstrKansasContributionAmount = _lstrKansasContributionAmount.PadLeft(15, '0');
                }
                return _lstrKansasContributionAmount;
            }
        }

       // private int person_id { get; set; }
        private string _lstrNationWideContributionAmount;
        public string lstrNationWideContributionAmount
        {
            get
            {
                decimal ldclTemp = 0.0M;
                if (contribution_amount < 0)
                {
                    ldclTemp = contribution_amount * (-100);
                    _lstrNationWideContributionAmount = ldclTemp.ToString("#");
                    _lstrNationWideContributionAmount = _lstrNationWideContributionAmount.PadLeft(11, '0');
                    _lstrNationWideContributionAmount = "-" + _lstrNationWideContributionAmount;
                }
                else
                {
                    ldclTemp = contribution_amount * 100;
                    _lstrNationWideContributionAmount = ldclTemp.ToString("#");
                    _lstrNationWideContributionAmount = _lstrNationWideContributionAmount.PadLeft(12, '0');
                }
                return _lstrNationWideContributionAmount;
            }
        }

        private string _lstrValicContributionAmount;
        public string lstrValicContributionAmount
        {
            get
            {
                decimal ldclTemp = 0.0M;
                if (contribution_amount < 0)
                {
                    ldclTemp = contribution_amount * (-100);
                    _lstrValicContributionAmount = ldclTemp.ToString("#");
                    _lstrValicContributionAmount = _lstrValicContributionAmount.PadLeft(6, '0');
                    _lstrValicContributionAmount = "-" + _lstrValicContributionAmount;
                }
                else
                {
                    ldclTemp = contribution_amount * 100;
                    _lstrValicContributionAmount = ldclTemp.ToString("#");
                    _lstrValicContributionAmount = _lstrValicContributionAmount.PadLeft(7, '0');
                }
                return _lstrValicContributionAmount;
            }
        }

        private string _istrValicLastName;
        public string istrValicLastName
        {
            get 
            {
                _istrValicLastName = string.Empty;
                if (!string.IsNullOrEmpty(last_name))
                {
                    if (last_name.Length > 13)
                        _istrValicLastName = last_name.Trim().ToUpper().Substring(0, 13);
                    else
                        _istrValicLastName = last_name.Trim().ToUpper();
                }
                return _istrValicLastName; 
            }
        }

        private string _lstrINGContributionAmount;
        public string lstrINGContributionAmount
        {
            get
            {
                if (contribution_amount < 0)
                {
                    decimal ldclTemp = contribution_amount * (-1);
                    _lstrINGContributionAmount = ldclTemp.ToString();
                    _lstrINGContributionAmount = _lstrINGContributionAmount.PadLeft(10, '0');
                    _lstrINGContributionAmount = "-" + _lstrINGContributionAmount;
                }
                else
                {
                    _lstrINGContributionAmount = contribution_amount.ToString();
                    _lstrINGContributionAmount = _lstrINGContributionAmount.PadLeft(11, '0');
                }
                return _lstrINGContributionAmount;
            }
        }

        private string _lstrWaddelReedContributionAmount;
        public string lstrWaddelReedContributionAmount
        {
            get
            {
                if (contribution_amount < 0)
                {
                    decimal ldclTemp = contribution_amount * (-100);
                    _lstrWaddelReedContributionAmount = Convert.ToInt32(ldclTemp).ToString();
                    _lstrWaddelReedContributionAmount = _lstrWaddelReedContributionAmount.PadLeft(9, '0');
                    _lstrWaddelReedContributionAmount = "-" + _lstrWaddelReedContributionAmount;
                }
                else
                {
                    decimal ldclTemp = contribution_amount * (100);
                    _lstrWaddelReedContributionAmount = Convert.ToInt32(ldclTemp).ToString();
                    _lstrWaddelReedContributionAmount = _lstrWaddelReedContributionAmount.PadLeft(10, '0');
                }
                return _lstrWaddelReedContributionAmount;
            }
        }

        private string _lstrBNDContributionAmount;
        public string lstrBNDContributionAmount
        {
            get
            {
                if (contribution_amount < 0)
                {
                    decimal ldclTemp = contribution_amount * (-100);
                    _lstrBNDContributionAmount = Convert.ToInt32(ldclTemp).ToString();
                    _lstrBNDContributionAmount = _lstrBNDContributionAmount.PadLeft(9, '0');
                    _lstrBNDContributionAmount = "-" + _lstrBNDContributionAmount;
                }
                else
                {
                    decimal ldclTemp = contribution_amount * (100);
                    _lstrBNDContributionAmount = Convert.ToInt32(ldclTemp).ToString();
                    _lstrBNDContributionAmount = _lstrBNDContributionAmount.PadLeft(10, '0');
                }
                return _lstrBNDContributionAmount;
            }
        }

        private string _lstrAmericanTrustContributionAmount;
        public string lstrAmericanTrustContributionAmount
        {
            get
            {
                if (contribution_amount < 0)
                {
                    decimal ldclTemp = contribution_amount * (-1);
                    _lstrAmericanTrustContributionAmount = ldclTemp.ToString();
                    _lstrAmericanTrustContributionAmount = _lstrAmericanTrustContributionAmount.PadLeft(7, '0');
                    _lstrAmericanTrustContributionAmount = "-" + _lstrAmericanTrustContributionAmount;
                }
                else
                {
                    _lstrAmericanTrustContributionAmount = contribution_amount.ToString();
                    _lstrAmericanTrustContributionAmount = _lstrAmericanTrustContributionAmount.PadLeft(8, '0');
                }
                return _lstrAmericanTrustContributionAmount;
            }
        }
        public string org_code { get; set; }

        public string istrDetailCounter { get; set; }

        public string istrSSNFormatted17Chr
        {
            get
            {
                if (string.IsNullOrEmpty(ssn))
                    return "0".PadLeft(17, '0');
                else
                    return ssn.PadLeft(17, '0');
            }
        }

        public string istrSSNFormatted13Chr
        {
            get
            {
                if (string.IsNullOrEmpty(ssn))
                    return "0".PadRight(13, '0');
                else
                    return ssn.PadRight(13, '0');
            }
        }

        //for aig valic file
        public string istrSSNFormattedWithDashes
        {
            get
            {
                if (!string.IsNullOrEmpty(ssn) && ssn.Length >= 9)
                {
                    string lstrSSN = ssn.Substring(0, 3) + "-" + ssn.Substring(3, 2) + "-" + ssn.Substring(5, 4);
                    return lstrSSN;
                }
                else
                    return "000-00-0000";
            }
        }

        public decimal total_contribution { get; set; }

        //PIR 26240 Update the Nationwide Plan number to be 12 digits // update with field ach account number from org
        public string lstrOrgAccAccountNumber { get; set; }
    } 
} 
