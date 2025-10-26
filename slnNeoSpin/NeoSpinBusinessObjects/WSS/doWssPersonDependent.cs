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
	/// Class NeoSpin.DataObjects.doWssPersonDependent:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssPersonDependent : doBase
    {
         
         public doWssPersonDependent() : base()
         {
         }
         public int wss_person_dependent_id { get; set; }
         public int wss_person_account_enrollment_request_id { get; set; }
         public int target_person_dependent_id { get; set; }
         public string first_name { get; set; }
         public string last_name { get; set; }
         public string middle_name { get; set; }
         public DateTime date_of_birth { get; set; }
         public string ssn { get; set; }
         public int relationship_id { get; set; }
         public string relationship_description { get; set; }
         public string relationship_value { get; set; }
         public int gender_id { get; set; }
         public string gender_description { get; set; }
         public string gender_value { get; set; }
         public string full_time_student_flag { get; set; }
         public int marital_status_id { get; set; }
         public string marital_status_description { get; set; }
         public string marital_status_value { get; set; }
         public DateTime effective_start_date { get; set; }
         public DateTime effective_end_date { get; set; }
         public string medicare_claim_no { get; set; }
         public DateTime medicare_part_a_effective_date { get; set; }
         public DateTime medicare_part_b_effective_date { get; set; }
         public int current_plan_enrollment_option_id { get; set; }
         public string current_plan_enrollment_option_description { get; set; }
         public string current_plan_enrollment_option_value { get; set; }
    }
    [Serializable]
    public enum enmWssPersonDependent
    {
         wss_person_dependent_id ,
         wss_person_account_enrollment_request_id ,
         target_person_dependent_id ,
         first_name ,
         last_name ,
         middle_name ,
         date_of_birth ,
         ssn ,
         relationship_id ,
         relationship_description ,
         relationship_value ,
         gender_id ,
         gender_description ,
         gender_value ,
         full_time_student_flag ,
         marital_status_id ,
         marital_status_description ,
         marital_status_value ,
         effective_start_date ,
         effective_end_date ,
         medicare_claim_no ,
         medicare_part_a_effective_date ,
         medicare_part_b_effective_date ,
         current_plan_enrollment_option_id ,
         current_plan_enrollment_option_description ,
         current_plan_enrollment_option_value ,
    }
}

