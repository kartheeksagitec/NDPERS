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
	public class cdoPersonAccountFlexcompConversion : doPersonAccountFlexcompConversion
	{
		public cdoPersonAccountFlexcompConversion() : base()
		{
		}
        private string _istrOrgCodeID;
        public string istrOrgCodeID
        {
            get { return _istrOrgCodeID; }
            set { _istrOrgCodeID = value; }
        }

        public DateTime effective_end_date_no_null
        {
            get
            {
                if (effective_end_date == DateTime.MinValue)
                    return DateTime.MaxValue;
                return effective_end_date;
            }
        }

        // PROD PIR 9531
        public string is_pretax_premium
        {
            get
            {
                if (effective_start_date != DateTime.MinValue && effective_end_date_no_null >= DateTime.Now)
                    return BusinessObjects.busConstant.Flag_Yes_Value;
                return BusinessObjects.busConstant.Flag_No_Value;
            }
        }

    }
} 
