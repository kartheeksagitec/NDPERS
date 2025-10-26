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
	public class busProviderReportDataInsurance : busProviderReportDataInsuranceGen
	{
        private int _lintAssignedNumber;
        public int lintAssignedNumber
        {
            get { return _lintAssignedNumber; }
            set { _lintAssignedNumber = value; }
        }

        public string lstrAssignedNumber
        {
            get { return lintAssignedNumber.ToString(); }
        }
        public string lstrProviderName { get; set; }
        public string lstrLastFourDigitsOfSSN
        {
            get
            {
                if ((icdoProviderReportDataInsurance.ssn != null) && (icdoProviderReportDataInsurance.ssn.Length == 9))
                {
                    return icdoProviderReportDataInsurance.ssn.Substring(5);

                }
                return string.Empty;
            }
        }
        public string lstrPersonName
        {
            get
            {
                string seperator = ", ";
                StringBuilder sb = new StringBuilder();
                sb.Append(this.icdoProviderReportDataInsurance.last_name);
                if (this.icdoProviderReportDataInsurance.first_name != null && this.icdoProviderReportDataInsurance.first_name.Trim() != "")
                    sb.Append(seperator + this.icdoProviderReportDataInsurance.first_name);
                return sb.ToString();
            }
        }
        // PIR 24921
        public string lstrProviderNamebyOrgid
        {
            get
            {
                if (icdoProviderReportDataInsurance.provider_org_id != 0)
                    lstrProviderName = busGlobalFunctions.GetOrgNameByOrgID(icdoProviderReportDataInsurance.provider_org_id);
                return lstrProviderName;
            }
        }
    }
}
