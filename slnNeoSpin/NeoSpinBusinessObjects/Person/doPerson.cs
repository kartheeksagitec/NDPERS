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
	/// Class NeoSpin.DataObjects.doPerson:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPerson : doBase
    {
         
         public doPerson() : base()
         {
         }
         public int person_id { get; set; }
         public string first_name { get; set; }
         public string middle_name { get; set; }
         public string last_name { get; set; }
         public int name_prefix_id { get; set; }
         public string name_prefix_description { get; set; }
         public string name_prefix_value { get; set; }
         public int name_suffix_id { get; set; }
         public string name_suffix_description { get; set; }
         public string name_suffix_value { get; set; }
         public DateTime date_of_birth { get; set; }
         public string date_of_birth_certified { get; set; }
         public DateTime date_of_death { get; set; }
         public string date_of_death_certified { get; set; }
         public DateTime date_of_death_reported_on { get; set; }
         public DateTime date_of_death_certified_on { get; set; }
         public string ssn { get; set; }
         public int gender_id { get; set; }
         public string gender_description { get; set; }
         public string gender_value { get; set; }
         public int marital_status_id { get; set; }
         public string marital_status_description { get; set; }
         public string marital_status_value { get; set; }
         public string home_phone_no { get; set; }
         public string work_phone_no { get; set; }
         public string cell_phone_no { get; set; }
         public string fax_no { get; set; }
         public string email_address { get; set; }
         public int communication_preference_id { get; set; }
         public string communication_preference_description { get; set; }
         public string communication_preference_value { get; set; }
         public string peoplesoft_id { get; set; }
         public string restriction_flag { get; set; }
         public string restricted_by { get; set; }
         public DateTime restricted_date { get; set; }
         public int restriction_reason_id { get; set; }
         public string restriction_reason_description { get; set; }
         public string restriction_reason_value { get; set; }
         public string welcome_batch_letter_sent_flag { get; set; }
         public string beneficiary_required_flag { get; set; }
         public string unrestricted_by { get; set; }
         public string suppress_ssn_flag { get; set; }
         public string is_person_deleted_flag { get; set; }
         public string deleted_ssn { get; set; }
         public string suppress_annual_statement_flag { get; set; }
         public decimal job_service_sick_leave { get; set; }
         public string ndpers_login_id { get; set; }
         public string previous_ndpers_login_id { get; set; }
         public DateTime ms_change_date { get; set; }
         public string ms_change_batch_flag { get; set; }
         public string work_phone_ext { get; set; }
         public string suppress_newsletter_flag { get; set; }
         public string db_addl_contrib { get; set; }
         public string phone_waiver_flag { get; set; }
         public DateTime certify_date { get; set; }
         public DateTime email_waiver_date { get; set; }
         public string email_waiver_flag { get; set; }
         public string activation_code { get; set; }
         public string pre_email_address { get; set; }
         public string is_user_locked { get; set; }
         public int failed_login_attempt_count { get; set; }         
         public string activation_code_flag { get; set; }
         public DateTime activation_code_date { get; set; }
		 public string limit_401a { get; set; }
         public string tffr_request { get; set; }
         public string end_of_life_docs { get; set; }
    }
    [Serializable]
    public enum enmPerson
    {
         person_id ,
         first_name ,
         middle_name ,
         last_name ,
         name_prefix_id ,
         name_prefix_description ,
         name_prefix_value ,
         name_suffix_id ,
         name_suffix_description ,
         name_suffix_value ,
         date_of_birth ,
         date_of_birth_certified ,
         date_of_death ,
         date_of_death_certified ,
         date_of_death_reported_on ,
         date_of_death_certified_on ,
         ssn ,
         gender_id ,
         gender_description ,
         gender_value ,
         marital_status_id ,
         marital_status_description ,
         marital_status_value ,
         home_phone_no ,
         work_phone_no ,
         cell_phone_no ,
         fax_no ,
         email_address ,
         communication_preference_id ,
         communication_preference_description ,
         communication_preference_value ,
         peoplesoft_id ,
         restriction_flag ,
         restricted_by ,
         restricted_date ,
         restriction_reason_id ,
         restriction_reason_description ,
         restriction_reason_value ,
         welcome_batch_letter_sent_flag ,
         beneficiary_required_flag ,
         unrestricted_by ,
         suppress_ssn_flag ,
         is_person_deleted_flag ,
         deleted_ssn ,
         suppress_annual_statement_flag ,
         job_service_sick_leave ,
         ndpers_login_id ,
         previous_ndpers_login_id ,
         ms_change_date ,
         ms_change_batch_flag ,
         work_phone_ext ,
         suppress_newsletter_flag ,
         db_addl_contrib ,
         phone_waiver_flag ,
         certify_date ,
         email_waiver_date ,
         email_waiver_flag ,
         activation_code ,
         pre_email_address ,
         is_user_locked ,
         failed_login_attempt_count ,        
         activation_code_flag ,
         activation_code_date ,
		 limit_401a ,
    }
}

