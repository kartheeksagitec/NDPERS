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
	/// Class NeoSpin.CustomDataObjects.cdoPsPerson:
	/// Inherited from doPsPerson, the class is used to customize the database object doPsPerson.
	/// </summary>
    [Serializable]
	public class cdoPsPerson : doPsPerson
	{
		public cdoPsPerson() : base()
		{
		}
    } 
} 
