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
	public class cdoPersonAccountGhdvHistory : doPersonAccountGhdvHistory
	{
		public cdoPersonAccountGhdvHistory() : base()
		{
		}

        public string plan_name { get; set; }

        public string istrCovergeCode { get; set; }

        public string rate_structure
        {
            get
            {
                if (!string.IsNullOrEmpty(overridden_structure_code)) 
                    return overridden_structure_code;
                if (!string.IsNullOrEmpty(rate_structure_code))
                    return rate_structure_code;
                return string.Empty;
            }
        }

        public string istrStartDateFormatted
        {
            get
            {
                if (start_date != DateTime.MinValue)
                    return start_date.ToString(BusinessObjects.busConstant.DateFormatLongDate);
                else
                    return string.Empty;
            }
        }

        public string istrStartDateMinusOneDayFormatted
        {
            get
            {
                if (start_date != DateTime.MinValue)
                    return start_date.AddDays(-1).ToString(BusinessObjects.busConstant.DateFormatLongDate);
                else
                    return string.Empty;
            }
        }

        public string premium_conversion_indicator_flag_description
        {
            get
            {
                if (premium_conversion_indicator_flag == BusinessObjects.busConstant.Flag_Yes)
                    return BusinessObjects.busConstant.Flag_Yes_Value;
                return BusinessObjects.busConstant.Flag_No_Value;
            }
        }

        // PIR 9294
        public string health_plan_option
        {
            get
            {
                if (alternate_structure_code_value == BusinessObjects.busConstant.AlternateStructureCodeHDHP)
                    return alternate_structure_code_description;
                return plan_option_description;
            }
        }

        public string istrTypeOfCoverageForMSSHistory
        {
            get
            {
                if (alternate_structure_code_value == BusinessObjects.busConstant.AlternateStructureCodeHDHP)
                    return alternate_structure_code_description;
                return "PPO/Basic";
            }
        }
    } 
}
