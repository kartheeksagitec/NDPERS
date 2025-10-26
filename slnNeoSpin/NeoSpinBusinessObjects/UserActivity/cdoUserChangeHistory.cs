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
	/// Class NeoSpin.CustomDataObjects.cdoUserChangeHistory:
	/// Inherited from doUserChangeHistory, the class is used to customize the database object doUserChangeHistory.
	/// </summary>
    [Serializable]
	public class cdoUserChangeHistory : doUserChangeHistory
	{
		public cdoUserChangeHistory() : base()
		{
		}
    } 
} 
