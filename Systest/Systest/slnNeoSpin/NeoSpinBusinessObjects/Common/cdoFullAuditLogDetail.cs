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
	/// Class NeoSpin.CustomDataObjects.cdoFullAuditLogDetail:
	/// Inherited from doFullAuditLogDetail, the class is used to customize the database object doFullAuditLogDetail.
	/// </summary>
    [Serializable]
	public class cdoFullAuditLogDetail : doFullAuditLogDetail
	{
		public cdoFullAuditLogDetail() : base()
		{
		}
    } 
} 
