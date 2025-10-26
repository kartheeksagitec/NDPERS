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
	public class cdoPersonAccountLtcOption : doPersonAccountLtcOption
	{
		public cdoPersonAccountLtcOption() : base()
		{
		}

        public DateTime effective_end_date_no_null
        {
            get
            {
                if (effective_end_date == DateTime.MinValue)
                    return DateTime.MaxValue;
                else
                    return effective_end_date;
            }
        }
    } 
} 
