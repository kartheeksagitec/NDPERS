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
    public class cdoPersonAccountRetirementContribution : doPersonAccountRetirementContribution
    {
        public cdoPersonAccountRetirementContribution()
            : base()
        {
        }
        // This derived property is used to display the  Service purchase header id if the subsystem value is Purchase
        public string service_purchase_id
        {
            get
            {
                if (subsystem_value == "PURC")
                {
                    return subsystem_ref_id.ToString();
                }
                return string.Empty;
            }
        }
        // This derived property is used to display the  Payroll detail id if the subsystem value is Payroll
        public string payroll_detail_id
        {
            get
            {
                if (subsystem_value == "PAYR")
                {
                    return subsystem_ref_id.ToString();
                }
                return string.Empty;
            }
        }
        // This derived property is used to display the  pension_service_credit in custom formated
        public string pension_service_credit_formatted
        {
            get
            {
                if (pension_service_credit < 0)
                    return String.Format("{0} Years {1} Months", Math.Ceiling(pension_service_credit / 12).ToString(),
                                     Math.Round((pension_service_credit % 12), 6, MidpointRounding.AwayFromZero).ToString());

                return String.Format("{0} Years {1} Months", Math.Floor(pension_service_credit / 12).ToString(),
                                     Math.Round((pension_service_credit % 12), 6, MidpointRounding.AwayFromZero).ToString());
            }
        }

        // This derived property is used to display the vested_service_credit in custom formated
        public string vested_service_credit_formatted
        {
            get
            {
                if(vested_service_credit < 0)
                    return String.Format("{0} Years {1} Months", Math.Ceiling(vested_service_credit / 12).ToString(),
                                     Math.Round((vested_service_credit % 12), 6, MidpointRounding.AwayFromZero).ToString());

                return String.Format("{0} Years {1} Months", Math.Floor(vested_service_credit / 12).ToString(),
                                     Math.Round((vested_service_credit % 12), 6, MidpointRounding.AwayFromZero).ToString());
            }
        }
        private decimal _ee_rhic_ser_pur_cont;
        public decimal ee_rhic_ser_pur_cont
        {
            get
            {
                return _ee_rhic_ser_pur_cont;
            }

            set
            {
                _ee_rhic_ser_pur_cont = value;
            }
        }
        private decimal _er_rhic_ser_pur_cont;
        public decimal er_rhic_ser_pur_cont
        {
            get
            {
                return _er_rhic_ser_pur_cont;
            }

            set
            {
                _er_rhic_ser_pur_cont = value;
            }
        }
        private decimal _post_tax_ee_ser_pur_cont;
        public decimal post_tax_ee_ser_pur_cont
        {
            get
            {
                return _post_tax_ee_ser_pur_cont;
            }

            set
            {
                _post_tax_ee_ser_pur_cont = value;
            }
        }
        private decimal _pre_tax_ee_ser_pur_cont;
        public decimal pre_tax_ee_ser_pur_cont
        {
            get
            {
                return _pre_tax_ee_ser_pur_cont;
            }

            set
            {
                _pre_tax_ee_ser_pur_cont = value;
            }
        }
        private decimal _ee_er_pickup_ser_pur_cont;
        public decimal ee_er_pickup_ser_pur_cont
        {
            get
            {
                return _ee_er_pickup_ser_pur_cont;
            }

            set
            {
                _ee_er_pickup_ser_pur_cont = value;
            }
        }

        public string PayPeriodYearMonth
        {
            get
            {
                return pay_period_year.ToString().PadLeft(4, '0') + pay_period_month.ToString().PadLeft(2, '0');                       
            }
        }

        public string PayPeriodYearMonth_no_null
        {
            get
            {
                if (pay_period_year == 0)
                {
                    return effective_date.Year.ToString().PadLeft(4, '0') + effective_date.Month.ToString().PadLeft(2, '0');    
                }
                else
                {
                    return PayPeriodYearMonth;
                }                
            }
        }

        //is record selected for transfer
        public string isChecked { get; set; }
        public string org_code { get; set; } //PIR 15927
        public string BenCalc_Plan_Name { get; set; }
        public decimal MonthlySalaryChangePerc { get; set; }

        public int BlockNo { get; set; }
        //PIR PIR 16890 Fixed for Correspondence btn issue
        public int iintPrimaryId { get; set; } 
    }
}
