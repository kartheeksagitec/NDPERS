#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.BusinessObjects;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
    public class cdoServicePurchaseDetail : doServicePurchaseDetail
    {
        public cdoServicePurchaseDetail()
            : base()
        {
        }

        private decimal _idecSickLeaveTotalPurchaseCost = 0;
        public decimal SickLeaveTotalPurchaseCost
        {
            get
            {
                _idecSickLeaveTotalPurchaseCost = (retirement_cost_for_sick_leave_purchase + rhic_cost_for_sick_leave_purchase);
                return _idecSickLeaveTotalPurchaseCost;
            }
        }

        // This derived property is created to calculate the Earliest Normal Retirement Age based on the stored value of Year and month
        // and using the custom Round<T> method.
        public int EarlierstNormalRetirementAgeRoundedToNearestYear
        {
            get
            {
                return busGlobalFunctions.Round(earliest_normal_retirement_age);
            }
        }
        public int EarlierstNormalRetirementAgeCustomRoundedToNearestYear
        {
            get
            {
                return busGlobalFunctions.CustomRound(earliest_normal_retirement_age);
            }
        }

        public int EarlierstNormalRetirementAgeWithoutServicePurchasedRoundedToNearestYear
        {
            get
            {
                return busGlobalFunctions.Round(earliest_nor_ret_age_without_service_purchased);
            }
        }

        public int EarlierstNormalRetirementAgeWithoutServicePurchasedCustomRoundedToNearestYear
        {
            get
            {
                return busGlobalFunctions.CustomRound(earliest_nor_ret_age_without_service_purchased);
            }
        }
        //formatted to round of total time of purchase
        public int RoundedTotalTimeOfPurchase
        {
            get
            {
                return busGlobalFunctions.Round(total_time_to_purchase);
            }
        }

        //formatted to round of total time of purchase for unused sick leave. 
        //Fraction of month are deemed to be full month
        public int RoundedTotalTimeOfPurchaseForSickLeave
        {
            get
            {
                return ((int)Math.Ceiling(total_time_to_purchase));
            }
        }

        //formatted to round of total time of purchase
        public int RoundedTotalTimeOfPurchaseExcludeFreeService
        {
            get
            {
                return busGlobalFunctions.Round(total_time_to_purchase_exclude_free_service);
            }
        }

        private int _projected_age_year_part;
        public int projected_age_year_part
        {
            get
            {
                return _projected_age_year_part;
            }

            set
            {
                _projected_age_year_part = value;
            }
        }

        private int _projected_age_month_part;
        public int projected_age_month_part
        {
            get
            {
                return _projected_age_month_part;
            }

            set
            {
                _projected_age_month_part = value;
            }
        }

        public int ProjectedAgeRoundedToNearestYear
        {
            get
            {
                return busGlobalFunctions.Round(projected_age_year_part + (Convert.ToDecimal(projected_age_month_part) / 12));
            }
        }

        public string userra_active_duty_start_date_long_date
        {
            get
            {
                return userra_active_duty_start_date.ToString(busConstant.DateFormatLongDate);
            }
        }
        public string userra_active_duty_end_date_long_date
        {
            get
            {
                return userra_active_duty_end_date.ToString(busConstant.DateFormatLongDate);
            }
        }

    }

}
