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
	/// Class NeoSpin.DataObjects.doPlanJobClassCrossref:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPlanJobClassCrossref : doBase
    {
         
         public doPlanJobClassCrossref() : base()
         {
         }
         public int plan_jobclass_id { get; set; }
         public int plan_id { get; set; }
         public int job_type_id { get; set; }
         public string job_type_description { get; set; }
         public string job_type_value { get; set; }
         public int job_class_id { get; set; }
         public string job_class_description { get; set; }
         public string job_class_value { get; set; }
         public string dc_transfer_eligible { get; set; }
    }
    [Serializable]
    public enum enmPlanJobClassCrossref
    {
         plan_jobclass_id ,
         plan_id ,
         job_type_id ,
         job_type_description ,
         job_type_value ,
         job_class_id ,
         job_class_description ,
         job_class_value ,
         dc_transfer_eligible ,
    }
}

