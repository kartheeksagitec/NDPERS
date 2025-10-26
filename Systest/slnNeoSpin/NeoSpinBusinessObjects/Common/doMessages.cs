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
	/// Class NeoSpin.DataObjects.doMessages:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doMessages : doBase
    {
         
         public doMessages() : base()
         {
         }
         public int message_id { get; set; }
         public string display_message { get; set; }
         public int severity_id { get; set; }
         public string severity_description { get; set; }
         public string severity_value { get; set; }
         public string internal_instructions { get; set; }
         public string employer_instructions { get; set; }
    }
    [Serializable]
    public enum enmMessages
    {
         message_id ,
         display_message ,
         severity_id ,
         severity_description ,
         severity_value ,
         internal_instructions ,
         employer_instructions ,
    }
}

