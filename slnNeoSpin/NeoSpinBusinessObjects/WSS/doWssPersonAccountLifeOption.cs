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
	/// Class NeoSpin.DataObjects.doWssPersonAccountLifeOption:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssPersonAccountLifeOption : doBase
    {
         
         public doWssPersonAccountLifeOption() : base()
         {
         }
         public int wss_person_account_life_option_id { get; set; }
         public int wss_person_account_enrollment_request_id { get; set; }
         public int level_of_coverage_id { get; set; }
         public string level_of_coverage_description { get; set; }
         public string level_of_coverage_value { get; set; }
         public int plan_option_status_id { get; set; }
         public string plan_option_status_description { get; set; }
         public string plan_option_status_value { get; set; }
         public decimal coverage_amount { get; set; }
         public string spouse_name { get; set; }
         public DateTime spouse_dob { get; set; }
         public decimal supplemental_amount { get; set; }
         public string supplemental_waiver_flag { get; set; }
         public string dependent_waiver_flag { get; set; }
         public int dependent_coverage_option_id { get; set; }
         public string dependent_coverage_option_description { get; set; }
         public string dependent_coverage_option_value { get; set; }
         public decimal spouse_supplemental_amount { get; set; }
         public string spouse_supplemental_waiver_flag { get; set; }
         public string pre_tax_payroll_deduction { get; set; }
    }
    [Serializable]
    public enum enmWssPersonAccountLifeOption
    {
         wss_person_account_life_option_id ,
         wss_person_account_enrollment_request_id ,
         level_of_coverage_id ,
         level_of_coverage_description ,
         level_of_coverage_value ,
         plan_option_status_id ,
         plan_option_status_description ,
         plan_option_status_value ,
         coverage_amount ,
         spouse_name ,
         spouse_dob ,
         supplemental_amount ,
         supplemental_waiver_flag ,
         dependent_waiver_flag ,
         dependent_coverage_option_id ,
         dependent_coverage_option_description ,
         dependent_coverage_option_value ,
         spouse_supplemental_amount ,
         spouse_supplemental_waiver_flag ,
         pre_tax_payroll_deduction ,
    }
}

