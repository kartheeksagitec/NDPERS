#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;
using NeoSpin.BusinessObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
    public class cdoPersonAccount : doPersonAccount
    {
        public cdoPersonAccount()
            : base()
        {
        }
        private string _EmployerName;
        public string EmployerName
        {
            get { return _EmployerName; }
            set { _EmployerName = value; }
        }

        // This derived property is used to display the  pension_service_credit in custom formated
        public string pension_service_credit_formatted
        {
            get
            {
                if (Total_PSC < 0)
                    return String.Format("{0} Years {1} Months", Math.Ceiling(Total_PSC / 12).ToString(),
                                     Math.Round((Total_PSC % 12), 4, MidpointRounding.AwayFromZero).ToString());

                return String.Format("{0} Years {1} Months", Math.Floor(Total_PSC / 12).ToString(),
                                     Math.Round((Total_PSC % 12), 4, MidpointRounding.AwayFromZero).ToString());
            }
        }

        // This derived property is used to display the vested_service_credit in custom formated
        public string vested_service_credit_formatted
        {
            get
            {
                if (Total_VSC < 0)
                    return String.Format("{0} Years {1} Months", Math.Ceiling(Total_VSC / 12).ToString(),
                                     Math.Round((Total_VSC % 12), 4, MidpointRounding.AwayFromZero).ToString());

                return String.Format("{0} Years {1} Months", Math.Floor(Total_VSC / 12).ToString(),
                                     Math.Round((Total_VSC % 12), 4, MidpointRounding.AwayFromZero).ToString());
            }
        }

        // This derived property is used to display the vested_service_credit Years in custom formated for Vesting Threshold Batch Correpondence
        public string vested_service_credit_formatted_year
        {
            get
            {
                if (Total_VSC < 0)
                    return String.Format("{0}", Math.Ceiling(Total_VSC / 12).ToString());

                return String.Format("{0}", Math.Floor(Total_VSC / 12).ToString());
            }
        }

        private int _person_employment_dtl_id;
        public int person_employment_dtl_id
        {
            get { return _person_employment_dtl_id; }
            set { _person_employment_dtl_id = value; }
        }

        public int person_employment_dtl_id_from_screen { get; set; }

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

        public DateTime history_change_date_no_null
        {
            get
            {
                if (history_change_date == DateTime.MinValue)
                    return DateTime.MaxValue;
                return history_change_date;
            }
        }

        public DateTime current_plan_start_date
        {
            get
            {
                if (history_change_date == DateTime.MinValue)
                    return start_date;
                return history_change_date;
            }
        }

        public DateTime current_plan_start_date_no_null
        {
            get
            {
                if (current_plan_start_date == DateTime.MinValue)
                    return DateTime.Now;
                return current_plan_start_date;
            }
        }

        public DateTime derived_plan_start_date
        {
            get
            {
                if (end_date == DateTime.MinValue)
                    return current_plan_start_date;
                return start_date;
            }
        }

        private decimal _Total_PSC;
        public decimal Total_PSC
        {
            get
            {
                return _Total_PSC;
            }
            set
            {
                _Total_PSC = value;
            }
        }

        private decimal _Total_VSC;
        public decimal Total_VSC
        {
            get
            {
                return _Total_VSC;
            }
            set
            {
                _Total_VSC = value;
            }
        }

        public decimal Total_PSC_in_Years
        {
            get
            {
                return Total_PSC / 12;
            }
        }

        public decimal Total_VSC_in_Years
        {
            get
            {
                return Total_VSC / 12;
            }
        }

        public int Rounded_Total_VSC
        {
            get
            {
                return ((int)Math.Ceiling(Total_VSC));
            }
        }

        public int Rounded_Total_VSC_In_Years_Floor
        {
            get
            {
                return ((int)Math.Floor(Total_VSC_in_Years));
            }
        }

        public string header_plan_participation_status_value { get; set; }

        // This property used in ucs - 22 Outbound files(457,DC)
        private string _status_code;

        public string status_code
        {
            get { return _status_code; }
            set { _status_code = value; }
        }
        /// <summary>
        /// *** BR - 01 *** COBRA ‘Expiration Date’
        /// </summary>
        /// <param name="astrOperation"></param>
        /// <param name="astrColumnName"></param>
        /// Returns base method if the Column is 
        public override bool AuditColumn(string astrOperation, string astrColumnName)
        {
            if (astrColumnName == "cobra_expiration_date")
            {
                if (cobra_expiration_date != DateTime.MinValue)
                    return true;
                return false;
            }
            if (is_From_Update_History_Form == true)
            {
                if (astrColumnName == "history_change_date" || astrColumnName == "person_account_id")
                {
                    return true;
                }
                return false;
            }
            return base.AuditColumn(astrOperation, astrColumnName);
        }
        public bool is_From_Update_History_Form
        { get; set; }
        public bool is_plan_cancelled
        {
            get
            {
                if ((plan_participation_status_value == "CAN1") ||
                   (plan_participation_status_value == "CAN2") ||
                   (plan_participation_status_value == "CAN3") ||
                   (plan_participation_status_value == "CAN4"))
                {
                    return true;
                }
                return false;
            }
        }

        //This Property is used to load the drop down in GHDV Screen (From Person_Account)
        public string person_full_name_with_person_account_id { get; set; }

        // PIR ID 1798
        public decimal vested_employer_cont_percent { get; set; }

        public string cobra_expiration_date_long_formatted
        {
            get
            {
                //PIR 17358 
                if (cobra_expiration_date == DateTime.MinValue)
                    return history_change_date != DateTime.MinValue ? history_change_date.AddDays(-1).ToString("MMMM dd, yyyy") : history_change_date.ToString("MMMM dd, yyyy");
                else
                    return cobra_expiration_date.ToString("MMMM dd, yyyy");
            }
        }

        public string istrStartDate
        {
            get
            {
                if (start_date == DateTime.MinValue)
                    return string.Empty;
                else
                    return start_date.ToString(busConstant.DateFormatLongDate);
            }
        }

        public string istrPayrollStartDate
        {
            get
            {
                if (history_change_date == DateTime.MinValue)
                    return string.Empty;
                else
                {
                    return new DateTime(history_change_date.Year, 01, 01).ToString(busConstant.DateFormatYearMonthDay);
                }
            }
        }

        public string istrPayrollEndDate
        {
            get
            {
                if (history_change_date == DateTime.MinValue)
                    return string.Empty;
                else
                {
                    DateTime ldteTemp = new DateTime(history_change_date.AddYears(1).Year, 01, 01);
                    return ldteTemp.AddDays(-1).ToString(busConstant.DateFormatYearMonthDay);
                }
            }
        }

        public string change_effective_date_minus_oneday
        {
            get
            {
                if (history_change_date != DateTime.MinValue)
                    return history_change_date.AddDays(-1).ToString(busConstant.DateFormatLongDate);
                return string.Empty;
            }
        }

        public bool is_from_mss { get; set; } // PIR 9702

        public bool iblnIsEmploymentEnded { get; set; } //PIR 20259
        
        //PIR 26579 
        //used to show on screen the Pre tax enrollment status of previous and current years of Dental, Vision and Life plan
        public string istrIsEnrolledInCY { get; set; }
        public string istrIsEnrolledInCYMinus1 { get; set; }
        public string istrIsEnrolledInCYMinus2 { get; set; }
        public string istrIsEnrolledInCYMinus3 { get; set; }
        public string istrPlanNameForGrid { get; set; }
        public string istrLinkForPlan { get; set; }
        public string istrCurrentEnrollmentStatus { get; set; }
        public string istrAddl_ee_contribution_percent_zero { get; set; }// PIR 25920 DC 2025 
        public bool iblnIs_Addl_ee_contribution_percent_NULL { get; set; }// PIR 25920 DC 2025 
        public string istrAddl_ee_contribution_percent_Perm_zero { get; set; }// PIR 25920 DC 2025 
        public string istrAddl_ee_contribution_percent_Temp_zero { get; set; }// PIR 25920 DC 2025 
    }
}
