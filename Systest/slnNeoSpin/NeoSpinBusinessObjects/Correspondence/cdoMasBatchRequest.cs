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
	/// Class NeoSpin.CustomDataObjects.cdoMasBatchRequest:
	/// Inherited from doMasBatchRequest, the class is used to customize the database object doMasBatchRequest.
	/// </summary>
    [Serializable]
	public class cdoMasBatchRequest : doMasBatchRequest
	{
		public cdoMasBatchRequest() : base()
		{
		}

        public DateTime annual_statement_effective_date { get; set; }

        public DateTime individual_statement_effective_date { get; set; }

        public DateTime targeted_statement_effective_date { get; set; }

        public int retired_person_id { get; set; }

        public DateTime retiree_annual_statement_effective_date { get; set; }

        public DateTime retiree_individual_statement_effective_date { get; set; }

        public DateTime retiree_targeted_statement_effective_date { get; set; }

        public int record_count { get; set; }

        public int statement_year
        {
            get
            {
                return statement_effective_date.Year;
            }
        }

        public string org_code_non_retired { get; set; }

        public string org_code_retired { get; set; }
    }
}
