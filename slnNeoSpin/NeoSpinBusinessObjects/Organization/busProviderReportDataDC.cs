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
	public class busProviderReportDataDC : busProviderReportDataDCGen
	{
        private decimal _ldclTotalContributionAmount;
        public decimal ldclTotalContributionAmount
        {
            get { return _ldclTotalContributionAmount; }
            set { _ldclTotalContributionAmount = value; }
        }
        private decimal _total_contribution_person;
        public decimal total_contribution_person
        {
            get
            {
                return _total_contribution_person;

            }
            set
            {
                _total_contribution_person = value;
            }
        }
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

        private string _lstrTotalContributionAmount;
        public string lstrTotalContributionAmount
        {
            get
            {
                decimal ldclTemp = 0.0M;
                string lstrKey, lstrValue;
                /// Loads the HashTable Values
                LoadSigns();
                /// To Display the amount field as mentioned in Appendix 036-1.xls. Ex: 120.01 would be like 000000001200A
                if (_ldclTotalContributionAmount < 0)
                {
                    ldclTemp = _ldclTotalContributionAmount * (-100);
                    _lstrTotalContributionAmount = ldclTemp.ToString("#");                    
                    _lstrTotalContributionAmount = _lstrTotalContributionAmount.PadLeft(13, '0');
                    lstrKey = _lstrTotalContributionAmount.Substring(_lstrTotalContributionAmount.Length - 1, 1);
                    _lstrTotalContributionAmount = _lstrTotalContributionAmount.Substring(0, _lstrTotalContributionAmount.Length - 1);
                    lstrValue = Convert.ToString(lhstNegativeValues[lstrKey]);
                    _lstrTotalContributionAmount = _lstrTotalContributionAmount + lstrValue;
                }
                else
                {
                    ldclTemp = _ldclTotalContributionAmount * 100;
                    _lstrTotalContributionAmount = ldclTemp.ToString("#");
                    _lstrTotalContributionAmount = _lstrTotalContributionAmount.PadLeft(13, '0');
                    lstrKey = _lstrTotalContributionAmount.Substring(_lstrTotalContributionAmount.Length - 1, 1);
                    _lstrTotalContributionAmount = _lstrTotalContributionAmount.Substring(0, _lstrTotalContributionAmount.Length - 1);
                    lstrValue = Convert.ToString(lhstPositiveValues[lstrKey]);
                    _lstrTotalContributionAmount = _lstrTotalContributionAmount + lstrValue;
                }
                return _lstrTotalContributionAmount;
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
                if (_ldclTotalContributionAmount < 0)
                {
                    ldclTemp = _ldclTotalContributionAmount * (-100);
                    _lstrTotalContributionAmountForNegative = ldclTemp.ToString("#");
                    _lstrTotalContributionAmountForNegative = _lstrTotalContributionAmountForNegative.PadLeft(11, '0');
                    lstrKey = _lstrTotalContributionAmountForNegative.Substring(_lstrTotalContributionAmountForNegative.Length - 1, 1);
                    _lstrTotalContributionAmountForNegative = _lstrTotalContributionAmountForNegative.Substring(0, _lstrTotalContributionAmountForNegative.Length - 1);
                    lstrValue = Convert.ToString(lhstNegativeValues[lstrKey]);
                    _lstrTotalContributionAmountForNegative = _lstrTotalContributionAmountForNegative + lstrValue;
                }
                else
                {
                    ldclTemp = _ldclTotalContributionAmount * (100);
                    _lstrTotalContributionAmountForNegative = ldclTemp.ToString("#");
                    _lstrTotalContributionAmountForNegative = _lstrTotalContributionAmountForNegative.PadLeft(11, '0');
                }
                return _lstrTotalContributionAmountForNegative;
            }
        }
        public int PostContribution(string astrSubSystem, int aintRefID, string astrSSN, int aintPersonID, string astrLastName, string astrFirstName,
            int aintPlanID, DateTime adatEffective, decimal adecPostTaxERAmt, decimal adecPostTaxEEAmt, decimal adecPreTaxERAmt,
            decimal adecPreTaxEEAmt, decimal adecPickupAmt, decimal adecInterestAmt, decimal adecMemberInterest, int aintPersonAccountID,int aintProviderOrgID)
        {
            cdoProviderReportDataDc lcdoProviderReportDataDC = new cdoProviderReportDataDc();
            lcdoProviderReportDataDC.subsystem_value = astrSubSystem;
            lcdoProviderReportDataDC.subsystem_ref_id = aintRefID;
            lcdoProviderReportDataDC.ssn = astrSSN;
            lcdoProviderReportDataDC.person_id = aintPersonID;
            lcdoProviderReportDataDC.last_name = astrLastName;
            lcdoProviderReportDataDC.first_name = astrFirstName;
            lcdoProviderReportDataDC.provider_org_id = aintProviderOrgID;
            lcdoProviderReportDataDC.plan_id = aintPlanID;
            lcdoProviderReportDataDC.effective_date = adatEffective;
            lcdoProviderReportDataDC.ee_contribution = adecPostTaxEEAmt;
            lcdoProviderReportDataDC.ee_employer_pickup = adecPickupAmt;
            lcdoProviderReportDataDC.ee_pre_tax = adecPreTaxEEAmt;
            lcdoProviderReportDataDC.er_contribution = adecPreTaxERAmt;
            lcdoProviderReportDataDC.employer_interest = adecInterestAmt;
            lcdoProviderReportDataDC.member_interest = adecMemberInterest;
            lcdoProviderReportDataDC.employer_rhic_interest = 0.00M;
            return lcdoProviderReportDataDC.Insert();
        }

        //prod pir 7691
        public DateTime idtNextRunDate { get; set; }
        // PIR 24921
        public string lstrProviderName { get; set; }
        public decimal idecPre_tax_amount
        {
            get
            {
                if (icdoProviderReportDataDc.IsNotNull())
                {
                    return (icdoProviderReportDataDc.ee_pre_tax + icdoProviderReportDataDc.ee_employer_pickup);

                }
                return decimal.Zero;
            }
        }
        // PIR 24921
        public string lstrLastFourDigitsOfSSN
        {
            get
            {
                if ((icdoProviderReportDataDc.ssn != null) && (icdoProviderReportDataDc.ssn.Length == 9))
                {
                    return icdoProviderReportDataDc.ssn.Substring(5);

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
                sb.Append(this.icdoProviderReportDataDc.last_name);
                if (this.icdoProviderReportDataDc.first_name != null && this.icdoProviderReportDataDc.first_name.Trim() != "")
                    sb.Append(seperator + this.icdoProviderReportDataDc.first_name);
                return sb.ToString();
            }
        }
        // PIR 24921
        public string lstrProviderNamebyOrgid
        {
            get
            {
                if (icdoProviderReportDataDc.provider_org_id != 0)
                    lstrProviderName = busGlobalFunctions.GetOrgNameByOrgID(icdoProviderReportDataDc.provider_org_id);
                return lstrProviderName;
            }
        }

    }
}
