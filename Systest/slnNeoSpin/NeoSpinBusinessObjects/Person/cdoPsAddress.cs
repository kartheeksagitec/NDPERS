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
	/// Class NeoSpin.CustomDataObjects.cdoPsAddress:
	/// Inherited from doPsAddress, the class is used to customize the database object doPsAddress.
	/// </summary>
    [Serializable]
	public class cdoPsAddress : doPsAddress
	{
		public cdoPsAddress() : base()
		{
		}
    } 
} 
