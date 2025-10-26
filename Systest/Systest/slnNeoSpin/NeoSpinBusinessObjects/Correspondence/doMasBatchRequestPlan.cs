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
	/// Class NeoSpin.DataObjects.doMasBatchRequestPlan:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doMasBatchRequestPlan : doBase
    {
         
         public doMasBatchRequestPlan() : base()
         {
         }
         public int mas_batch_request_plan_id { get; set; }
         public int mas_batch_request_id { get; set; }
         public int plan_id { get; set; }
    }
    [Serializable]
    public enum enmMasBatchRequestPlan
    {
         mas_batch_request_plan_id ,
         mas_batch_request_id ,
         plan_id ,
    }
}

