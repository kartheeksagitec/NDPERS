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
	public class busProviderReportDataDeffComp : busProviderReportDataDeffCompGen
	{
        private Hashtable lhstPositiveValues = new Hashtable();
        private Hashtable lhstNegativeValues = new Hashtable();

        private void LoadSigns()
        {
            lhstPositiveValues.Add("0", "{");
            lhstPositiveValues.Add("1", "A");
            lhstPositiveValues.Add("2", "B");
            lhstPositiveValues.Add("3", "C");
            lhstPositiveValues.Add("4", "D");
            lhstPositiveValues.Add("5", "E");
            lhstPositiveValues.Add("6", "F");
            lhstPositiveValues.Add("7", "G");
            lhstPositiveValues.Add("8", "H");
            lhstPositiveValues.Add("9", "I");

            lhstNegativeValues.Add("0", "}");
            lhstNegativeValues.Add("1", "J");
            lhstNegativeValues.Add("2", "K");
            lhstNegativeValues.Add("3", "L");
            lhstNegativeValues.Add("4", "M");
            lhstNegativeValues.Add("5", "N");
            lhstNegativeValues.Add("6", "O");
            lhstNegativeValues.Add("7", "P");
            lhstNegativeValues.Add("8", "Q");
            lhstNegativeValues.Add("9", "R");
        }
        public string lstrAXAContributionAmount
        {
            get
            {
                string lstrContributionAmount = string.Empty;
                decimal ldclTemp = 0.0M;
                /// To Display the amount field as mentioned in Appendix 036-1.xls. Ex: 120.01 would be like 000000001200A
                if (icdoProviderReportDataDeffComp.contribution_amount < 0)
                {
                    ldclTemp = icdoProviderReportDataDeffComp.contribution_amount * (-100);
                    lstrContributionAmount = ldclTemp.ToString("#");
                    lstrContributionAmount = lstrContributionAmount.PadLeft(9, '0');
                    lstrContributionAmount = lstrContributionAmount.PadLeft(10, '-'); // PIR 12211
                    lstrContributionAmount = lstrContributionAmount.PadRight(1, '0');
                }
                else
                {
                    ldclTemp = icdoProviderReportDataDeffComp.contribution_amount * 100;
                    lstrContributionAmount = ldclTemp.ToString("#");
                    lstrContributionAmount = lstrContributionAmount.PadLeft(10, '0');
                    lstrContributionAmount = lstrContributionAmount.PadRight(1, '0');
                }
                return lstrContributionAmount;
            }
        }
        private string _lstrContributionAmountBySign;
        public string lstrContributionAmountBySign
        {
            get
            {
                decimal ldclTemp = 0.0M;
                string lstrKey, lstrValue;
                /// Loads the HashTable Values
                LoadSigns();
                /// To Display the amount field as mentioned in Appendix 036-1.xls. Ex: 120.01 would be like 000000001200A
                if (icdoProviderReportDataDeffComp.contribution_amount < 0)
                {
                    ldclTemp = icdoProviderReportDataDeffComp.contribution_amount * (-100);
                    _lstrContributionAmountBySign = ldclTemp.ToString("#");                    
                    _lstrContributionAmountBySign = _lstrContributionAmountBySign.PadLeft(10, '0');
                    lstrKey = _lstrContributionAmountBySign.Substring(_lstrContributionAmountBySign.Length - 1, 1);
                    _lstrContributionAmountBySign = _lstrContributionAmountBySign.Substring(0, _lstrContributionAmountBySign.Length - 1);
                    lstrValue = Convert.ToString(lhstNegativeValues[lstrKey]);
                    _lstrContributionAmountBySign = _lstrContributionAmountBySign + lstrValue;
                }
                else
                {
                    ldclTemp = icdoProviderReportDataDeffComp.contribution_amount * 100;
                    _lstrContributionAmountBySign = ldclTemp.ToString("#");
                    _lstrContributionAmountBySign = _lstrContributionAmountBySign.PadLeft(10, '0');
                    lstrKey = _lstrContributionAmountBySign.Substring(_lstrContributionAmountBySign.Length - 1, 1);
                    _lstrContributionAmountBySign = _lstrContributionAmountBySign.Substring(0, _lstrContributionAmountBySign.Length - 1);
                    lstrValue = Convert.ToString(lhstPositiveValues[lstrKey]);
                    _lstrContributionAmountBySign = _lstrContributionAmountBySign + lstrValue;
                }
                if (icdoProviderReportDataDeffComp.org_code == busGlobalFunctions.GetData1ByCodeValue(5012, busConstant.Provider_Fidelity, iobjPassInfo))
                    _lstrContributionAmountBySign = _lstrContributionAmountBySign.PadLeft(13, '0');                
                return _lstrContributionAmountBySign;
            }
        }
        private string _lstrContributionAmountSign;
        public string lstrContributionAmountSign
        {
            get
            {
                decimal ldclTemp = 0.0M;
                string lstrKey, lstrValue;
                /// Loads the HashTable Values
                LoadSigns();
                /// To Display the amount field as mentioned in Appendix 036-1.xls. Ex: 120.01 would be like 000000001200A
                if (icdoProviderReportDataDeffComp.total_contribution < 0)
                {
                    ldclTemp = icdoProviderReportDataDeffComp.total_contribution * (-100);
                    _lstrContributionAmountSign = ldclTemp.ToString("#");
                    _lstrContributionAmountSign = _lstrContributionAmountBySign.PadLeft(10, '0');
                    lstrKey = _lstrContributionAmountBySign.Substring(_lstrContributionAmountBySign.Length - 1, 1);
                    _lstrContributionAmountBySign = _lstrContributionAmountBySign.Substring(0, _lstrContributionAmountBySign.Length - 1);
                    lstrValue = Convert.ToString(lhstNegativeValues[lstrKey]);
                    _lstrContributionAmountBySign = _lstrContributionAmountBySign + lstrValue;
                }
                else
                {
                    ldclTemp = icdoProviderReportDataDeffComp.total_contribution * 100;
                    _lstrContributionAmountSign = ldclTemp.ToString("#");
                    _lstrContributionAmountSign = _lstrContributionAmountSign.PadLeft(10, '0');
                    lstrKey = _lstrContributionAmountSign.Substring(_lstrContributionAmountSign.Length - 1, 1);
                    _lstrContributionAmountSign = _lstrContributionAmountSign.Substring(0, _lstrContributionAmountSign.Length - 1);
                    lstrValue = Convert.ToString(lhstPositiveValues[lstrKey]);
                    _lstrContributionAmountSign = _lstrContributionAmountSign + lstrValue;
                }
                if (icdoProviderReportDataDeffComp.org_code == busGlobalFunctions.GetData1ByCodeValue(5012, busConstant.Provider_Fidelity, iobjPassInfo))
                    _lstrContributionAmountSign = _lstrContributionAmountSign.PadLeft(13, '0');
                return _lstrContributionAmountSign;
            }
        }
        private string _lstrTotalContributionAmountForNegative;
        public string lstrTotalContributionAmountForNegative
        {
            get
            {
                decimal ldclTemp = 0.0M;
                string lstrKey, lstrValue;
                /// Loads the HashTable Values
                LoadSigns();
                /// To Display the amount field as mentioned in Appendix 036-1.xls. Ex: 120.01 would be like 000000001200A
                if (icdoProviderReportDataDeffComp.total_contribution < 0)
                {
                    ldclTemp = icdoProviderReportDataDeffComp.total_contribution * (-100);
                    _lstrTotalContributionAmountForNegative = ldclTemp.ToString("#");
                    _lstrTotalContributionAmountForNegative = _lstrTotalContributionAmountForNegative.PadLeft(9, '0');
                    //lstrKey = _lstrTotalContributionAmountForNegative.Substring(_lstrTotalContributionAmountForNegative.Length - 1, 1);
                    //_lstrTotalContributionAmountForNegative = _lstrTotalContributionAmountForNegative.Substring(0, _lstrTotalContributionAmountForNegative.Length - 1);
                    //lstrValue = Convert.ToString(lhstNegativeValues[lstrKey]);
                    //_lstrTotalContributionAmountForNegative = _lstrTotalContributionAmountForNegative + lstrValue;
                }
                else
                {
                    ldclTemp = icdoProviderReportDataDeffComp.total_contribution * (100);
                    _lstrTotalContributionAmountForNegative = ldclTemp.ToString("#");
                    _lstrTotalContributionAmountForNegative = _lstrTotalContributionAmountForNegative.PadLeft(9, '0');
                }
                return _lstrTotalContributionAmountForNegative;
            }
        }
        private string _istrFirstLastName;
        public string istrFirstLastName
        {
            get 
            {
                if (icdoProviderReportDataDeffComp.first_name != null)
                    _istrFirstLastName = icdoProviderReportDataDeffComp.first_name.Trim().ToUpper() + " ";
                if(icdoProviderReportDataDeffComp.last_name!=null)
                    _istrFirstLastName += icdoProviderReportDataDeffComp.last_name.Trim().ToUpper();
                return _istrFirstLastName; 
            }
        }

        private string _istrAmericanTrustName;
        public string istrAmericanTrustName
        {
            get
            {
                if (icdoProviderReportDataDeffComp.first_name != null)
                    _istrAmericanTrustName = icdoProviderReportDataDeffComp.first_name.Trim() + " ";
                if (icdoProviderReportDataDeffComp.last_name != null)
                    _istrAmericanTrustName += icdoProviderReportDataDeffComp.last_name.Trim();
                return _istrAmericanTrustName;
            }
        }

        private string _istrLastFirstName;
        public string istrLastFirstName
        {
            get
            {
                if (icdoProviderReportDataDeffComp.last_name != null)
                    _istrLastFirstName += icdoProviderReportDataDeffComp.last_name.Trim().ToUpper()+ ",";
                if (icdoProviderReportDataDeffComp.first_name != null)
                    _istrLastFirstName += icdoProviderReportDataDeffComp.first_name.Trim().ToUpper();
                if (!string.IsNullOrEmpty(_istrLastFirstName) && _istrLastFirstName.Length > 20)
                    _istrLastFirstName = _istrLastFirstName.Substring(0, 20);
                return _istrLastFirstName;
            }
        }

        private string _istrFirstInitial;
        public string istrFirstInitial
        {
            get 
            {
                _istrFirstInitial = string.Empty;
                if (icdoProviderReportDataDeffComp.first_name != null)
                    _istrFirstInitial = icdoProviderReportDataDeffComp.first_name.Substring(0, 1);
                return _istrFirstInitial; 
            }
        }

        private string _lstrDFIAccountNumber;
        public string lstrDFIAccountNumber
        {
            get 
            {
                _lstrDFIAccountNumber = "99001208";
                if (icdoProviderReportDataDeffComp.ssn != null)
                    _lstrDFIAccountNumber += icdoProviderReportDataDeffComp.ssn;
                return _lstrDFIAccountNumber; 
            }
        }

        private string _lstrSource;
        public string lstrSource
        {
            get { return _lstrSource; }
            set { _lstrSource = value; }
        }

        public void LoadSource()
        {
            string lstrMutualWindowFlag = Convert.ToString(DBFunction.DBExecuteScalar("cdoProviderReportDataDeffComp.GetSource", new object[0] { },
                                                        iobjPassInfo.iconFramework,iobjPassInfo.itrnFramework));
            if (lstrMutualWindowFlag == string.Empty)
                _lstrSource = "02";
            else 
                _lstrSource = lstrMutualWindowFlag;
        }

        public string FillerZero4
        {
            get { return "0000"; }
        }

        public DateTime payment_date { get; set; } // PROD PIR ID 5682
        public DateTime idtNextRunDate { get; set; }
        public string lstrProviderName { get; set; }// PIR 24921
        // PIR 24921
        public string lstrLastFourDigitsOfSSN
        {
            get
            {
                if ((icdoProviderReportDataDeffComp.ssn != null) && (icdoProviderReportDataDeffComp.ssn.Length == 9))
                {
                    return icdoProviderReportDataDeffComp.ssn.Substring(5);

                }
                return string.Empty;
            }
        }
        // PIR 24921
        public string lstrPersonName
        {
            get
            {
                string seperator = ", ";
                StringBuilder sb = new StringBuilder();
                sb.Append(this.icdoProviderReportDataDeffComp.last_name);
                if (this.icdoProviderReportDataDeffComp.first_name != null && this.icdoProviderReportDataDeffComp.first_name.Trim() != "")
                    sb.Append(seperator + this.icdoProviderReportDataDeffComp.first_name);
                return sb.ToString();
            }
        }
        // PIR 24921
        public string lstrProviderNamebyOrgid
        {
            get
            {
                if (icdoProviderReportDataDeffComp.provider_org_id != 0)
                    lstrProviderName = busGlobalFunctions.GetOrgNameByOrgID(icdoProviderReportDataDeffComp.provider_org_id);
                return lstrProviderName;
            }
        }

        public string istrFillerForFile
        {
            get
            {
                return " ";
            }
        }
    }
}
