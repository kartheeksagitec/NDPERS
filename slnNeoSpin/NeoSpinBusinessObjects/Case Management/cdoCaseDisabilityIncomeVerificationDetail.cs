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
    public class cdoCaseDisabilityIncomeVerificationDetail : doCaseDisabilityIncomeVerificationDetail
    {
        public cdoCaseDisabilityIncomeVerificationDetail()
            : base()
        {
        }

        public string istrMonth
        {
            get
            {
                if (idtTempDate != DateTime.MinValue)
                    return idtTempDate.ToString("MMM");
                return string.Empty;
            }
        }

        public decimal idecTotal { get; set; }

        public DateTime idtTempDate
        {
            get
            {
                DateTime ldt = new DateTime(year, month, 01);
                return ldt;
            }
        }

        public string month_year
        {
            get
            {
                string lstrMonthYear = string.Empty;
                if ((!string.IsNullOrEmpty(istrMonth)) && (year != 0))
                {
                    lstrMonthYear = istrMonth +" / "+ Convert.ToString(year);
                }
                return lstrMonthYear;
            }
        }
    }
}
