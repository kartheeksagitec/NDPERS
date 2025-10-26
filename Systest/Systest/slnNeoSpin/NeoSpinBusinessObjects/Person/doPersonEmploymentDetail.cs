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
	/// Class NeoSpin.DataObjects.doPersonEmploymentDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonEmploymentDetail : doBase
    {
         
         public doPersonEmploymentDetail() : base()
         {
         }
         public int person_employment_dtl_id { get; set; }
         public int person_employment_id { get; set; }
         public DateTime start_date { get; set; }
         public DateTime end_date { get; set; }
         public int hourly_id { get; set; }
         public string hourly_description { get; set; }
         public string hourly_value { get; set; }
         public int seasonal_id { get; set; }
         public string seasonal_description { get; set; }
         public string seasonal_value { get; set; }
         public int type_id { get; set; }
         public string type_description { get; set; }
         public string type_value { get; set; }
         public int job_class_id { get; set; }
         public string job_class_description { get; set; }
         public string job_class_value { get; set; }
         public DateTime term_begin_date { get; set; }
         public int official_list_id { get; set; }
         public string official_list_description { get; set; }
         public string official_list_value { get; set; }
         public DateTime recertified_date { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public string recertified_batch_run_flag { get; set; }
         public string loa_320_letter_sent_flag { get; set; }
         public string loa_365_letter_sent_flag { get; set; }
         public string loa_730_letter_sent_flag { get; set; }
         public int cobra_letter_status_id { get; set; }
         public string cobra_letter_status_description { get; set; }
         public string cobra_letter_status_value { get; set; }
    }
    [Serializable]
    public enum enmPersonEmploymentDetail
    {
         person_employment_dtl_id ,
         person_employment_id ,
         start_date ,
         end_date ,
         hourly_id ,
         hourly_description ,
         hourly_value ,
         seasonal_id ,
         seasonal_description ,
         seasonal_value ,
         type_id ,
         type_description ,
         type_value ,
         job_class_id ,
         job_class_description ,
         job_class_value ,
         term_begin_date ,
         official_list_id ,
         official_list_description ,
         official_list_value ,
         recertified_date ,
         status_id ,
         status_description ,
         status_value ,
         recertified_batch_run_flag ,
         loa_320_letter_sent_flag ,
         loa_365_letter_sent_flag ,
         loa_730_letter_sent_flag ,
         cobra_letter_status_id ,
         cobra_letter_status_description ,
         cobra_letter_status_value ,
    }
}

