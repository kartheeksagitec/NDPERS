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
	/// Class NeoSpin.CustomDataObjects.cdoPsEmployment:
	/// Inherited from doPsEmployment, the class is used to customize the database object doPsEmployment.
	/// </summary>
    [Serializable]
	public class cdoPsEmployment : doPsEmployment
	{
		public cdoPsEmployment() : base()
		{
		}
    } 
} 
