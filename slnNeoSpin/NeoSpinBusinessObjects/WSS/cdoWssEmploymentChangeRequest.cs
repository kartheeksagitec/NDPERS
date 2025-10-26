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
	/// <summary>
	/// Class NeoSpin.CustomDataObjects.cdoWssEmploymentChangeRequest:
	/// Inherited from doWssEmploymentChangeRequest, the class is used to customize the database object doWssEmploymentChangeRequest.
	/// </summary>
    [Serializable]
	public class cdoWssEmploymentChangeRequest : doWssEmploymentChangeRequest
	{
		public cdoWssEmploymentChangeRequest() : base()
		{
		}
        public int person_employment_id { get; set; }
        public int person_employment_detail_id { get; set; }
        public string type_of_change { get; set; }
        public int plan_id { get; set; }
        public string istrEmpDetailStatusDescription { get; set; }
        public string istrEmpDetailStartDate { get; set; }

        /// <summary>
        /// pir 7952 : 3 Properties added to Set details of Panel Requested By
        /// </summary>
        /// <returns></returns>
        public string Requested_By_Contact_id { get; set; }
        public string Requested_By_Contact_Name { get; set; }
        public string Requested_By_Contact_Phone_No { get; set; }

        //PIR 11653
        public string istrRequestByContactEmail { get; set; }

        public string istrLastMonthOnEmployerBilling { get; set; }
        public string istrLastRetirementTransmittalOfDeduction { get; set; }

        public string last_month_year_on_employer_billing
        {
            get
            {
                if (last_month_on_employer_billing == DateTime.MinValue)
                    return string.Empty;
                else
                    return last_month_on_employer_billing.ToString(BusinessObjects.busConstant.DateFormatMonthYear);
            }
        }

        public string last_month_year_retirement_transmittal_of_deduction
        {
            get
            {
                if (last_retirement_transmittal_of_deduction == DateTime.MinValue)
                    return string.Empty;
                else
                    return last_retirement_transmittal_of_deduction.ToString(BusinessObjects.busConstant.DateFormatMonthYear);
            }
        }
        public string istrEmploymentEndDateDefault
        {
            get
            {
                if ((last_retirement_transmittal_of_deduction.Month == last_date_of_service.Month && last_retirement_transmittal_of_deduction.Year == last_date_of_service.Year) || (last_retirement_transmittal_of_deduction == DateTime.MinValue) || 
                    (last_retirement_transmittal_of_deduction.Year == 1900))
                    return last_date_of_service.ToString("MM/dd/yyyy");
                else if (last_retirement_transmittal_of_deduction.Month == date_of_last_regular_paycheck.Month && last_retirement_transmittal_of_deduction.Year == date_of_last_regular_paycheck.Year)
                    return date_of_last_regular_paycheck.ToString("MM/dd/yyyy");
                else
                    return DateTime.MinValue.ToString();
            }
        }
    } 
} 
