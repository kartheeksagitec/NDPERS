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
	public class cdoProviderReportDataDc : doProviderReportDataDc
	{
		public cdoProviderReportDataDc() : base()
		{
		}
        private string _Source;
        public string Source
        {
            get
            {
                return _Source;
            }

            set
            {
                _Source = value;
            }
        }

        private decimal _Amount;
        public decimal Amount
        {
            get
            {
               return _Amount;
            }

            set
            {
                _Amount = value;
            }
        }
    } 
} 
