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
	/// Class NeoSpin.CustomDataObjects.cdoRateChangeLetterRequest:
	/// Inherited from doRateChangeLetterRequest, the class is used to customize the database object doRateChangeLetterRequest.
	/// </summary>
    [Serializable]
	public class cdoRateChangeLetterRequest : doRateChangeLetterRequest
	{
		public cdoRateChangeLetterRequest() : base()
		{
		}

        public string istrOrgCodeId { get; set; }

        public int iintPlanId { get; set; }

        public string effective_long_date
        {
            get
            {
                return effective_date.ToString(BusinessObjects.busConstant.DateFormatLongDate);
            }
        }
    }
}
