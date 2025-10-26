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
    public class cdoBenefitRhicCombine : doBenefitRhicCombine
    {
        public cdoBenefitRhicCombine()
            : base()
        {
        }

        public decimal total_balance_rhic_amount
        {
            get
            {
                return combined_rhic_amount - (total_other_rhic_amount + total_js_rhic_amount);
            }
        }

        public DateTime start_date_no_null
        {
            get
            {
                if (start_date == DateTime.MinValue)
                    return DateTime.Now;
                return start_date;
            }
        }
        public DateTime end_date_no_null
        {
            get
            {
                if (end_date == DateTime.MinValue)
                    return DateTime.MaxValue;
                return end_date;
            }
        }

        public DateTime future_start_date_no_null
        {
            get
            {
                if (start_date > DateTime.Now)
                    return start_date;
                return DateTime.Now;
            }
        }

        //UAT Corr PIR: 2367
        public string start_date_no_null_longDate
        {
            get
            {
                return start_date_no_null.ToString((BusinessObjects.busConstant.DateFormatLongDate));
            }
        }

    }
}
