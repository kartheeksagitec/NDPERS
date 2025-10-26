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
	/// Class NeoSpin.CustomDataObjects.cdoCafrReportBatchRequest:
	/// Inherited from doCafrReportBatchRequest, the class is used to customize the database object doCafrReportBatchRequest.
	/// </summary>
    [Serializable]
	public class cdoCafrReportBatchRequest : doCafrReportBatchRequest
	{
		public cdoCafrReportBatchRequest() : base()
		{
		}
    } 
} 
