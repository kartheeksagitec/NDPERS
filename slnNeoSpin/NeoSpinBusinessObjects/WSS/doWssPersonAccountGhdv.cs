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
	/// Class NeoSpin.DataObjects.doWssPersonAccountGhdv:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssPersonAccountGhdv : doBase
    {
         
         public doWssPersonAccountGhdv() : base()
         {
         }
         public int wss_person_account_ghdv_id { get; set; }
         public int wss_person_account_enrollment_request_id { get; set; }
         public int target_person_account_ghdv_id { get; set; }
         public int level_of_coverage_id { get; set; }
         public string level_of_coverage_description { get; set; }
         public string level_of_coverage_value { get; set; }
         public string coverage_code { get; set; }
         public string keeping_other_coverage_flag { get; set; }
         public string is_eligible_dependent_exists_flag { get; set; }
         public string keep_current_flag_policy_flag { get; set; }
         public string keep_current_flag_policy_reason { get; set; }
         public string dependent_receiving_workers_compensation_flag { get; set; }
         public string dependent_receiving_no_fault_benefits_flag { get; set; }
         public string is_dependent_medicare_eligible { get; set; }
         public string is_dependent_medicare_esrd { get; set; }
         public string medicare_claim_no { get; set; }
         public DateTime medicare_part_a_effective_date { get; set; }
         public DateTime medicare_part_b_effective_date { get; set; }
         public string is_dependent_adult_child_flag { get; set; }
         public string is_dependent_adult_child_married_flag { get; set; }
         public string is_dependent_adult_child_disabled_flag { get; set; }
         public int type_of_coverage_id { get; set; }
         public string type_of_coverage_description { get; set; }
         public string type_of_coverage_value { get; set; }
         public string pre_tax_payroll_deduction_flag { get; set; }
        public string waive_reason { get; set; }
        public string waive_reason_text { get; set; }
    }
    [Serializable]
    public enum enmWssPersonAccountGhdv
    {
         wss_person_account_ghdv_id ,
         wss_person_account_enrollment_request_id ,
         target_person_account_ghdv_id ,
         level_of_coverage_id ,
         level_of_coverage_description ,
         level_of_coverage_value ,
         coverage_code ,
         keeping_other_coverage_flag ,
         is_eligible_dependent_exists_flag ,
         keep_current_flag_policy_flag ,
         keep_current_flag_policy_reason ,
         dependent_receiving_workers_compensation_flag ,
         dependent_receiving_no_fault_benefits_flag ,
         is_dependent_medicare_eligible ,
         is_dependent_medicare_esrd ,
         medicare_claim_no ,
         medicare_part_a_effective_date ,
         medicare_part_b_effective_date ,
         is_dependent_adult_child_flag ,
         is_dependent_adult_child_married_flag ,
         is_dependent_adult_child_disabled_flag ,
         type_of_coverage_id ,
         type_of_coverage_description ,
         type_of_coverage_value ,
         pre_tax_payroll_deduction_flag ,
         waive_reason,
         waive_reason_text,
    }
}

