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
	public class cdoServicePurchaseDetailConsolidated : doServicePurchaseDetailConsolidated
	{
		public cdoServicePurchaseDetailConsolidated() : base()
		{
		}

        private string _istrOrgCodeId;
        public string istrOrgCodeId
        {
            get { return _istrOrgCodeId; }
            set { _istrOrgCodeId = value; }
        }

        //UAT Corr PIR:2348
        public string service_purchase_start_date_longDate
        {
            get
            {
                return service_purchase_start_date.ToString((BusinessObjects.busConstant.DateFormatLongDate));
            }
        }

        //UAT Corr PIR: 2348
        public string service_purchase_end_date_longDate
        {
            get
            {
                return service_purchase_end_date.ToString((BusinessObjects.busConstant.DateFormatLongDate));
            }
        }

        public string time_to_purchase_years
        {
            get
            {
                decimal ldecTimeToPurchase = Convert.ToDecimal(time_to_purchase);
                if (time_to_purchase < 0)
                    return String.Format("{0} Years {1} Months", Math.Ceiling(ldecTimeToPurchase / 12).ToString(), Math.Round((ldecTimeToPurchase % 12), 4, MidpointRounding.AwayFromZero).ToString());

                return String.Format("{0} Years {1} Months", Math.Floor(ldecTimeToPurchase / 12).ToString(), Math.Round((ldecTimeToPurchase % 12), 4, MidpointRounding.AwayFromZero).ToString());
            }
        }

        public string calculated_time_to_purchase_years
        {
            get
            {
                decimal ldecTimeToPurchase = Convert.ToDecimal(calculated_time_to_purchase);
                if (calculated_time_to_purchase < 0)
                    return String.Format("{0} Years {1} Months", Math.Ceiling(ldecTimeToPurchase / 12).ToString(), Math.Round((ldecTimeToPurchase % 12), 4, MidpointRounding.AwayFromZero).ToString());

                return String.Format("{0} Years {1} Months", Math.Floor(ldecTimeToPurchase / 12).ToString(), Math.Round((ldecTimeToPurchase % 12), 4, MidpointRounding.AwayFromZero).ToString());
            }
        }
    } 
} 
