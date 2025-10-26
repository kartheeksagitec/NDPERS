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
	/// Class NeoSpin.DataObjects.doPayment1099rRequest:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPayment1099rRequest : doBase
    {
        
         public doPayment1099rRequest() : base()
         {
         }
         public int request_id { get; set; }
         public int tax_year { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public int request_type_id { get; set; }
         public string request_type_description { get; set; }
         public string request_type_value { get; set; }
         public DateTime process_date { get; set; }
         public int tax_month { get; set; }
         public string bulk_insert_1099r_data_flag { get; set; }
         public string created_1099r_details_report { get; set; }
         public string created_irs_file { get; set; }
         public string created_945_report { get; set; }
    }
    [Serializable]
    public enum enmPayment1099rRequest
    {
         request_id ,
         tax_year ,
         status_id ,
         status_description ,
         status_value ,
         request_type_id ,
         request_type_description ,
         request_type_value ,
         process_date ,
         tax_month ,
         bulk_insert_1099r_data_flag ,
         created_1099r_details_report ,
         created_irs_file ,
         created_945_report ,
    }
}

