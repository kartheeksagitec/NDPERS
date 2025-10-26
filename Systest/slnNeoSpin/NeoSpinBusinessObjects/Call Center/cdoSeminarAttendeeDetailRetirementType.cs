#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
	/// <summary>
	/// Class NeoSpin.CustomDataObjects.cdoSeminarAttendeeDetailRetirementType:
	/// Inherited from doSeminarAttendeeDetailRetirementType, the class is used to customize the database object doSeminarAttendeeDetailRetirementType.
	/// </summary>
    [Serializable]
	public class cdoSeminarAttendeeDetailRetirementType : doSeminarAttendeeDetailRetirementType
	{
		public cdoSeminarAttendeeDetailRetirementType() : base()
		{
		}
    } 
} 
