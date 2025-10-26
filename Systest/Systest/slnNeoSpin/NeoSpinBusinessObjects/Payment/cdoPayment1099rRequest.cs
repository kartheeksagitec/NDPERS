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
	/// Class NeoSpin.CustomDataObjects.cdoPayment1099rRequest:
	/// Inherited from doPayment1099rRequest, the class is used to customize the database object doPayment1099rRequest.
	/// </summary>
    [Serializable]
	public class cdoPayment1099rRequest : doPayment1099rRequest
	{
		public cdoPayment1099rRequest() : base()
		{
		}
    } 
} 
