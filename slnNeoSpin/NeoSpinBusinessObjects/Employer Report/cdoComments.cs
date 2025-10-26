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
	/// Class NeoSpin.CustomDataObjects.cdoComments:
	/// Inherited from doComments, the class is used to customize the database object doComments.
	/// </summary>
    [Serializable]
	public class cdoComments : doComments
	{
		public cdoComments() : base()
		{
		}
    } 
} 
