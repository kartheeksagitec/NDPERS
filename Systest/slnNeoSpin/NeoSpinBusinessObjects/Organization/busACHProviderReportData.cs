#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busACHProviderReportData : busExtendBase
    {
        private string _lstrRoutingNumber;
        public string lstrRoutingNumber
        {
            get { return _lstrRoutingNumber; }
            set { _lstrRoutingNumber = value; }
        }

        private string _lstrDFIAccountNo;
        public string lstrDFIAccountNo
        {
            get { return _lstrDFIAccountNo; }
            set { _lstrDFIAccountNo = value; }
        }
        public string istrRoutingNumberFirstEightDigits { get; set; }
        //Property to store last digit of routing number
        public string istrCheckLastDigit { get; set; }

        public string istrDetailCount { get; set; }

        public void LoadDFIAccountNo(int AintOrgID)
        {
            DataTable ldtbLists = Select("cdoOrgBank.GetDepositAccountNo", new object[1] { AintOrgID });
            if (ldtbLists.Rows.Count > 0)
            {
                if (ldtbLists.Rows[0]["ACCOUNT_NO"] != DBNull.Value)
                {
                    _lstrDFIAccountNo = Convert.ToString(ldtbLists.Rows[0]["ACCOUNT_NO"]);
                }
                if (ldtbLists.Rows[0]["ROUTING_NO"] != DBNull.Value)
                {
                    _lstrRoutingNumber = Convert.ToString(ldtbLists.Rows[0]["ROUTING_NO"]);
                    istrRoutingNumberFirstEightDigits = lstrRoutingNumber.Substring(0, lstrRoutingNumber.Length - 1).PadLeft(8, '0');
                }
            }
        }

        public void LoadDFIAccountNoByPERSLinkID(int AintPersonAccountID)
        {
            DataTable ldtbLists = Select<cdoPersonAccountAchDetail>(new string[2] { "PERSON_ACCOUNT_ID", "PRE_NOTE_FLAG" }, 
                            new object[2] { AintPersonAccountID ,busConstant.Flag_No}, null, "ach_start_date desc");
            if (ldtbLists.Rows.Count > 0)
            {
                if ((ldtbLists.Rows[0]["BANK_ACCOUNT_NUMBER"] != DBNull.Value) &&
                   (ldtbLists.Rows[0]["ABA_NUMBER"] != DBNull.Value))
                {
                    _lstrDFIAccountNo = Convert.ToString(ldtbLists.Rows[0]["BANK_ACCOUNT_NUMBER"]);
                    _lstrRoutingNumber = Convert.ToString(ldtbLists.Rows[0]["ABA_NUMBER"]);
                }
            }
        }

        private decimal _ldclContributionAmount;
        public decimal ldclContributionAmount
        {
            get { return _ldclContributionAmount; }
            set { _ldclContributionAmount = value; }
        }

        public string ldclContributionAmountFormatted
        {
            get
            {
                string ldecAmount = (Convert.ToInt32((ldclContributionAmount * 100))).ToString().PadLeft(10, '0');
                return ldecAmount; 
            }
        }
        private string _lstrOrgCodeID;
        public string lstrOrgCodeID
        {
            get { return _lstrOrgCodeID; }
            set { _lstrOrgCodeID = value; }
        }

        private string _lstrOrgName;
        public string lstrOrgName
        {
            get 
            {
                if (_lstrOrgName != string.Empty) 
                {
                    if (_lstrOrgName.Length > 22)
                        _lstrOrgName = _lstrOrgName.Trim().Substring(0, 22);
                    else
                        _lstrOrgName = _lstrOrgName.Trim().ToUpper();
                }
                return _lstrOrgName; 
            }
            set { _lstrOrgName = value; }
        }

        private string _lstrReportType;
        public string lstrReportType
        {
            get { return _lstrReportType; }
            set { _lstrReportType = value; }
        }

        /// Used in UCS - 033 Pull ACH File

        private string _lstrTransactionCode;
        public string lstrTransactionCode
        {
            get { return _lstrTransactionCode; }
            set { _lstrTransactionCode = value; }
        }

        private int _lintPERSLinkID;
        public int lintPERSLinkID
        {
            get { return _lintPERSLinkID; }
            set { _lintPERSLinkID = value; }
        }

        private string _lstrPERSLinkID;
        public string lstrPERSLinkID
        {
            get 
            {
                if (_lintPERSLinkID != 0)
                    _lstrPERSLinkID = Convert.ToString(_lintPERSLinkID).PadLeft(15, '0');
                return _lstrPERSLinkID; 
            }
        }

        public string istrOrgCode { get; set; }

        public string istrOrgName { get; set; }

        private string _lstrPersonName;
        public string lstrPersonName
        {
            get 
            {
                _lstrPersonName = string.Empty;
                if (_lintPERSLinkID != 0)
                {
                    busPerson lobjPerson = new busPerson();
                    lobjPerson.FindPerson(_lintPERSLinkID);
                    _lstrPersonName = lobjPerson.icdoPerson.PersonNameForSorting;
                    if (_lstrPersonName.Length > 20)
                        _lstrPersonName = _lstrPersonName.Substring(0, 20);
                }
                return _lstrPersonName; 
            }
        }


    }
}
