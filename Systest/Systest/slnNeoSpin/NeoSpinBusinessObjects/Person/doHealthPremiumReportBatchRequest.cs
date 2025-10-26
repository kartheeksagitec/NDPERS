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
	/// Class NeoSpin.DataObjects.doHealthPremiumReportBatchRequest:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doHealthPremiumReportBatchRequest : doBase
    {
         
         public doHealthPremiumReportBatchRequest() : base()
         {
         }
         public int batch_request_id { get; set; }
         public int perslink_id { get; set; }
         public int org_id { get; set; }
         public int health_insurance_type_id { get; set; }
         public string health_insurance_type_description { get; set; }
         public string health_insurance_type_value { get; set; }
         public string coverage_code { get; set; }
         public DateTime plan_start_date { get; set; }
         public DateTime plan_end_date { get; set; }
         public DateTime history_date { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public string rate_structure_code { get; set; }
    }
    [Serializable]
    public enum enmHealthPremiumReportBatchRequest
    {
         batch_request_id ,
         perslink_id ,
         org_id ,
         health_insurance_type_id ,
         health_insurance_type_description ,
         health_insurance_type_value ,
         coverage_code ,
         plan_start_date ,
         plan_end_date ,
         history_date ,
         status_id ,
         status_description ,
         status_value ,
         rate_structure_code ,
    }
}

