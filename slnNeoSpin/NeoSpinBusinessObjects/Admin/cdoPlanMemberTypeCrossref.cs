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
	/// Class NeoSpin.CustomDataObjects.cdoPlanMemberTypeCrossref:
	/// Inherited from doPlanMemberTypeCrossref, the class is used to customize the database object doPlanMemberTypeCrossref.
	/// </summary>
    [Serializable]
	public class cdoPlanMemberTypeCrossref : doPlanMemberTypeCrossref
	{
		public cdoPlanMemberTypeCrossref() : base()
		{
		}
    } 
} 
