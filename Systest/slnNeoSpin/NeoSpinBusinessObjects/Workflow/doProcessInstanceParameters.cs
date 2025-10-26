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
	/// Class NeoSpin.DataObjects.doProcessInstanceParameters:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doProcessInstanceParameters : doBase
    {
         
         public doProcessInstanceParameters() : base()
         {
         }
         public int process_instance_parameter_id { get; set; }
         public int process_instance_id { get; set; }
         public string parameter_name { get; set; }
         public string parameter_value { get; set; }
    }
    [Serializable]
    public enum enmProcessInstanceParameters
    {
         process_instance_parameter_id ,
         process_instance_id ,
         parameter_name ,
         parameter_value ,
    }
}

