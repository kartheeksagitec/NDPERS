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
	/// Class NeoSpin.DataObjects.doPostRetirementIncreaseBatchRequest:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPostRetirementIncreaseBatchRequest : doBase
    {
         
         public doPostRetirementIncreaseBatchRequest() : base()
         {
         }
         public int post_retirement_increase_batch_request_id { get; set; }
         public int post_retirement_increase_type_id { get; set; }
         public string post_retirement_increase_type_description { get; set; }
         public string post_retirement_increase_type_value { get; set; }
         public DateTime base_date { get; set; }
         public DateTime effective_date { get; set; }
         public decimal increase_percentage { get; set; }
         public decimal increase_flat_amount { get; set; }
         public string comments { get; set; }
         public string suppress_warnings_flag { get; set; }
         public string suppress_warnings_by { get; set; }
         public DateTime suppress_warnings_date { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public int action_status_id { get; set; }
         public string action_status_description { get; set; }
         public string action_status_value { get; set; }
         public string approved_by { get; set; }
         public int batch_request_status_id { get; set; }
         public string batch_request_status_description { get; set; }
         public string batch_request_status_value { get; set; }
    }
    [Serializable]
    public enum enmPostRetirementIncreaseBatchRequest
    {
         post_retirement_increase_batch_request_id ,
         post_retirement_increase_type_id ,
         post_retirement_increase_type_description ,
         post_retirement_increase_type_value ,
         base_date ,
         effective_date ,
         increase_percentage ,
         increase_flat_amount ,
         comments ,
         suppress_warnings_flag ,
         suppress_warnings_by ,
         suppress_warnings_date ,
         status_id ,
         status_description ,
         status_value ,
         action_status_id ,
         action_status_description ,
         action_status_value ,
         approved_by ,
         batch_request_status_id ,
         batch_request_status_description ,
         batch_request_status_value ,
    }
}

