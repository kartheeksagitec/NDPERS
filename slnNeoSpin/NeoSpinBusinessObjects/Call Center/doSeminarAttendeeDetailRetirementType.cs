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
	/// Class NeoSpin.DataObjects.doSeminarAttendeeDetailRetirementType:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doSeminarAttendeeDetailRetirementType : doBase
    {
         public doSeminarAttendeeDetailRetirementType() : base()
         {
         }
         public int seminar_attendee_detail_retirement_type_id { get; set; }
         public int seminar_attendee_detail_id { get; set; }
         public int retirement_type_id { get; set; }
         public string retirement_type_description { get; set; }
         public string retirement_type_value { get; set; }
    }
    [Serializable]
    public enum enmSeminarAttendeeDetailRetirementType
    {
         seminar_attendee_detail_retirement_type_id ,
         seminar_attendee_detail_id ,
         retirement_type_id ,
         retirement_type_description ,
         retirement_type_value ,
    }
}

