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
	/// Class NeoSpin.DataObjects.doWssPersonAccountFlexCompOption:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssPersonAccountFlexCompOption : doBase
    {
         
         public doWssPersonAccountFlexCompOption() : base()
         {
         }
         public int wss_person_account_flex_comp_option_id { get; set; }
         public int wss_person_account_enrollment_request_id { get; set; }
         public int level_of_coverage_id { get; set; }
         public string level_of_coverage_description { get; set; }
         public string level_of_coverage_value { get; set; }
         public decimal medical_salary_per_pay_period { get; set; }
         public int medical_number_of_checks { get; set; }
         public decimal medical_annual_pledge_amount { get; set; }
         public DateTime effective_start_date { get; set; }
         public DateTime effective_end_date { get; set; }
         public decimal dependent_salary_per_pay_period { get; set; }
         public int dependent_number_of_checks { get; set; }
         public decimal dependent_annual_pledge_amount { get; set; }
         public string explanation_of_change_qualified_status { get; set; }
         public string upla_options { get; set; }
    }
    [Serializable]
    public enum enmWssPersonAccountFlexCompOption
    {
         wss_person_account_flex_comp_option_id ,
         wss_person_account_enrollment_request_id ,
         level_of_coverage_id ,
         level_of_coverage_description ,
         level_of_coverage_value ,
         medical_salary_per_pay_period ,
         medical_number_of_checks ,
         medical_annual_pledge_amount ,
         effective_start_date ,
         effective_end_date ,
         dependent_salary_per_pay_period ,
         dependent_number_of_checks ,
         dependent_annual_pledge_amount ,
         explanation_of_change_qualified_status ,
         upla_options ,
    }
}

