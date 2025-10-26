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
	/// Class NeoSpin.CustomDataObjects.cdoMasFlexConversion:
	/// Inherited from doMasFlexConversion, the class is used to customize the database object doMasFlexConversion.
	/// </summary>
    [Serializable]
	public class cdoMasFlexConversion : doMasFlexConversion
	{
		public cdoMasFlexConversion() : base()
		{
		}
    } 
} 
