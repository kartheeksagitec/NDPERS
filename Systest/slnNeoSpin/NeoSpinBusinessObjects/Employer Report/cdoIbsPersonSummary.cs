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
	/// Class NeoSpin.CustomDataObjects.cdoIbsPersonSummary:
	/// Inherited from doIbsPersonSummary, the class is used to customize the database object doIbsPersonSummary.
	/// </summary>
    [Serializable]
	public class cdoIbsPersonSummary : doIbsPersonSummary
	{
		public cdoIbsPersonSummary() : base()
		{
		}
    } 
} 
