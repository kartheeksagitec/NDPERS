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
	/// Class NeoSpin.DataObjects.doPsEmployment:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPsEmployment : doBase
    {
         
         public doPsEmployment() : base()
         {
         }
         public int ps_employment_id { get; set; }
         public string ssn { get; set; }
         public string org_code { get; set; }
         public string ps_empl_record_number { get; set; }
         public DateTime empl_start_date { get; set; }
         public DateTime empl_end_date { get; set; }
         public string job_type_value { get; set; }
         public string job_class_value { get; set; }
         public string empl_status_value { get; set; }
         public string hourly_value { get; set; }
         public DateTime loa_start_date { get; set; }
         public DateTime loa_date_of_return { get; set; }
         public DateTime date_of_last_regular_paycheck { get; set; }
         public DateTime last_month_on_employer_billing { get; set; }
         public DateTime last_retirement_transmittal_of_deduction { get; set; }
         public DateTime term_begin_date { get; set; }
         public string processed_flag { get; set; }
         public decimal monthly_salary { get; set; }
         public string official_list_value { get; set; }
         public string peoplesoft_id { get; set; }
         public string first_name { get; set; }
         public string last_name { get; set; }
         public string error { get; set; }
    }
    [Serializable]
    public enum enmPsEmployment
    {
         ps_employment_id ,
         ssn ,
         org_code ,
         ps_empl_record_number ,
         empl_start_date ,
         empl_end_date ,
         job_type_value ,
         job_class_value ,
         empl_status_value ,
         hourly_value ,
         loa_start_date ,
         loa_date_of_return ,
         date_of_last_regular_paycheck ,
         last_month_on_employer_billing ,
         last_retirement_transmittal_of_deduction ,
         term_begin_date ,
         processed_flag ,
         monthly_salary ,
         official_list_value ,
         peoplesoft_id ,
         first_name ,
         last_name ,
         error ,
    }
}

