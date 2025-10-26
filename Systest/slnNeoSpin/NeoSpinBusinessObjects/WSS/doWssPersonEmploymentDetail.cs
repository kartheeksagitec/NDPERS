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
	/// Class NeoSpin.DataObjects.doWssPersonEmploymentDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssPersonEmploymentDetail : doBase
    {
         
         public doWssPersonEmploymentDetail() : base()
         {
         }
         public int wss_person_employment_dtl_id { get; set; }
         public int member_record_request_id { get; set; }
         public int person_employment_dtl_id { get; set; }
         public int wss_person_employment_id { get; set; }
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
         public int employment_status_id { get; set; }
         public string employment_status_description { get; set; }
         public string employment_status_value { get; set; }
         public string retr_status { get; set; }
         public string eap_status { get; set; }
         public string life_status { get; set; }
    }
    [Serializable]
    public enum enmWssPersonEmploymentDetail
    {
         wss_person_employment_dtl_id ,
         member_record_request_id ,
         person_employment_dtl_id ,
         wss_person_employment_id ,
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
         employment_status_id ,
         employment_status_description ,
         employment_status_value ,
         retr_status ,
         eap_status ,
         life_status ,
    }
}

