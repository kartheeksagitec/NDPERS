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
	/// Class NeoSpin.CustomDataObjects.cdoMasBatchRequestPlan:
	/// Inherited from doMasBatchRequestPlan, the class is used to customize the database object doMasBatchRequestPlan.
	/// </summary>
    [Serializable]
	public class cdoMasBatchRequestPlan : doMasBatchRequestPlan
	{
		public cdoMasBatchRequestPlan() : base()
		{
		}
    } 
} 
