#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.DataObjects;

#endregion

namespace NeoSpin.DataObjects
{
	/// <summary>
	/// Class NeoSpin.DataObjects.doWssBenefitCalculator:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssBenefitCalculator : doBase
    {
         
         public doWssBenefitCalculator() : base()
         {
         }
         public int wss_benefit_calculator_id { get; set; }
         public int plan_id { get; set; }
         public int benefit_calculation_id { get; set; }
         public int unused_sick_leave_service_purchase_header_id { get; set; }
         public int consolidated_service_purchase_header_id { get; set; }
         public string is_disability { get; set; }
         public decimal down_payments { get; set; }
         public int number_of_payments { get; set; }
         public decimal payment_amount { get; set; }
         public string reduced_benefit_flag { get; set; }
         public string is_unused_sick_leave_purchase_flag { get; set; }
         public string is_additional_service_months_flag { get; set; }
         public string is_tffr_tiaa_service_flag { get; set; }
         public decimal tffr_tiaa_service_credit { get; set; }
         public DateTime spouse_date_of_birth { get; set; }
         public int estimated_service_purchase_type_id { get; set; }
         public string estimated_service_purchase_type_description { get; set; }
         public string estimated_service_purchase_type_value { get; set; }
         public string is_estimate_added_flag { get; set; }
         public int payment_frequency_id { get; set; }
         public string payment_frequency_description { get; set; }
         public string payment_frequency_value { get; set; }
         public string is_pre_retirement_death_flag { get; set; }
    }
    [Serializable]
    public enum enmWssBenefitCalculator
    {
         wss_benefit_calculator_id ,
         plan_id ,
         benefit_calculation_id ,
         unused_sick_leave_service_purchase_header_id ,
         consolidated_service_purchase_header_id ,
         is_disability ,
         down_payments ,
         number_of_payments ,
         payment_amount ,
         reduced_benefit_flag ,
         is_unused_sick_leave_purchase_flag ,
         is_additional_service_months_flag ,
         is_tffr_tiaa_service_flag ,
         tffr_tiaa_service_credit ,
         spouse_date_of_birth ,
         estimated_service_purchase_type_id ,
         estimated_service_purchase_type_description ,
         estimated_service_purchase_type_value ,
         is_estimate_added_flag ,
         payment_frequency_id ,
         payment_frequency_description ,
         payment_frequency_value ,
         is_pre_retirement_death_flag ,
    }
}

