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
	/// Class NeoSpin.CustomDataObjects.cdoMasFlexOptions:
	/// Inherited from doMasFlexOptions, the class is used to customize the database object doMasFlexOptions.
	/// </summary>
    [Serializable]
	public class cdoMasFlexOption : doMasFlexOption
	{
		public cdoMasFlexOption() : base()
		{
		}
    } 
} 
