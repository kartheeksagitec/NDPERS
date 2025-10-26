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
	/// <summary>
	/// Class NeoSpin.CustomDataObjects.cdoUpdateOrgPlanProvider:
	/// Inherited from doUpdateOrgPlanProvider, the class is used to customize the database object doUpdateOrgPlanProvider.
	/// </summary>
    [Serializable]
	public class cdoUpdateOrgPlanProvider : doUpdateOrgPlanProvider
	{
		public cdoUpdateOrgPlanProvider() : base()
		{
		}

        public string employer_org_code { get; set; }

        public string from_provider_org_code { get; set; }

        public string to_provider_org_code { get; set; }       
    } 
} 
