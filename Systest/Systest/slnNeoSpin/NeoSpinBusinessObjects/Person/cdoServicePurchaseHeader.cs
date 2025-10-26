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
    public class cdoServicePurchaseHeader : doServicePurchaseHeader
    {
        public cdoServicePurchaseHeader()
            : base()
        {
        }

        // This derived property is created to calculate the Nearest Age based on the stored value of Year and month
        // and using the custom Round<T> method.
        public int CurrentAgeRoundedToNearestYear
        {
            get
            {
                return busGlobalFunctions.Round(current_age);
            }
        }
        public int CurrentAgeCustomRoundedToNearestYear
        {
            get
            {
                return busGlobalFunctions.CustomRound(current_age);
            }
        }

        // This derived property is used to display the prorated vsc in custom formated
        public string prorated_vsc_formatted
        {
            get
            {
                if (prorated_vsc < 0)
                    return String.Format("{0} Years {1} Months", Math.Ceiling(prorated_vsc / 12).ToString(),
                                     Math.Round((prorated_vsc % 12), 6, MidpointRounding.AwayFromZero).ToString());

                return String.Format("{0} Years {1} Months", Math.Floor(prorated_vsc / 12).ToString(),
                                     Math.Round((prorated_vsc % 12), 6, MidpointRounding.AwayFromZero).ToString());
            }
        }

        // This derived property is used to display the prorated psc in custom formated
        public string prorated_psc_formatted
        {
            get
            {
                if (prorated_psc < 0)
                    return String.Format("{0} Years {1} Months", Math.Ceiling(prorated_psc / 12).ToString(),
                                     Math.Round((prorated_psc % 12), 6, MidpointRounding.AwayFromZero).ToString());

                return String.Format("{0} Years {1} Months", Math.Floor(prorated_psc / 12).ToString(),
                                     Math.Round((prorated_psc % 12), 6, MidpointRounding.AwayFromZero).ToString());
            }
        }     

        public decimal current_age
        {
            get
            {
                return current_age_year_part + (Convert.ToDecimal(current_age_month_part) / 12);
            }
        }

        public decimal current_age_in_months
        {
            get
            {
                return (current_age_year_part * 12) + current_age_month_part;
            }
        }

        public string ServiceCreditTypeData1
        {
            get
            {
                string lstrResult = "Header";
                if (service_purchase_type_value == busConstant.Service_Purchase_Type_Consolidated_Purchase)
                {
                    return "Consolidated";
                }
                return lstrResult;
            }
        }
        // PIR -17011
        public string ServiceCreditType
        {
            get
            {
                if (service_purchase_type_value == busConstant.Service_Purchase_Type_Consolidated_Purchase)
                {
                    return service_purchase_type_description;

                }
                else if (service_purchase_type_value == busConstant.Service_Purchase_Type_USERRA_Military_Service)
                {
                    return service_purchase_type_description;
                }
                else if (service_purchase_type_value == busConstant.Service_Purchase_Type_Unused_Sick_Leave)
                {
                    return service_purchase_type_description;
                }
                return string.Empty;
            }
        }

        public decimal paid_pension_service_credit
        {
            get
            {
                return prorated_psc;
            }
            set
            {
                prorated_psc = value;
            }
        }
        public decimal paid_vesting_service_credit
        {
            get
            {
                return prorated_vsc;
            }
            set
            {
                prorated_vsc = value;
            }
        }       

        //this property is used for correspondence
        public decimal Rounded_prorated_PSC
        {
            get
            {
                decimal ldecRoundedPSC = 0.00M;
                if (prorated_psc != 0.00M)
                    ldecRoundedPSC = Math.Ceiling(prorated_psc);
                return ldecRoundedPSC;
            }
        }
        public decimal total_psc { get; set; }
        public decimal total_vsc { get; set; }
        // This derived property is used to display the  pension_service_credit in custom formated
        public string total_psc_formatted
        {
            get
            {
                if (total_psc < 0)
                    return String.Format("{0} Years {1} Months", Math.Ceiling(total_psc / 12).ToString(),
                                     Math.Round((total_psc % 12), 4, MidpointRounding.AwayFromZero).ToString());

                return String.Format("{0} Years {1} Months", Math.Floor(total_psc / 12).ToString(),
                                     Math.Round((total_psc % 12), 4, MidpointRounding.AwayFromZero).ToString());
            }
        }
        // This derived property is used to display the  pension_service_credit in correspondence
        public string total_psc_formattedandRounded
        {
            get
            {
                if (total_psc < 0)
                    return String.Format("{0} Years {1} Months", Math.Ceiling(total_psc / 12).ToString(),
                                     Math.Round((total_psc % 12), 0, MidpointRounding.AwayFromZero).ToString());

                return String.Format("{0} Years {1} Months", Math.Floor(total_psc / 12).ToString(),
                                     Math.Round((total_psc % 12), 0, MidpointRounding.AwayFromZero).ToString());
            }
        }
        
        // This derived property is used to display the vested_service_credit in custom formated
        public string total_vsc_formatted
        {
            get
            {
                if (total_vsc < 0)
                    return String.Format("{0} Years {1} Months", Math.Ceiling(total_vsc / 12).ToString(),
                                     Math.Round((total_vsc % 12), 4, MidpointRounding.AwayFromZero).ToString());

                return String.Format("{0} Years {1} Months", Math.Floor(total_vsc / 12).ToString(),
                                     Math.Round((total_vsc % 12), 4, MidpointRounding.AwayFromZero).ToString());
            }
        }
		//PIR 24243
        public decimal idecUserFASSalary
        {
            get { return this.overridden_final_average_salary > 0 ? this.overridden_final_average_salary : this.final_average_salary; }
        }
    }
}
