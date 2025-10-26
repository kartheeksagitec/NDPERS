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
	/// Class NeoSpin.DataObjects.doCafrReportBatchRequest:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doCafrReportBatchRequest : doBase
    {
         
         public doCafrReportBatchRequest() : base()
         {
         }
         public int cafr_report_batch_request_id { get; set; }
         public DateTime effective_date { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
    }
    [Serializable]
    public enum enmCafrReportBatchRequest
    {
         cafr_report_batch_request_id ,
         effective_date ,
         status_id ,
         status_description ,
         status_value ,
    }
}

