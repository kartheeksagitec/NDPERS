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
	/// Class NeoSpin.CustomDataObjects.cdoMasDefCompProvider:
	/// Inherited from doMasDefCompProvider, the class is used to customize the database object doMasDefCompProvider.
	/// </summary>
    [Serializable]
	public class cdoMasDefCompProvider : doMasDefCompProvider
	{
		public cdoMasDefCompProvider() : base()
		{
		}
    } 
} 
