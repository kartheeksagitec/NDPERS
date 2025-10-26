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
	/// Class NeoSpin.CustomDataObjects.cdoWssDebitAchRequest:
	/// Inherited from doWssDebitAchRequest, the class is used to customize the database object doWssDebitAchRequest.
	/// </summary>
    [Serializable]
	public class cdoWssDebitAchRequest : doWssDebitAchRequest
	{
		public cdoWssDebitAchRequest() : base()
		{
		}
    } 
} 
