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
	public class cdoOrgPlanProvider : doOrgPlanProvider
	{
		public cdoOrgPlanProvider() : base()
		{

		}

        public string istrPlanRateLink { get; set; }

        private string _ProviderOrgName;
        public string ProviderOrgName
        {
            get { return _ProviderOrgName; }
            set { _ProviderOrgName = value; }
        }
        private string _istrOrgCodeID;

        public string istrOrgCodeID
        {
            get { return _istrOrgCodeID; }
            set { _istrOrgCodeID = value; }
        }

    } 
} 
