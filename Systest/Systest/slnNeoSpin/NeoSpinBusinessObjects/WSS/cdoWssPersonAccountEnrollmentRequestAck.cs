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
	/// Class NeoSpin.CustomDataObjects.cdoWssPersonAccountEnrollmentRequestAck:
	/// Inherited from doWssPersonAccountEnrollmentRequestAck, the class is used to customize the database object doWssPersonAccountEnrollmentRequestAck.
	/// </summary>
    [Serializable]
	public class cdoWssPersonAccountEnrollmentRequestAck : doWssPersonAccountEnrollmentRequestAck
	{
		public cdoWssPersonAccountEnrollmentRequestAck() : base()
		{
		}
    } 
} 
