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
	/// Class NeoSpin.CustomDataObjects.cdoPersonInvalidSsnRef:
	/// Inherited from doPersonInvalidSsnRef, the class is used to customize the database object doPersonInvalidSsnRef.
	/// </summary>
    [Serializable]
	public class cdoPersonInvalidSsnRef : doPersonInvalidSsnRef
	{
		public cdoPersonInvalidSsnRef() : base()
		{
		}
    } 
} 
