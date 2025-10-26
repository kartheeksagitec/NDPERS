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
	/// <summary>
	/// Class NeoSpin.BusinessObjects.busProviderReportDataMedicarePartD:
	/// Inherited from busProviderReportDataMedicarePartDGen, the class is used to customize the business object busProviderReportDataMedicarePartDGen.
	/// </summary>
	[Serializable]
	public class busProviderReportDataMedicarePartD : busProviderReportDataMedicarePartDGen
	{
        public string lstrProviderName { get; set; }
        public string lstrLastFourDigitsOfSSN
        {
            get
            {
                if ((icdoProviderReportDataMedicare.ssn != null) && (icdoProviderReportDataMedicare.ssn.Length == 9))
                {
                    return icdoProviderReportDataMedicare.ssn.Substring(5);

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
                sb.Append(this.icdoProviderReportDataMedicare.last_name);
                if (this.icdoProviderReportDataMedicare.first_name != null && this.icdoProviderReportDataMedicare.first_name.Trim() != "")
                    sb.Append(seperator + this.icdoProviderReportDataMedicare.first_name);
                return sb.ToString();
            }
        }
        // PIR 24921
        public string lstrProviderNamebyOrgid
        {
            get
            {
                if (icdoProviderReportDataMedicare.provider_org_id != 0)
                    lstrProviderName = busGlobalFunctions.GetOrgNameByOrgID(icdoProviderReportDataMedicare.provider_org_id);
                return lstrProviderName;
            }
        }
    }
}
