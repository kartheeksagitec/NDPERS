#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;
using NeoSpin.BusinessObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
	public class cdoCase : doCase
	{
		public cdoCase() : base()
		{
		}

        public string istrPlanName { get; set; }

        public int iintPlanId { get; set; }

        public bool isImageDataCollectionChanged { get; set; }

        public string action_by_user { get; set; }

        public string recertification_long_date
        {
            get
            {
                return recertification_date.ToString(busConstant.DateFormatLongDate);
            }
        }
    }
}
