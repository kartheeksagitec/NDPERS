using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NeoSpin.CustomDataObjects;

namespace NeoSpin.BusinessObjects
{
    public static class busIBSHelper
    {
        public static DateTime GetLastPostedIBSBatchDate()
        {
            DateTime ldtLastPostedIBSDate = DateTime.MinValue;

            DataTable ldtResult = busNeoSpinBase.Select<cdoIbsHeader>(
                                    new string[2] { "report_type_value", "REPORT_STATUS_VALUE" },
                                    new object[2] { busConstant.IBSHeaderReportTypeRegular, busConstant.IBSHeaderStatusPosted }, null, "BILLING_MONTH_AND_YEAR desc");
            if ((ldtResult != null) && (ldtResult.Rows.Count > 0))
            {
                if (!Convert.IsDBNull(ldtResult.Rows[0]["BILLING_MONTH_AND_YEAR"]))
                {
                    ldtLastPostedIBSDate = Convert.ToDateTime(ldtResult.Rows[0]["BILLING_MONTH_AND_YEAR"]);
                }
            }
            return ldtLastPostedIBSDate;
        }
    }
}
