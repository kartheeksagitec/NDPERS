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
	public class cdoProviderReportDataInsurance : doProviderReportDataInsurance
	{
		public cdoProviderReportDataInsurance() : base()
		{
        }

        private string _lstrDateTimePeriod;
        public string lstrDateTimePeriod
        {
            get
            {
                _lstrDateTimePeriod = string.Empty;
                if ( effective_date != DateTime.MinValue)
                {
                    DateTime ldtStartDate = new DateTime(effective_date.Year, effective_date.Month, 1);
                    DateTime ldtEndDate = new DateTime(effective_date.Year, effective_date.Month, DateTime.DaysInMonth(effective_date.Year, effective_date.Month));
                    _lstrDateTimePeriod = ldtStartDate.ToString("yyyyMMdd") + "-" + ldtEndDate.ToString("yyyyMMdd");
                }
                return _lstrDateTimePeriod;
            }
        }
        //prod pir 4318
        public decimal rhic_amount_for_gl { get; set; }
        //pir 7705
        public decimal hsa_amount_for_gl { get; set; }
        public decimal vendor_amount_for_gl { get; set; }
    } 
} 
