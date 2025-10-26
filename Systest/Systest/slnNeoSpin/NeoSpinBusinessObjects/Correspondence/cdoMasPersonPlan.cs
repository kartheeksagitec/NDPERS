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
	/// Class NeoSpin.CustomDataObjects.cdoMasPersonPlan:
	/// Inherited from doMasPersonPlan, the class is used to customize the database object doMasPersonPlan.
	/// </summary>
    [Serializable]
	public class cdoMasPersonPlan : doMasPersonPlan
	{
		public cdoMasPersonPlan() : base()
		{
		}
    } 
} 
