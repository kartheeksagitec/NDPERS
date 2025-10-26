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
	public class cdoOrganization : doOrganization
	{
		public cdoOrganization() : base()
		{
		}
        //elayaraja :: Added for displaying custom format in the PayeeAccountMaitenance screen
        // Display as ‘OrgCode  Org Name'.
        public String RecipientOrg
        {
            get
            {
                const string lstrSeperator = ", ";
                const string lstrSpaceSeperator = " ";
                StringBuilder lsb = new StringBuilder();
                if (!String.IsNullOrEmpty(org_code))
                {
                    lsb.Append(org_code);
                }
                if (!String.IsNullOrEmpty(org_name))
                {
                    lsb.Append(lstrSpaceSeperator + org_name);
                }               
                return lsb.ToString();
            }
        }

        //UAT PIR 897 - to display confirmation message on change of routing number
        public string suppress_warnings_flag { get; set; }

        public string org_name_caps
        {
            get
            {
                return string.IsNullOrEmpty(org_name) ? string.Empty : org_name.ToUpper();
            }
        }
    
        public int PROVIDER_PLAN_ID_CHECK { get; set; }// PIR 7585
        public string istrProviderOrgName { get; set; } //PIR 26656

        public bool iblnIsRouitngNumberExists1 { get; set; } //PIR 18503
        public bool iblnIsRouitngNumberExists2 { get; set; } //PIR 18503
        public bool iblnIsRoutingNumberExists { get; set; }
        public string istrRoutingText { get; set; }
    } 
} 
