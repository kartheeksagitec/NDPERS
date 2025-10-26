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
	/// Class NeoSpin.DataObjects.doWssEmploymentChangeRequest:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssEmploymentChangeRequest : doBase
    {
         
         public doWssEmploymentChangeRequest() : base()
         {
         }
         public int employment_change_request_id { get; set; }
         public int person_id { get; set; }
         public int org_id { get; set; }
         public int change_type_id { get; set; }
         public string change_type_description { get; set; }
         public string change_type_value { get; set; }
         public DateTime loa_start_date { get; set; }
         public string loa_reason { get; set; }
         public DateTime recertified_date { get; set; }
         public DateTime date_of_return { get; set; }
         public DateTime job_class_change_effective_date { get; set; }
         public int job_class_id { get; set; }
         public string job_class_description { get; set; }
         public string job_class_value { get; set; }
         public DateTime term_begin_date { get; set; }
         public int official_list_id { get; set; }
         public string official_list_description { get; set; }
         public string official_list_value { get; set; }
         public DateTime employment_type_change_effective_date { get; set; }
         public int hourly_id { get; set; }
         public string hourly_description { get; set; }
         public string hourly_value { get; set; }
         public int seasonal_id { get; set; }
         public string seasonal_description { get; set; }
         public string seasonal_value { get; set; }
         public int type_id { get; set; }
         public string type_description { get; set; }
         public string type_value { get; set; }
         public DateTime employment_end_date { get; set; }
         public int end_dated_employment_dtl_id { get; set; }
         public int new_employment_dtl_id { get; set; }
         public string posted_in_perslink_by { get; set; }
         public DateTime posted_date { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public DateTime date_of_last_regular_paycheck { get; set; }
         public DateTime last_month_on_employer_billing { get; set; }
         public DateTime last_date_of_service { get; set; }
         public DateTime last_retirement_transmittal_of_deduction { get; set; }
         public DateTime last_retirement_transmittal_due { get; set; }
         public string rejection_reason { get; set; }
         public int contact_id { get; set; }
         public int employment_status_id { get; set; }
         public string employment_status_description { get; set; }
         public string employment_status_value { get; set; }
         public string health_insurance_continued { get; set; }
         public string life_insurance_continued { get; set; }
         public string dental_insurance_continued { get; set; }
         public string vision_insurance_continued { get; set; }
         public string eap_insurance_continued { get; set; }
         public string ps_empl_record_number { get; set; }
         public string ltc_continued { get; set; }
         public string flex_comp_continued { get; set; }
         public string peoplesoft_id { get; set; }
         public int is_on_teaching_contract_id { get; set; }
         public string is_on_teaching_contract_description { get; set; }
         public string is_on_teaching_contract_value { get; set; }
         public string employee_never_started { get; set; }
    }
    [Serializable]
    public enum enmWssEmploymentChangeRequest
    {
         employment_change_request_id ,
         person_id ,
         org_id ,
         change_type_id ,
         change_type_description ,
         change_type_value ,
         loa_start_date ,
         loa_reason ,
         recertified_date ,
         date_of_return ,
         job_class_change_effective_date ,
         job_class_id ,
         job_class_description ,
         job_class_value ,
         term_begin_date ,
         official_list_id ,
         official_list_description ,
         official_list_value ,
         employment_type_change_effective_date ,
         hourly_id ,
         hourly_description ,
         hourly_value ,
         seasonal_id ,
         seasonal_description ,
         seasonal_value ,
         type_id ,
         type_description ,
         type_value ,
         employment_end_date ,
         end_dated_employment_dtl_id ,
         new_employment_dtl_id ,
         posted_in_perslink_by ,
         posted_date ,
         status_id ,
         status_description ,
         status_value ,
         date_of_last_regular_paycheck ,
         last_month_on_employer_billing ,
         last_date_of_service ,
         last_retirement_transmittal_of_deduction ,
         last_retirement_transmittal_due ,
         rejection_reason ,
         contact_id ,
         employment_status_id ,
         employment_status_description ,
         employment_status_value ,
         health_insurance_continued ,
         life_insurance_continued ,
         dental_insurance_continued ,
         vision_insurance_continued ,
         eap_insurance_continued ,
         ps_empl_record_number ,
         ltc_continued ,
         flex_comp_continued ,
         peoplesoft_id ,
         is_on_teaching_contract_id ,
         is_on_teaching_contract_description ,
         is_on_teaching_contract_value ,
         employee_never_started ,
    }
}

