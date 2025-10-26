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
	/// Class NeoSpin.DataObjects.doDuesRateChangeRequest:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doDuesRateChangeRequest : doBase
    {
         public doDuesRateChangeRequest() : base()
         {
         }
         public int dues_rate_change_request_id { get; set; }
         public DateTime effective_date { get; set; }
         public int vendor_org_id { get; set; }
         public decimal monthly_amount { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
    }
    [Serializable]
    public enum enmDuesRateChangeRequest
    {
         dues_rate_change_request_id ,
         effective_date ,
         vendor_org_id ,
         monthly_amount ,
         status_id ,
         status_description ,
         status_value ,
    }
}

