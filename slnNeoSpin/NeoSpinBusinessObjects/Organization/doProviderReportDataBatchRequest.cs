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
	/// Class NeoSpin.DataObjects.doProviderReportDataBatchRequest:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doProviderReportDataBatchRequest : doBase
    {
         
         public doProviderReportDataBatchRequest() : base()
         {
         }
         public int provider_report_data_batch_request_id { get; set; }
         public string visibility_flag { get; set; }
         public int org_id { get; set; }
         public int plan_id { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public DateTime effective_start_date { get; set; }
         public DateTime effective_end_date { get; set; }
    }
    [Serializable]
    public enum enmProviderReportDataBatchRequest
    {
         provider_report_data_batch_request_id ,
         visibility_flag ,
         org_id ,
         plan_id ,
         status_id ,
         status_description ,
         status_value ,
         effective_start_date ,
         effective_end_date ,
    }
}

